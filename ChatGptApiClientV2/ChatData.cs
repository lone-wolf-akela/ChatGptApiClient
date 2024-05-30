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
using System.Globalization;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;
using NJsonSchema;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;

// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMemberInSuper.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace ChatGptApiClientV2;

public class ImageDataConverter : JsonConverter<ImageData>
{
    public override bool CanWrite => true;
    public override bool CanRead => true;

    public override void WriteJson(JsonWriter writer, ImageData? value, JsonSerializer serializer)
    {
        if (value is null)
        {
            writer.WriteNull();
            return;
        }
        serializer.Serialize(writer, value.ToUri());
    }

    public override ImageData? ReadJson(JsonReader reader, Type objectType, ImageData? existingValue,
        bool hasExistingValue, JsonSerializer serializer)
    {
        var uri = serializer.Deserialize<string>(reader);
        return uri is null ? null : ImageData.CreateFromUri(uri);
    }
}

[JsonConverter(typeof(ImageDataConverter))]
public partial class ImageData : ICloneable
{
    private ImageData(BinaryData data, string? mimeType, System.Windows.Size? size = null) 
    {
        Data = data;
        MimeType = mimeType;
        if (size is not null)
        {
            Size = size.Value;
        }
        else
        {
            var bitmap = ToBitmapImage();
            Size = new System.Windows.Size(bitmap.PixelWidth, bitmap.PixelHeight);
        }
    }
    public static async Task<ImageData> CreateFromFile(string file, CancellationToken cancellationToken = default)
    {
        var mime = MimeTypes.GetMimeType(file);
        if (!mime.StartsWith("image/"))
        {
            throw new ArgumentException("The file is not an image.", nameof(file));
        }
        var bytes = await File.ReadAllBytesAsync(file, cancellationToken);
        return new ImageData(new BinaryData(bytes), mime);
    }

    [GeneratedRegex(@"^data:(?<mime>[a-z\-\+\.]+\/[a-z\-\+\.]+);base64,(?<data>.+)$")]
    private static partial Regex Base64UrlExtract();
    public static ImageData CreateFromUri(string uri)
    {
        var r = Base64UrlExtract();
        var match = r.Match(uri);
        if (!match.Success)
        {
            // not a valid data URI, maybe passed in a raw base64 string by mistake
            return CreateFromBase64Str(uri);
        }
        else
        {
            var mime = match.Groups["mime"].Value;
            var data = match.Groups["data"].Value;
            return CreateFromBase64Str(data, mime);
        }
    }
    public static ImageData CreateFromBase64Str(string base64, string? mime = null)
    {
        var bytes = Convert.FromBase64String(base64);
        return new ImageData(new BinaryData(bytes), mime);
    }
    public ImageData ReSaveAsPng()
    {
        using var msIn = Data.ToStream();
        var imageSource = BitmapFrame.Create(msIn, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);

        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(imageSource));

        using var msOut = new MemoryStream();
        encoder.Save(msOut);
        msOut.Seek(0, SeekOrigin.Begin);

        return new ImageData(new BinaryData(msOut.ToArray()), "image/png");
    }

    public BitmapImage ToBitmapImage()
    {
        using var ms = Data.ToStream();
        var bitmapImage = new BitmapImage();
        bitmapImage.BeginInit();
        bitmapImage.StreamSource = ms;
        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapImage.EndInit();
        bitmapImage.Freeze();
        return bitmapImage;
    }

    public BinaryData Data { get; }
    public string? MimeType { get; }
    public System.Windows.Size Size { get; }
    public string ToUri()
    {
        return $"data:{MimeType ?? MimeTypes.FallbackMimeType};base64,{ToBase64()}";
    }
    
    public string ToBase64()
    {
        return Convert.ToBase64String(Data.ToArray());
    }

    public object Clone()
    {
        var clonedData = Data.ToArray();
        return new ImageData(new BinaryData(clonedData), MimeType, Size);
    }
}

[JsonConverter(typeof(StringEnumConverter))]
public enum RoleType
{
    [EnumMember(Value = "system")] System,
    [EnumMember(Value = "user")] User,
    [EnumMember(Value = "assistant")] Assistant,
    [EnumMember(Value = "tool")] Tool
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

