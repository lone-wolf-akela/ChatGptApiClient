using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.AI.OpenAI;

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
    public ServiceType Service { get; init; }
    public string Endpoint { get; init; } = "";
    public string Key { get; init; } = "";
    public string AzureKey { get; init; } = "";
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

    public async Task BuildSession(ChatCompletionRequest session)
    {
        try
        {
            var chatCompletionsOptions = new ChatCompletionsOptions
            {
                DeploymentName = session.Model,
                MaxTokens = (int?)session.MaxTokens,
                PresencePenalty = (float)session.PresencePenalty,
                Seed = session.Seed,
                Temperature = (float)session.Temperature,
                NucleusSamplingFactor = (float)session.TopP
            };

            foreach(var tool in session.Tools ?? [])
            {
                var toolDef = new ChatCompletionsFunctionToolDefinition
                {
                    Name = tool.Function.Name,
                    Description = tool.Function.Description,
                    Parameters = BinaryData.FromString(tool.Function.Parameters.ToJson())
                };
                chatCompletionsOptions.Tools.Add(toolDef);
            }

            foreach(var msg in session.Messages)
            {
                var contents = msg.Content.ToList();
                ChatRequestMessage message;

                if (msg.Role is RoleType.User)
                {
                    contents.AddRange(((UserMessage)msg).GenerateAttachmentContentList());
                    var contentLst = new List<ChatMessageContentItem>();
                    foreach(var content in contents)
                    {
                        if (content.Type is IMessage.ContentCategory.Text)
                        {
                            contentLst.Add(new ChatMessageTextContentItem(((IMessage.TextContent)content).Text));
                        }
                        else if (content.Type is IMessage.ContentCategory.ImageUrl)
                        {
                            var imageContent = (IMessage.ImageContent)content;
                            var uri = new Uri("data:image/png;base64,example");

                            /************************************************************/
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
                            uriField.SetValue(uri, imageContent.ImageUrl.Url);

                            var uriInfoField = uri.GetType().GetField("_info", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                                ?? throw new InvalidOperationException("Could not find _info field");

                            var uriInfo = uriInfoField.GetValue(uri);
                            var uriInfoStringField = (uriInfo?.GetType().GetField("String", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)) 
                                ?? throw new InvalidOperationException("Could not find String field in _info");
                            uriInfoStringField.SetValue(uriInfo, imageContent.ImageUrl.Url);
                            
                            var uriInfoMoreInfoField = (uriInfo?.GetType().GetField("_moreInfo", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)) 
                                ?? throw new InvalidOperationException("Could not find _moreInfo field in _info");
                            var uriInfoMoreInfo = uriInfoMoreInfoField.GetValue(uriInfo);
                            
                            var uriInfoMoreInfoAbsoluteUriField = (uriInfoMoreInfo?.GetType().GetField("AbsoluteUri", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)) 
                                ?? throw new InvalidOperationException("Could not find AbsoluteUri field in MoreInfo");
                            uriInfoMoreInfoAbsoluteUriField.SetValue(uriInfoMoreInfo, imageContent.ImageUrl.Url);

                            var uriInfoMoreInfoPathField = (uriInfoMoreInfo?.GetType().GetField("Path", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)) 
                                ?? throw new InvalidOperationException("Could not find Path field in MoreInfo");
                            uriInfoMoreInfoPathField.SetValue(uriInfoMoreInfo, imageContent.ImageUrl.Url["data:".Length..]);
                            /************************************************************/

                            var detailLevel = 
                                imageContent.ImageUrl.Detail == IMessage.ImageContent.ImageUrlType.ImageDetail.Low 
                                ? ChatMessageImageDetailLevel.Low 
                                : ChatMessageImageDetailLevel.High;
                            contentLst.Add(new ChatMessageImageContentItem(uri, detailLevel));
                        }
                        else
                        {
                            throw new ArgumentException("Invalid content type");
                        }
                    }
                    var userMsg = new ChatRequestUserMessage(contentLst);
                    if (msg.Name is not null)
                    {
                        userMsg.Name = msg.Name;
                    }
                    message = userMsg;
                }
                else
                {
                    foreach(var content in contents)
                    {
                        if (content.Type is not IMessage.ContentCategory.Text)
                        {
                            throw new ArgumentException("Non user message content must be text");
                        }
                    }
                    var textContents = from content in contents select ((IMessage.TextContent)content).Text;
                    var combinedContent = string.Join("\n\n", textContents);
                    if (msg.Role == RoleType.System)
                    {
                        message = new ChatRequestSystemMessage(combinedContent);
                    }
                    else if (msg.Role == RoleType.Assistant)
                    {
                        var assistantMsg = new ChatRequestAssistantMessage(combinedContent);
                        foreach(var toolCall in ((AssistantMessage)msg).ToolCalls?? [])
                        {
                            var convertedToolCall = new ChatCompletionsFunctionToolCall(
                                toolCall.Id, toolCall.Function.Name, toolCall.Function.Arguments);
                            assistantMsg.ToolCalls.Add(convertedToolCall);
                        }
                        message = assistantMsg;
                    }
                    else if (msg.Role == RoleType.Tool)
                    {
                        message = new ChatRequestToolMessage(combinedContent, ((ToolMessage)msg).ToolCallId);
                    }
                    else
                    {
                        throw new ArgumentException("Invalid role");
                    }
                }
                chatCompletionsOptions.Messages.Add(message);
            }

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
    private readonly Dictionary<int, string> toolCallIdsByIndex = new();
    private readonly Dictionary<int, string> funcNamesByIndex = new();
    private readonly Dictionary<int, StringBuilder> funcArgsByIndex = new();
}

public class ClaudeEndpoint : IServerEndpoint
{
    public ClaudeEndpoint(ServerEndpointOptions o)
    {
        options = o;
        throw new NotImplementedException();
    }
    private ServerEndpointOptions options;
    public Task BuildSession(ChatCompletionRequest session)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<string> Streaming()
    {
        throw new NotImplementedException();
    }

    public string SystemFingerprint { get; }
    public AssistantMessage ResponseMessage { get; }
    public IEnumerable<ToolCallType> ToolCalls { get; }
}
