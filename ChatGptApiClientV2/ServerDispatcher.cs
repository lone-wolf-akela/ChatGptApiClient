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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Drawing.Charts;
using Flurl;
using HandyControl.Tools.Extension;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using OpenAI;
using OpenAI.Chat;
using static ChatGptApiClientV2.ChatCompletionRequest;
using ReflectionMagic;

// ReSharper disable PropertyCanBeMadeInitOnly.Global
namespace ChatGptApiClientV2;

public class ServerEndpointOptions
{
    public enum ServiceType
    {
        OpenAI,
        Claude,
        DeepSeek,
        OtherOpenAICompat,
        Custom
    }

    public ServiceType Service { get; set; }
    public string Endpoint { get; set; } = "";
    public string Key { get; set; } = "";
    public string Model { get; set; } = "";
    public int? MaxTokens { get; set; }
    public float? PresencePenalty { get; set; }
    public long? Seed { get; set; }
    public float? Temperature { get; set; }
    public float? TopP { get; set; }
    public IEnumerable<ToolType>? Tools { get; set; }
    public string? UserId { get; set; }
    public IEnumerable<string>? StopSequences { get; set; }

    public bool IsO1 { get; set; } = false;
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
        _options = o;

        var defaultTimeout = TimeSpan.FromMinutes(10);

