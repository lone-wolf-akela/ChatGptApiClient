﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;
using NJsonSchema;
using Newtonsoft.Json.Serialization;

namespace ChatGptApiClientV2;

[JsonConverter(typeof(StringEnumConverter))]
public enum RoleType
{
    [EnumMember(Value = "system")]
    System,
    [EnumMember(Value = "user")]
    User,
    [EnumMember(Value = "assistant")]
    Assistant,
    [EnumMember(Value = "tool")]
    Tool
}
    
public class ToolCallType : ICloneable
{
    public class FunctionType : ICloneable
    {
        /// <summary>
        /// The name of the function to call.
        /// </summary>
        public string Name { get; set; } = "";
        /// <summary>
        /// The arguments to call the function with, as generated by the model in JSON format. 
        /// Note that the model does not always generate valid JSON, and may hallucinate 
        /// parameters not defined by your function schema. Validate the arguments in your code 
        /// before calling your function.
        /// </summary>
        public string Arguments { get; set; } = "";
        public object Clone() => new FunctionType
        {
            Name = Name,
            Arguments = Arguments
        };
    }
    public long Index { get; set; }
    /// <summary>
    /// The ID of the tool call.
    /// </summary>
    public string Id { get; set; } = "";
    /// <summary>
    /// The type of the tool. Currently, only `function` is supported.
    /// </summary>
    public string Type { get; set; } = "";
    public FunctionType Function { get; set; } = new();
    public object Clone() => new ToolCallType
    {
        Index = Index,
        Id = Id,
        Type = Type,
        Function = (FunctionType)Function.Clone()
    };
    public static List<ToolCallType> MergeList(IEnumerable<ToolCallType>? toolCallListA, IEnumerable<ToolCallType>? toolCallListB)
    {
        var mergedList = new List<ToolCallType>();
        mergedList.AddRange(toolCallListA ?? []);
        foreach(var toolcall in toolCallListB ?? [])
        {
            var existingToolcall = mergedList.FirstOrDefault(t => t.Index == toolcall.Index);
            if (existingToolcall is null)
            {
                mergedList.Add(toolcall);
            }
            else
            {
                existingToolcall.Id = string.Join(string.Empty, existingToolcall.Id, toolcall.Id);
                existingToolcall.Type = string.Join(string.Empty, existingToolcall.Type, toolcall.Type);
                existingToolcall.Function.Name = string.Join(string.Empty, existingToolcall.Function.Name, toolcall.Function.Name);
                existingToolcall.Function.Arguments = string.Join(string.Empty, existingToolcall.Function.Arguments, toolcall.Function.Arguments);
            }
        }
        return mergedList;
    }
}
public class ChatCompletionChunk
{
    public class ChoiceType
    {
        public class DeltaType
        {
            /// <summary>
            /// The contents of the chunk message.
            /// </summary>
            public string? Content { get; set; } = null;
            /// <summary>
            /// The role of the author of this message.
            /// </summary>
            public RoleType? Role { get; set; } = RoleType.System;
            /// <summary>
            /// list of called tools
            /// </summary>
            public List<ToolCallType> ToolCalls { get; set; } = [];
        }

        /// <summary>
        /// The reason the model stopped generating tokens. Can be one of: `stop`, `length`, `content_filter`, `tool_calls`.
        /// </summary>
        public string? FinishReason { get; set; } = null;
        /// <summary>
        /// The index of the choice in the list of choices.
        /// </summary>
        public long Index { get; set; } = 0;
        /// <summary>
        /// A chat completion delta generated by streamed model responses.
        /// </summary>
        public DeltaType Delta { get; set; } = new();
    }
    /// <summary>
    /// A unique identifier for the chat completion. Each chunk has the same ID.
    /// </summary>
    public string Id { get; set; } = "";
    /// <summary>
    /// The Unix timestamp (in seconds) of when the chat completion was created. Each chunk has the same timestamp.
    /// </summary>
    public long Created { get; set; } = 0;
    /// <summary>
    /// The model to generate the completion.
    /// </summary>
    public string Model { get; set; } = "";
    /// <summary>
    /// This fingerprint represents the backend configuration that the model runs with. 
    /// Can be used in conjunction with the `seed` request parameter to understand when 
    /// backend changes have been made that might impact determinism.
    /// </summary>
    public string SystemFingerprint { get; set; } = "";
    /// <summary>
    /// The object type, which is always chat.completion.chunk.
    /// </summary>
    public string Object { get; set; } = "";
    /// <summary>
    /// A list of chat completion choices. Can be more than one if `n` is greater than 1.
    /// </summary>
    public List<ChoiceType> Choices { get; set; } = [];
}

