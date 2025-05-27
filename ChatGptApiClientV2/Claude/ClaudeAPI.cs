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
using System.Runtime.Serialization;
using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NJsonSchema;

// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace ChatGptApiClientV2.Claude;

// https://docs.anthropic.com/claude/reference

[JsonConverter(typeof(StringEnumConverter))]
public enum Role
{
    [EnumMember(Value = "user")] User,
    [EnumMember(Value = "assistant")] Assistant
}

[JsonConverter(typeof(StringEnumConverter))]
public enum ContentCategory
{
    [EnumMember(Value = "text")] Text,
    [EnumMember(Value = "image")] Image,
    [EnumMember(Value = "tool_use")] ToolUse,
    [EnumMember(Value = "tool_result")] ToolResult
}

public class ContentConverter : JsonConverter<IContent>
{
    private bool canWrite = true;
    private bool canRead = true;
    public override bool CanWrite => canWrite;
    public override bool CanRead => canRead;

    public override void WriteJson(JsonWriter writer, IContent? value, JsonSerializer serializer)
    {
        canWrite = false;
        if (value is null)
        {
            writer.WriteNull();
        }
        else
        {
            switch (value.Type)
            {
                case ContentCategory.Text:
                    serializer.Serialize(writer, (TextContent)value);
                    break;
                case ContentCategory.Image:
                    serializer.Serialize(writer, (ImageContent)value);
                    break;
                case ContentCategory.ToolUse:
                    serializer.Serialize(writer, (ToolUseContent)value);
                    break;
                case ContentCategory.ToolResult:
                    serializer.Serialize(writer, (ToolResultContent)value);
                    break;
                default:
                    throw new JsonSerializationException();
            }
        }

        canWrite = true;
    }

    public override IContent ReadJson(JsonReader reader, Type objectType, IContent? existingValue,
        bool hasExistingValue, JsonSerializer serializer)
    {
        canRead = false;
        var jobj = JObject.Load(reader);
        var type = jobj["type"]?.ToObject<ContentCategory>();
        IContent result = type switch
        {
            ContentCategory.Text => jobj.ToObject<TextContent>(serializer) ?? throw new JsonSerializationException(),
            ContentCategory.Image => jobj.ToObject<ImageContent>(serializer) ?? throw new JsonSerializationException(),
            ContentCategory.ToolUse => jobj.ToObject<ToolUseContent>(serializer) ??
                                       throw new JsonSerializationException(),
            ContentCategory.ToolResult => jobj.ToObject<ToolResultContent>(serializer) ??
                                          throw new JsonSerializationException(),
            _ => throw new JsonSerializationException()
        };
        canRead = true;
        return result;
    }
}

[JsonConverter(typeof(ContentConverter))]
public interface IContent
{
    public ContentCategory Type { get; }
}

public class TextContent : IContent
{
    public ContentCategory Type => ContentCategory.Text;
    public string Text { get; set; } = "";
}

public class ToolUseContent : IContent
{
    public ContentCategory Type => ContentCategory.ToolUse;
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public JObject Input { get; set; } = [];
}

public class ToolResultContent : IContent
{
    public ContentCategory Type => ContentCategory.ToolResult;
    public string ToolUseId { get; set; } = "";
    public string Content { get; set; } = "";
}

[JsonConverter(typeof(StringEnumConverter))]
public enum MediaType
{
    [EnumMember(Value = "image/jpeg")] Jpeg,
    [EnumMember(Value = "image/png")] Png,
    [EnumMember(Value = "image/gif")] Gif,
    [EnumMember(Value = "image/webp")] Webp
}

public class ImageContent : IContent
{
    public class SourceType
    {
        public string Type => "base64";
        public MediaType MediaType { get; set; }
        public string Data { get; set; } = "";

        public void SetMediaType(string mime)
        {
            MediaType = mime switch
            {
                "image/jpeg" => MediaType.Jpeg,
                "image/png" => MediaType.Png,
                "image/gif" => MediaType.Gif,
                "image/webp" => MediaType.Webp,
                _ => throw new ArgumentException("Invalid mime type")
            };
        }
    }

    public ContentCategory Type => ContentCategory.Image;
    public SourceType Source { get; set; } = new();
}

public class Message
{
    public Role Role { get; set; }
    public IEnumerable<IContent> Content { get; set; } = [];
}

public class Tool
{
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public JsonSchema InputSchema { get; set; } = new();
}

public class Metadata
{
    /// <summary>
    /// <para> An external identifier for the user who is associated with the request. </para>
    /// <para> This should be a uuid, hash value, or other opaque identifier. Anthropic may
    /// use this id to help detect abuse. Do not include any identifying information such as
    /// name, email address, or phone number. </para>
    /// </summary>
    public string UserId { get; set; } = "";
}

[JsonConverter(typeof(StringEnumConverter))]
public enum ThinkingType
{
    [EnumMember(Value = "enabled")] Enabled,
    [EnumMember(Value = "disabled")] Disabled
}

public class ExtendedThinking
{
    public ThinkingType Type { get; set; } = ThinkingType.Disabled;
    public int? BudgetTokens { get; set; }
}

/// <summary>
/// <para> Create a Message. </para>
/// <para> Send a structured list of input messages with text and/or image content, and 
/// the model will generate the next message in the conversation. </para>
/// <para> The Messages API can be used for either single queries or stateless 
/// multi-turn conversations. </para>
/// </summary>
public class CreateMessage
{
    /// <summary>
    /// <para> The model that will complete your prompt. </para>
    /// One of the followings: <br/>
    /// - claude-3-opus-20240229 <br/>
    /// - claude-3-sonnet-20240229 <br/>
    /// - claude-3-haiku-20240307 <br/>
    /// </summary>
    public string Model { get; set; } = "";

