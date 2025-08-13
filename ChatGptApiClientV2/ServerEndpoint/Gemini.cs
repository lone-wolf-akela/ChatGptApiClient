using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Gemini;

namespace ChatGptApiClientV2.ServerEndpoint;

public class GeminiEndpoint : IServerEndpoint
{
    public GeminiEndpoint(ServerEndpointOptions o)
    {
        _options = o;

        // TODO
        var defaultTimeout = TimeSpan.FromMinutes(10);

        switch (_options.Service)
        {
            case ServerEndpointOptions.ServiceType.Google:
                {
                    var httpClientHandler = new SocketsHttpHandler
                    {
                        AutomaticDecompression = DecompressionMethods.All,
                    };

                    var httpClient = new HttpClient(httpClientHandler);
                    httpClient.Timeout = defaultTimeout;
                    httpClient.DefaultRequestHeaders.Add("x-goog-api-key", _options.Key);
                    httpClient.BaseAddress = new Uri(_options.Endpoint);
                    _client = new GeminiApi(httpClient, new Uri(_options.Endpoint));
                    break;
                }
            case ServerEndpointOptions.ServiceType.OpenAI:
            case ServerEndpointOptions.ServiceType.Claude:
            case ServerEndpointOptions.ServiceType.DeepSeek:
            case ServerEndpointOptions.ServiceType.OtherOpenAICompat:
            case ServerEndpointOptions.ServiceType.Custom:
            default:
                {
                    throw new InvalidOperationException("Invalid service type");
                }
        }

    }

    /*private IEnumerable<ChatTool> GetToolDefinitions()
    {
        var lst =
            from tool in _options.Tools ?? []
            select ChatTool.CreateFunctionTool(
                tool.Function.Name,
                tool.Function.Description,
                BinaryData.FromString(tool.Function.Parameters.ToJson())
                );
        return lst;
    }*/

    private static Content ChatDataMessageToGeminiContent(IMessage chatDataMsg)
    {
        var message = new Content();
        var contents = chatDataMsg.Content.ToList();

        var contentLst = new List<Part>();
        message.Parts = contentLst;

        if (chatDataMsg is UserMessage userMsg)
        {
            message.Role = "user";
            contents.AddRange(userMsg.GenerateAttachmentContentList());
            foreach (var content in contents)
            {
                if (content is IMessage.TextContent textContent)
                {
                    contentLst.Add(new Part { Text = textContent.Text });
                }
                else if (content is IMessage.ImageContent imageContent)
                {
                    /*var detailLevel =
                        imageContent.ImageUrl.Detail == IMessage.ImageContent.ImageUrlType.ImageDetail.Low
                            ? ChatImageDetailLevel.Low
                            : ChatImageDetailLevel.High;
                    var imageData = imageContent.ImageUrl.Url;
                    contentLst.Add(ChatMessageContentPart.CreateImagePart(imageData?.Data, imageData?.MimeType, detailLevel));*/
                    throw new NotImplementedException();
                }
                else
                {
                    throw new ArgumentException("Invalid content type");
                }
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
            message.Role = "user";
            contentLst.Add(new Part { Text = combinedContent });
        }

        if (chatDataMsg is AssistantMessage assistantMessage)
        {
            /*var chatRequestAssistantMsg = new AssistantChatMessage(combinedContent);
            var convertedToolCalls =
                from toolCall in assistantMessage.ToolCalls ?? []
                select ChatToolCall.CreateFunctionToolCall(
                    toolCall.Id,
                    toolCall.Function.Name,
                    BinaryData.FromString(toolCall.Function.Arguments)
                );
            chatRequestAssistantMsg.ToolCalls.AddRange(convertedToolCalls);
            return chatRequestAssistantMsg;*/
            message.Role = "model";
            contentLst.Add(new Part { Text = combinedContent });
        }

        if (chatDataMsg is ToolMessage toolMessage)
        {
            throw new NotImplementedException();
        }

        return message;
    }

    public Task BuildSession(ChatCompletionRequest session, CancellationToken cancellationToken = default)
    {
        try
        {
            var messages = new List<Content>();
            Content? systemMessage = null;
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
                        var systemMsgPart = new Part { Text = string.Join("\n\n", allText) };
                        systemMessage = new Content { Parts = [systemMsgPart] };
                        continue;
                    }
                }

                messages.Add(ChatDataMessageToGeminiContent(msg));
            }