public class ChatCompletion
{
    public class ChoiceType
    {
        public class MessageType
        {
            /// <summary>
            /// The contents of the message.
            /// </summary>
            public string? Content { get; set; }
            /// <summary>
            /// The role of the author of this message.
            /// </summary>
            public RoleType Role { get; set; } = RoleType.System;
            /// <summary>
            /// The tool calls generated by the model, such as function calls.
            /// </summary>
            public List<ToolCallType>? ToolCalls { get; set; } = [];
        }
        /// <summary>
        /// The reason the model stopped generating tokens. Can be one of: `stop`, `length`, `content_filter`, `tool_calls`.
        /// </summary>
        public string? FinishReason { get; set; }
        /// <summary>
        /// The index of the choice in the list of choices.
        /// </summary>
        public long Index { get; set; }
        /// <summary>
        /// A chat completion message generated by the model.
        /// </summary>
        public MessageType Message { get; set; } = new();
    }
    public class UsageType
    {
        /// <summary>
        /// Number of tokens in the generated completion.
        /// </summary>
        public long CompletionTokens { get; init; } = 0;
        /// <summary>
        /// Number of tokens in the prompt.
        /// </summary>
        public long PromptTokens { get; init; } = 0;
        /// <summary>
        /// Total number of tokens used in the request (prompt + completion).
        /// </summary>
        public long TotalTokens { get; init; } = 0;
    }
    /// <summary>
    /// A unique identifier for the chat completion.
    /// </summary>
    public string Id { get; init; } = "";
    /// <summary>
    /// The Unix timestamp (in seconds) of when the chat completion was created.
    /// </summary>
    public long Created { get; init; }
    /// <summary>
    /// The model used for the chat completion.
    /// </summary>
    public string Model { get; set; } = "";
    /// <summary>
    /// This fingerprint represents the backend configuration that the model runs with.
    /// Can be used in conjunction with the `seed` request parameter to 
    /// understand when backend changes have been made that might impact determinism.
    /// </summary>
    public string SystemFingerprint { get; set; } = "";
    /// <summary>
    /// The object type, which is always `chat.completion`.
    /// </summary>
    public string Object { get; set; } = "";
    /// <summary>
    /// Usage statistics for the completion request.
    /// </summary>
    public UsageType? Usage { get; set; }
    /// <summary>
    /// A list of chat completion choices. Can be more than one if `n` is greater than 1.
    /// </summary>
    public List<ChoiceType> Choices { get; set; } = [];

