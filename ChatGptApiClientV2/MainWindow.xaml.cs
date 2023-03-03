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

namespace ChatGptApiClientV2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly HttpClient client = new HttpClient();

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
                var jobj = new JObject();
                jobj["role"] =
                    Type == ChatType.User ? "user" :
                    Type == ChatType.Bot ? "assistant" :
                    "system";
                jobj["content"] = Content;

                return jobj;
            }
            public static ChatRecord FromJson(JObject jobj)
            {
                var type =
                    jobj["role"].ToString() == "user" ? ChatType.User :
                    jobj["role"].ToString() == "assistant" ? ChatType.Bot :
                    ChatType.System;
                var content = jobj["content"].ToString();
                return new ChatRecord(type, content);
            }
            public void Display()
            {
                Console.WriteLine(new string('-', Console.WindowWidth));
                switch (Type)
                {
                    case ChatType.User:
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
                Console.WriteLine(document.VT100EncodedString);
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
        }

        private ChatRecordList current_session_record;

        class Config
        {
            public string API_KEY { get; set; }
            public double temperature { get; set; }
        }

        private Config config = new Config();
        private bool first_input = true;
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            File.WriteAllText("config.json", JsonConvert.SerializeObject(config));


            if (current_session_record is null)
            {
                string prompt = ((cbx_initial.SelectedItem as ComboBoxItem).Content as TextBlock).Text;
                prompt = prompt.Replace("{DateTime}", DateTime.Now.ToString("D", CultureInfo.GetCultureInfo("en-US")));
                var record = new ChatRecord(ChatRecord.ChatType.System, prompt);
                current_session_record = new ChatRecordList(record);
                record.Display();
            }
            var new_user_input = new ChatRecord(ChatRecord.ChatType.User, txtbx_input.Text);
            new_user_input.Display();
            current_session_record.ChatRecords.Add(new_user_input);

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", config.API_KEY);

            var msg = new JObject();
            msg["model"] = "gpt-3.5-turbo";
            msg["messages"] = current_session_record.ToJson();
            msg["temperature"] = config.temperature;
            var post_content = new StringContent(msg.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", post_content);
            var responseString = await response.Content.ReadAsStringAsync();
            var responseJson = JObject.Parse(responseString);
            if (responseJson["error"] != null)
            {
                Console.WriteLine(responseJson["error"]["message"].ToString());
                Console.WriteLine("");
                return;
            }
            var bot_response = responseJson["choices"][0]["message"];
            var bot_response_record = ChatRecord.FromJson(bot_response as JObject);
            bot_response_record.Display();
            current_session_record.ChatRecords.Add(bot_response_record);
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

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            config.temperature = slid_temp.Value;
            if (!(txt_temp is null))
            {
                txt_temp.Text = $"{config.temperature}";
            }
        }

        private void txtbx_apikey_TextChanged(object sender, TextChangedEventArgs e)
        {
            config.API_KEY = txtbx_apikey.Text;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (File.Exists("config.json"))
            {
                string saved_config = File.ReadAllText("config.json");
                config = JsonConvert.DeserializeObject<Config>(saved_config);
                txtbx_apikey.Text = config.API_KEY;
                slid_temp.Value = config.temperature;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                double value = double.Parse(txt_temp.Text);
                if (value < 0 || value > 2)
                {
                    value = Math.Max(0.0, value);
                    value = Math.Min(2.0, value);
                    txt_temp.Text = value.ToString();
                }
                slid_temp.Value = value;
            }
            catch (FormatException)
            {
                // do nothing
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string saved_session = JsonConvert.SerializeObject(current_session_record);
            var dlg = new SaveFileDialog();
            dlg.FileName = "session";
            dlg.DefaultExt = ".json";
            dlg.Filter = "JSON documents|*.json";
            if (dlg.ShowDialog() == true)
            {
                File.WriteAllText(dlg.FileName, saved_session);
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.DefaultExt = ".json";
            dlg.Filter = "JSON documents|*.json";
            if (dlg.ShowDialog() == true)
            {
                string saved_session = File.ReadAllText(dlg.FileName);
                current_session_record = JsonConvert.DeserializeObject<ChatRecordList>(saved_session);
                Console.WriteLine(new string('=', Console.WindowWidth));
                foreach (var record in current_session_record.ChatRecords)
                {
                    record.Display();
                }
            }
        }
    }
}
