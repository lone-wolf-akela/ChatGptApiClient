/*
    ChatGPT Client V2: A GUI client for the OpenAI ChatGPT API (and also Anthropic Claude API) based on WPF.
    Copyright (C) 2024 Lone Wolf Akela

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.AI.OpenAI;
using Flurl;
using HandyControl.Tools.Extension;
using HarmonyLib;
using Namotion.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using OpenAI;
using OpenAI.Chat;
using static ChatGptApiClientV2.ChatCompletionRequest;

// ReSharper disable PropertyCanBeMadeInitOnly.Global
namespace ChatGptApiClientV2;

// Some 3rd party API provider will return "finish_reason" as an empty string "", which cause
// error with openai sdk, as it expects a null instead. we need to patch it here

public static class Patcher
{
    [ModuleInitializer]
    public static void Patch()
    {
        var harmony = new Harmony("Lone Wolf Patch");
        var assembly = Assembly.GetExecutingAssembly();
        harmony.PatchAll(assembly);
    }
}

[HarmonyPatch(typeof(StreamingChatCompletionUpdate), "DeserializeStreamingChatCompletionUpdate")]
class Patch
{
    static void Prefix(ref JsonElement element, ModelReaderWriterOptions options)
    {
        using var doc = JsonDocument.Parse(element.GetRawText());
        var root = doc.RootElement.Clone();

        var jsonObject = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(root.GetRawText());

        if (jsonObject is null || !jsonObject.TryGetValue("choices", out var choicesObj))
        {
            return;
        }

        if (choicesObj is JsonElement { ValueKind: JsonValueKind.Array } choicesElement)
        {
            var choicesList = new List<Dictionary<string, object>>();
            foreach (var choiceElement in choicesElement.EnumerateArray())
            {
                var choiceDict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(choiceElement.GetRawText());

                if (choiceDict != null &&
                    choiceDict.TryGetValue("finish_reason", out var finishReason) &&
                    finishReason is JsonElement { ValueKind: JsonValueKind.String } finishReasonStr &&
                    string.IsNullOrEmpty(finishReasonStr.GetString()))
                {
                    choiceDict.Remove("finish_reason");
                }

                choicesList.Add(choiceDict!);
            }

            jsonObject["choices"] = choicesList;
        }

        var modifiedJson = System.Text.Json.JsonSerializer.SerializeToElement(jsonObject);
        element = modifiedJson;
    }
}

public class ServerEndpointOptions
{
    public enum ServiceType
    {
        OpenAI,
        Azure,
        Claude,
        OtherOpenAICompat,
        Custom
    }

    public ServiceType Service { get; set; }
    public string Endpoint { get; set; } = "";
    public string Key { get; set; } = "";
    public string AzureKey { get; set; } = "";
    public string Model { get; set; } = "";
    public int? MaxTokens { get; set; }
    public float? PresencePenalty { get; set; }
    public long? Seed { get; set; }
    public float? Temperature { get; set; }
    public float? TopP { get; set; }
    public IEnumerable<ToolType>? Tools { get; set; }
    public string? UserId { get; set; }
    public IEnumerable<string>? StopSequences { get; set; }
}

public interface IServerEndpoint
{
    public static IServerEndpoint BuildServerEndpoint(ServerEndpointOptions options)
    {
        return options.Service == ServerEndpointOptions.ServiceType.Claude
            ? new ClaudeEndpoint(options)
            : new OpenAIEndpoint(options);
    }

    public Task BuildSession(ChatCompletionRequest session, CancellationToken cancellationToken = default);
    public IAsyncEnumerable<string> Streaming(CancellationToken cancellationToken = default);
    public string SystemFingerprint { get; }
    public AssistantMessage ResponseMessage { get; }
    public IEnumerable<ToolCallType> ToolCalls { get; }
}

public class OpenAIEndpoint : IServerEndpoint
{
    public OpenAIEndpoint(ServerEndpointOptions o)
    {
        options = o;

        var defaultTimeout = TimeSpan.FromMinutes(10);

        switch (options.Service)
        {
            case ServerEndpointOptions.ServiceType.Azure:
            {
                var opt = new AzureOpenAIClientOptions { NetworkTimeout = defaultTimeout };
                var azure = new AzureOpenAIClient(new Uri(options.Endpoint), new ApiKeyCredential(options.AzureKey), opt);
                client = azure.GetChatClient(options.Model);
                break;
            }
            case ServerEndpointOptions.ServiceType.OpenAI:
            {
                var opt = new OpenAIClientOptions { NetworkTimeout = defaultTimeout };
                var openai = new OpenAIClient(new ApiKeyCredential(options.Key), opt);
                client = openai.GetChatClient(options.Model);
                break;
            }
            case ServerEndpointOptions.ServiceType.Custom:
            {
                var opt = new OpenAIClientOptions { Endpoint = new Uri(options.Endpoint), NetworkTimeout = defaultTimeout };
                var openai = new OpenAIClient(new ApiKeyCredential(options.Key), opt);
                client = openai.GetChatClient(options.Model);
                break;
            }
            case ServerEndpointOptions.ServiceType.OtherOpenAICompat:
            {
                var opt = new OpenAIClientOptions { Endpoint = new Uri(options.Endpoint), NetworkTimeout = defaultTimeout };
                var openai = new OpenAIClient(new ApiKeyCredential(options.Key), opt);
                client = openai.GetChatClient(options.Model);
                break;
            }
            case ServerEndpointOptions.ServiceType.Claude:
            default:
            {
                throw new InvalidOperationException("Invalid service type");
            }
        }

    }

    private IEnumerable<ChatTool> GetToolDefinitions()
    {
        var lst =
            from tool in options.Tools ?? []
            select ChatTool.CreateFunctionTool(
                tool.Function.Name,
                tool.Function.Description,
                BinaryData.FromString(tool.Function.Parameters.ToJson())
                );
        return lst;
    }

    private static ChatMessage ChatDataMessageToChatRequestMessage(IMessage chatDataMsg)
    {
        var contents = chatDataMsg.Content.ToList();
        if (chatDataMsg is UserMessage userMsg)
        {
            contents.AddRange(userMsg.GenerateAttachmentContentList());
            var contentLst = new List<ChatMessageContentPart>();
            foreach (var content in contents)
            {
                if (content is IMessage.TextContent textContent)
                {
                    contentLst.Add(ChatMessageContentPart.CreateTextPart(textContent.Text));
                }
                else if (content is IMessage.ImageContent imageContent)
                {
                    var detailLevel =
                        imageContent.ImageUrl.Detail == IMessage.ImageContent.ImageUrlType.ImageDetail.Low
                            ? ChatImageDetailLevel.Low
                            : ChatImageDetailLevel.High;
                    var imageData = imageContent.ImageUrl.Url;
                    contentLst.Add(ChatMessageContentPart.CreateImagePart(imageData?.Data, imageData?.MimeType, detailLevel));
                }
                else
                {
                    throw new ArgumentException("Invalid content type");
                }
            }

            if (chatDataMsg.Name is not null)
            {
                return new UserChatMessage(contentLst)
                {
                    ParticipantName = chatDataMsg.Name
                };
            }
            else
            {
                return new UserChatMessage(contentLst);
            }
        }

        foreach (var content in contents)
        {
            if (content.Type is not IMessage.ContentCategory.Text)
            {
                throw new ArgumentException("Non user message content must be text");
            }
        }

        var textContents = from content in contents select ((IMessage.TextContent)content).Text;
        var combinedContent = string.Join("\n\n", textContents);

        if (chatDataMsg is SystemMessage)
        {
            return new SystemChatMessage(combinedContent);
        }

        if (chatDataMsg is AssistantMessage assistantMessage)
        {
            var chatRequestAssistantMsg = new AssistantChatMessage(combinedContent);
            var convertedToolCalls =
                from toolCall in assistantMessage.ToolCalls ?? []
                select ChatToolCall.CreateFunctionToolCall(
                    toolCall.Id,
                    toolCall.Function.Name,
                    BinaryData.FromString(toolCall.Function.Arguments)
                );
            chatRequestAssistantMsg.ToolCalls.AddRange(convertedToolCalls);
            return chatRequestAssistantMsg;
        }

        if (chatDataMsg is ToolMessage toolMessage)
        {
            return new ToolChatMessage(toolMessage.ToolCallId, combinedContent);
        }

        throw new ArgumentException("Invalid role");
    }

    private static IEnumerable<ChatMessage> GetChatRequestMessages(IEnumerable<IMessage> msgLst)
    {
        var lst =
            from msg in msgLst
            select ChatDataMessageToChatRequestMessage(msg);
        return lst;
    }

    public Task BuildSession(ChatCompletionRequest session, CancellationToken cancellationToken = default)
    {
        try
        {
            var chatCompletionsOptions = new ChatCompletionOptions
            {
                MaxOutputTokenCount = options.MaxTokens,
                PresencePenalty = options.PresencePenalty,
#pragma warning disable OPENAI001
                Seed = options.Seed,
#pragma warning restore OPENAI001
                Temperature = options.Temperature,
                TopP = options.TopP,
                EndUserId = options.UserId
            };
            chatCompletionsOptions.Tools.AddRange(GetToolDefinitions());
            if (options.StopSequences is not null)
            {
                chatCompletionsOptions.StopSequences.AddRange(options.StopSequences);
            }

            streamingResponse = client.CompleteChatStreamingAsync(GetChatRequestMessages(session.Messages), chatCompletionsOptions, cancellationToken);
            responseSb.Clear();
            errorMessage = null;
            systemFingerprint = "";
            toolCallIdsByIndex.Clear();
            funcNamesByIndex.Clear();
            funcArgsByIndex.Clear();
        }
        catch (Exception e)
        {
            errorMessage = e.Message;
        }
        return Task.CompletedTask;
    }

    public async IAsyncEnumerable<string> Streaming(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (errorMessage is not null)
        {
            yield break;
        }

        if (streamingResponse is null)
        {
            throw new InvalidOperationException("Session not built");
        }

        var enumerator = streamingResponse.GetAsyncEnumerator(cancellationToken);

        try
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                bool hasMore;

                try
                {
                    hasMore = await enumerator.MoveNextAsync().ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    errorMessage = e.Message;
                    break;
                }

                if (!hasMore)
                {
                    break;
                }

                var chatUpdate = enumerator.Current;
                if (!string.IsNullOrEmpty(chatUpdate.SystemFingerprint) && !chatUpdate.SystemFingerprint.StartsWith("stopping_word"))
                {
                    systemFingerprint = chatUpdate.SystemFingerprint;
                }

                foreach(var toolCallUpdate in chatUpdate.ToolCallUpdates)
                {
                    if (!string.IsNullOrEmpty(toolCallUpdate.ToolCallId))
                    {
                        toolCallIdsByIndex[toolCallUpdate.Index] = toolCallUpdate.ToolCallId;
                    }

                    if (!string.IsNullOrEmpty(toolCallUpdate.FunctionName))
                    {
                        funcNamesByIndex[toolCallUpdate.Index] = toolCallUpdate.FunctionName;
                    }

                    if (!string.IsNullOrEmpty(toolCallUpdate.FunctionArgumentsUpdate.ToString()))
                    {
                        if (!funcArgsByIndex.TryGetValue(toolCallUpdate.Index, out var arg))
                        {
                            arg = new StringBuilder();
                            funcArgsByIndex[toolCallUpdate.Index] = arg;
                        }

                        arg.Append(toolCallUpdate.FunctionArgumentsUpdate);
                    }
                }

                foreach(var part in chatUpdate.ContentUpdate)
                {
                    responseSb.Append(part.Text);
                    yield return part.Text;
                }

                if (!string.IsNullOrEmpty(chatUpdate.SystemFingerprint) && chatUpdate.SystemFingerprint.StartsWith("stopping_word"))
                {
                    responseSb.Append(chatUpdate.SystemFingerprint["stopping_word ".Length..]);
                }
            }
        }
        finally
        {
            await enumerator.DisposeAsync().ConfigureAwait(false);
        }
    }

    public string SystemFingerprint => systemFingerprint;

    public AssistantMessage ResponseMessage
    {
        get
        {
            if (!string.IsNullOrEmpty(errorMessage))
            {
                responseSb.AppendLine();
                responseSb.AppendLine(errorMessage);
            }
            var response = new AssistantMessage
            {
                Content =
                [
                    new IMessage.TextContent { Text = responseSb.ToString() }
                ],
                ToolCalls = ToolCalls.ToList(),
                Provider = options.Service == ServerEndpointOptions.ServiceType.OtherOpenAICompat ? 
                    ModelInfo.ProviderEnum.OtherOpenAICompat : ModelInfo.ProviderEnum.OpenAI
            };
            return response;
        }
    }

    public IEnumerable<ToolCallType> ToolCalls
    {
        get
        {
            foreach (var (index, id) in toolCallIdsByIndex)
            {
                var toolcall = new ToolCallType
                {
                    Id = id,
                    Function = new ToolCallType.FunctionType
                    {
                        Name = funcNamesByIndex[index],
                        Arguments = funcArgsByIndex[index].ToString()
                    }
                };
                yield return toolcall;
            }
        }
    }

    private readonly ServerEndpointOptions options;
    private readonly ChatClient client;
    private AsyncCollectionResult<StreamingChatCompletionUpdate>? streamingResponse;
    private readonly StringBuilder responseSb = new();
    private string? errorMessage;
    private string systemFingerprint = "";
    private readonly Dictionary<int, string> toolCallIdsByIndex = [];
    private readonly Dictionary<int, string> funcNamesByIndex = [];
    private readonly Dictionary<int, StringBuilder> funcArgsByIndex = [];
}

public partial class ClaudeEndpoint : IServerEndpoint
{
    private const string ApiVersion = "2023-06-01";
    private const string ApiBeta = "tools-2024-05-16";

    public ClaudeEndpoint(ServerEndpointOptions o)
    {
        options = o;

        if (options.Service is not ServerEndpointOptions.ServiceType.Claude)
        {
            throw new ArgumentException("Invalid service type");
        }

        var httpClientHandler = new HttpClientHandler
        {
            AutomaticDecompression = System.Net.DecompressionMethods.All
        };
        httpClient = new HttpClient(httpClientHandler);
        httpClient.DefaultRequestHeaders.Add("x-api-key", options.Key);
        httpClient.DefaultRequestHeaders.Add("anthropic-version", ApiVersion);

        if (!string.IsNullOrWhiteSpace(ApiBeta))
        {
            httpClient.DefaultRequestHeaders.Add("anthropic-beta", ApiBeta);
        }
    }

    private IEnumerable<Claude.Tool> GetToolDefinitions()
    {
        var tools =
            from tool in options.Tools ?? []
            select new Claude.Tool
            {
                Name = tool.Function.Name,
                Description = tool.Function.Description,
                InputSchema = tool.Function.Parameters
            };
        return tools;
    }

    private static Claude.Message ChatDataMessageToClaudeMessage(IMessage chatDataMsg)
    {
        var contents = chatDataMsg.Content.ToList();
        var claudeContentList = new List<Claude.IContent>();
        if (chatDataMsg is UserMessage userMsg)
        {
            contents.AddRange(userMsg.GenerateAttachmentContentList());
        }

        foreach (var content in contents)
        {
            if (chatDataMsg is ToolMessage tooMsg)
            {
                var toolResultContent = new Claude.ToolResultContent
                {
                    ToolUseId = tooMsg.ToolCallId,
                    Content = ((IMessage.TextContent)content).Text
                };
                claudeContentList.Add(toolResultContent);
            }
            else if (content is IMessage.TextContent textContent)
            {
                claudeContentList.Add(new Claude.TextContent
                {
                    Text = textContent.Text
                });
            }
            else if (content is IMessage.ImageContent imageContent)
            {
                var imageData = imageContent.ImageUrl.Url;
                var mime = imageData?.MimeType;
                var data = imageData?.ToBase64();
                var claudeImageContent = new Claude.ImageContent();
                if (mime is null || data is null)
                {
                    throw new ArgumentException("Invalid image content");
                }
                claudeImageContent.Source.SetMediaType(mime);
                claudeImageContent.Source.Data = data;
                claudeContentList.Add(claudeImageContent);
            }
            else
            {
                throw new ArgumentException("Invalid content type");
            }
        }

        if (chatDataMsg is AssistantMessage assistantMsg)
        {
            foreach (var toolCall in assistantMsg.ToolCalls ?? [])
            {
                var toolUseContent = new Claude.ToolUseContent
                {
                    Id = toolCall.Id,
                    Name = toolCall.Function.Name,
                    Input = JObject.Parse(toolCall.Function.Arguments)
                };
                claudeContentList.Add(toolUseContent);
            }
        }

        var role = chatDataMsg.Role switch
        {
            RoleType.User => Claude.Role.User,
            RoleType.Assistant => Claude.Role.Assistant,
            RoleType.System => Claude.Role.User,
            RoleType.Tool => Claude.Role.User,
            _ => throw new ArgumentException("Invalid role")
        };
        return new Claude.Message
        {
            Role = role,
            Content = claudeContentList
        };
    }

    /// <summary>
    /// if two adjacent messages are both user or assistant role, merge them
    /// </summary>
    /// <param name="messages"> message list that has not been normalized</param>
    /// <returns></returns>
    private static List<Claude.Message> NormalizeMessages(IEnumerable<Claude.Message> messages)
    {
        var mergedMessages = new List<Claude.Message>();
        Claude.Role? lastRole = null;
        foreach (var msg in messages)
        {
            if (msg.Role == lastRole)
            {
                var lastMsg = mergedMessages.Last();
                var lastContent = lastMsg.Content;
                var newContent = lastContent.Concat(msg.Content);
                lastMsg.Content = newContent.ToList();
            }
            else
            {
                mergedMessages.Add(msg);
                lastRole = msg.Role;
            }
        }

        return mergedMessages;
    }

    public async Task BuildSession(ChatCompletionRequest session, CancellationToken cancellationToken = default)
    {
        try
        {
            var messages = new List<Claude.Message>();
            string? systemMessage = null;
            var firstMsg = true;
            foreach (var msg in session.Messages)
            {
                if (firstMsg)
                {
                    firstMsg = false;
                    if (msg.Role == RoleType.System)
                    {
                        if (msg.Content.Any(c => c.Type != IMessage.ContentCategory.Text))
                        {
                            throw new ArgumentException("System message must be text");
                        }

                        var allText = from content in msg.Content
                            select ((IMessage.TextContent)content).Text;
                        systemMessage = string.Join("\n\n", allText);
                        continue;
                    }
                }

                messages.Add(ChatDataMessageToClaudeMessage(msg));
            }

            var tools = GetToolDefinitions().ToList();
            var createMsg = new Claude.CreateMessage
            {
                Model = options.Model,
                Messages = NormalizeMessages(messages),
                MaxTokens = options.MaxTokens ?? 4096,
                System = systemMessage,
                Temperature = options.Temperature,
                Tools = tools.Count != 0 ? tools : null,
                TopP = options.TopP,
                Stream = true,
                Metadata = options.UserId is null ? null : new Claude.Metadata { UserId = options.UserId },
                StopSequences = options.StopSequences
            };

            var postStr = createMsg.ToJson();
            var postContent = new StringContent(postStr, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(Url.Combine(options.Endpoint, "messages")),
                Content = postContent
            };
            httpResponse =
                await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            errorMessage = null;

            indexToContentBlockStart.Clear();
            indexToContentBlockDeltas.Clear();
        }
        catch (Exception e)
        {
            errorMessage = e.Message;
        } 
    }

    public async IAsyncEnumerable<string> Streaming(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (errorMessage is not null)
        {
            yield break;
        }

        if (httpResponse is null)
        {
            throw new InvalidOperationException("Session not built");
        }

        var contractResolver = new DefaultContractResolver
        {
            NamingStrategy = new SnakeCaseNamingStrategy()
        };
        var settings = new JsonSerializerSettings
        {
            ContractResolver = contractResolver
        };

        await using var responseStream =
            await httpResponse.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        if (!httpResponse.IsSuccessStatusCode)
        {
            try
            {
                using var errorReader = new StreamReader(responseStream);
                var responseStr = await errorReader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
                var error = JsonConvert.DeserializeObject<Claude.ErrorResponse>(responseStr, settings)
                            ?? throw new JsonSerializationException(responseStr);
                errorMessage = $"{error.Error.Type}: {error.Error.Message}";
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
            }

            yield break;
        }

        using var reader = new SseReader(responseStream);
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var line = await reader.TryReadLineAsync(cancellationToken).ConfigureAwait(false);
            if (line is null)
            {
                break;
            }

            if (line.Value.IsEmpty)
            {
                continue;
            }

            if (line.Value.IsComment)
            {
                continue;
            }

            var fieldName = line.Value.FieldName;
            var fieldValue = line.Value.FieldValue;

            if (!fieldName.Span.SequenceEqual("data".AsSpan()))
            {
                continue;
            }

            if (fieldValue.Span.SequenceEqual("[DONE]".AsSpan()))
            {
                break;
            }

            var chatResponse = fieldValue.ToString();

            Claude.IStreamingResponse? responseChunk;
            try
            {
                responseChunk = JsonConvert.DeserializeObject<Claude.IStreamingResponse>(chatResponse, settings);
                if (responseChunk is null)
                {
                    throw new JsonSerializationException(chatResponse);
                }
            }
            catch (JsonSerializationException exception)
            {
                errorMessage = $"Error: Invalid chat response: {exception.Message}";
                yield break;
            }

            if (responseChunk is Claude.StreamingMessageStart /*msgStart*/)
            {
                // not used
                // messageStart = msgStart;
            }
            else if (responseChunk is Claude.StreamingContentBlockStart contentStart)
            {
                indexToContentBlockStart[contentStart.Index] = contentStart;
                if (contentStart.ContentBlock is Claude.StreamingContentBlockText textContent)
                {
                    yield return textContent.Text;
                }
            }
            else if (responseChunk is Claude.StreamingPing)
            {
                // do nothing
            }
            else if (responseChunk is Claude.StreamingContentBlockDelta contentDelta)
            {
                if (!indexToContentBlockDeltas.TryGetValue(contentDelta.Index, out var deltaList))
                {
                    deltaList = [];
                    indexToContentBlockDeltas[contentDelta.Index] = deltaList;
                }

                deltaList.Add(contentDelta);
                if (contentDelta.Delta is Claude.StreamingContentBlockTextDelta delta)
                {
                    yield return delta.Text;
                }
            }
            else if (responseChunk is Claude.StreamingContentBlockStop)
            {
                // do nothing
            }
            else if (responseChunk is Claude.StreamingMessageDelta /*msgDelta*/)
            {
                // not used
                // messageDeltas.Add(msgDelta);
            }
            else if (responseChunk is Claude.StreamingMessageStop)
            {
                // do nothing
            }
            else if (responseChunk is Claude.StreamingError error)
            {
                errorMessage = $"{error.Error.Type}: {error.Error.Message}";
                yield break;
            }
            else
            {
                errorMessage = $"Error: Invalid chat response: {chatResponse}";
                yield break;
            }
        }
    }

    public string SystemFingerprint => "";

    [GeneratedRegex(@"\p{IsCJKUnifiedIdeographs}")]
    private static partial Regex CjkCharRegex();

    private static bool IsChinese(char c)
    {
        var r = CjkCharRegex();
        return r.IsMatch(c.ToString());
    }

    private static bool IsEnglishOrNumber(char c)
    {
        return c is (>= 'a' and <= 'z') or (>= 'A' and <= 'Z') or (>= '0' and <= '9');
    }

    private static string NormalizeChinesePunctuation(string input)
    {
        var punctuationPair = new Dictionary<char, char>
        {
            { ',', '，' },
            { '?', '？' },
            { ';', '；' },
            { ':', '：' },
            { '!', '！' }
        };
        // for each appearance of key in input
        // if the character before it is a chinese character
        // replace it with the value
        // 
        // and if a char is an English letter or a number, and the char before it is a Chinese character,
        // add a space before it
        //
        // and if a char is a chinese character, and the char before it is an English letter or a number,
        // add a space before it
        var sb = new StringBuilder();
        for (var i = 0; i < input.Length; i++)
        {
            var lastChar = i > 0 ? input[i - 1] : '\0';
            if (punctuationPair.TryGetValue(input[i], out var chinesePunc) && IsChinese(lastChar))
            {
                sb.Append(chinesePunc);
                continue;
            }

            if (IsEnglishOrNumber(input[i]) && IsChinese(lastChar))
            {
                sb.Append(' ');
            }
            else if (IsChinese(input[i]) && IsEnglishOrNumber(lastChar))
            {
                sb.Append(' ');
            }

            sb.Append(input[i]);
        }

        return sb.ToString();
    }

    public AssistantMessage ResponseMessage
    {
        get
        {
            var assistantTextContent = new List<IMessage.TextContent>();
            StringBuilder msgBuilder = new();

            var contentIndex = indexToContentBlockStart.Keys.ToList();
            contentIndex.Sort();
            foreach (var index in contentIndex)
            {
                msgBuilder.Clear();
                var start = indexToContentBlockStart[index].ContentBlock;
                if (start is not Claude.StreamingContentBlockText textContentStart)
                {
                    continue;
                }

                msgBuilder.Append(textContentStart.Text);
                foreach (var delta in indexToContentBlockDeltas[index])
                {
                    if (delta.Delta is not Claude.StreamingContentBlockTextDelta textContentDelta)
                    {
                        throw new InvalidOperationException("Invalid delta type");
                    }

                    msgBuilder.Append(textContentDelta.Text);
                }

                assistantTextContent.Add(new IMessage.TextContent
                    { Text = NormalizeChinesePunctuation(msgBuilder.ToString()) });
            }

            if (errorMessage is not null)
            {
                assistantTextContent.Add(new IMessage.TextContent { Text = errorMessage });
            }

            var response = new AssistantMessage
            {
                Content = assistantTextContent,
                ToolCalls = ToolCalls.ToList(),
                Provider = ModelInfo.ProviderEnum.Anthropic
            };
            return response;
        }
    }
    public IEnumerable<ToolCallType> ToolCalls
    {
        get
        {
            StringBuilder argsBuilder = new();

            var contentIndex = indexToContentBlockStart.Keys.ToList();
            contentIndex.Sort();
            foreach (var index in contentIndex)
            {
                argsBuilder.Clear();
                var start = indexToContentBlockStart[index].ContentBlock;
                if (start is not Claude.StreamingContentBlockToolUse toolUseStart)
                {
                    continue;
                }
                foreach (var delta in indexToContentBlockDeltas[index])
                {
                    if (delta.Delta is not Claude.StreamingContentBlockInputJsonDelta inputJsonDelta)
                    {
                        throw new InvalidOperationException("Invalid delta type");
                    }
                    argsBuilder.Append(inputJsonDelta.PartialJson);
                }
                var toolCall = new ToolCallType
                {
                    Id = toolUseStart.Id,
                    Function = new ToolCallType.FunctionType
                    {
                        Name = toolUseStart.Name,
                        Arguments = argsBuilder.ToString()
                    }
                };
                yield return toolCall;
            }
        }
    }

    private readonly HttpClient httpClient;
    private string? errorMessage;
    private HttpResponseMessage? httpResponse;
    private readonly ServerEndpointOptions options;
    private readonly Dictionary<int, Claude.StreamingContentBlockStart> indexToContentBlockStart = [];
    private readonly Dictionary<int, List<Claude.StreamingContentBlockDelta>> indexToContentBlockDeltas = [];
}