    public static ChatCompletion FromChunks(IEnumerable<ChatCompletionChunk> chunks)
    {
        var completion = new ChatCompletion
        {
            Id = new Guid().ToString(),
            Created = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Object = "chat.completion",
            Usage = null,
            SystemFingerprint = "",
            Choices = []
        };

        // model, system_fingerprint, choices filled with chunks data
        foreach(var chunk in chunks)
        {
            completion.Model = chunk.Model;
            if (!string.IsNullOrEmpty(chunk.SystemFingerprint))
            {
                completion.SystemFingerprint = chunk.SystemFingerprint;
            }
            // merge choices with the same index
            foreach(var choice in chunk.Choices)
            {
                var existingChoice = completion.Choices.FirstOrDefault(c => c.Index == choice.Index);
                if (existingChoice is null)
                {
                    ChoiceType newChoice = new()
                    {
                        FinishReason = choice.FinishReason,
                        Index = choice.Index,
                        Message = new ChoiceType.MessageType
                        {
                            Content = choice.Delta.Content,
                            Role = choice.Delta.Role ?? RoleType.Assistant,
                            ToolCalls = choice.Delta.ToolCalls
                        }
                    };
                    completion.Choices.Add(newChoice);
                }
                else
                {
                    existingChoice.FinishReason = string.Join(string.Empty, existingChoice.FinishReason, choice.FinishReason);
                    existingChoice.Message.Content = string.Join(string.Empty, existingChoice.Message.Content, choice.Delta.Content);
                    if (choice.Delta.Role is not null)
                    {
                        existingChoice.Message.Role = choice.Delta.Role.Value;
                    }
                    existingChoice.Message.ToolCalls = ToolCallType.MergeList(existingChoice.Message.ToolCalls, choice.Delta.ToolCalls);
                }
            }
        }
        foreach(var choice in completion.Choices)
        {
            choice.Message.ToolCalls = choice.Message.ToolCalls?.Count > 0 ? choice.Message.ToolCalls : null;
        }
        return completion;
    }
}
public interface IMessage : ICloneable
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ContentCategory
    {
        [EnumMember(Value = "text")]
        Text,
        [EnumMember(Value = "image_url")]
        ImageUrl
    }
    public interface IContent : ICloneable
    {
        public ContentCategory Type { get; }
    }
    public class TextContent : IContent
    {
        public ContentCategory Type => ContentCategory.Text;
        public string Text { get; set; } = "";
        public object Clone() => new TextContent
        {
            Text = Text
        };
    }
    public class ImageContent : IContent
    {
        public class ImageUrlType
        {
            [JsonConverter(typeof(StringEnumConverter))]
            public enum ImageDetail
            {
                [EnumMember(Value = "low")]
                Low,
                [EnumMember(Value = "high")]
                High
            }
            public string Url { get; set; } = "";
            public ImageDetail Detail { get; set; } = ImageDetail.Low;
        }
        public ContentCategory Type => ContentCategory.ImageUrl;
        public ImageUrlType ImageUrl { get; set; } = new();
        public object Clone() => new ImageContent
        {
            ImageUrl = new ImageUrlType
            {
                Url = ImageUrl.Url,
                Detail = ImageUrl.Detail
            }
        };
    }
    /// <summary>
    /// The contents of the message.
    /// </summary>
    public IEnumerable<IContent> Content { get; set; }
    /// <summary>
    /// The role of the messages author
    /// </summary>
    public RoleType Role { get; }
    /// <summary>
    /// An optional name for the participant. Provides the model information to differentiate between participants of the same role.
    /// </summary>
    public string? Name { get; }
    [JsonIgnore]
    public bool IsSavingToDisk { set; }
    public bool Hidden { get; }
}
public class SystemMessage : IMessage
{
    public IEnumerable<IMessage.IContent> Content { get; set; } = new List<IMessage.TextContent>();
    public RoleType Role => RoleType.System;
    public string? Name { get; set; }
    public bool IsSavingToDisk { get; set; } = false;
    public bool Hidden => false;
    public bool ShouldSerializeHidden() => IsSavingToDisk;
    public object Clone() => new SystemMessage
    {
        Content = from c in Content select c.Clone() as IMessage.IContent,
        Name = Name
    };
}
public class UserMessage : IMessage
{
    public IEnumerable<IMessage.IContent> Content { get; set; } = new List<IMessage.IContent>();
    public RoleType Role => RoleType.User;
    public string? Name { get; set; }
    public bool IsSavingToDisk { get; set; } = false;
    public bool Hidden => false;
    public bool ShouldSerializeHidden() => IsSavingToDisk;
    public object Clone() => new UserMessage
    {
        Content = from c in Content select c.Clone() as IMessage.IContent,
        Name = Name
    };
}
public class AssistantMessage : IMessage
{
    public IEnumerable<IMessage.IContent> Content { get; set; } = new List<IMessage.TextContent>();
    public RoleType Role => RoleType.Assistant;
    public string? Name { get; set; }
    public List<ToolCallType>? ToolCalls { get; set; }
    public bool IsSavingToDisk { get; set; } = false;
    public bool Hidden => false;
    public bool ShouldSerializeHidden() => IsSavingToDisk;
    public object Clone() => new AssistantMessage
    {
        Content = from c in Content select c.Clone() as IMessage.IContent,
        Name = Name,
        ToolCalls = (from tc in ToolCalls select tc.Clone() as ToolCallType).ToList()
    };
}
public class ToolMessage : IMessage
{
    public IEnumerable<IMessage.IContent> Content { get; set; } = new List<IMessage.TextContent>();
    public RoleType Role => RoleType.Tool;
    public string? Name => null;
    public string ToolCallId { get; set; } = "";
    public class GeneratedImage
    {
        public string ImageBase64Url { get; set; } = "";
        public string Description { get; set; } = "";
    }
    public List<GeneratedImage> GeneratedImages { get; set; } = [];
    public bool ShouldSerializeGeneratedImages() => IsSavingToDisk;
    public bool IsSavingToDisk { get; set; } = false;
    public bool Hidden { get; set; }
    public bool ShouldSerializeHidden() => IsSavingToDisk;
    public object Clone() => new ToolMessage
    {
        Content = from c in Content select c.Clone() as IMessage.IContent,
        ToolCallId = ToolCallId,
        Hidden = Hidden,
        GeneratedImages = (from gi in GeneratedImages select new GeneratedImage
        {
            ImageBase64Url = gi.ImageBase64Url,
            Description = gi.Description
        }).ToList()
    };
}
public class ChatCompletionRequest
{
    public class ToolType
    {
        public class FunctionType
        {
            public string? Description { get; set; }
            public string Name { get; set; } = "";
            public JsonSchema Parameters { get; set; } = new();
        }
        public string Type => "function";
        public FunctionType Function { get; set; } = new();
    }
    public List<IMessage> Messages { get; set; } = [];
    public string Model { get; set; } = "";
    // frequency_penalty
    // logit_bias
    public long? MaxTokens { get; set; }
    // n -> 1
    public double PresencePenalty { get; set; } = 0.0;
    // response_format
    public long Seed { get; set; }
    // stop
    public bool Stream { get; set; } = true;
    public double Temperature { get; set; } = 1.0;
    public double TopP { get; set; } = 1.0;
    public List<ToolType>? Tools { get; set; }
    // tool_choice
    // user