        public object Clone()
        {
            return new FunctionType
            {
                Name = Name,
                Arguments = Arguments
            };
        }
    }

    /// <summary>
    /// The ID of the tool call.
    /// </summary>
    public string Id { get; set; } = "";

    public FunctionType Function { get; set; } = new();

    public object Clone()
    {
        return new ToolCallType
        {
            Id = Id,
            Function = (FunctionType)Function.Clone()
        };
    }
}

public class MessageConverter : JsonConverter<IMessage>
{
    private bool canWrite = true;
    private bool canRead = true;
    public override bool CanWrite => canWrite;
    public override bool CanRead => canRead;

    public override void WriteJson(JsonWriter writer, IMessage? value, JsonSerializer serializer)
    {
        canWrite = false;

        if (value is null)
        {
            writer.WriteNull();
        }
        else
        {
            switch (value.Role)
            {
                case RoleType.System:
                    serializer.Serialize(writer, (SystemMessage)value);
                    break;
                case RoleType.User:
                    serializer.Serialize(writer, (UserMessage)value);
                    break;
                case RoleType.Assistant:
                    serializer.Serialize(writer, (AssistantMessage)value);
                    break;
                case RoleType.Tool:
                    serializer.Serialize(writer, (ToolMessage)value);
                    break;
                default:
                    throw new JsonSerializationException();
            }
        }

        canWrite = true;
    }

    public override IMessage ReadJson(JsonReader reader, Type objectType, IMessage? existingValue,
        bool hasExistingValue, JsonSerializer serializer)
    {
        canRead = false;
        var jobj = JObject.Load(reader);
        var role = jobj["role"]?.ToObject<RoleType>();

        IMessage result = role switch
        {
            RoleType.System => jobj.ToObject<SystemMessage>(serializer) ?? throw new JsonSerializationException(),
            RoleType.User => jobj.ToObject<UserMessage>(serializer) ?? throw new JsonSerializationException(),
            RoleType.Assistant => jobj.ToObject<AssistantMessage>(serializer) ?? throw new JsonSerializationException(),
            RoleType.Tool => jobj.ToObject<ToolMessage>(serializer) ?? throw new JsonSerializationException(),
            _ => throw new JsonSerializationException()
        };
        canRead = true;
        return result;
    }
}

[JsonConverter(typeof(MessageConverter))]
public interface IMessage : ICloneable
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ContentCategory
    {
        [EnumMember(Value = "text")] Text,
        [EnumMember(Value = "image_url")] ImageUrl
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
                    case ContentCategory.ImageUrl:
                        serializer.Serialize(writer, (ImageContent)value);
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
                ContentCategory.Text => jobj.ToObject<TextContent>(serializer) ??
                                        throw new JsonSerializationException(),
                ContentCategory.ImageUrl => jobj.ToObject<ImageContent>(serializer) ??
                                            throw new JsonSerializationException(),
                _ => throw new JsonSerializationException()
            };
            canRead = true;
            return result;
        }
    }

    [JsonConverter(typeof(ContentConverter))]
    public interface IContent : ICloneable
    {
        public ContentCategory Type { get; }
        public int CountToken(ModelVersionInfo.TokenizerEnum tokenizer);
    }

    public class TextContent : IContent
    {
        public ContentCategory Type => ContentCategory.Text;

        public int CountToken(ModelVersionInfo.TokenizerEnum tokenizer)
        {
            return Utils.GetStringTokenCount(Text, tokenizer);
        }

        public string Text { get; set; } = "";

        public object Clone()
        {
            return new TextContent
            {
                Text = Text
            };
        }
    }

    public class ImageContent : IContent
    {
        public class ImageUrlType : ICloneable
        {
            [JsonConverter(typeof(StringEnumConverter))]
            public enum ImageDetail
            {
                [EnumMember(Value = "low")] Low,
                [EnumMember(Value = "high")] High
            }

            public ImageData? Url { get; set; }
            public ImageDetail Detail { get; set; } = ImageDetail.Low;

            public object Clone()
            {
                return new ImageUrlType
                {
                    Url = (ImageData?)Url?.Clone(),
                    Detail = Detail
                };
            }
        }

        public ContentCategory Type => ContentCategory.ImageUrl;

        public int CountToken(ModelVersionInfo.TokenizerEnum tokenizer)
        {
            // see https://platform.openai.com/docs/guides/vision
            const int baseTokenNum = 85;
            const int tokenPerTile = 170;
            const int tileSize = 512;

            var count = baseTokenNum;

            if (ImageUrl.Detail != ImageUrlType.ImageDetail.High)
            {
                return count;
            }

            var imageData = ImageUrl.Url;
            if (imageData is null)
            {
                return count;
            }

            var imageSize = imageData.Size;

            // if size is too large, scale down to fit in 2048x2048, keep aspect ratio
            if (imageSize.Width > 2048 || imageSize.Height > 2048)
            {
                var ratio = Math.Min(2048.0 / imageSize.Width, 2048.0 / imageSize.Height);
                imageSize = new System.Windows.Size(Math.Round(imageSize.Width * ratio),
                    Math.Round(imageSize.Height * ratio));
            }

            // further scale down to make the shortest side fit in 768px, keep aspect ratio
            var isWidthShorter = imageSize.Width < imageSize.Height;
            if (isWidthShorter && imageSize.Width > 768)
            {
                var ratio = 768.0 / imageSize.Width;
                imageSize = new System.Windows.Size(Math.Round(imageSize.Width * ratio),
                    Math.Round(imageSize.Height * ratio));
            }
            else if (!isWidthShorter && imageSize.Height > 768)
            {
                var ratio = 768.0 / imageSize.Height;
                imageSize = new System.Windows.Size(Math.Round(imageSize.Width * ratio),
                    Math.Round(imageSize.Height * ratio));
            }

            var tileCount = Math.Ceiling(imageSize.Width / tileSize) *
                          Math.Ceiling(imageSize.Height / tileSize);
            count += (int)tileCount * tokenPerTile;

            return count;
        }

        public ImageUrlType ImageUrl { get; set; } = new();

        public object Clone()
        {
            return new ImageContent
            {
                ImageUrl = (ImageUrlType)ImageUrl.Clone()
            };
        }
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

    public bool Hidden { get; }

    public int CountTokenBase(ModelVersionInfo.TokenizerEnum tokenizer)
    {
        var count = 3 + Content.Sum(c => c.CountToken(tokenizer));

        if (Name is not null)
        {
            count += 1 + Utils.GetStringTokenCount(Name, tokenizer);
        }

        return count;
    }

    public int CountToken(ModelVersionInfo.TokenizerEnum tokenizer);
    public DateTime DateTime { get; set; }
}