    /// <summary>
    /// <para> Input messages. </para>
    /// <para> Our models are trained to operate on alternating user and assistant conversational turns. 
    /// When creating a new Message, you specify the prior conversational turns with the messages parameter, 
    /// and the model then generates the next Message in the conversation. </para>
    /// </summary>
    public IEnumerable<Message> Messages { get; set; } = [];

    /// <summary>
    /// <para> The maximum number of tokens to generate before stopping. </para>
    /// Different models may have different maximum values for this parameter: <br/>
    /// - Currently all models are 4096 tokens max <br/>
    /// </summary>
    public int MaxTokens { get; set; } = 4096;

    public Metadata? Metadata { get; set; }

    /// <summary>
    /// <para> Custom text sequences that will cause the model to stop generating. </para>
    /// <para> Our models will normally stop when they have naturally completed their turn,
    /// which will result in a response <c>stop_reason</c> of <c>"end_turn"</c>. </para>
    /// <para> If you want the model to stop generating when it encounters custom strings of text,
    /// you can use the <c>stop_sequences</c> parameter. If the model encounters one of the custom sequences,
    /// the response <c>stop_reason</c> value will be <c>"stop_sequence"</c> and the response <c>stop_sequence</c>
    /// value will contain the matched stop sequence. </para>
    /// </summary>
    public IEnumerable<string>? StopSequences { get; set; }

    /// <summary>
    /// Whether to incrementally stream the response using server-sent events.
    /// </summary>
    public bool? Stream { get; set; }

    /// <summary>
    /// System prompt.
    /// </summary>
    public string? System { get; set; }

    /// <summary>
    /// <para> Amount of randomness injected into the response. </para>
    /// <para> Defaults to 1.0. Ranges from 0.0 to 1.0. Use temperature closer to 0.0 
    /// for analytical / multiple choice, and closer to 1.0 for creative and 
    /// generative tasks. </para>
    /// </summary>
    public float? Temperature
    {
        get;
        set => field = value.HasValue ? Math.Clamp(value.Value, 0.0f, 1.0f) : null;
    }

    /// <summary>
    /// <para> Configuration for enabling Claude's extended thinking. </para>
    /// <para> When enabled, responses include <c>thinking</c> content blocks showing Claude's
    /// thinking process before the final answer. Requires a minimum budget of 1,024 tokens and
    /// counts towards your <c>max_tokens</c> limit. </para>
    /// </summary>
    public ExtendedThinking? Thinking { get; set; }

    /// <summary>
    /// [beta] Definitions of tools that the model may use
    /// </summary>
    public IEnumerable<Tool>? Tools { get; set; }

    /// <summary>
    /// Use nucleus sampling.
    /// </summary>
    public float? TopP { get; set; }

    public string ToJson()
    {
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
}

[JsonConverter(typeof(StringEnumConverter))]
public enum StopReason
{
    /// <summary>
    /// the model reached a natural stopping point
    /// </summary>
    [EnumMember(Value = "end_turn")] EndTurn,

    /// <summary>
    /// we exceeded the requested max_tokens or the model's maximum
    /// </summary>
    [EnumMember(Value = "max_tokens")] MaxTokens,

    /// <summary>
    /// one of your provided custom stop_sequences was generated
    /// </summary>
    [EnumMember(Value = "stop_sequence")] StopSequence,

    /// <summary>
    /// (no description from official document)
    /// </summary>
    [EnumMember(Value = "tool_use")] ToolUse,

    /// <summary>
    /// The model declines to generate for safety reasons, due to the increased intelligence of Claude 4 models
    /// </summary>
    [EnumMember(Value = "refusal")] Refusal
}

public class CreateMessageResponse
{
    /// <summary>
    /// Unique object identifier.
    /// </summary>
    public string Id { get; set; } = "";

    /// <summary>
    /// <para> Object type. </para>
    /// <para> For Messages, this is always "message". </para>
    /// </summary>
    public string Type { get; set; } = "";

    /// <summary>
    /// <para> Conversational role of the generated message. </para>
    /// <para> This will always be "assistant". </para>
    /// </summary>
    public Role Role { get; set; }

    /// <summary>
    /// <para> Content generated by the model. </para>
    /// <para> "text" or "tool_use" </para>
    /// </summary>
    public IEnumerable<IContent> Content { get; set; } = [];

    /// <summary>
    /// The model that handled the request.
    /// </summary>
    public string Model { get; set; } = "";

    /// <summary>
    /// The reason that we stopped.
    /// </summary>
    public StopReason? StopReason { get; set; }

    /// <summary>
    /// Which custom stop sequence was generated, if any.
    /// </summary>
    public string? StopSequence { get; set; }

    /// <summary>
    /// Billing and rate-limit usage.
    /// </summary>
    public Usage Usage { get; set; } = new();
}

public class Usage
{
    /// <summary>
    /// The number of input tokens which were used.
    /// </summary>
    public int InputTokens { get; set; }

    /// <summary>
    /// The number of output tokens which were used.
    /// </summary>
    public int OutputTokens { get; set; }
}

public class ErrorResponse
{
    public class ErrorType
    {
        public string Type { get; set; } = "";
        public string Message { get; set; } = "";
    }

    public string Type { get; set; } = "";
    public ErrorType Error { get; set; } = new();
}