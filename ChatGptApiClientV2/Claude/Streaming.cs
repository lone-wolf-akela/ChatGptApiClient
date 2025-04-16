using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;
using System;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global

namespace ChatGptApiClientV2.Claude;

[JsonConverter(typeof(StringEnumConverter))]
public enum StreamingResponseType
{
    [EnumMember(Value = "message_start")] 
    MessageStart,
    [EnumMember(Value = "content_block_start")]
    ContentBlockStart,
    [EnumMember(Value = "ping")] 
    Ping,
    [EnumMember(Value = "content_block_delta")]
    ContentBlockDelta,
    [EnumMember(Value = "content_block_stop")]
    ContentBlockStop,
    [EnumMember(Value = "message_delta")] 
    MessageDelta,
    [EnumMember(Value = "message_stop")] 
    MessageStop,
    [EnumMember(Value = "error")] 
    Error
}

public class StreamingResponseConverter : JsonConverter<IStreamingResponse>
{
    private bool _canWrite = true;
    private bool _canRead = true;
    public override bool CanWrite => _canWrite;
    public override bool CanRead => _canRead;

    public override void WriteJson(JsonWriter writer, IStreamingResponse? value, JsonSerializer serializer)
    {
        _canWrite = false;
        if (value is null)
        {
            writer.WriteNull();
        }
        else
        {
            switch (value.Type)
            {
                case StreamingResponseType.MessageStart:
                    serializer.Serialize(writer, (StreamingMessageStart)value);
                    break;
                case StreamingResponseType.ContentBlockStart:
                    serializer.Serialize(writer, (StreamingContentBlockStart)value);
                    break;
                case StreamingResponseType.Ping:
                    serializer.Serialize(writer, (StreamingPing)value);
                    break;
                case StreamingResponseType.ContentBlockDelta:
                    serializer.Serialize(writer, (StreamingContentBlockDelta)value);
                    break;
                case StreamingResponseType.ContentBlockStop:
                    serializer.Serialize(writer, (StreamingContentBlockStop)value);
                    break;
                case StreamingResponseType.MessageDelta:
                    serializer.Serialize(writer, (StreamingMessageDelta)value);
                    break;
                case StreamingResponseType.MessageStop:
                    serializer.Serialize(writer, (StreamingMessageStop)value);
                    break;
                case StreamingResponseType.Error:
                    serializer.Serialize(writer, (StreamingError)value);
                    break;
                default:
                    throw new JsonSerializationException();
            }
        }

        _canWrite = true;
    }

    public override IStreamingResponse ReadJson(JsonReader reader, Type objectType, IStreamingResponse? existingValue,
        bool hasExistingValue, JsonSerializer serializer)
    {
        _canRead = false;
        var jobj = JObject.Load(reader);
        var type = jobj["type"]?.ToObject<StreamingResponseType>();
        IStreamingResponse result = type switch
        {
            StreamingResponseType.MessageStart => jobj.ToObject<StreamingMessageStart>(serializer) ??
                                                  throw new JsonSerializationException(),
            StreamingResponseType.ContentBlockStart => jobj.ToObject<StreamingContentBlockStart>(serializer) ??
                                                       throw new JsonSerializationException(),
            StreamingResponseType.Ping => jobj.ToObject<StreamingPing>(serializer) ??
                                          throw new JsonSerializationException(),
            StreamingResponseType.ContentBlockDelta => jobj.ToObject<StreamingContentBlockDelta>(serializer) ??
                                                       throw new JsonSerializationException(),
            StreamingResponseType.ContentBlockStop => jobj.ToObject<StreamingContentBlockStop>(serializer) ??
                                                      throw new JsonSerializationException(),
            StreamingResponseType.MessageDelta => jobj.ToObject<StreamingMessageDelta>(serializer) ??
                                                  throw new JsonSerializationException(),
            StreamingResponseType.MessageStop => jobj.ToObject<StreamingMessageStop>(serializer) ??
                                                 throw new JsonSerializationException(),
            StreamingResponseType.Error => jobj.ToObject<StreamingError>(serializer) ??
                                           throw new JsonSerializationException(),
            _ => throw new JsonSerializationException()
        };
        _canRead = true;
        return result;
    }
}

[JsonConverter(typeof(StreamingResponseConverter))]
public interface IStreamingResponse
{
    StreamingResponseType Type { get; }
}

public class StreamingMessageStart : IStreamingResponse
{
    public StreamingResponseType Type => StreamingResponseType.MessageStart;
    public CreateMessageResponse Message { get; set; } = new();
}

