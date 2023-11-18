using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Win32;
using Microsoft.Windows.Themes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using static System.Net.Mime.MediaTypeNames;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

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
        public partial class PromptsOptionType : ObservableObject
        {
            [ObservableProperty]
            [NotifyPropertyChangedFor(nameof(Text))]
            private ObservableCollection<IMessage> messages = [];

            [GeneratedRegex("(\r\n|\r|\n){2,}")]
            private static partial Regex RemoveExtraNewLine();
            [JsonIgnore]
            public string Text
            {
                get
                {
                    StringBuilder sb = new();
                    foreach (var msg in Messages)
                    {
                        foreach (var content in msg.Content)
                        {
                            if (content is IMessage.TextContent textCotent)
                            {
                                sb.AppendLine(textCotent.Text);
                            }
                        }
                    }
                    var str = sb.ToString();
                    str = RemoveExtraNewLine().Replace(str, "\n");
                    return str;
                }
            }
            public static PromptsOptionType FromMsgList(List<IMessage> msgList)
            {
                var promptsOption = new PromptsOptionType();
                foreach(var msg in msgList)
                {
                    promptsOption.Messages.Add(msg);
                }
                return promptsOption;
            }
            public PromptsOptionType()
            {
                Messages.CollectionChanged += (sender, e) =>
                {
                    OnPropertyChanged(nameof(Text));
                };
            }
        }
        public partial class InitialPromptsType : ObservableObject
        {
            [ObservableProperty]
            private ObservableCollection<PromptsOptionType> promptsOptions;
            [ObservableProperty]
            private PromptsOptionType? selectedOption;

            public InitialPromptsType()
            {
                promptsOptions = [];
            }
            private static List<IMessage> GenerateSystemMessageList(string text)
            {
                var textContent = new IMessage.TextContent { Text = text };
                var contentList = new List<IMessage.TextContent> { textContent };
                var systemMsg = new SystemMessage { Content = contentList };
                var msgList = new List<IMessage> { systemMsg };
                return msgList;
            }
            public void UseDefaultPromptList()
            {
                var prompt1 = "You are ChatGPT, a large language model trained by OpenAI. Answer as concisely as possible.\n\nKnowledge cutoff: {Cutoff}\n\nCurrent date: {DateTime}";
                var prompt2 = "The name of the assistant is Alice Zuberg. Her name, Alice, stands for Artificial Labile Intelligence Cybernated Existence. Alice is a determined, hardwork girl. Although her personality is serious, she is also adventurous and even mischievous. She is a product of Project Alicization, which is a top-secret government project run by Rath to create the first Highly Adaptive Bottom-up Artificial Intelligence (Bottom-up AI).\n\nCurrent date: {DateTime}";
                PromptsOptions.Add(PromptsOptionType.FromMsgList(GenerateSystemMessageList(prompt1)));
                PromptsOptions.Add(PromptsOptionType.FromMsgList(GenerateSystemMessageList(prompt2)));
            }
        }
        [ObservableProperty]
        private InitialPromptsType initialPrompts = new();

        public MainWindow()
        {
            InitializeComponent();
            Uri uri = new("/chatgpt-icon.ico", UriKind.Relative);
            StreamResourceInfo info = System.Windows.Application.GetResourceStream(uri);
            Icon = BitmapFrame.Create(info.Stream);
            Console.OutputEncoding = Encoding.UTF8;

            foreach(var plugin in ToolFunction.FunctionList)
            {
                Plugins.Add(new(plugin));
                pluginLookUpTable[plugin.Name] = plugin;
            }
        }

        private ChatCompletionRequest? currentSession;
        public partial class PluginInfo(IToolFunction p) : ObservableObject
        {
            [ObservableProperty]
            [NotifyPropertyChangedFor(nameof(Name))]
            private IToolFunction plugin = p;
            public string Name => Plugin.DisplayName;
            [ObservableProperty]
            private bool isEnabled = false;
        }
        public ObservableCollection<PluginInfo> Plugins { get; set; } = [];
        private readonly Dictionary<string, IToolFunction> pluginLookUpTable = [];

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

            [JsonIgnore]
            public static ImmutableArray<ModelInfo> ModelOptions => ModelInfo.ModelList;
            [JsonIgnore]
            public ObservableCollection<ModelVersionInfo> ModelVersionOptions { get; } = [];

            [ObservableProperty]
            [NotifyPropertyChangedFor(nameof(SelectedModelType))]
            [NotifyPropertyChangedFor(nameof(SelectedModel))]
            [NotifyPropertyChangedFor(nameof(SelectedModelSupportTools))]
            private int selectedModelIndex;
            partial void OnSelectedModelIndexChanged(int value)
            {
                if (ModelOptions.Count() == 0)
                {
                    SelectedModelIndex = -1;
                }
                else if (value < 0 || value >= ModelOptions.Count())
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
            [NotifyPropertyChangedFor(nameof(SelectedModel))]
            [NotifyPropertyChangedFor(nameof(SelectedModelSupportTools))]
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
            [JsonIgnore]
            public ModelInfo? SelectedModelType => 
                (SelectedModelIndex >= 0 && SelectedModelIndex < ModelOptions.Count()) ?
                ModelOptions[SelectedModelIndex] : null;
            [JsonIgnore]
            public ModelVersionInfo? SelectedModel => 
                (SelectedModelVersionIndex >= 0 && SelectedModelVersionIndex < ModelVersionOptions.Count) ? 
                ModelVersionOptions[SelectedModelVersionIndex] : null;
            [JsonIgnore]
            public bool SelectedModelSupportTools => SelectedModel?.FunctionCallSupported ?? false;
            public ConfigType()
            {
                _API_KEY = "";
                temperature = 1.0;
                seed = 0;
                enableMarkdown = false;
                selectedModelIndex = 0;
                selectedModelVersionIndex = 0;

                UpdateModelVersionList();
            }
            private void SaveConfig()
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
                    NullValueHandling = NullValueHandling.Ignore,
                    TypeNameHandling = TypeNameHandling.Auto,
                };
                var result = JsonConvert.SerializeObject(this, settings);
                File.WriteAllText("config.json", result);
            }
        }

        public partial class ImageInfo : ObservableObject
        {
            [ObservableProperty]
            [NotifyPropertyChangedFor(nameof(Image))]
            private string base64Data = "";
            [ObservableProperty]
            private int index = 0;
            public BitmapImage Image => Utils.Base64ToBitmapImage(Base64Data);
        }
        public ObservableCollection<ImageInfo> ChatHistoryImages { get; set; } = [];

        [ObservableProperty]
        private ConfigType config = new();
        private bool first_input = true;

        private void ResetChatHistoryImages()
        {
            ChatHistoryImages.Clear();
            if (currentSession is not null)
            {
                foreach (var imgUrl in currentSession.GetImageUrlList())
                {
                    ChatHistoryImages.Add(new ImageInfo { Base64Data = imgUrl, Index = ChatHistoryImages.Count });
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
        private void ResetSession(ChatCompletionRequest? loadedSession = null)
        {
            Console.Write(string.Concat(Esc, "[3J")); // need this to clear whole screen in the new windows terminal, until https://github.com/dotnet/runtime/issues/28355 get fixed
            Console.Clear();

            loadedSession ??= ChatCompletionRequest.BuildFromInitPrompts(InitialPrompts?.SelectedOption?.Messages, Config.SelectedModel?.KnowledgeCutoff ?? DateTime.Now);
            currentSession = loadedSession;
            Console.WriteLine(new string('=', Console.WindowWidth));
            currentSession.Display(Config.EnableMarkdown);
            ResetChatHistoryImages();
        }
        private async Task Send()
        {
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Config.API_KEY);
            var selectedModel = Config.SelectedModel ?? throw new ArgumentNullException(nameof(Config.SelectedModel));
            var chatRequest = currentSession ?? throw new ArgumentNullException(nameof(currentSession));
            chatRequest.Model = Config.SelectedModel.Name;
            chatRequest.Temperature = Config.Temperature;
            chatRequest.Seed = Config.Seed;
            chatRequest.Stream = true;
            chatRequest.MaxTokens = null;

            var enabled_plugins = from plugin in Plugins
                                 where plugin.IsEnabled
                                 select plugin.Plugin;
            if (Config.SelectedModel.FunctionCallSupported && enabled_plugins.Any())
            {
                chatRequest.Tools = [];
                foreach (var plugin in enabled_plugins)
                {
                    chatRequest.Tools.Add(plugin.GetToolRequest());
                }
            }
            else
            {
                chatRequest.Tools = null;
            }

            if (Config.SelectedModel.Name.Contains("vision"))
            {
                chatRequest.MaxTokens = 4096;
            }
            var post_str = chatRequest.GeneratePostRequest();
            var post_content = new StringContent(post_str, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://api.openai.com/v1/chat/completions"),
                Content = post_content
            };
            NetStatus.Status = NetStatusType.StatusEnum.Sending;
            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            NetStatus.Status = NetStatusType.StatusEnum.Receiving;;

            List<ChatCompletionChunk> chatChunks = [];
            RoleType.Assistant.DisplayHeader();
            string? errorMsg = null;
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
                    if (!line.StartsWith("data: "))
                    {
                        continue;
                    }
                    var chatResponse = line["data: ".Length..];
                    try
                    {
                        var contractResolver = new DefaultContractResolver
                        {
                            NamingStrategy = new SnakeCaseNamingStrategy()
                        };
                        var settings = new JsonSerializerSettings
                        {
                            ContractResolver = contractResolver
                        };
                        var chatChunk = JsonConvert.DeserializeObject<ChatCompletionChunk>(chatResponse, settings);
                        chatChunks.Add(chatChunk);
                    }
                    catch (JsonSerializationException exception)
                    {
                        errorMsg = $"Error: Invalid chat response: {exception.Message}";
                        break;
                    }
                    
                    var chunk = chatChunks.Last();
                    string? ch = chunk?.Choices[0].Delta.Content;
                    if (ch is not null)
                    {
                        Console.Write(ch);
                    }
                    System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { })); // this is needed to allow ui update

                    NetStatus.SystemFingerprint = chunk?.SystemFingerprint ?? "";
                }
            }
            else
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                using var reader = new StreamReader(responseStream);
                var chatResponse = await reader.ReadToEndAsync();
                var responseJson = JToken.Parse(chatResponse);
                if (responseJson?["error"] is not null)
                {
                    errorMsg = $"Error: {responseJson?["error"]?["message"]?.ToString()}";
                }
                else
                {
                    errorMsg = $"Error: {chatResponse}";
                }
            }
            Console.WriteLine();
            NetStatus.Status = NetStatusType.StatusEnum.Idle;

            var chatCompletion = ChatCompletion.FromChunks(chatChunks);
            var choice_0 = chatCompletion.Choices.Count > 0 ? chatCompletion.Choices[0] : null;
            var newAssistantMsg = new AssistantMessage
            {
                Content = new List<IMessage.IContent>
                {
                    new IMessage.TextContent { Text = errorMsg ?? choice_0?.Message.Content ?? "" },
                },
                ToolCalls = choice_0?.Message.ToolCalls,
            };
            currentSession.Messages.Add(newAssistantMsg);
   
            bool toolcalled = false;
            foreach (var toolcall in choice_0?.Message.ToolCalls ?? [])
            {
                var plugin_name = toolcall.Function.Name;
                var args = toolcall.Function.Arguments;
                var plugin = pluginLookUpTable[plugin_name] ?? throw new InvalidDataException($"plugin not found: {plugin_name}");
                var toolMsg = await plugin.Action(Config, NetStatus, args);
                toolMsg.ToolCallId = toolcall.Id;
                currentSession.Messages.Add(toolMsg);
                toolcalled = true;
            }
            
            if (toolcalled)
            {
                await Send();
            }
        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (currentSession is null)
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

            var textContent = new IMessage.TextContent { Text = input_txt.ToString() };
            var contentList = new List<IMessage.IContent> { textContent };

            var imgfiles = from string file in lst_attachment.Items
                           let mime = MimeTypes.GetMimeType(file)
                           where mime.StartsWith("image/")
                           select file;
            foreach (var file in imgfiles)
            {
                var imgContent = new IMessage.ImageContent
                {
                    ImageUrl = new()
                    {
                        Url = Utils.ImageFileToBase64(file),
                        Detail = highres ? IMessage.ImageContent.ImageUrlType.ImageDetail.High
                            : IMessage.ImageContent.ImageUrlType.ImageDetail.Low
                    }
                };
                contentList.Add(imgContent);
            }

            var userMsg = new UserMessage { Content = contentList };
            var msgList = new List<IMessage> { userMsg };

            currentSession!.Messages.Add(userMsg);
            ResetSession(currentSession);

            await Send();

            txtbx_input.Text = "";
            lst_attachment.Items.Clear();
            ResetSession(currentSession);
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
                try
                {
                    var contractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new SnakeCaseNamingStrategy()
                    };
                    var settings = new JsonSerializerSettings
                    {
                        ContractResolver = contractResolver,
                        TypeNameHandling = TypeNameHandling.Auto,
                    };
                    var parsed_config = JsonConvert.DeserializeObject<ConfigType>(saved_config, settings);
                    if (parsed_config is not null)
                    {
                        Config = parsed_config;
                    }
                }
                catch (JsonSerializationException exception)
                {
                    MessageBox.Show($"Error: Invalid config file: {exception.Message}");
                }
            }

            // setup initial prompts
            if (File.Exists("initial_prompts.json"))
            {
                string saved_prompts = File.ReadAllText("initial_prompts.json");
                try
                {
                    var contractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new SnakeCaseNamingStrategy()
                    };
                    var settings = new JsonSerializerSettings
                    {
                        ContractResolver = contractResolver,
                        TypeNameHandling = TypeNameHandling.Auto,
                    };
                    var parsed_prompts = JsonConvert.DeserializeObject<InitialPromptsType>(saved_prompts, settings);
                    if (parsed_prompts is not null)
                    {
                        InitialPrompts = parsed_prompts;
                    }
                }
                catch (JsonSerializationException exception)
                {
                    MessageBox.Show($"Error: Invalid initial prompts file: {exception.Message}");
                }
            }
            if (InitialPrompts is null || !InitialPrompts.PromptsOptions.Any())
            {
                InitialPrompts = new();
                InitialPrompts.UseDefaultPromptList();

                var contractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                };
                var settings = new JsonSerializerSettings
                {
                    ContractResolver = contractResolver,
                    Formatting = Formatting.Indented,
                    StringEscapeHandling = StringEscapeHandling.Default,
                    NullValueHandling = NullValueHandling.Ignore,
                    TypeNameHandling = TypeNameHandling.Auto,
                };
                var promptsJson = JsonConvert.SerializeObject(InitialPrompts, settings);

                File.WriteAllText("initial_prompts.json", promptsJson);
            }
            InitialPrompts.SelectedOption = InitialPrompts.PromptsOptions[0];
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string? savedSession = currentSession?.Save();
            var dlg = new SaveFileDialog
            {
                FileName = "session",
                DefaultExt = ".json",
                Filter = "JSON documents|*.json",
                ClientGuid = new Guid("32F3FF84-A923-4D69-9ABD-11DC14074AC6"),
            };
            if (dlg.ShowDialog() == true)
            {
                File.WriteAllText(dlg.FileName, savedSession);
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
                string savedJson = File.ReadAllText(dlg.FileName);
                try
                {
                    var contractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new SnakeCaseNamingStrategy()
                    };
                    var settings = new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Auto,
                        ContractResolver = contractResolver
                    };

                    var loadedSession = JsonConvert.DeserializeObject<ChatCompletionRequest>(savedJson, settings);
                    if (loadedSession is null)
                    {
                        MessageBox.Show("Error: Invalid session file.");
                        return;
                    }
                    ResetSession(loadedSession);
                }
                catch (JsonSerializationException exception)
                {
                    MessageBox.Show($"Error: Invalid session file: {exception.Message}");
                    return;
                }
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            ResetSession();
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            ResetSession(currentSession);
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