    public string Save()
    {
        foreach(var msg in Messages)
        {
            msg.IsSavingToDisk = true;
        }
        var contractResolver = new DefaultContractResolver
        {
            NamingStrategy = new SnakeCaseNamingStrategy()
        };
        var settings = new JsonSerializerSettings
        {
            ContractResolver = contractResolver,
            Formatting = Formatting.Indented,
            StringEscapeHandling = StringEscapeHandling.Default,
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto
        };
        var result = JsonConvert.SerializeObject(this, settings);
        foreach (var msg in Messages)
        {
            msg.IsSavingToDisk = false;
        }
        return result;
    }
    public string GeneratePostRequest()
    {
        foreach (var msg in Messages)
        {
            msg.IsSavingToDisk = false;
        }
        var contractResolver = new DefaultContractResolver
        {
            NamingStrategy = new SnakeCaseNamingStrategy()
        };
        var settings = new JsonSerializerSettings
        {
            ContractResolver = contractResolver,
            Formatting = Formatting.None,
            StringEscapeHandling = StringEscapeHandling.Default,
            NullValueHandling = NullValueHandling.Ignore
        };
        var result = JsonConvert.SerializeObject(this, settings);
        return result;
    }
    public List<string> GetImageUrlList()
    {
        var imageList = new List<string>();
        foreach(var msg in Messages)
        {
            foreach(var content in msg.Content)
            {
                if (content is IMessage.ImageContent imgContent)
                {
                    imageList.Add(imgContent.ImageUrl.Url);
                }
            }
            if (msg is ToolMessage toolMsg)
            {
                var imgUrls = from img in toolMsg.GeneratedImages select img.ImageBase64Url;
                imageList.AddRange(imgUrls);
            }
        }
        return imageList;
    }
    public static ChatCompletionRequest BuildFromInitPrompts(IEnumerable<IMessage>? initPrompts, DateTime knowledgeCutoff)
    {
        var request = new ChatCompletionRequest();
        var messages = (from p in initPrompts select p.Clone() as IMessage).ToList();
        foreach (var msg in messages)
        {
            var contentList = msg.Content.ToList();
            foreach(var content in contentList)
            {
                if (content is not IMessage.TextContent textContent)
                {
                    continue;
                }
                var ci = CultureInfo.CurrentUICulture;

                var prompt = textContent.Text;
                prompt = prompt.Replace("{DateTime}", DateTime.Now.ToString("MMM dd yyy", CultureInfo.GetCultureInfo("en-US")));
                prompt = prompt.Replace("{Cutoff}", knowledgeCutoff.ToString("MMM yyyy", CultureInfo.GetCultureInfo("en-US")));
                prompt = prompt.Replace("{Language}", ci.DisplayName);
                textContent.Text = prompt;
            }
            msg.Content = contentList;
        }
        request.Messages = messages;

        return request;
    }
}