        switch (_options.Service)
        {
            case ServerEndpointOptions.ServiceType.OpenAI:
            {
                var opt = new OpenAIClientOptions { NetworkTimeout = defaultTimeout };
                var openai = new OpenAIClient(new ApiKeyCredential(_options.Key), opt);
                _client = openai.GetChatClient(_options.Model);
                break;
            }
            case ServerEndpointOptions.ServiceType.DeepSeek:
            case ServerEndpointOptions.ServiceType.Custom:
            case ServerEndpointOptions.ServiceType.OtherOpenAICompat:
            {
                var opt = new OpenAIClientOptions { Endpoint = new Uri(_options.Endpoint), NetworkTimeout = defaultTimeout };
                var openai = new OpenAIClient(new ApiKeyCredential(_options.Key), opt);
                _client = openai.GetChatClient(_options.Model);
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
            from tool in _options.Tools ?? []
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
                MaxOutputTokenCount = _options.MaxTokens,
#pragma warning disable OPENAI001
                Seed = _options.Seed,
#pragma warning restore OPENAI001
                EndUserId = _options.UserId
            };

            if (!_options.IsO1)
            {
                chatCompletionsOptions.TopP = _options.TopP;
                chatCompletionsOptions.PresencePenalty = _options.PresencePenalty;
                chatCompletionsOptions.Temperature = _options.Temperature;
            }

            chatCompletionsOptions.Tools.AddRange(GetToolDefinitions());
            if (_options.StopSequences is not null)
            {
                chatCompletionsOptions.StopSequences.AddRange(_options.StopSequences);
            }

            var messages = GetChatRequestMessages(session.Messages);

            if (_options.IsO1)
            {
                messages = from m in messages where m is not SystemChatMessage select m;
            }

            _streamingResponse = _client.CompleteChatStreamingAsync(messages, chatCompletionsOptions, cancellationToken);
            _lastResponseIsReasoning = false;
            _responseSb.Clear();
            _reasoningSb.Clear();
            _errorMessage = null;
            SystemFingerprint = "";
            _toolCallIdsByIndex.Clear();
            _funcNamesByIndex.Clear();
            _funcArgsByIndex.Clear();
        }
        catch (Exception e)
        {
            _errorMessage = e.Message;
        }
        return Task.CompletedTask;
    }

    public async IAsyncEnumerable<string> Streaming(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (_errorMessage is not null)
        {
            yield break;
        }

        if (_streamingResponse is null)
        {
            throw new InvalidOperationException("Session not built");
        }

        var enumerator = _streamingResponse.GetAsyncEnumerator(cancellationToken);

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
                    _errorMessage = e.Message;
                    break;
                }

                if (!hasMore)
                {
                    break;
                }

                var chatUpdate = enumerator.Current;
                if (!string.IsNullOrEmpty(chatUpdate.SystemFingerprint) && !chatUpdate.SystemFingerprint.StartsWith("stopping_word"))
                {
                    SystemFingerprint = chatUpdate.SystemFingerprint;
                }

                foreach (var toolCallUpdate in chatUpdate.ToolCallUpdates)
                {
                    if (!string.IsNullOrEmpty(toolCallUpdate.ToolCallId))
                    {
                        _toolCallIdsByIndex[toolCallUpdate.Index] = toolCallUpdate.ToolCallId;
                    }

                    if (!string.IsNullOrEmpty(toolCallUpdate.FunctionName))
                    {
                        _funcNamesByIndex[toolCallUpdate.Index] = toolCallUpdate.FunctionName;
                    }

                    if (!toolCallUpdate.FunctionArgumentsUpdate.ToMemory().IsEmpty &&
                        !string.IsNullOrEmpty(toolCallUpdate.FunctionArgumentsUpdate.ToString()))
                    {
                        if (!_funcArgsByIndex.TryGetValue(toolCallUpdate.Index, out var arg))
                        {
                            arg = new StringBuilder();
                            _funcArgsByIndex[toolCallUpdate.Index] = arg;
                        }

                        arg.Append(toolCallUpdate.FunctionArgumentsUpdate);
                    }
                }

                var dynChatUpdate = chatUpdate.AsDynamic();
                if (dynChatUpdate.Choices.Count > 0)
                {
                    IDictionary<string, BinaryData>
                        rawData = dynChatUpdate.Choices[0].Delta.SerializedAdditionalRawData;
                    if (rawData.TryGetValue("reasoning_content", out var reasoningContent))
                    {
                        var reasoningString = reasoningContent.ToString();
                        if (!string.IsNullOrEmpty(reasoningString) && reasoningString != "null")
                        {
                            reasoningString = JsonConvert.DeserializeObject<string>(reasoningString);
                            if (!string.IsNullOrEmpty(reasoningString))
                            {
                                if (!_lastResponseIsReasoning)
                                {
                                    yield return "思考……\n\n";
                                }
                                _lastResponseIsReasoning = true;
                                _reasoningSb.Append(reasoningString);
                                yield return reasoningString;
                            }
                        }
                    }
                }

                foreach (var part in chatUpdate.ContentUpdate)
                {
                    if (_lastResponseIsReasoning)
                    {
                        _lastResponseIsReasoning = false;
                        yield return "\n================\n\n";
                    }
                    _responseSb.Append(part.Text);
                    yield return part.Text;
                }

                if (!string.IsNullOrEmpty(chatUpdate.SystemFingerprint) && chatUpdate.SystemFingerprint.StartsWith("stopping_word"))
                {
                    _responseSb.Append(chatUpdate.SystemFingerprint["stopping_word ".Length..]);
                }

                if (chatUpdate.Usage is not null)
                {
                    _usageInputToken = chatUpdate.Usage.InputTokenCount;
                    _usageOutputToken = chatUpdate.Usage.OutputTokenCount;
                }
            }
        }
        finally
        {
            await enumerator.DisposeAsync().ConfigureAwait(false);
        }
    }

    public AssistantMessage ResponseMessage
    {
        get
        {
            if (!string.IsNullOrEmpty(_errorMessage))
            {
                _responseSb.AppendLine();
                _responseSb.AppendLine(_errorMessage);
            }

            var resoningContent = _reasoningSb.ToString();
            var responseContent = _responseSb.ToString();
            if (string.IsNullOrEmpty(resoningContent) && 
                responseContent.StartsWith("<think>") && 
                responseContent.Contains("</think>"))
            {
                // extract reasoning content from response content
                var thinkEndIndex = responseContent.IndexOf("</think>", StringComparison.InvariantCulture);
                resoningContent = responseContent["<think>".Length..thinkEndIndex].Trim().Trim('\r', '\n').Trim();
                responseContent = responseContent[(thinkEndIndex + "</think>".Length)..].Trim().Trim('\r', '\n').Trim();
            }

            var response = new AssistantMessage
            {
                Content =
                [
                    new IMessage.TextContent { Text = responseContent }
                ],
                ResoningContent = resoningContent,
                ToolCalls = ToolCalls.ToList(),
                Provider = _options.Service switch
                {
                    ServerEndpointOptions.ServiceType.OtherOpenAICompat => ModelInfo.ProviderEnum.OtherOpenAICompat,
                    ServerEndpointOptions.ServiceType.OpenAI => ModelInfo.ProviderEnum.OpenAI,
                    ServerEndpointOptions.ServiceType.DeepSeek => ModelInfo.ProviderEnum.DeepSeek,
                    ServerEndpointOptions.ServiceType.Claude => ModelInfo.ProviderEnum.Anthropic,
                    _ => ModelInfo.ProviderEnum.OpenAI
                },
                ServerInputTokenNum = _usageInputToken,
                ServerOutputTokenNum = _usageOutputToken
            };
            return response;
        }
    }

    public IEnumerable<ToolCallType> ToolCalls
    {
        get
        {
            foreach (var (index, id) in _toolCallIdsByIndex)
            {
                var toolcall = new ToolCallType
                {
                    Id = id,
                    Function = new ToolCallType.FunctionType
                    {
                        Name = _funcNamesByIndex[index],
                        Arguments = _funcArgsByIndex[index].ToString()
                    }
                };
                yield return toolcall;
            }
        }
    }

    public string SystemFingerprint { get; private set; } = "";

    private bool _lastResponseIsReasoning;

    private int _usageInputToken;
    private int _usageOutputToken;
    private readonly ServerEndpointOptions _options;
    private readonly ChatClient _client;
    private AsyncCollectionResult<StreamingChatCompletionUpdate>? _streamingResponse;
    private readonly StringBuilder _responseSb = new();
    private readonly StringBuilder _reasoningSb = new();
    private string? _errorMessage;
    private readonly Dictionary<int, string> _toolCallIdsByIndex = [];
    private readonly Dictionary<int, string> _funcNamesByIndex = [];
    private readonly Dictionary<int, StringBuilder> _funcArgsByIndex = [];
}

