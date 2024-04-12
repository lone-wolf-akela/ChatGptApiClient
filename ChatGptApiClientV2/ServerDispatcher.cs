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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Azure;
using Azure.AI.OpenAI;
using Flurl;
using HandyControl.Tools.Extension;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using static ChatGptApiClientV2.ChatCompletionRequest;
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace ChatGptApiClientV2;

public class ServerEndpointOptions
{
    public enum ServiceType
    {
        OpenAI,
        Azure,
        Claude,
        Custom
    };
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
}

public interface IServerEndpoint
{
    public static IServerEndpoint BuildServerEndpoint(ServerEndpointOptions options)
    {
        if (options.Service == ServerEndpointOptions.ServiceType.Claude)
        {
            return new ClaudeEndpoint(options);
        }
        else
        {
            return new OpenAIEndpoint(options);
        }
    }
    public Task BuildSession(ChatCompletionRequest session);
    public IAsyncEnumerable<string> Streaming();
    public string SystemFingerprint { get; }
    public AssistantMessage ResponseMessage { get; }
    public IEnumerable<ToolCallType> ToolCalls { get; }
}

public class OpenAIEndpoint : IServerEndpoint
{
    public OpenAIEndpoint(ServerEndpointOptions o)
    {
        options = o;

        if (options.Service is not (ServerEndpointOptions.ServiceType.OpenAI
            or ServerEndpointOptions.ServiceType.Azure
            or ServerEndpointOptions.ServiceType.Custom))
        {
            throw new ArgumentException("Invalid service type");
        }

        client = options.Service == ServerEndpointOptions.ServiceType.Azure
            ? new OpenAIClient(new Uri(options.Endpoint), new AzureKeyCredential(options.AzureKey))
            : new OpenAIClient(options.Key);
        if (options.Service == ServerEndpointOptions.ServiceType.Custom)
        {
            // use reflection to hijack the _endpoint (Uri Type) field, which is private
            var endpointField =
                client.GetType().GetField("_endpoint", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?? throw new InvalidOperationException("Could not find _endpoint field");
            endpointField.SetValue(client, new Uri(options.Endpoint));
        }
    }
    private IEnumerable<ChatCompletionsFunctionToolDefinition> GetToolDefinitions()
    {
        var lst =
            from tool in options.Tools ?? []
            select new ChatCompletionsFunctionToolDefinition
            {
                Name = tool.Function.Name,
                Description = tool.Function.Description,
                Parameters = BinaryData.FromString(tool.Function.Parameters.ToJson())
            };
        return lst;
    }
    private Uri ImageBase64StrToUri(string base64Uri)
    {
        if (!base64Uri.StartsWith("data:"))
        {
            throw new ArgumentException("Invalid base64 image uri");
        }

        var uri = new Uri("data:image/png;base64,example");

        // WARNING: VERY HACKY!
        // System.Uri does not want us to encode more than 65520 bytes in the URI
        // When we want to encode the whole image in the URI, it throws:
        // Invalid URI: The Uri string is too long.
        // we have to manually modify the internal fields of the Uri object to make it work

        // use reflection to force modify the fields below from uri:
        // _string
        // _info.String
        // _info._moreInfo.AbsoluteUri
        // _info._moreInfo.Path # this one need to remove the `data:` prefix

        // need this to make sure _info and other fields is not null
        Console.WriteLine(uri.ToString());
        Console.WriteLine(uri.AbsolutePath);

        var uriField = uri.GetType().GetField("_string", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?? throw new InvalidOperationException("Could not find _string field");
        uriField.SetValue(uri, base64Uri);

        var uriInfoField = uri.GetType().GetField("_info", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?? throw new InvalidOperationException("Could not find _info field");

        var uriInfo = uriInfoField.GetValue(uri);
        var uriInfoStringField = (uriInfo?.GetType().GetField("String", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
            ?? throw new InvalidOperationException("Could not find String field in _info");
        uriInfoStringField.SetValue(uriInfo, base64Uri);

        var uriInfoMoreInfoField = (uriInfo.GetType().GetField("_moreInfo", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance))
            ?? throw new InvalidOperationException("Could not find _moreInfo field in _info");
        var uriInfoMoreInfo = uriInfoMoreInfoField.GetValue(uriInfo);

        var uriInfoMoreInfoAbsoluteUriField = (uriInfoMoreInfo?.GetType().GetField("AbsoluteUri", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
            ?? throw new InvalidOperationException("Could not find AbsoluteUri field in MoreInfo");
        uriInfoMoreInfoAbsoluteUriField.SetValue(uriInfoMoreInfo, base64Uri);

        var uriInfoMoreInfoPathField = (uriInfoMoreInfo.GetType().GetField("Path", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
            ?? throw new InvalidOperationException("Could not find Path field in MoreInfo");
        uriInfoMoreInfoPathField.SetValue(uriInfoMoreInfo, base64Uri["data:".Length..]);

        return uri;
    }
    private ChatRequestMessage ChatDataMessageToChatRequestMessage(IMessage chatDataMsg)
    {
        var contents = chatDataMsg.Content.ToList();
        if (chatDataMsg is UserMessage userMsg)
        {
            contents.AddRange(userMsg.GenerateAttachmentContentList());
            var contentLst = new List<ChatMessageContentItem>();
            foreach (var content in contents)
            {
                if (content is IMessage.TextContent textContent)
                {
                    contentLst.Add(new ChatMessageTextContentItem(textContent.Text));
                }
                else if (content is IMessage.ImageContent imageContent)
                {
                    var detailLevel =
                        imageContent.ImageUrl.Detail == IMessage.ImageContent.ImageUrlType.ImageDetail.Low
                        ? ChatMessageImageDetailLevel.Low
                        : ChatMessageImageDetailLevel.High;
                    var uri = ImageBase64StrToUri(imageContent.ImageUrl.Url);
                    contentLst.Add(new ChatMessageImageContentItem(uri, detailLevel));
                }
                else
                {
                    throw new ArgumentException("Invalid content type");
                }
            }
            var chatRequestUserMessage = new ChatRequestUserMessage(contentLst);
            if (chatDataMsg.Name is not null)
            {
                chatRequestUserMessage.Name = chatDataMsg.Name;
            }
            return chatRequestUserMessage;
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
            return new ChatRequestSystemMessage(combinedContent);
        }

        if (chatDataMsg is AssistantMessage assistantMessage)
        {
            var chatRequestAssistantMsg = new ChatRequestAssistantMessage(combinedContent);
            var convertedToolCalls =
                from toolCall in assistantMessage.ToolCalls ?? []
                select new ChatCompletionsFunctionToolCall(
                    toolCall.Id,
                    toolCall.Function.Name,
                    toolCall.Function.Arguments
                    );
            chatRequestAssistantMsg.ToolCalls.AddRange(convertedToolCalls);
            return chatRequestAssistantMsg;
        }

        if (chatDataMsg is ToolMessage toolMessage)
        {
            return new ChatRequestToolMessage(combinedContent, toolMessage.ToolCallId);
        }

        throw new ArgumentException("Invalid role");
    }

    private IEnumerable<ChatRequestMessage> GetChatRequestMessages(IEnumerable<IMessage> msgLst)
    {
        var lst =
            from msg in msgLst
            select ChatDataMessageToChatRequestMessage(msg);
        return lst;
    }

    public async Task BuildSession(ChatCompletionRequest session)
    {
        try
        {
            var chatCompletionsOptions = new ChatCompletionsOptions
            {
                DeploymentName = options.Model,
                MaxTokens = options.MaxTokens,
                PresencePenalty = options.PresencePenalty,
                Seed = options.Seed,
                Temperature = options.Temperature,
                NucleusSamplingFactor = options.TopP
            };
            chatCompletionsOptions.Tools.AddRange(GetToolDefinitions());
            chatCompletionsOptions.Messages.AddRange(GetChatRequestMessages(session.Messages));

            streamingResponse = await client.GetChatCompletionsStreamingAsync(chatCompletionsOptions);
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
    }

    public async IAsyncEnumerable<string> Streaming()
    {
        if (errorMessage is not null)
        {
            yield break;
        }

        if (streamingResponse is null)
        {
            throw new InvalidOperationException("Session not built");
        }

        var enumerator = streamingResponse.EnumerateValues().GetAsyncEnumerator();

        try
        {
            while (true)
            {
                bool hasMore;

                try
                {
                    hasMore = await enumerator.MoveNextAsync();
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
                if (!string.IsNullOrEmpty(chatUpdate.SystemFingerprint))
                {
                    systemFingerprint = chatUpdate.SystemFingerprint;
                }

                if (chatUpdate.ToolCallUpdate is StreamingFunctionToolCallUpdate toolCallUpdate)
                {
                    if (!string.IsNullOrEmpty(toolCallUpdate.Id))
                    {
                        toolCallIdsByIndex[toolCallUpdate.ToolCallIndex] = toolCallUpdate.Id;
                    }

                    if (!string.IsNullOrEmpty(toolCallUpdate.Name))
                    {
                        funcNamesByIndex[toolCallUpdate.ToolCallIndex] = toolCallUpdate.Name;
                    }

                    if (!string.IsNullOrEmpty(toolCallUpdate.ArgumentsUpdate))
                    {
                        if (!funcArgsByIndex.TryGetValue(toolCallUpdate.ToolCallIndex, out var arg))
                        {
                            arg = new StringBuilder();
                            funcArgsByIndex[toolCallUpdate.ToolCallIndex] = arg;
                        }
                        arg.Append(toolCallUpdate.ArgumentsUpdate);
                    }
                }

                if (!string.IsNullOrEmpty(chatUpdate.ContentUpdate))
                {
                    responseSb.Append(chatUpdate.ContentUpdate);
                    yield return chatUpdate.ContentUpdate;
                }
            }
        }
        finally
        {
            await enumerator.DisposeAsync();
        }
    }

    public string SystemFingerprint => systemFingerprint;
    public AssistantMessage ResponseMessage
    {
        get
        {
            var response = new AssistantMessage
            {
                Content =
                [
                    new IMessage.TextContent { Text = errorMessage ?? responseSb.ToString() }
                ],
                ToolCalls = ToolCalls.ToList()
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
    private readonly OpenAIClient client;
    private StreamingResponse<StreamingChatCompletionsUpdate>? streamingResponse;
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
    private const string ApiBeta = "tools-2024-04-04";
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

    private Claude.Message ChatDataMessageToClaudeMessage(IMessage chatDataMsg)
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
                var url = imageContent.ImageUrl.Url;
                var mime = Utils.ExtractMimeFromUrl(url);
                var data = Utils.ExtractBase64FromUrl(url);
                var claudeImageContent = new Claude.ImageContent();
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
    private List<Claude.Message> NormalizeMessages(IEnumerable<Claude.Message> messages)
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

    public Task BuildSession(ChatCompletionRequest session)
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

            var createMsg = new Claude.CreateMessage
            {
                Model = options.Model,
                Messages = NormalizeMessages(messages),
                MaxTokens = options.MaxTokens ?? 4096,
                System = systemMessage,
                Temperature = options.Temperature,
                Tools = GetToolDefinitions(),
                TopP = options.TopP
            };

            var postStr = createMsg.ToJson();
            var postContent = new StringContent(postStr, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(Url.Combine(options.Endpoint, "messages")),
                Content = postContent
            };
            responseTask = httpClient.SendAsync(request);

            textResponseLst.Clear();
            toolUseResponseLst.Clear();
            errorMessage = null;
        }
        catch (Exception e)
        {
            errorMessage = e.Message;
        }
        return Task.CompletedTask;
    }

    public async IAsyncEnumerable<string> Streaming()
    {
        if (errorMessage is not null)
        {
            yield break;
        }

        if (responseTask is null)
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

        var response = await responseTask;
        await using var responseStream = await response.Content.ReadAsStreamAsync();
        using var reader = new StreamReader(responseStream);
        var responseStr = await reader.ReadToEndAsync();

        string? combinedTextResponse;
        try
        {
            if (!response.IsSuccessStatusCode)
            {

                var error = JsonConvert.DeserializeObject<Claude.ErrorResponse>(responseStr, settings)
                    ?? throw new JsonSerializationException(responseStr);
                errorMessage = $"{error.Error.Type}: {error.Error.Message}";
                yield break;
            }

            var responseData = JsonConvert.DeserializeObject<Claude.CreateMessageResponse>(responseStr, settings)
                ?? throw new JsonSerializationException($"Unable to parse response: {responseStr}");

            foreach (var content in responseData.Content)
            {
                if (content.Type == Claude.ContentCategory.Text)
                {
                    textResponseLst.Add((Claude.TextContent)content);
                }
                else if (content.Type == Claude.ContentCategory.ToolUse)
                {
                    toolUseResponseLst.Add((Claude.ToolUseContent)content);
                }
                else
                {
                    throw new InvalidOperationException($"Invalid content type: {content.Type}");
                }
            }
            var allTextResponse = from content in textResponseLst select content.Text;
            combinedTextResponse = string.Join("\n\n", allTextResponse);
        }
        catch (Exception e)
        {
            errorMessage = e.Message;
            yield break;
        }
        if (!string.IsNullOrEmpty(combinedTextResponse))
        {
            yield return combinedTextResponse;
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
            { ',', '，'},
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
            char lastChar = i > 0 ? input[i - 1] : '\0';
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
            if (errorMessage is not null)
            {
                assistantTextContent.Add(new IMessage.TextContent { Text = errorMessage });
            }
            else
            {
                foreach (var content in textResponseLst)
                {
                    assistantTextContent.Add(new IMessage.TextContent { Text = NormalizeChinesePunctuation(content.Text) });
                }
            }
            var response = new AssistantMessage
            {
                Content = assistantTextContent,
                ToolCalls = ToolCalls.ToList()
            };
            return response;
        }
    }
    public IEnumerable<ToolCallType> ToolCalls
    {
        get
        {
            foreach (var toolUse in toolUseResponseLst)
            {
                var toolCall = new ToolCallType
                {
                    Id = toolUse.Id,
                    Function = new ToolCallType.FunctionType
                    {
                        Name = toolUse.Name,
                        Arguments = toolUse.Input.ToString()
                    }
                };
                yield return toolCall;
            }
        }
    }

    private readonly HttpClient httpClient;
    private string? errorMessage;
    private Task<HttpResponseMessage>? responseTask;
    private readonly ServerEndpointOptions options;
    private readonly List<Claude.TextContent> textResponseLst = [];
    private readonly List<Claude.ToolUseContent> toolUseResponseLst = [];
}
