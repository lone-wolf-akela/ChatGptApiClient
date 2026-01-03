using HandyControl.Tools.Extension;
using HarmonyLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenAI;
using OpenAI.Chat;
using ReflectionMagic;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ChatGptApiClientV2.ServerEndpoint;

#pragma warning disable SCME0001 // this allow use Json Patch

// Some 3rd party API provider (i.e. Nvidia) will return "finish_reason" as an empty string "",
// and some others (i.e. Google) return "other" as the finish reason when the response is censored.
// These are not valid according to OpenAI API spec, and will cause an error with openai sdk,
// as it expects a null instead.
// we need to patch it here
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
    static void Prefix(ref JsonElement element, ref ModelReaderWriterOptions options)
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
                    (string.IsNullOrEmpty(finishReasonStr.GetString()) || finishReasonStr.GetString() == "other"))
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
            case ServerEndpointOptions.ServiceType.Google:
            case ServerEndpointOptions.ServiceType.Ali:
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
                ChatMessage a = new UserChatMessage(contentLst);
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
                EndUserId = _options.UserId
            };

            if (_options.Service != ServerEndpointOptions.ServiceType.Google)
            {
                chatCompletionsOptions.StoredOutputEnabled = false;
            }

            if (!_options.TemperatureSettingNotSupported)
            {
                chatCompletionsOptions.Temperature = _options.Temperature;
            }

            if (!_options.TopPSettingNotSupported)
            {
                chatCompletionsOptions.TopP = _options.TopP;
            }

            if (!_options.PenaltySettingNotSupported)
            {
                chatCompletionsOptions.PresencePenalty = _options.PresencePenalty;
            }

            chatCompletionsOptions.Tools.AddRange(GetToolDefinitions());
            if (_options.StopSequences is not null)
            {
                chatCompletionsOptions.StopSequences.AddRange(_options.StopSequences);
            }

            if (_options.EnableThinking)
            {
                chatCompletionsOptions.Patch.Set("$.enable_thinking"u8, true);
            }

            var messages = GetChatRequestMessages(session.Messages);

            if (_options.SystemPromptNotSupported)
            {
                messages = from m in messages where m is not SystemChatMessage select m;
            }
            
            if (!_options.StreamingNotSupported)
            {
                _streamingResponse = _client.CompleteChatStreamingAsync(messages, chatCompletionsOptions, cancellationToken);
                _nonStreamingResponse = null;
            }
            else
            {
                _nonStreamingResponse = _client.CompleteChatAsync(messages, chatCompletionsOptions, cancellationToken);
                _streamingResponse = null;
            }
            
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

        if (_streamingResponse is null && _nonStreamingResponse is null)
        {
            throw new InvalidOperationException("Session not built");
        }

        if (_streamingResponse is not null)
        {

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
                    if (!string.IsNullOrEmpty(chatUpdate.SystemFingerprint) &&
                        !chatUpdate.SystemFingerprint.StartsWith("stopping_word"))
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
                        var delta = dynChatUpdate.Choices[0].Delta;
                        ReadOnlyMemory<byte> reasoningContentSpan = delta._patch._rawJson.Value;
                        var resoningContentJson = Encoding.UTF8.GetString(reasoningContentSpan.Span);
                        var deltaObj = JObject.Parse(resoningContentJson);

                        if (deltaObj.TryGetValue("reasoning_content", out var reasoningContent))
                        {
                            var reasoningString = reasoningContent.ToString();
                            if (!string.IsNullOrEmpty(reasoningString) && reasoningString != "null")
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

                    if (!string.IsNullOrEmpty(chatUpdate.SystemFingerprint) &&
                        chatUpdate.SystemFingerprint.StartsWith("stopping_word"))
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

        if (_nonStreamingResponse is not null)
        {
            ChatCompletion response;
            try
            {
                response = await _nonStreamingResponse.ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _errorMessage = e.Message;
                yield break;
            }
            if (!string.IsNullOrEmpty(response.SystemFingerprint) &&
                        !response.SystemFingerprint.StartsWith("stopping_word"))
            {
                SystemFingerprint = response.SystemFingerprint;
            }

            foreach (var (index, toolCall) in response.ToolCalls.Index())
            {
         
                _toolCallIdsByIndex[index] = toolCall.Id;
                _funcNamesByIndex[index] = toolCall.FunctionName;
                _funcArgsByIndex[index] = new StringBuilder(toolCall.FunctionArguments.ToString());
            }

            foreach (var part in response.Content)
            {
                if (_lastResponseIsReasoning)
                {
                    _lastResponseIsReasoning = false;
                    yield return "\n================\n\n";
                }

                _responseSb.Append(part.Text);
                yield return part.Text;
            }

            if (!string.IsNullOrEmpty(response.SystemFingerprint) &&
                response.SystemFingerprint.StartsWith("stopping_word"))
            {
                _responseSb.Append(response.SystemFingerprint["stopping_word ".Length..]);
            }

            if (response.Usage is not null)
            {
                _usageInputToken = response.Usage.InputTokenCount;
                _usageOutputToken = response.Usage.OutputTokenCount;
            }
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

            var resoningContent = _reasoningSb.ToString().Trim();
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
                    ServerEndpointOptions.ServiceType.Google => ModelInfo.ProviderEnum.Google,
                    ServerEndpointOptions.ServiceType.Ali => ModelInfo.ProviderEnum.Qwen,
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
    private Task<ClientResult<ChatCompletion>>? _nonStreamingResponse;
    private readonly StringBuilder _responseSb = new();
    private readonly StringBuilder _reasoningSb = new();
    private string? _errorMessage;
    private readonly Dictionary<int, string> _toolCallIdsByIndex = [];
    private readonly Dictionary<int, string> _funcNamesByIndex = [];
    private readonly Dictionary<int, StringBuilder> _funcArgsByIndex = [];
}