using ChatGptApiClientV2.Claude;
using Flurl;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ChatGptApiClientV2.ServerEndpoint;

public partial class ClaudeEndpoint : IServerEndpoint
{
    private const string ApiVersion = "2023-06-01";
    private const string ApiBeta = "tools-2024-05-16,interleaved-thinking-2025-05-14,output-128k-2025-02-19";

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

    private IEnumerable<Tool> GetToolDefinitions()
    {
        var tools =
            from tool in _options.Tools ?? []
            select new Tool
            {
                Name = tool.Function.Name,
                Description = tool.Function.Description,
                InputSchema = tool.Function.Parameters
            };
        return tools;
    }

    private static Message ChatDataMessageToClaudeMessage(IMessage chatDataMsg)
    {
        var contents = chatDataMsg.Content.ToList();
        var claudeContentList = new List<Claude.IContent>();
        if (chatDataMsg is UserMessage userMsg)
        {
            contents.AddRange(userMsg.GenerateAttachmentContentList());
        }

        if (chatDataMsg is AssistantMessage assistantMsg1)
        {
            if (assistantMsg1.ResoningContent is not null && assistantMsg1.ReasoningSignature is not null)
            {
                var thinkingContent = new ThinkingContent
                {
                    Thinking = assistantMsg1.ResoningContent,
                    Signature = assistantMsg1.ReasoningSignature
                };
                claudeContentList.Add(thinkingContent);
            }
        }

        foreach (var content in contents)
        {
            if (chatDataMsg is ToolMessage toolMsg)
            {
                var toolResultContent = new ToolResultContent
                {
                    ToolUseId = toolMsg.ToolCallId,
                    Content = ((IMessage.TextContent)content).Text
                };
                claudeContentList.Add(toolResultContent);
            }
            else if (content is IMessage.TextContent textContent)
            {
                claudeContentList.Add(new TextContent
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

        if (chatDataMsg is AssistantMessage assistantMsg2)
        {
            foreach (var toolCall in assistantMsg2.ToolCalls ?? [])
            {
                var toolUseContent = new ToolUseContent
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
            RoleType.User => Role.User,
            RoleType.Assistant => Role.Assistant,
            RoleType.System => Role.User,
            RoleType.Tool => Role.User,
            _ => throw new ArgumentException("Invalid role")
        };
        return new Message
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
            if (_options.MaxTokens is null)
            {
                throw new ArgumentException("Anthropic model must have max_tokens setting");
            }
            var createMsg = new CreateMessage
            {
                Model = _options.Model,
                Messages = NormalizeMessages(messages),
                MaxTokens = _options.MaxTokens.Value,
                System = systemMessage,
                Tools = tools.Count != 0 ? tools : null,
                Stream = true,
                Metadata = _options.UserId is null ? null : new Metadata { UserId = _options.UserId },
                StopSequences = _options.StopSequences
            };

            if (_options.EnableThinking)
            {
                createMsg.Thinking = new ExtendedThinking
                {
                    BudgetTokens = Math.Clamp(_options.ThinkingLength, 1024, _options.MaxTokens.Value - 1000),
                    Type = ThinkingType.Enabled
                };
                createMsg.Temperature = null;
                createMsg.TopP = null;
            }
            else
            {
                if (!_options.TemperatureSettingNotSupported)
                {
                    createMsg.Temperature = _options.Temperature;
                }

                if (!_options.TopPSettingNotSupported)
                {
                    createMsg.TopP = _options.TopP;
                }
            }

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

            _lastResponseIsReasoning = false;
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

            {
                string? thinking = null;
                if (responseChunk is Claude.StreamingContentBlockStart
                    {
                        ContentBlock: Claude.StreamingContentBlockThinking thinkingContent
                    })
                {
                    thinking = thinkingContent.Thinking;
                }
                else if (responseChunk is Claude.StreamingContentBlockDelta
                {
                    Delta: Claude.StreamingContentBlockThinkingDelta thinkingDelta
                })
                {
                    thinking = thinkingDelta.Thinking;
                }

                if (!string.IsNullOrEmpty(thinking))
                {
                    if (!_lastResponseIsReasoning)
                    {
                        yield return "思考……\n\n";
                    }

                    yield return thinking;
                    _lastResponseIsReasoning = true;
                }
            }

            {
                string? response = null;
                if (responseChunk is Claude.StreamingContentBlockStart
                    {
                        ContentBlock: Claude.StreamingContentBlockText textContent
                    })
                {
                    response = textContent.Text;
                }
                else if (responseChunk is Claude.StreamingContentBlockDelta
                {
                    Delta: Claude.StreamingContentBlockTextDelta textDelta
                })
                {
                    response = textDelta.Text;
                }

                if (!string.IsNullOrEmpty(response))
                {
                    if (_lastResponseIsReasoning)
                    {
                        yield return "\n================\n\n";
                    }

                    yield return response;
                    _lastResponseIsReasoning = false;
                }
            }

            if (responseChunk is Claude.StreamingMessageStart msgStart)
            {
                _inputTokens += msgStart.Message.Usage.InputTokens;
                _outputTokens += msgStart.Message.Usage.OutputTokens;
            }
            else if (responseChunk is Claude.StreamingContentBlockStart contentStart)
            {
                _indexToContentBlockStart[contentStart.Index] = contentStart;
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
            var reasoningContent = new List<string>();
            StringBuilder msgBuilder = new();
            string? reasoningSignature = null;

            var contentIndex = _indexToContentBlockStart.Keys.ToList();
            contentIndex.Sort();
            foreach (var index in contentIndex)
            {
                msgBuilder.Clear();
                var start = _indexToContentBlockStart[index].ContentBlock;
                if (start is Claude.StreamingContentBlockText textContentStart)
                {
                    msgBuilder.Append(textContentStart.Text);
                    foreach (var delta in _indexToContentBlockDeltas[index])
                    {
                        if (delta.Delta is not Claude.StreamingContentBlockTextDelta textContentDelta)
                        {
                            throw new InvalidOperationException("Invalid delta type");
                        }

                        msgBuilder.Append(textContentDelta.Text);
                    }

                    var text = msgBuilder.ToString();
                    if (_options.NeedChinesePunctuationNormalization)
                    {
                        text = NormalizeChinesePunctuation(text);
                    }
                    assistantTextContent.Add(new IMessage.TextContent { Text = text });
                }
                else if (start is StreamingContentBlockThinking thinkingContentStart)
                {
                    msgBuilder.Append(thinkingContentStart.Thinking);
                    foreach (var delta in _indexToContentBlockDeltas[index])
                    {
                        if (delta.Delta is StreamingContentBlockSignatureDelta signatureDelta)
                        {
                            reasoningSignature = signatureDelta.Signature;
                            continue;
                        }
                        if (delta.Delta is not StreamingContentBlockThinkingDelta thinkingContentDelta)
                        {
                            throw new InvalidOperationException("Invalid delta type");
                        }

                        msgBuilder.Append(thinkingContentDelta.Thinking);
                    }

                    var thinking = msgBuilder.ToString();
                    if (_options.NeedChinesePunctuationNormalization)
                    {
                        thinking = NormalizeChinesePunctuation(thinking);
                    }
                    reasoningContent.Add(thinking);
                }
            }

            if (_errorMessage is not null)
            {
                assistantTextContent.Add(new IMessage.TextContent { Text = _errorMessage });
            }

            var response = new AssistantMessage
            {
                Content = assistantTextContent,
                ResoningContent = string.Concat(reasoningContent),
                ReasoningSignature = reasoningSignature,
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

    private bool _lastResponseIsReasoning;

    private int _inputTokens;
    private int _outputTokens;
    private readonly HttpClient _httpClient;
    private string? _errorMessage;
    private HttpResponseMessage? _httpResponse;
    private readonly ServerEndpointOptions _options;
    private readonly Dictionary<int, Claude.StreamingContentBlockStart> _indexToContentBlockStart = [];
    private readonly Dictionary<int, List<Claude.StreamingContentBlockDelta>> _indexToContentBlockDeltas = [];
}