            var chatCompletionsOptions = new GenerationConfig
            {
                MaxOutputTokens = _options.MaxTokens,
                // ThinkingConfig = new ThinkingConfig(true, null)
            };

            if (!_options.TemperatureSettingNotSupported)
            {
                chatCompletionsOptions.Temperature = _options.Temperature;
            }

            if (!_options.TopPSettingNotSupported)
            {
                chatCompletionsOptions.TopP = _options.TopP;
            }

            //todo: chatCompletionsOptions.Tools.AddRange(GetToolDefinitions());

            if (_options.StopSequences is not null)
            {
                chatCompletionsOptions.StopSequences = [.. _options.StopSequences];
            }

            /*if (_options.SystemPromptNotSupported)
            {
                messages = from m in messages where m is not SystemChatMessage select m;
            }*/

            List<SafetySetting> safetySettings = [
                new (HarmCategory.HATESPEECH, SafetySettingThreshold.BLOCKNONE),
                new (HarmCategory.SEXUALLYEXPLICIT, SafetySettingThreshold.BLOCKNONE),
                new (HarmCategory.DANGEROUSCONTENT, SafetySettingThreshold.BLOCKNONE),
                new (HarmCategory.HARASSMENT, SafetySettingThreshold.BLOCKNONE),
            ];

            _request = new GenerateContentRequest(
                _options.Model,
                messages,
                systemMessage,
                null,
                null,
                safetySettings,
                chatCompletionsOptions,
                null
                );

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

        if (_request is null)
        {
            throw new InvalidOperationException("Session not built");
        }

        var response = await _client.GenerateContentAsync(_options.Model, _request, cancellationToken)
            .ConfigureAwait(false);

        var usage = response?.UsageMetadata;

        var nOut = usage?.ThoughtsTokenCount + usage?.CandidatesTokenCount;
        var nIn = usage?.PromptTokenCount;

        if (nOut is not null)
        {
            _usageOutputToken = nOut.Value;
        }

        if (nIn is not null)
        {
            _usageInputToken = nIn.Value;
        }

        var parts = response?.Candidates?.FirstOrDefault()?.Content?.Parts ?? [];
        foreach (var part in parts)
        {
            if (part.Thought is true)
            {
                if (!_lastResponseIsReasoning)
                {
                    yield return "思考……\n\n";
                }
                _lastResponseIsReasoning = true;
                _reasoningSb.Append(part.Text);
            }
            else
            {
                if (_lastResponseIsReasoning)
                {
                    _lastResponseIsReasoning = false;
                    yield return "\n================\n\n";
                }
                _responseSb.Append(part.Text);
            }
            yield return part.Text ?? "";
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

            var resoningContent = _reasoningSb.ToString().TrimEnd();
            var responseContent = _responseSb.ToString().TrimEnd();

            var response = new AssistantMessage
            {
                Content =
                [
                    new IMessage.TextContent { Text = responseContent }
                ],
                ResoningContent = resoningContent,
                ToolCalls = ToolCalls.ToList(),
                Provider = ModelInfo.ProviderEnum.Google,
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
    private readonly GeminiApi _client;
    private GenerateContentRequest? _request;
    private readonly StringBuilder _responseSb = new();
    private readonly StringBuilder _reasoningSb = new();
    private string? _errorMessage;
    private readonly Dictionary<int, string> _toolCallIdsByIndex = [];
    private readonly Dictionary<int, string> _funcNamesByIndex = [];
    private readonly Dictionary<int, StringBuilder> _funcArgsByIndex = [];
}