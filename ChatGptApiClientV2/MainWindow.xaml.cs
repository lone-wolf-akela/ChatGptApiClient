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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.IO;
using Microsoft.Win32;
using static Crayon.Output;
using Microsoft.PowerShell.MarkdownRender;
using System.Windows.Resources;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Collections.ObjectModel;

namespace ChatGptApiClientV2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const char Esc = (char)0x1B;
        private readonly HttpClient client = new();

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
            public JObject ToJson()
            {
                var jobj = new JObject
                {
                    ["role"] =
                        Type == ChatType.User ? "user" :
                        Type == ChatType.Bot ? "assistant" :
                        "system",
                    ["content"] = Content
                };
                return jobj;
            }
            public static ChatRecord FromJson(JObject jobj)
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
                StringBuilder sb = new StringBuilder();
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
                            throw new ArgumentOutOfRangeException();
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
                            throw new ArgumentOutOfRangeException();
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
            public ChatRecordList(ChatRecordList? initial_prompt=null)
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
            public JArray ToJson()
            {
                var jarray = new JArray();
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
                get { return _promptsOptions; }
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
                get { return _selectedOption; }
                set
                {
                    _selectedOption = value;
                    if (PropertyChanged is not null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs(nameof(SelectedOption)));
                    }
                }
            }
            public InitialPrompts(bool useDefault = false)
            {
                _promptsOptions = new ObservableCollection<ChatRecordList>()
                {
                    new()
                };
                if (useDefault)
                {
                    _promptsOptions[0].ChatRecords.Add(new(ChatRecord.ChatType.System, "You are ChatGPT, a large language model trained by OpenAI. Answer as concisely as possible. Session starts at: {DateTime}"));
                }
            }
        }
        private InitialPrompts? initial_prompts;

        public MainWindow()
        {
            InitializeComponent();
            Uri uri = new Uri("/chatgpt-icon.ico", UriKind.Relative);
            StreamResourceInfo info = Application.GetResourceStream(uri);
            this.Icon = BitmapFrame.Create(info.Stream);
            this.DataContext = config;
        }

        private ChatRecordList? current_session_record;

        class Config : INotifyPropertyChanged
        {
            private string _api_key;

            public event PropertyChangedEventHandler? PropertyChanged;

            public string API_KEY
            {
                get { return _api_key; }
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
                get { return _temperature; }
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
                get { return _enableMarkdown; }
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
                File.WriteAllText("config.json", JsonConvert.SerializeObject(this));
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

            var msg = new JObject
            {
                ["model"] = "gpt-3.5-turbo",
                ["messages"] = current_session_record.ToJson(),
                ["temperature"] = config.Temperature
            };
            var post_content = new StringContent(msg.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", post_content);
            var responseString = await response.Content.ReadAsStringAsync();
            var responseJson = JObject.Parse(responseString);
            if (responseJson["error"] is not null)
            {
                Console.WriteLine(responseJson?["error"]?["message"]?.ToString());
                return;
            }
            var bot_response = responseJson?["choices"]?[0]?["message"];
            if (bot_response is JObject response_obj)
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
            if (File.Exists("config.json"))
            {
                string saved_config = File.ReadAllText("config.json");
                var parsed_config = JsonConvert.DeserializeObject<Config>(saved_config);
                if (parsed_config is not null)
                {
                    config = parsed_config;
                    this.DataContext = config;
                }
            }

            if (File.Exists("initial_prompts.json"))
            {
                string saved_prompts = File.ReadAllText("initial_prompts.json");
                var parsed_prompts = JsonConvert.DeserializeObject<InitialPrompts>(saved_prompts);
                if (parsed_prompts is not null)
                {
                    initial_prompts = parsed_prompts;
                }
            }
            if (initial_prompts is null || !initial_prompts.PromptsOptions.Any())
            {
                initial_prompts = new(useDefault:true);
                File.WriteAllText("initial_prompts.json", JsonConvert.SerializeObject(initial_prompts));
            }
            initial_prompts.SelectedOption = initial_prompts.PromptsOptions[0];
            cbx_initial.DataContext = initial_prompts;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string saved_session = JsonConvert.SerializeObject(current_session_record);
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
                var loaded_session = JsonConvert.DeserializeObject<ChatRecordList>(saved_session);
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