[JsonConverter(typeof(StringEnumConverter))]
public enum StreamingContentBlockType
{
    [EnumMember(Value = "text")] Text,
    [EnumMember(Value = "tool_use")] ToolUse,
    [EnumMember(Value = "thinking")] Thinking
}

public class StreamingContentBlockConverter : JsonConverter<IStreamingContentBlock>
{
    private bool _canWrite = true;
    private bool _canRead = true;
    public override bool CanWrite => _canWrite;
    public override bool CanRead => _canRead;

    public override void WriteJson(JsonWriter writer, IStreamingContentBlock? value, JsonSerializer serializer)
    {
        _canWrite = false;
        if (value is null)
        {
            writer.WriteNull();
        }
        else
        {
            switch (value.Type)
            {
                case StreamingContentBlockType.Text:
                    serializer.Serialize(writer, (StreamingContentBlockText)value);
                    break;
                case StreamingContentBlockType.ToolUse:
                    serializer.Serialize(writer, (StreamingContentBlockToolUse)value);
                    break;
                case StreamingContentBlockType.Thinking:
                    serializer.Serialize(writer, (StreamingContentBlockThinking)value);
                    break;
                default:
                    throw new JsonSerializationException();
            }
        }

        _canWrite = true;
    }

    public override IStreamingContentBlock ReadJson(JsonReader reader, Type objectType,
        IStreamingContentBlock? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        _canRead = false;
        var jobj = JObject.Load(reader);
        var type = jobj["type"]?.ToObject<StreamingContentBlockType>();
        IStreamingContentBlock result = type switch
        {
            StreamingContentBlockType.Text => jobj.ToObject<StreamingContentBlockText>(serializer) ??
                                              throw new JsonSerializationException(),
            StreamingContentBlockType.ToolUse => jobj.ToObject<StreamingContentBlockToolUse>(serializer) ??
                                                 throw new JsonSerializationException(),
            StreamingContentBlockType.Thinking => jobj.ToObject<StreamingContentBlockThinking>(serializer) ??
                                                  throw new JsonSerializationException(),
            _ => throw new JsonSerializationException()
        };
        _canRead = true;
        return result;
    }
}

[JsonConverter(typeof(StreamingContentBlockConverter))]
public interface IStreamingContentBlock
{
    public StreamingContentBlockType Type { get; }
}

public class StreamingContentBlockText : IStreamingContentBlock
{
    public StreamingContentBlockType Type => StreamingContentBlockType.Text;
    public string Text { get; set; } = "";
}

public class StreamingContentBlockToolUse : IStreamingContentBlock
{
    public StreamingContentBlockType Type => StreamingContentBlockType.ToolUse;
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
}

public class StreamingContentBlockThinking : IStreamingContentBlock
{
    public StreamingContentBlockType Type => StreamingContentBlockType.Thinking;
    public string Thinking { get; set; } = "";
}

public class StreamingContentBlockStart : IStreamingResponse
{
    public StreamingResponseType Type => StreamingResponseType.ContentBlockStart;
    public int Index { get; set; }
    public IStreamingContentBlock ContentBlock { get; set; } = new StreamingContentBlockText();
}

[JsonConverter(typeof(StringEnumConverter))]
public enum StreamingContentBlockDeltaContentType
{
    [EnumMember(Value = "text_delta")] TextDelta,
    [EnumMember(Value = "input_json_delta")] InputJsonDelta,
    [EnumMember(Value = "thinking_delta")] ThinkingDelta,
    [EnumMember(Value = "signature_delta")] SignatureDelta
}

public class StreamingContentBlockDeltaContentConverter : JsonConverter<IStreamingContentBlockDeltaContent>
{
    private bool _canWrite = true;
    private bool _canRead = true;
    public override bool CanWrite => _canWrite;
    public override bool CanRead => _canRead;

    public override void WriteJson(JsonWriter writer, IStreamingContentBlockDeltaContent? value,
        JsonSerializer serializer)
    {
        _canWrite = false;
        if (value is null)
        {
            writer.WriteNull();
        }
        else
        {
            switch (value.Type)
            {
                case StreamingContentBlockDeltaContentType.TextDelta:
                    serializer.Serialize(writer, (StreamingContentBlockTextDelta)value);
                    break;
                case StreamingContentBlockDeltaContentType.InputJsonDelta:
                    serializer.Serialize(writer, (StreamingContentBlockInputJsonDelta)value);
                    break;
                case StreamingContentBlockDeltaContentType.ThinkingDelta:
                    serializer.Serialize(writer, (StreamingContentBlockThinkingDelta)value);
                    break;
                case StreamingContentBlockDeltaContentType.SignatureDelta:
                    serializer.Serialize(writer, (StreamingContentBlockSignatureDelta)value);
                    break;
                default:
                    throw new JsonSerializationException();
            }
        }

        _canWrite = true;
    }