public partial class ClaudeEndpoint : IServerEndpoint
{
    private const string ApiVersion = "2023-06-01";
    private const string ApiBeta = "tools-2024-05-16";

    public ClaudeEndpoint(ServerEndpointOptions o)
    {
        _options = o;

        if (_options.Service is not ServerEndpointOptions.ServiceType.Claude)
        {
            throw new ArgumentException("Invalid service type");
        }

        var httpClientHandler = new HttpClientHandler
        {
            AutomaticDecompression = System.Net.DecompressionMethods.All
        };
        _httpClient = new HttpClient(httpClientHandler);
        _httpClient.DefaultRequestHeaders.Add("x-api-key", _options.Key);
        _httpClient.DefaultRequestHeaders.Add("anthropic-version", ApiVersion);

        if (!string.IsNullOrWhiteSpace(ApiBeta))
        {
            _httpClient.DefaultRequestHeaders.Add("anthropic-beta", ApiBeta);
        }
    }

    private IEnumerable<Claude.Tool> GetToolDefinitions()
    {
        var tools =
            from tool in _options.Tools ?? []
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
                Model = _options.Model,
                Messages = NormalizeMessages(messages),
                MaxTokens = _options.MaxTokens ?? 4096,
                System = systemMessage,
                Temperature = _options.Temperature,
                Tools = tools.Count != 0 ? tools : null,
                TopP = _options.TopP,
                Stream = true,
                Metadata = _options.UserId is null ? null : new Claude.Metadata { UserId = _options.UserId },
                StopSequences = _options.StopSequences
            };

            var postStr = createMsg.ToJson();
            var postContent = new StringContent(postStr, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(Url.Combine(_options.Endpoint, "messages")),
                Content = postContent
            };
            _httpResponse =
                await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            _errorMessage = null;

