using DocumentFormat.OpenXml.Vml;
using Google.GenAI;
using Google.GenAI.Types;
using System;
using System.Buffers.Text;
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
                    _client = new Client(apiKey: _options.Key, httpOptions: new HttpOptions
                    {
                        BaseUrl = _options.Endpoint,
                        Timeout = (int)defaultTimeout.TotalMilliseconds
                    });
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

    private Tool GetToolDefinitions()
    {
        var lst =
            from tool in _options.Tools ?? []
            select new FunctionDeclaration
            {
                Name = tool.Function.Name,
                Description = tool.Function.Description ?? "",
                ParametersJsonSchema = tool.Function.Parameters.ToJson()
            };

        return new Tool
        {
            FunctionDeclarations = lst.ToList()
        };
    }

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
                    var imageData = imageContent.ImageUrl.Url;
                    contentLst.Add(new Part
                    {
                        InlineData = new Blob
                        {
                            MimeType = imageData?.MimeType,
                            Data = imageData?.Data?.ToArray()
                        },
                        MediaResolution = imageContent.ImageUrl.Detail == IMessage.ImageContent.ImageUrlType.ImageDetail.Low
                            ? new PartMediaResolution { Level = PartMediaResolutionLevel.MEDIA_RESOLUTION_MEDIUM }
                            : new PartMediaResolution { Level = PartMediaResolutionLevel.MEDIA_RESOLUTION_HIGH }
                    });
                }
                else
                {
                    throw new ArgumentException("Invalid content type");
                }
            }
        }

        var textContents = from content in contents select ((IMessage.TextContent)content).Text;

        if (chatDataMsg is SystemMessage)
        {
            message.Role = "user";
            foreach (var content in contents)
            {
                if (content is IMessage.TextContent textContent)
                {
                    contentLst.Add(new Part { Text = textContent.Text });
                }
                else
                {
                    throw new ArgumentException("Invalid content type");
                }
            }
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
            if (assistantMessage.ReasoningSignature is not null)
            {
                contentLst.Add(new Part
                {
                    Thought = true, 
                    Text = assistantMessage.ResoningContent,
                    ThoughtSignature = Convert.FromBase64String(assistantMessage.ReasoningSignature)
                });
            }
            foreach (var content in contents)
            {
                if (content is IMessage.TextContent textContent)
                {
                    contentLst.Add(new Part { Text = textContent.Text });
                }
                else
                {
                    throw new ArgumentException("Invalid content type");
                }
            }
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

            var chatCompletionsOptions = new GenerateContentConfig
            {
                MaxOutputTokens = _options.MaxTokens,
                ThinkingConfig = new ThinkingConfig 
                { 
                    IncludeThoughts = _options.IncludeThoughts
                }
            };

            if (!_options.ThinkingLevelSettingNotSupported)
            {
                chatCompletionsOptions.ThinkingConfig.ThinkingLevel = _options.ThinkingLevel switch
                {
                    0 => ThinkingLevel.LOW,
                    1 => ThinkingLevel.HIGH,
                    _ => throw new ArgumentException("Invalid thinking level")
                };
            }

            if (!_options.TemperatureSettingNotSupported)
            {
                chatCompletionsOptions.Temperature = _options.Temperature;
            }

            if (!_options.TopPSettingNotSupported)
            {
                chatCompletionsOptions.TopP = _options.TopP;
            }

            if (_options.StopSequences is not null)
            {
                chatCompletionsOptions.StopSequences = [.. _options.StopSequences];
            }

            List<SafetySetting> safetySettings = [
                new SafetySetting { Category=HarmCategory.HARM_CATEGORY_HATE_SPEECH, Threshold=HarmBlockThreshold.BLOCK_NONE },
                new SafetySetting { Category=HarmCategory.HARM_CATEGORY_SEXUALLY_EXPLICIT, Threshold=HarmBlockThreshold.BLOCK_NONE },
                new SafetySetting { Category=HarmCategory.HARM_CATEGORY_DANGEROUS_CONTENT, Threshold=HarmBlockThreshold.BLOCK_NONE },
                new SafetySetting { Category=HarmCategory.HARM_CATEGORY_HARASSMENT, Threshold=HarmBlockThreshold.BLOCK_NONE },
                new SafetySetting { Category=HarmCategory.HARM_CATEGORY_CIVIC_INTEGRITY, Threshold=HarmBlockThreshold.BLOCK_NONE },
            ];

            chatCompletionsOptions.SystemInstruction = systemMessage;
            chatCompletionsOptions.Tools = [GetToolDefinitions()];
            chatCompletionsOptions.SafetySettings = safetySettings;

            _request = _client.Models.GenerateContentAsync(
                model: _options.Model,
                contents: messages,
                config: chatCompletionsOptions
                );

            _lastResponseIsReasoning = false;
            _response = null;
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

        try
        {
            _response = await _request.ConfigureAwait(false);
        }
        catch (Exception e)
        {
            _errorMessage = $"{e.Message}";
        }

        if (_errorMessage is not null)
        {
            yield break;
        }

        var usage = _response?.UsageMetadata;

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

        var parts = _response?.Candidates?.FirstOrDefault()?.Content?.Parts ?? [];
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
            var content = new List<IMessage.IContent>();
            
            var assistantMsg = new AssistantMessage
            {
                Content = content,
                ToolCalls = [.. ToolCalls],
                Provider = ModelInfo.ProviderEnum.Google,
                ServerInputTokenNum = _usageInputToken,
                ServerOutputTokenNum = _usageOutputToken
            };

            if (!string.IsNullOrEmpty(_errorMessage))
            {
                content.Add(new IMessage.TextContent { Text = _errorMessage });
                return assistantMsg;
            }

            if (_response is null)
            {
                throw new InvalidOperationException("No response received");
            }

            foreach (var part in _response.Candidates?.First()?.Content?.Parts ?? [])
            {
                if (part.ThoughtSignature is not null)
                {
                    assistantMsg.ReasoningSignature = Convert.ToBase64String(part.ThoughtSignature);
                }
                if (part.Thought is not true && part.Text is not null)
                {
                    content.Add(new IMessage.TextContent { Text = part.Text});
                }
            }

            if (_reasoningSb.Length != 0)
            {
                assistantMsg.ResoningContent = _reasoningSb.ToString().Trim();
            }

            return assistantMsg;
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
    private readonly Client _client;
    private Task<GenerateContentResponse>? _request;
    private GenerateContentResponse? _response;
    private readonly StringBuilder _responseSb = new();
    private readonly StringBuilder _reasoningSb = new();
    private string? _errorMessage;
    private readonly Dictionary<int, string> _toolCallIdsByIndex = [];
    private readonly Dictionary<int, string> _funcNamesByIndex = [];
    private readonly Dictionary<int, StringBuilder> _funcArgsByIndex = [];
}