    public override IStreamingContentBlockDeltaContent ReadJson(JsonReader reader, Type objectType,
        IStreamingContentBlockDeltaContent? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        _canRead = false;
        var jobj = JObject.Load(reader);
        var type = jobj["type"]?.ToObject<StreamingContentBlockDeltaContentType>();
        IStreamingContentBlockDeltaContent result = type switch
        {
            StreamingContentBlockDeltaContentType.TextDelta =>
                jobj.ToObject<StreamingContentBlockTextDelta>(serializer) ?? throw new JsonSerializationException(),
            StreamingContentBlockDeltaContentType.InputJsonDelta =>
                jobj.ToObject<StreamingContentBlockInputJsonDelta>(serializer) ?? throw new JsonSerializationException(),
            StreamingContentBlockDeltaContentType.ThinkingDelta =>
                jobj.ToObject<StreamingContentBlockThinkingDelta>(serializer) ?? throw new JsonSerializationException(),
            StreamingContentBlockDeltaContentType.SignatureDelta =>
                jobj.ToObject<StreamingContentBlockSignatureDelta>(serializer) ?? throw new JsonSerializationException(),
            _ => throw new JsonSerializationException()
        };
        _canRead = true;
        return result;
    }
}

[JsonConverter(typeof(StreamingContentBlockDeltaContentConverter))]
public interface IStreamingContentBlockDeltaContent
{
    public StreamingContentBlockDeltaContentType Type { get; }
}

public class StreamingContentBlockTextDelta : IStreamingContentBlockDeltaContent
{
    public StreamingContentBlockDeltaContentType Type => StreamingContentBlockDeltaContentType.TextDelta;
    public string Text { get; set; } = "";
}

public class StreamingContentBlockInputJsonDelta : IStreamingContentBlockDeltaContent
{
    public StreamingContentBlockDeltaContentType Type => StreamingContentBlockDeltaContentType.InputJsonDelta;
    public string PartialJson { get; set; } = "";
}

public class StreamingContentBlockThinkingDelta : IStreamingContentBlockDeltaContent
{
    public StreamingContentBlockDeltaContentType Type => StreamingContentBlockDeltaContentType.ThinkingDelta;
    public string Thinking { get; set; } = "";
}

public class StreamingContentBlockSignatureDelta : IStreamingContentBlockDeltaContent
{
    public StreamingContentBlockDeltaContentType Type => StreamingContentBlockDeltaContentType.SignatureDelta;
    public string Signature { get; set; } = "";
}

public class StreamingContentBlockDelta : IStreamingResponse
{
    public StreamingResponseType Type => StreamingResponseType.ContentBlockDelta;
    public int Index { get; set; }
    public IStreamingContentBlockDeltaContent Delta { get; set; } = new StreamingContentBlockTextDelta();
}

public class StreamingContentBlockStop : IStreamingResponse
{
    public StreamingResponseType Type => StreamingResponseType.ContentBlockStop;
    public int Index { get; set; }
}

public class StreamingMessageDelta : IStreamingResponse
{
    public class DeltaContent
    {
        /// <summary>
        /// The reason that we stopped.
        /// </summary>
        public StopReason? StopReason { get; set; }

        /// <summary>
        /// Which custom stop sequence was generated, if any.
        /// </summary>
        public string? StopSequence { get; set; }
    }

    public StreamingResponseType Type => StreamingResponseType.MessageDelta;
    public DeltaContent Delta { get; set; } = new();
    public Usage Usage { get; set; } = new();
}

public class StreamingMessageStop : IStreamingResponse
{
    public StreamingResponseType Type => StreamingResponseType.MessageStop;
}

public class StreamingPing : IStreamingResponse
{
    public StreamingResponseType Type => StreamingResponseType.Ping;
}

public class StreamingError : IStreamingResponse
{
    public class ErrorType
    {
        public string Type { get; set; } = "";
        public string Message { get; set; } = "";
    }

    public StreamingResponseType Type => StreamingResponseType.Error;
    public ErrorType Error { get; set; } = new();
}