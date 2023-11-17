using Microsoft.PowerShell.MarkdownRender;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static Crayon.Output;

namespace ChatGptApiClientV2
{
    public class ChatRecord
    {
        public enum ChatType
        {
            User,
            Bot,
            System,
            Function,
        }
        public ChatType Type { get; set; }
        public string Content { get; set; }
        public class ImageInfo
        {
            public string Data { get; set; } = "";
            public bool UploadToBot { get; set; } = true;
        }
        public List<ImageInfo> Images { get; set; }
        private readonly Dictionary<string, string> imageConsoleSeqCache = [];
        public bool HighResImage { get; set; }
        public bool Hidden { get; set; }
        public ChatRecord(ChatType type, string content, List<ImageInfo>? images = null, bool highresimage = false, bool hidden = false)
        {
            Type = type;
            Content = content;
            Images = images ?? [];
            Hidden = hidden;
            HighResImage = highresimage;
        }
        public void AddImageFromFile(string filename)
        {
            Images.Add(new() { Data = Utils.ImageFileToBase64(filename), UploadToBot = true });
        }
        public void AddImageFromBase64(string base64url, bool upload_to_bot)
        {
            Images.Add(new() { Data = base64url, UploadToBot = upload_to_bot });
        }
        public JsonObject ToJson()
        {
            var content = new JsonArray
            {
                new JsonObject
                {
                    ["type"] = "text",
                    ["text"] = Content
                }
            };
            foreach(var img in Images)
            {
                if (!img.UploadToBot)
                {
                    continue;
                }
                content.Add(new JsonObject
                {
                    ["type"] = "image_url",
                    ["image_url"] = new JsonObject
                    {
                        ["url"] = Utils.AssertIsBase64Url(img.Data),
                        ["detail"] = HighResImage ? "high" : "low"
                    }
                });
            }   
            var jobj = new JsonObject
            {
                ["role"] =
                    Type == ChatType.User ? "user" :
                    Type == ChatType.Bot ? "assistant" :
                    Type == ChatType.Function ? "function" :
                    "system",
                ["content"] = content
            };
            return jobj;
        }
        public static ChatRecord FromJson(JsonObject jobj)
        {
            var type =
                jobj["role"]?.ToString() == "user" ? ChatType.User :
                jobj["role"]?.ToString() == "assistant" ? ChatType.Bot :
                jobj["role"]?.ToString() == "function" ? ChatType.Function :
                ChatType.System;
            var content = jobj["content"]?.ToString();
            return new ChatRecord(type, content ?? "[Error: Empty Content]");
        }
        public string GetHeader(bool advancedFormat = true)
        {
            StringBuilder sb = new();
            if (advancedFormat)
            {
                switch (Type)
                {
                    case ChatType.User:
                        sb.AppendLine(new string('-', Console.WindowWidth));
                        sb.AppendLine(Bold().Green("用户："));
                        break;
                    case ChatType.Bot:
                        sb.AppendLine(Bold().Yellow("助手："));
                        break;
                    case ChatType.Function:
                        sb.AppendLine(Bold().Magenta("函数："));
                        break;
                    case ChatType.System:
                        sb.AppendLine(Bold().Blue("系统："));
                        break;
                    default:
                        throw new InvalidEnumArgumentException();
                }
            }
            else
            {
                switch (Type)
                {
                    case ChatType.User:
                        sb.AppendLine("用户：");
                        break;
                    case ChatType.Bot:
                        sb.AppendLine("助手：");
                        break;
                    case ChatType.Function:
                        sb.AppendLine("函数：");
                        break;
                    case ChatType.System:
                        sb.AppendLine("系统：");
                        break;
                    default:
                        throw new InvalidEnumArgumentException();
                }
            }
            return sb.ToString();
        }
        public string ToString(bool useMarkdown, bool advancedFormat = true)
        {
            if (useMarkdown && !advancedFormat)
            {
                throw new ArgumentException("Markdown is not supported in simple format.");
            }
            StringBuilder sb = new();
            sb.Append(GetHeader(advancedFormat));
            if (useMarkdown)
            {
                var document = MarkdownConverter.Convert(Content, MarkdownConversionType.VT100, new PSMarkdownOptionInfo());
                sb.Append(document.VT100EncodedString);
            }
            else
            {
                sb.AppendLine(Content);
            }
            return sb.ToString();
        }
        public void Display(bool useMarkdown)
        {
            if (Hidden)
            {
                return;
            }
            Console.Write(this.ToString(useMarkdown));
            foreach (var img_url in Images)
            {
                if (!imageConsoleSeqCache.TryGetValue(img_url.Data, out string? value))
                {
                    var bitmap = Utils.Base64ToBitmap(img_url.Data);
                    value = Utils.ConvertImageToConsoleSeq(bitmap);
                    imageConsoleSeqCache[img_url.Data] = value;
                }
                var seq = value;
                Utils.ConsolePrintImage(seq);
            }
        }
    }
    public class ChatRecordList
    {
        public List<ChatRecord> ChatRecords { get; set; }
        [JsonConstructor]
        public ChatRecordList() : this(null, DateTime.Now)
        {
        }
        public ChatRecordList(ChatRecordList? initial_prompt, DateTime knowledge_cutoff)
        {
            ChatRecords = [];
            if (initial_prompt is not null)
            {
                foreach (var record in initial_prompt.ChatRecords)
                {
                    string prompt = record.Content.Replace("{DateTime}", DateTime.Now.ToString("MMM dd yyy", CultureInfo.GetCultureInfo("en-US")));
                    prompt = prompt.Replace("{Cutoff}", knowledge_cutoff.ToString("MMM yyyy", CultureInfo.GetCultureInfo("en-US")));
                    ChatRecords.Add(new(record.Type, prompt));
                }
            }
        }
        public JsonArray ToJson()
        {
            var jarray = new JsonArray();
            foreach (var record in ChatRecords)
            {
                jarray.Add(record.ToJson());
            }
            return jarray;
        }
        public string Text => string.Join(string.Empty, (from record in ChatRecords select record.ToString(false, false)));
    }
}