public class SystemMessage : IMessage
{
    public IEnumerable<IMessage.IContent> Content { get; set; } = new List<IMessage.TextContent>();
    public RoleType Role => RoleType.System;
    public string? Name { get; set; }
    public bool Hidden => false;
    public int CountToken(ModelVersionInfo.TokenizerEnum tokenizer) => ((IMessage)this).CountTokenBase(tokenizer);

    public object Clone()
    {
        return new SystemMessage
        {
            Content = (from c in Content select (IMessage.IContent)c.Clone()).ToList(),
            Name = Name,
            DateTime = DateTime
        };
    }
    public DateTime DateTime { get; set; } = DateTime.Now;
}

public class UserMessage : IMessage
{
    public class AttachmentInfoConverter : JsonConverter<IAttachmentInfo>
    {
        private bool canWrite = true;
        private bool canRead = true;
        public override bool CanWrite => canWrite;
        public override bool CanRead => canRead;

        public override void WriteJson(JsonWriter writer, IAttachmentInfo? value, JsonSerializer serializer)
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
                    case IAttachmentInfo.AttachmentType.Text:
                        serializer.Serialize(writer, (TextAttachmentInfo)value);
                        break;
                    case IAttachmentInfo.AttachmentType.Image:
                        serializer.Serialize(writer, (ImageAttachmentInfo)value);
                        break;
                    default:
                        throw new JsonSerializationException();
                }
            }

            canWrite = true;
        }

        public override IAttachmentInfo ReadJson(JsonReader reader, Type objectType, IAttachmentInfo? existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            canRead = false;
            var jobj = JObject.Load(reader);
            var type = jobj["type"]?.ToObject<IAttachmentInfo.AttachmentType>();
            IAttachmentInfo result = type switch
            {
                IAttachmentInfo.AttachmentType.Text => jobj.ToObject<TextAttachmentInfo>(serializer) ??
                                                       throw new JsonSerializationException(),
                IAttachmentInfo.AttachmentType.Image => jobj.ToObject<ImageAttachmentInfo>(serializer) ??
                                                        throw new JsonSerializationException(),
                _ => throw new JsonSerializationException()
            };
            canRead = true;
            return result;
        }
    }

    [JsonConverter(typeof(AttachmentInfoConverter))]
    public interface IAttachmentInfo : ICloneable
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public enum AttachmentType
        {
            [EnumMember(Value = "text")] Text,
            [EnumMember(Value = "image")] Image
        }

        public AttachmentType Type { get; }
        public string FileName { get; }
    }

    public class TextAttachmentInfo : IAttachmentInfo
    {
        public IAttachmentInfo.AttachmentType Type => IAttachmentInfo.AttachmentType.Text;
        public string FileName { get; set; } = "";
        public string Content { get; set; } = "";

        public object Clone()
        {
            return new TextAttachmentInfo
            {
                FileName = FileName,
                Content = Content
            };
        }
    }

    public class ImageAttachmentInfo : IAttachmentInfo
    {
        public IAttachmentInfo.AttachmentType Type => IAttachmentInfo.AttachmentType.Image;
        public string FileName { get; set; } = "";
        public ImageData? ImageBase64Url { get; set; }
        public bool HighResMode { get; set; }

        public object Clone()
        {
            return new ImageAttachmentInfo
            {
                FileName = FileName,
                ImageBase64Url = (ImageData?)ImageBase64Url?.Clone(),
                HighResMode = HighResMode,
            };
        }
    }

    public IEnumerable<IMessage.IContent> Content { get; set; } = [];
    public RoleType Role => RoleType.User;
    public string? Name { get; set; }
    public bool Hidden => false;
    public DateTime DateTime { get; set; } = DateTime.Now;

    public int CountToken(ModelVersionInfo.TokenizerEnum tokenizer)
    {
        var count = ((IMessage)this).CountTokenBase(tokenizer);
        var attachments = GenerateAttachmentContentList();
        count += attachments.Sum(c => c.CountToken(tokenizer));

        return count;
    }

    public List<IAttachmentInfo> Attachments { get; set; } = [];

    public IEnumerable<IMessage.IContent> GenerateAttachmentContentList()
    {
        List<IMessage.IContent> contents = [];
        foreach (var file in Attachments)
        {
            IMessage.IContent msg = file switch
            {
                TextAttachmentInfo textFile => new IMessage.TextContent
                {
                    Text = $"\n\nAttachment:\n\n{textFile.Content}"
                },
                ImageAttachmentInfo imageFile => new IMessage.ImageContent
                {
                    ImageUrl = new IMessage.ImageContent.ImageUrlType
                    {
                        Url = imageFile.ImageBase64Url,
                        Detail = imageFile.HighResMode
                            ? IMessage.ImageContent.ImageUrlType.ImageDetail.High
                            : IMessage.ImageContent.ImageUrlType.ImageDetail.Low
                    },
                },
                _ => throw new InvalidOperationException()
            };
            contents.Add(msg);
        }

        return contents;
    }

    public object Clone()
    {
        return new UserMessage
        {
            Content = (from c in Content select (IMessage.IContent)c.Clone()).ToList(),
            Name = Name,
            Attachments = (from a in Attachments select (IAttachmentInfo)a.Clone()).ToList(),
            DateTime = DateTime
        };
    }
}

