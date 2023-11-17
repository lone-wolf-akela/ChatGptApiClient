using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Threading;

namespace ChatGptApiClientV2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [INotifyPropertyChanged]
    public partial class MainWindow : Window
    {
        private const char Esc = (char)0x1B;
        private readonly HttpClient client = new();
        static readonly JsonSerializerOptions JSerializerOptions = new();

        public partial class NetStatusType : ObservableObject
        {
            public enum StatusEnum
            {
                Idle,
                Sending,
                Receiving,
            }

            [ObservableProperty]
            [NotifyPropertyChangedFor(nameof(StatusText))]
            [NotifyPropertyChangedFor(nameof(StatusColor))]
            private StatusEnum status = StatusEnum.Idle;

            [ObservableProperty]
            private string systemFingerprint = "";
            public string StatusText => Status switch
            {
                StatusEnum.Idle => $"空闲，等待输入。",
                StatusEnum.Sending => $"正在发送数据……",
                StatusEnum.Receiving => $"正在接收数据……",
                _ => throw new System.ComponentModel.InvalidEnumArgumentException(),
            };
            public Brush StatusColor => Status switch
            {
                StatusEnum.Idle => Brushes.Black,
                StatusEnum.Sending => Brushes.Blue,
                StatusEnum.Receiving => Brushes.Green,
                _ => throw new System.ComponentModel.InvalidEnumArgumentException(),
            };
        }
        [ObservableProperty]
        private NetStatusType netStatus = new();

        public partial class InitialPromptsType : ObservableObject
        {
            [ObservableProperty]
            private ObservableCollection<ChatRecordList> promptsOptions;
            [ObservableProperty]
            private ChatRecordList? selectedOption;

            public InitialPromptsType()
            {
                promptsOptions = [];
            }
            public void UseDefaultPromptList()
            {
                PromptsOptions = [new()];
                PromptsOptions.Last().ChatRecords.Add(new(ChatRecord.ChatType.System, "You are ChatGPT, a large language model trained by OpenAI. Answer as concisely as possible.\nKnowledge cutoff: {Cutoff}\nCurrent date: {DateTime}"));
                PromptsOptions.Add(new());
                PromptsOptions.Last().ChatRecords.Add(new(ChatRecord.ChatType.System, "The name of the assistant is Alice Zuberg. Her name, Alice, stands for Artificial Labile Intelligence Cybernated Existence. Alice is a determined, hardwork girl. Although her personality is serious, she is also adventurous and even mischievous. She is a product of Project Alicization, which is a top-secret government project run by Rath to create the first Highly Adaptive Bottom-up Artificial Intelligence (Bottom-up AI). Session starts at: {DateTime}"));
            }
        }
        [ObservableProperty]
        private InitialPromptsType initialPrompts = new();

        public MainWindow()
        {
            InitializeComponent();
            Uri uri = new("/chatgpt-icon.ico", UriKind.Relative);
            StreamResourceInfo info = Application.GetResourceStream(uri);
            Icon = BitmapFrame.Create(info.Stream);
            Console.OutputEncoding = Encoding.UTF8;

            JSerializerOptions.WriteIndented = true;
            JSerializerOptions.IgnoreReadOnlyFields = true;
            JSerializerOptions.IgnoreReadOnlyProperties = true;
            JSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
        }

        private ChatRecordList? current_session_record;
        
        public partial class ConfigType : ObservableObject
        {
            [ObservableProperty]
            private string _API_KEY;
            partial void OnAPI_KEYChanged(string value)
            {
                SaveConfig();
            }

            [ObservableProperty]
            private double temperature;
            partial void OnTemperatureChanged(double value)
            {
                SaveConfig();
            }

            [ObservableProperty]
            private int seed;
            partial void OnSeedChanged(int value)
            {
                SaveConfig();
            }

            [ObservableProperty]
            private bool enableMarkdown;
            partial void OnEnableMarkdownChanged(bool value)
            {
                SaveConfig();
            }

            [Obsolete("Python plugin is removed. This config does nothing now.")]
            public bool PluginPythonEnable { get => false; set { } }

            [JsonIgnore]
            public ObservableCollection<ModelInfo> ModelOptions { get; } = [];
            [JsonIgnore]
            public ObservableCollection<ModelVersionInfo> ModelVersionOptions { get; } = [];

            [ObservableProperty]
            [NotifyPropertyChangedFor(nameof(SelectedModelType))]
            private int selectedModelIndex;
            partial void OnSelectedModelIndexChanged(int value)
            {
                if (ModelOptions.Count == 0)
                {
                    SelectedModelIndex = -1;
                }
                else if (value < 0 || value >= ModelOptions.Count)
                {
                    SelectedModelIndex = 0;
                }

                SaveConfig();
                UpdateModelVersionList();
            }
            private void UpdateModelVersionList()
            {
                ModelVersionOptions.Clear();
                
                if (SelectedModelType is not null)
                {
                    var models = from model in ModelVersionInfo.VersionList
                                 where model.ModelType == SelectedModelType.Name
                                 select model;
                    foreach (var model in models)
                    {
                        ModelVersionOptions.Add(model);
                    }
                }
                
                SelectedModelVersionIndex = 0;
            }

            [ObservableProperty]
            private int selectedModelVersionIndex;
            partial void OnSelectedModelVersionIndexChanged(int value)
            {
                if (ModelVersionOptions.Count == 0)
                {
                    SelectedModelVersionIndex = -1;
                }
                else if (value < 0 || value >= ModelVersionOptions.Count)
                {
                    SelectedModelVersionIndex = 0;
                }
                SaveConfig();
            }

            public ModelInfo? SelectedModelType => 
                (SelectedModelIndex >= 0 && SelectedModelIndex < ModelOptions.Count) ?
                ModelOptions[SelectedModelIndex] : null;

            public ModelVersionInfo? SelectedModel => 
                (SelectedModelVersionIndex >= 0 && SelectedModelVersionIndex < ModelVersionOptions.Count) ? 
                ModelVersionOptions[SelectedModelVersionIndex] : null;

            public ConfigType()
            {
                _API_KEY = "";
                temperature = 1.0;
                seed = 0;
                enableMarkdown = false;
                selectedModelIndex = 0;
                selectedModelVersionIndex = 0;

                ModelOptions.Clear();
                foreach (var opt in ModelInfo.ModelList)
                {
                    ModelOptions.Add(opt);
                }
                UpdateModelVersionList();
            }
            private void SaveConfig()
            {
                File.WriteAllText("config.json", JsonSerializer.Serialize(this, JSerializerOptions));
            }
        }

        public class ImageInfo
        {
            public string Base64Data { get; set; } = "";
            public int Index { get; set; } = 0;
            public BitmapImage Image => Utils.Base64ToBitmapImage(Base64Data);
        }
        public ObservableCollection<ImageInfo> ChatHistoryImages { get; set; } = [];

        [ObservableProperty]
        private ConfigType config = new();
        private bool first_input = true;

        private void ResetChatHistoryImages()
        {
            ChatHistoryImages.Clear();
            if (current_session_record is not null)
            {
                foreach (var record in current_session_record.ChatRecords)
                {
                    foreach (var img in record.Images)
                    {
                        ChatHistoryImages.Add(new ImageInfo { Base64Data = img.Data, Index = ChatHistoryImages.Count });
                    }
                }

            }

            var imgs = from string filename in lst_attachment.Items
                       let mime = MimeTypes.GetMimeType(filename)
                       where mime.StartsWith("image/")
                       select Utils.ImageFileToBase64(filename);
            foreach(var base64 in imgs)
            {
                ChatHistoryImages.Add(new ImageInfo { Base64Data = base64, Index = ChatHistoryImages.Count });
            }
        }
        private void ResetSession(ChatRecordList? loaded_session = null)
        {
            Console.Write(string.Concat(Esc, "[3J")); // need this to clear whole screen in the new windows terminal, until https://github.com/dotnet/runtime/issues/28355 get fixed
            Console.Clear();

            loaded_session ??= new ChatRecordList(InitialPrompts?.SelectedOption, Config.SelectedModel?.KnowledgeCutoff ?? DateTime.Now);
            current_session_record = loaded_session;
            Console.WriteLine(new string('=', Console.WindowWidth));
            foreach (var record in current_session_record.ChatRecords)
            {
                record.Display(Config.EnableMarkdown);
            }

            ResetChatHistoryImages();
        }
        private async Task Send()
        {
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Config.API_KEY);
            if (Config.SelectedModel is null)
            {
                throw new ArgumentNullException(nameof(Config.SelectedModel));
            }
            var msg = new JsonObject
            {
                ["model"] = Config.SelectedModel.Name,
                ["messages"] = current_session_record!.ToJson(),
                ["temperature"] = Config.Temperature,
                ["stream"] = true,
                ["seed"] = Config.Seed,
            };

            if (Config.SelectedModel.Name.Contains("vision"))
            {
                msg["max_tokens"] = 4096;
            }

            var post_content = new StringContent(msg.ToString(), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://api.openai.com/v1/chat/completions"),
                Content = post_content
            };
            NetStatus.Status = NetStatusType.StatusEnum.Sending;
            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            NetStatus.Status = NetStatusType.StatusEnum.Receiving;
            StringBuilder response_sb = new();
            ChatRecord response_record = new(ChatRecord.ChatType.Bot, "");
            Console.Write(response_record.GetHeader());
            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                using var reader = new StreamReader(responseStream);
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    if (string.IsNullOrEmpty(line))
                    {
                        continue;
                    }
                    if (line == "data: [DONE]")
                    {
                        break; // 结束聊天响应数据接收
                    }
                    if (line.StartsWith("data: "))
                    {
                        var chatResponse = line["data: ".Length..];
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
                        string? system_fingerprint = responseJson?["system_fingerprint"]?.ToString();
                        NetStatus.SystemFingerprint = system_fingerprint ?? "";
                    }
                }
            }
            else
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                using var reader = new StreamReader(responseStream);
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
            Console.WriteLine();
            response_record.Content = response_sb.ToString();
            current_session_record.ChatRecords.Add(response_record);
            NetStatus.Status = NetStatusType.StatusEnum.Idle;
        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (current_session_record is null)
            {
                ResetSession();
            }

            StringBuilder input_txt = new();
            input_txt.Append(txtbx_input.Text);

            var txtfiles = from string file in lst_attachment.Items
                           let mime = MimeTypes.GetMimeType(file)
                           where mime.StartsWith("text/")
                           select file;
            int text_attachment_count = 0;
            foreach (var file in txtfiles)
            {
                text_attachment_count += 1;
                input_txt.AppendLine("");
                input_txt.AppendLine($"Attachment {text_attachment_count}: ");
                var file_content = await File.ReadAllTextAsync(file);
                input_txt.AppendLine(file_content);
            }

            bool highres = chk_highres_img.IsChecked ?? false;
            var new_user_input = new ChatRecord(ChatRecord.ChatType.User, input_txt.ToString(), highresimage: highres);

            var imgfiles = from string file in lst_attachment.Items
                           let mime = MimeTypes.GetMimeType(file)
                           where mime.StartsWith("image/")
                           select file;
            foreach (var img in imgfiles)
            {
                new_user_input.AddImageFromFile(img);
            }

            new_user_input.Display(Config.EnableMarkdown);
            current_session_record!.ChatRecords.Add(new_user_input);

            await Send();

            txtbx_input.Text = "";
            lst_attachment.Items.Clear();
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
                var parsed_config = JsonSerializer.Deserialize<ConfigType>(saved_config);
                if (parsed_config is not null)
                {
                    Config = parsed_config;
                }
            }

            // setup initial prompts
            if (File.Exists("initial_prompts.json"))
            {
                string saved_prompts = File.ReadAllText("initial_prompts.json");
                var parsed_prompts = JsonSerializer.Deserialize<InitialPromptsType>(saved_prompts);
                if (parsed_prompts is not null)
                {
                    InitialPrompts = parsed_prompts;
                }
            }
            if (InitialPrompts is null || !InitialPrompts.PromptsOptions.Any())
            {
                InitialPrompts = new();
                InitialPrompts.UseDefaultPromptList();
                File.WriteAllText("initial_prompts.json", JsonSerializer.Serialize(InitialPrompts, JSerializerOptions));
            }
            InitialPrompts.SelectedOption = InitialPrompts.PromptsOptions[0];
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string saved_session = JsonSerializer.Serialize(current_session_record, JSerializerOptions);
            var dlg = new SaveFileDialog
            {
                FileName = "session",
                DefaultExt = ".json",
                Filter = "JSON documents|*.json",
                ClientGuid = new Guid("32F3FF84-A923-4D69-9ABD-11DC14074AC6"),
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
                Filter = "JSON documents|*.json",
                ClientGuid = new Guid("32F3FF84-A923-4D69-9ABD-11DC14074AC6"),
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
                Filter = "Any Files|*.*",
                ClientGuid = new Guid("B8F42507-693B-4713-8671-A76F02ED5ADB"),
            };
            if (dlg.ShowDialog() == true)
            {
                lst_attachment.Items.Add(dlg.FileName);
            }
            ResetChatHistoryImages();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            lst_attachment.Items.Remove(lst_attachment.SelectedItem);
            ResetChatHistoryImages();
        }

        private void ContentControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var img = ChatHistoryImages[lst_images.SelectedIndex].Image;
            var img_viewer = new ImageViewer();
            img_viewer.ShowImage(img);
            img_viewer.Show();
        }

        private static readonly Random random = new();
        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            Config.Seed = random.Next();
        }
    }
}
