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
using Microsoft.PowerShell.MarkdownRender;
using System.Windows.Resources;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Collections.ObjectModel;
using System.Text.Json.Nodes;
using System.Text.Encodings.Web;
using System.Reflection.Metadata;
using System.Windows.Threading;

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
        
        private readonly Plugins.PythonPlugin pythonPlugin = new();

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
        class ModelInfo
        {
            public string Name { get; set; }
            public string Description { get; set; }
        }
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
            private bool _plugin_python_enable;
            public bool PluginPythonEnable
            {
                get => _plugin_python_enable;
                set
                {
                    if (value == _plugin_python_enable) return;
                    _plugin_python_enable = value;
                    if (PropertyChanged is not null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs(nameof(PluginPythonEnable)));
                    }
                    SaveConfig();
                }
            }
            private ObservableCollection<ModelInfo> _modelOptions = new ObservableCollection<ModelInfo>
            {
                new ModelInfo{ Name="gpt-3.5-turbo-0613", Description="gpt-3.5-turbo-0613 (4k tokens)" },
                new ModelInfo{ Name="gpt-3.5-turbo-16k-0613", Description="gpt-3.5-turbo-16k-0613 (16k tokens)" },
                new ModelInfo{ Name="gpt-3.5-turbo-0301", Description="gpt-3.5-turbo-0301 (deprecated, 4k tokens)" },
            };
            public ObservableCollection<ModelInfo> ModelOptions { get => _modelOptions; }
            private ModelInfo? _selectedModel;
            public ModelInfo? SelectedModel
            {
                get => _selectedModel;
                set
                {
                    if (value == _selectedModel) return;
                    _selectedModel = value;
                    if (PropertyChanged is not null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs(nameof(SelectedModel)));
                    }
                    SaveConfig();
                }
            }
            public Config()
            {
                _api_key = "";
                _temperature = 1.0;
                _enableMarkdown = false;
                _selectedModel = _modelOptions.First();
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
            if (loaded_session is null)
            {
                loaded_session = new ChatRecordList(initial_prompts?.SelectedOption);
                if (config.PluginPythonEnable) 
                {
                    foreach(var line in Plugins.PythonPlugin.InitialPrompt)
                    {
                        loaded_session.ChatRecords.Add(line);
                    }
                }
            }
            current_session_record = loaded_session;
            Console.WriteLine(new string('=', Console.WindowWidth));
            foreach (var record in current_session_record.ChatRecords)
            {
                record.Display(config.EnableMarkdown);
            }
        }
        private async Task Send()
        {
            
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", config.API_KEY);
            if (config.SelectedModel is null)
            {
                throw new ArgumentNullException(nameof(config.SelectedModel));
            }
            var msg = new JsonObject
            {
                ["model"] = config.SelectedModel.Name,
                ["messages"] = current_session_record!.ToJson(),
                ["temperature"] = config.Temperature,
                ["stream"] = true
            };
            var post_content = new StringContent(msg.ToString(), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://api.openai.com/v1/chat/completions"),
                Content = post_content
            };
            netStatus.Status = NetStatus.StatusEnum.Sending;
            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            netStatus.Status = NetStatus.StatusEnum.Receiving;
            StringBuilder response_sb = new();
            ChatRecord response_record = new(ChatRecord.ChatType.Bot, "");
            Console.Write(response_record.GetHeader());
            if (response.IsSuccessStatusCode)
            {
                using (var responseStream = await response.Content.ReadAsStreamAsync())
                using (var reader = new StreamReader(responseStream))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = await reader.ReadLineAsync();
                        if (!string.IsNullOrEmpty(line))
                        {
                            if (line == "data: [DONE]")
                            {
                                break; // 结束聊天响应数据接收
                            }
                            if (line.StartsWith("data: "))
                            {
                                var chatResponse = line.Substring("data: ".Length);
                                var responseJson = JsonNode.Parse(chatResponse);
                                if (responseJson?["object"]?.ToString() != "chat.completion.chunk")
                                {
                                    Console.WriteLine(responseJson?.ToString());
                                    return;
                                }
                                string? ch = responseJson?["choices"]?[0]?["delta"]?["content"]?.ToString();
                                if (ch is not null)
                                {
                                    response_sb.Append(ch);
                                    Console.Write(ch);
                                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { })); // this is needed to allow ui update
                                }
                                string? role = responseJson?["choices"]?[0]?["delta"]?["role"]?.ToString();
                                if (role is not null && role != "assistant")
                                {
                                    throw new InvalidDataException($"Wrong reply role: {{{role}}}");
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                using (var responseStream = await response.Content.ReadAsStreamAsync())
                using (var reader = new StreamReader(responseStream))
                {
                    var chatResponse = await reader.ReadToEndAsync();
                    var responseJson = JsonNode.Parse(chatResponse);
                    if (responseJson?["error"] is not null)
                    {
                        response_sb.Append($"Error: {responseJson?["error"]?["message"]?.ToString()}");
                    }
                    else
                    {
                        response_sb.Append($"Error: {chatResponse}");
                    }
                }
            }
            Console.WriteLine();
            response_record.Content = response_sb.ToString();
            current_session_record.ChatRecords.Add(response_record);
            netStatus.Status = NetStatus.StatusEnum.Idle;

            // plugin processing
            bool plugin_usage_detected = pythonPlugin.DetectUsage(response_record.Content);
            if (plugin_usage_detected)
            {
                string plugin_response = pythonPlugin.ProcessData(response_record.Content);
                var plugin_response_record = new ChatRecord(ChatRecord.ChatType.System, plugin_response);
                plugin_response_record.Display(config.EnableMarkdown);
                current_session_record.ChatRecords.Add(plugin_response_record);
                await Send();
            }
        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (current_session_record is null)
            {
                ResetSession();
            }

            StringBuilder input_txt = new();
            input_txt.Append(txtbx_input.Text);
            if (txtblk_attachment.Text != "")
            {
                input_txt.AppendLine("");
                input_txt.AppendLine("Attachment: ");
                string file_content = await File.ReadAllTextAsync(txtblk_attachment.Text);
                input_txt.AppendLine(file_content);
            }
            var new_user_input = new ChatRecord(ChatRecord.ChatType.User, input_txt.ToString());
            new_user_input.Display(config.EnableMarkdown);
            current_session_record!.ChatRecords.Add(new_user_input);
            
            await Send();

            txtbx_input.Text = "";
            txtblk_attachment.Text = "";
            ResetSession(current_session_record);
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

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "Any Files|*.*"
            };
            if (dlg.ShowDialog() == true)
            {
                txtblk_attachment.Text = dlg.FileName;
            }
        }
    }
}