public class AssistantMessage : IMessage
{
    public IEnumerable<IMessage.IContent> Content { get; set; } = new List<IMessage.TextContent>();
    public RoleType Role => RoleType.Assistant;
    public string? Name { get; set; }
    public List<ToolCallType>? ToolCalls { get; set; }
    public bool Hidden => false;
    public DateTime DateTime { get; set; } = DateTime.Now;

    public int CountToken(ModelVersionInfo.TokenizerEnum tokenizer)
    {
        var count = ((IMessage)this).CountTokenBase(tokenizer);
        foreach (var tc in ToolCalls ?? [])
        {
            count += Utils.GetStringTokenCount(tc.Function.Name, tokenizer);
            count += Utils.GetStringTokenCount(tc.Function.Arguments, tokenizer);
        }

        return count;
    }

    public object Clone()
    {
        return new AssistantMessage
        {
            Content = (from c in Content select (IMessage.IContent)c.Clone()).ToList(),
            Name = Name,
            ToolCalls = (from t in ToolCalls select (ToolCallType)t.Clone()).ToList(),
            DateTime = DateTime
        };
    }
}

public class ToolMessage : IMessage
{
    public IEnumerable<IMessage.IContent> Content { get; set; } = new List<IMessage.TextContent>();
    public RoleType Role => RoleType.Tool;
    public string? Name => null;
    public string ToolCallId { get; set; } = "";

