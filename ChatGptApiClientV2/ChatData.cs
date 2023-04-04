using Microsoft.PowerShell.MarkdownRender;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static Crayon.Output;

namespace ChatGptApiClientV2
{
    class ChatRecord
    {
        public enum ChatType
        {
            User,
            Bot,
            System,
        }
        public ChatType Type { get; set; }
        public string Content { get; set; }
        public ChatRecord(ChatType type, string content)
        {
            Type = type;
            Content = content;
        }
        public JsonObject ToJson()
        {
            var jobj = new JsonObject
            {
                ["role"] =
                    Type == ChatType.User ? "user" :
                    Type == ChatType.Bot ? "assistant" :
                    "system",
                ["content"] = Content
            };
            return jobj;
        }
        public static ChatRecord FromJson(JsonObject jobj)
        {
            var type =
                jobj["role"]?.ToString() == "user" ? ChatType.User :
                jobj["role"]?.ToString() == "assistant" ? ChatType.Bot :
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
            Console.Write(this.ToString(useMarkdown));
        }
    }
    class ChatRecordList
    {
        public List<ChatRecord> ChatRecords { get; set; }
        [JsonConstructor]
        public ChatRecordList() : this(null)
        {
        }
        public ChatRecordList(ChatRecordList? initial_prompt)
        {
            ChatRecords = new();
            if (initial_prompt is not null)
            {
                foreach (var record in initial_prompt.ChatRecords)
                {
                    string prompt = record.Content.Replace("{DateTime}", DateTime.Now.ToString("F", CultureInfo.GetCultureInfo("en-US")));
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
        public string Text
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var record in ChatRecords)
                {
                    sb.Append(record.ToString(false, false));
                }
                return sb.ToString();
            }
        }
    }
}