            _indexToContentBlockStart.Clear();
            _indexToContentBlockDeltas.Clear();
            _inputTokens = 0;
            _outputTokens = 0;
        }
        catch (Exception e)
        {
            _errorMessage = e.Message;
        } 
    }

    public async IAsyncEnumerable<string> Streaming(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (_errorMessage is not null)
        {
            yield break;
        }

        if (_httpResponse is null)
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
            await _httpResponse.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        if (!_httpResponse.IsSuccessStatusCode)
        {
            try
            {
                using var errorReader = new StreamReader(responseStream);
                var responseStr = await errorReader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
                var error = JsonConvert.DeserializeObject<Claude.ErrorResponse>(responseStr, settings)
                            ?? throw new JsonSerializationException(responseStr);
                _errorMessage = $"{error.Error.Type}: {error.Error.Message}";
            }
            catch (Exception e)
            {
                _errorMessage = e.Message;
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
                _errorMessage = $"Error: Invalid chat response: {exception.Message}";
                yield break;
            }

            if (responseChunk is Claude.StreamingMessageStart msgStart)
            {
                _inputTokens += msgStart.Message.Usage.InputTokens;
                _outputTokens += msgStart.Message.Usage.OutputTokens;
            }
            else if (responseChunk is Claude.StreamingContentBlockStart contentStart)
            {
                _indexToContentBlockStart[contentStart.Index] = contentStart;
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
                if (!_indexToContentBlockDeltas.TryGetValue(contentDelta.Index, out var deltaList))
                { 
                    deltaList = [];
                    _indexToContentBlockDeltas[contentDelta.Index] = deltaList;
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
            else if (responseChunk is Claude.StreamingMessageDelta msgDelta)
            {
                _inputTokens += msgDelta.Usage.InputTokens;
                _outputTokens += msgDelta.Usage.OutputTokens;
            }
            else if (responseChunk is Claude.StreamingMessageStop)
            {
                // do nothing
            }
            else if (responseChunk is Claude.StreamingError error)
            {
                _errorMessage = $"{error.Error.Type}: {error.Error.Message}";
                yield break;
            }
            else
            {
                _errorMessage = $"Error: Invalid chat response: {chatResponse}";
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

            var contentIndex = _indexToContentBlockStart.Keys.ToList();
            contentIndex.Sort();
            foreach (var index in contentIndex)
            {
                msgBuilder.Clear();
                var start = _indexToContentBlockStart[index].ContentBlock;
                if (start is not Claude.StreamingContentBlockText textContentStart)
                {
                    continue;
                }

                msgBuilder.Append(textContentStart.Text);
                foreach (var delta in _indexToContentBlockDeltas[index])
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

            if (_errorMessage is not null)
            {
                assistantTextContent.Add(new IMessage.TextContent { Text = _errorMessage });
            }

            var response = new AssistantMessage
            {
                Content = assistantTextContent,
                ToolCalls = ToolCalls.ToList(),
                Provider = ModelInfo.ProviderEnum.Anthropic,
                ServerInputTokenNum = _inputTokens,
                ServerOutputTokenNum = _outputTokens
            };
            return response;
        }
    }
    public IEnumerable<ToolCallType> ToolCalls
    {
        get
        {
            StringBuilder argsBuilder = new();

            var contentIndex = _indexToContentBlockStart.Keys.ToList();
            contentIndex.Sort();
            foreach (var index in contentIndex)
            {
                argsBuilder.Clear();
                var start = _indexToContentBlockStart[index].ContentBlock;
                if (start is not Claude.StreamingContentBlockToolUse toolUseStart)
                {
                    continue;
                }
                foreach (var delta in _indexToContentBlockDeltas[index])
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

    private int _inputTokens;
    private int _outputTokens;
    private readonly HttpClient _httpClient;
    private string? _errorMessage;
    private HttpResponseMessage? _httpResponse;
    private readonly ServerEndpointOptions _options;
    private readonly Dictionary<int, Claude.StreamingContentBlockStart> _indexToContentBlockStart = [];
    private readonly Dictionary<int, List<Claude.StreamingContentBlockDelta>> _indexToContentBlockDeltas = [];
}