    [Obsolete("Prefer to use `GetToolcallMessage` method to provide generated data.")]
    public class GeneratedImage : ICloneable
    {
        public ImageData? ImageBase64Url { get; set; }
        public string Description { get; set; } = "";

        public object Clone()
        {
            return new GeneratedImage
            {
                ImageBase64Url = (ImageData?)ImageBase64Url?.Clone(),
                Description = Description
            };
        }
    }

    [Obsolete("Prefer to use `GetToolcallMessage` method to provide generated data.")]
    public List<GeneratedImage> GeneratedImages { get; set; } = [];

    public bool Hidden { get; set; }
    public DateTime DateTime { get; set; } = DateTime.Now;
    public int CountToken(ModelVersionInfo.TokenizerEnum tokenizer) => ((IMessage)this).CountTokenBase(tokenizer);

    public object Clone()
    {
        return new ToolMessage
        {
            Content = (from c in Content select (IMessage.IContent)c.Clone()).ToList(),
            ToolCallId = ToolCallId,
#pragma warning disable CS0618 // 类型或成员已过时
            GeneratedImages = (from g in GeneratedImages select (GeneratedImage)g.Clone()).ToList(),
#pragma warning restore CS0618 // 类型或成员已过时
            Hidden = Hidden,
            DateTime = DateTime
        };
    }
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

        public FunctionType Function { get; set; } = new();
    }

    public List<IMessage> Messages { get; set; } = [];
    private string? title;

    public string? Title
    {
        set => title = value;
        get
        {
            if (title is not null)
            {
                return title;
            }

            var firstUserMessage = Messages.FirstOrDefault(m => m.Role == RoleType.User);
            var firstTextContent =
                firstUserMessage?.Content.FirstOrDefault(c => c.Type == IMessage.ContentCategory.Text) as
                    IMessage.TextContent;
            var firstLine =
                firstTextContent?.Text.Trim().Split('\n').FirstOrDefault()
                    ?.Trim(); // need trim again at the end, to remove possible '\r'
            return firstLine;
        }
    }

    public Dictionary<string, List<string>> PluginData { get; set; } = [];

    public string Save()
    {
        var contractResolver = new DefaultContractResolver
        {
            NamingStrategy = new SnakeCaseNamingStrategy()
        };
        var settings = new JsonSerializerSettings
        {
            ContractResolver = contractResolver,
            Formatting = Formatting.Indented,
            StringEscapeHandling = StringEscapeHandling.Default,
            NullValueHandling = NullValueHandling.Ignore
        };
        var result = JsonConvert.SerializeObject(this, settings);
        return result;
    }

    public static ChatCompletionRequest BuildFromInitPrompts(
        IEnumerable<IMessage>? initPrompts,
        DateTime knowledgeCutoff,
        string productName,
        string modelProvider)
    {
        var request = new ChatCompletionRequest();
        var messages = (from msg in initPrompts select (IMessage)msg.Clone()).ToList();

        foreach (var msg in messages)
        {
            msg.DateTime = DateTime.Now;
            var contentList = msg.Content.ToList();
            foreach (var content in contentList)
            {
                if (content is not IMessage.TextContent textContent)
                {
                    continue;
                }

                var ci = CultureInfo.CurrentUICulture;

                var prompt = textContent.Text;
                prompt = prompt.Replace("{DateTime}",
                    DateTime.Now.ToString("MMM dd yyy", CultureInfo.GetCultureInfo("en-US")));
                prompt = prompt.Replace("{Cutoff}",
                    knowledgeCutoff.ToString("MMM yyyy", CultureInfo.GetCultureInfo("en-US")));
                prompt = prompt.Replace("{Language}", ci.DisplayName);
                prompt = prompt.Replace("{ProductName}", productName);
                prompt = prompt.Replace("{ModelProvider}", modelProvider);
                textContent.Text = prompt;
            }

            msg.Content = contentList;
        }

        request.Messages = messages;

        return request;
    }

    public int CountTokens(ModelVersionInfo.TokenizerEnum tokenizer)
    {
        var count = 0;
        foreach (var msg in Messages)
        {
            count += msg.CountToken(tokenizer);
        }

        return count;
    }
}