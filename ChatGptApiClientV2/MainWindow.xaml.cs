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

namespace ChatGptApiClientV2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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
            public void Display()
            {
                switch (Type)
                {
                    case ChatType.User:
                        Console.WriteLine(new string('-', Console.WindowWidth));
                        Console.WriteLine(Bold().Green("用户："));
                        break;
                    case ChatType.Bot:
                        Console.WriteLine(Bold().Yellow("助手："));
                        break;
                    case ChatType.System:
                        Console.WriteLine(Bold().Blue("系统："));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                var document = MarkdownConverter.Convert(Content, MarkdownConversionType.VT100, new PSMarkdownOptionInfo());
                Console.Write(document.VT100EncodedString);
            }
        }
        class ChatRecordList
        {
            public List<ChatRecord> ChatRecords { get; set; }
            public ChatRecordList(ChatRecord initial_prompt)
            {
                ChatRecords = new List<ChatRecord>
                {
                    initial_prompt
                };
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
        }

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
                    _temperature = value;
                    if (PropertyChanged is not null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs(nameof(Temperature)));
                    }
                    SaveConfig();
                }
            }
            public Config()
            {
                _api_key = "";
                _temperature = 1.0;
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
            if (loaded_session is null)
            {
                string prompt = ((TextBlock)((ComboBoxItem)cbx_initial.SelectedItem).Content).Text;
                prompt = prompt.Replace("{DateTime}", DateTime.Now.ToString("F", CultureInfo.GetCultureInfo("en-US")));
                var record = new ChatRecord(ChatRecord.ChatType.System, prompt);
                loaded_session = new ChatRecordList(record);
            }
            current_session_record = loaded_session;
            Console.WriteLine(new string('=', Console.WindowWidth));
            foreach (var record in current_session_record.ChatRecords)
            {
                record.Display();
            }
        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (current_session_record is null)
            {
                ResetSession();
            }
            var new_user_input = new ChatRecord(ChatRecord.ChatType.User, txtbx_input.Text);
            new_user_input.Display();
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
                bot_response_record.Display();
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
    }
}
