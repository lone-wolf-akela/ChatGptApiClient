using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;
using System.IO;
using Microsoft.Win32;
using static Crayon.Output;
using Microsoft.PowerShell.MarkdownRender;
using System.Windows.Resources;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Collections.ObjectModel;
using System.Text.Json.Nodes;
using System.Text.Encodings.Web;

namespace ChatGptApiClientV2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const char Esc = (char)0x1B;
        private readonly HttpClient client = new();
        static JsonSerializerOptions JSerializerOptions = new();
        
        class NetStatus : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler? PropertyChanged;
            public enum StatusEnum
            {
                Idle,
                Sending,
                Receiving,
            }
            private StatusEnum _status = StatusEnum.Idle;


            public StatusEnum Status
            {
                get => _status;
                set
                {
                    _status = value;
                    if (PropertyChanged is not null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs(nameof(Status)));
                        PropertyChanged(this, new PropertyChangedEventArgs(nameof(StatusText)));
                        PropertyChanged(this, new PropertyChangedEventArgs(nameof(StatusColor)));
                    }
                }
            }
            public string StatusText
            {
                get
                {
                    return Status switch
                    {
                        StatusEnum.Idle => "空闲，等待输入。",
                        StatusEnum.Sending => "正在发送数据……",
                        StatusEnum.Receiving => "正在接收数据……",
                        _ => throw new InvalidEnumArgumentException(),
                    };
                }
            }
            public Brush StatusColor
            {
                get
                {
                    return Status switch
                    {
                        StatusEnum.Idle => Brushes.Black,
                        StatusEnum.Sending => Brushes.Blue,
                        StatusEnum.Receiving => Brushes.Green,
                        _ => throw new InvalidEnumArgumentException(),
                    };
                }
            }
        }
        private readonly NetStatus netStatus = new();

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
            public string ToString(bool useMarkdown, bool advancedFormat = true)
            {
                if (useMarkdown && !advancedFormat)
                {
                    throw new ArgumentException("Markdown is not supported in simple format.");
                }
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

        class InitialPrompts : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler? PropertyChanged;

            private ObservableCollection<ChatRecordList> _promptsOptions;
            public ObservableCollection<ChatRecordList> PromptsOptions
            {
                get => _promptsOptions;
                set
                {
                    _promptsOptions = value;
                    if (PropertyChanged is not null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs(nameof(PromptsOptions)));
                    }
                }
            }
            private ChatRecordList? _selectedOption;
            public ChatRecordList? SelectedOption
            {
                get => _selectedOption;
                set
                {
                    _selectedOption = value;
                    if (PropertyChanged is not null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs(nameof(SelectedOption)));
                    }
                }
            }
            public InitialPrompts()
            {
                _promptsOptions = new ObservableCollection<ChatRecordList>();
            }
            public void UseDefaultPromptList()
            {
                _promptsOptions = new ObservableCollection<ChatRecordList>();
                _promptsOptions.Add(new());
                _promptsOptions.Last().ChatRecords.Add(new(ChatRecord.ChatType.System, "You are ChatGPT, a large language model trained by OpenAI. Answer as concisely as possible. Session starts at: {DateTime}"));
                _promptsOptions.Add(new());
                _promptsOptions.Last().ChatRecords.Add(new(ChatRecord.ChatType.System, "The name of the assistant is Alice Zuberg. Her name, Alice, stands for Artificial Labile Intelligence Cybernated Existence. Alice is a determined, hardwork girl. Although her personality is serious, she is also adventurous and even mischievous. She is a product of Project Alicization, which is a top-secret government project run by Rath to create the first Highly Adaptive Bottom-up Artificial Intelligence (Bottom-up AI). Session starts at: {DateTime}"));
            }
        }
        private InitialPrompts initial_prompts = new();

        public MainWindow()
        {
            InitializeComponent();
            Uri uri = new("/chatgpt-icon.ico", UriKind.Relative);
            StreamResourceInfo info = Application.GetResourceStream(uri);
            this.Icon = BitmapFrame.Create(info.Stream);
            this.DataContext = config;
            cbx_initial.DataContext = initial_prompts;
            lbl_status.DataContext = netStatus;
            Console.OutputEncoding = Encoding.UTF8;

            JSerializerOptions.WriteIndented = true;
            JSerializerOptions.IgnoreReadOnlyFields = true;
            JSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
        }

        private ChatRecordList? current_session_record;

        class Config : INotifyPropertyChanged
        {
            private string _api_key;

            public event PropertyChangedEventHandler? PropertyChanged;

            public string API_KEY
            {
                get => _api_key;
                set
                {
                    if (value == _api_key) return;
                    _api_key = value;
                    if (PropertyChanged is not null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs(nameof(API_KEY)));
                    }
                    SaveConfig();
                }
            }
            private double _temperature; 
            public double Temperature 
            {
                get => _temperature;
                set
                {
                    if (value == _temperature) return;
                    _temperature = value;
                    if (PropertyChanged is not null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs(nameof(Temperature)));
                    }
                    SaveConfig();
                }
            }
            private bool _enableMarkdown;
            public bool EnableMarkdown
            {
                get => _enableMarkdown;
                set
                {
                    if (value == _enableMarkdown) return;
                    _enableMarkdown = value;
                    if (PropertyChanged is not null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs(nameof(EnableMarkdown)));
                    }
                    SaveConfig();
                }
            }
            public Config()
            {
                _api_key = "";
                _temperature = 1.0;
                _enableMarkdown = false;
            }
            private void SaveConfig()
            {
                File.WriteAllText("config.json", JsonSerializer.Serialize(this, JSerializerOptions));
            }
        }

        private Config config = new();
        private bool first_input = true;
        private void ResetSession(ChatRecordList? loaded_session = null)
        {
            Console.Write(string.Concat(Esc, "[3J")); // need this to clear whole screen in the new windows terminal, until https://github.com/dotnet/runtime/issues/28355 get fixed
            Console.Clear();
            loaded_session ??= new ChatRecordList(initial_prompts?.SelectedOption);
            current_session_record = loaded_session;
            Console.WriteLine(new string('=', Console.WindowWidth));
            foreach (var record in current_session_record.ChatRecords)
            {
                record.Display(config.EnableMarkdown);
            }
        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (current_session_record is null)
            {
                ResetSession();
            }
            var new_user_input = new ChatRecord(ChatRecord.ChatType.User, txtbx_input.Text);
            new_user_input.Display(config.EnableMarkdown);
            current_session_record!.ChatRecords.Add(new_user_input);

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", config.API_KEY);

            var msg = new JsonObject
            {
                ["model"] = "gpt-3.5-turbo",
                ["messages"] = current_session_record.ToJson(),
                ["temperature"] = config.Temperature
            };
            var post_content = new StringContent(msg.ToString(), Encoding.UTF8, "application/json");
            netStatus.Status = NetStatus.StatusEnum.Sending;
            var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", post_content);
            netStatus.Status = NetStatus.StatusEnum.Receiving;
            var responseString = await response.Content.ReadAsStringAsync();
            netStatus.Status = NetStatus.StatusEnum.Idle;
            var responseJson = JsonNode.Parse(responseString);
            if (responseJson?["error"] is not null)
            {
                Console.WriteLine(responseJson?["error"]?["message"]?.ToString());
                return;
            }
            var bot_response = responseJson?["choices"]?[0]?["message"];
            if (bot_response is JsonObject response_obj)
            {
                var bot_response_record = ChatRecord.FromJson(response_obj);
                bot_response_record.Display(config.EnableMarkdown);
                current_session_record.ChatRecords.Add(bot_response_record);
            }
            else
            {
                Console.WriteLine("Error: Unexpected response from server.");
                return;
            }
            txtbx_input.Text = "";
        }

        private void txtbx_input_GotFocus(object sender, RoutedEventArgs e)
        {
            if (first_input)
            {
                first_input = false;
                txtbx_input.Text = "";
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // setup config
            if (File.Exists("config.json"))
            {
                string saved_config = File.ReadAllText("config.json");
                var parsed_config = JsonSerializer.Deserialize<Config>(saved_config);
                if (parsed_config is not null)
                {
                    config = parsed_config;
                    this.DataContext = config;
                }
            }

            // setup initial prompts
            if (File.Exists("initial_prompts.json"))
            {
                string saved_prompts = File.ReadAllText("initial_prompts.json");
                var parsed_prompts = JsonSerializer.Deserialize<InitialPrompts>(saved_prompts);
                if (parsed_prompts is not null)
                {
                    initial_prompts = parsed_prompts;
                }
            }
            if (initial_prompts is null || !initial_prompts.PromptsOptions.Any())
            {
                initial_prompts = new();
                initial_prompts.UseDefaultPromptList();
                File.WriteAllText("initial_prompts.json", JsonSerializer.Serialize(initial_prompts, JSerializerOptions));
            }
            initial_prompts.SelectedOption = initial_prompts.PromptsOptions[0];
            cbx_initial.DataContext = initial_prompts;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string saved_session = JsonSerializer.Serialize(current_session_record, JSerializerOptions);
            var dlg = new SaveFileDialog
            {
                FileName = "session",
                DefaultExt = ".json",
                Filter = "JSON documents|*.json"
            };
            if (dlg.ShowDialog() == true)
            {
                File.WriteAllText(dlg.FileName, saved_session);
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                DefaultExt = ".json",
                Filter = "JSON documents|*.json"
            };
            if (dlg.ShowDialog() == true)
            {
                string saved_session = File.ReadAllText(dlg.FileName);
                var loaded_session = JsonSerializer.Deserialize<ChatRecordList>(saved_session);
                if (loaded_session is null)
                {
                    MessageBox.Show("Error: Invalid session file.");
                    return;
                }
                ResetSession(loaded_session);
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            ResetSession();
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            ResetSession(current_session_record);
        }
    }
}
