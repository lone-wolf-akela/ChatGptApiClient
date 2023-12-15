using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ChatGptApiClientV2
{
    public partial class PromptsOption : ObservableObject
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
        public static PromptsOption FromMsgList(List<IMessage> msgList)
        {
            var promptsOption = new PromptsOption();
            foreach (var msg in msgList)
            {
                promptsOption.Messages.Add(msg);
            }
            return promptsOption;
        }
        public PromptsOption()
        {
            Messages.CollectionChanged += (sender, e) =>
            {
                OnPropertyChanged(nameof(Text));
            };
        }
    }
    public partial class InitialPrompts : ObservableObject
    {
        public ObservableCollection<PromptsOption> PromptsOptions { get; } = [];
        [ObservableProperty]
        private PromptsOption? selectedOption;

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
            PromptsOptions.Add(PromptsOption.FromMsgList(GenerateSystemMessageList(prompt1)));
            PromptsOptions.Add(PromptsOption.FromMsgList(GenerateSystemMessageList(prompt2)));
        }
    }
    public partial class PluginInfo(IToolCollection p) : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Name))]
        private IToolCollection plugin = p;
        public string Name => Plugin.DisplayName;
        [ObservableProperty]
        private bool isEnabled = false;
    }
    public partial class SystemState : ObservableObject
    {
        public Config Config { get; } = Config.LoadConfig();
        
        public NetStatus NetStatus { get; } = new();
        public ObservableCollection<PluginInfo> Plugins { get; } = [];
        public Dictionary<string, IToolFunction> PluginLookUpTable { get; } = [];

        [ObservableProperty]
        private InitialPrompts? initialPrompts = null;

        public SystemState()
        {
            foreach (var plugin in AllToolCollections.ToolList)
            {
                Plugins.Add(new(plugin));
                foreach (var func in plugin.Funcs)
                {
                    PluginLookUpTable[func.Name] = func;
                }
            }

            /***** setup initial prompts *****/
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
                    var parsed_prompts = JsonConvert.DeserializeObject<InitialPrompts>(saved_prompts, settings);
                    if (parsed_prompts is not null)
                    {
                        InitialPrompts = parsed_prompts;
                    }
                }
                catch (JsonSerializationException exception)
                {
                    HandyControl.Controls.MessageBox.Show($"Error: Invalid initial prompts file: {exception.Message}");
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
                    NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
                    TypeNameHandling = TypeNameHandling.Auto,
                };
                var promptsJson = JsonConvert.SerializeObject(InitialPrompts, settings);

                File.WriteAllText("initial_prompts.json", promptsJson);
            }
            InitialPrompts.SelectedOption = InitialPrompts.PromptsOptions[0];
            /*** end setup initial prompts ***/
        }

        private ChatCompletionRequest? currentSession;
        public delegate void ChatSessionChangedHandler(ChatCompletionRequest session);
        public event ChatSessionChangedHandler? ChatSessionChangedEvent;
        private ChatCompletionRequest ResetSession(ChatCompletionRequest? loadedSession = null)
        {
            loadedSession ??= ChatCompletionRequest.BuildFromInitPrompts(InitialPrompts?.SelectedOption?.Messages, Config.SelectedModel?.KnowledgeCutoff ?? DateTime.Now);
            currentSession = loadedSession;

            if (ChatSessionChangedEvent is not null)
            {
                ChatSessionChangedEvent(currentSession);
            }

            return currentSession;
        }
        private readonly HttpClient client = new();
        private static readonly Random random = new();

        public delegate void NewMessageHandler(RoleType role);
        public delegate void StreamTextHandler(string text);
        public event NewMessageHandler? NewMessageEvent;
        public event StreamTextHandler? StreamTextEvent;
        public void NewMessage(RoleType role)
        {
            NewMessageEvent?.Invoke(role);
        }
        public void StreamText(string text)
        {
            StreamTextEvent?.Invoke(text);
        }
        private async Task Send()
        {
            client.DefaultRequestHeaders.Authorization = Config.ServiceProvider switch
            {
                Config.ServiceProviderType.Azure => null,
                _ => new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Config.API_KEY),
            };
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
                HashSet<string> addedTools = [];

                chatRequest.Tools = [];
                foreach (var plugin in enabled_plugins)
                {
                    foreach (var func in plugin.Funcs)
                    {
                        if (!addedTools.Contains(func.Name))
                        {
                            chatRequest.Tools.Add(func.GetToolRequest());
                            addedTools.Add(func.Name);
                        }
                    }
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
                RequestUri = new Uri(Config.OpenAIChatServiceURL),
                Content = post_content
            };
            if (Config.ServiceProvider == Config.ServiceProviderType.Azure)
            {
                request.Headers.Add("api-key", Config.AzureAPIKey);
            }
            NetStatus.Status = NetStatus.StatusEnum.Sending;
            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            NetStatus.Status = NetStatus.StatusEnum.Receiving;

            List<ChatCompletionChunk> chatChunks = [];
            NewMessage(RoleType.Assistant);

            string? errorMsg = null;
            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                using var reader = new StreamReader(responseStream);
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    if (string.IsNullOrEmpty(line)) { continue; }
                    if (!line.StartsWith("data: ")) { continue; }

                    var chatResponse = line["data: ".Length..];
                    if (chatResponse == "[DONE]") { break; }
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
                        if (chatChunk is not null)
                        {
                            chatChunks.Add(chatChunk);
                        }
                    }
                    catch (JsonSerializationException exception)
                    {
                        errorMsg = $"Error: Invalid chat response: {exception.Message}";
                        break;
                    }

                    var chunk = chatChunks.Last();
                    var choices = chunk?.Choices;
                    if (choices?.Count > 0)
                    {
                        string? ch = choices[0].Delta.Content;
                        if (ch is not null)
                        {
                            StreamText(ch);
                        }
                    }
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { })); // this is needed to allow ui update

                    var fingerprint = chunk?.SystemFingerprint;
                    if (!string.IsNullOrEmpty(fingerprint))
                    {
                        NetStatus.SystemFingerprint = chunk?.SystemFingerprint ?? "";
                    }
                }
            }
            else
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                using var reader = new StreamReader(responseStream);
                var chatResponse = await reader.ReadToEndAsync();
                try
                {
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
                catch (JsonReaderException)
                {
                    errorMsg = $"Error: {chatResponse}";
                }
            }

            var chatCompletion = ChatCompletion.FromChunks(chatChunks);
            NetStatus.Status = NetStatus.StatusEnum.Idle;
            NetStatus.SystemFingerprint = chatCompletion.SystemFingerprint;

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
                var plugin = PluginLookUpTable[plugin_name] ?? throw new InvalidDataException($"plugin not found: {plugin_name}");
                var toolMsg = await plugin.Action(this, args);
                toolMsg.ToolCallId = toolcall.Id;
                currentSession.Messages.Add(toolMsg);
                toolcalled = true;
            }

            if (toolcalled)
            {
                await Send();
            }
        }
        public async Task UserSendText(string text, IEnumerable<string> files)
        {
            currentSession ??= ResetSession();

            if (Config.UseRandomSeed)
            {
                Config.Seed = random.Next();
            }

            StringBuilder inputTxt = new();
            inputTxt.Append(text);

            var txtfiles = from file in files
                           let mime = MimeTypes.GetMimeType(file)
                           where mime.StartsWith("text/")
                           select file;
            int textAttachmentCount = 0;
            foreach (var file in txtfiles)
            {
                textAttachmentCount += 1;
                inputTxt.AppendLine("");
                inputTxt.AppendLine($"Attachment {textAttachmentCount}: ");
                inputTxt.AppendLine(await File.ReadAllTextAsync(file));
            }

            var textContent = new IMessage.TextContent { Text = inputTxt.ToString() };
            var contentList = new List<IMessage.IContent> { textContent };

            var imgfiles = from file in files
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
                        Detail = Config.UploadHiresImage
                            ? IMessage.ImageContent.ImageUrlType.ImageDetail.High
                            : IMessage.ImageContent.ImageUrlType.ImageDetail.Low
                    }
                };
                contentList.Add(imgContent);
            }

            var userMsg = new UserMessage
            {
                Content = contentList,
                Name = string.IsNullOrEmpty(Config.UserNickName) ? null : Config.UserNickName
            };
            var msgList = new List<IMessage> { userMsg };

            currentSession!.Messages.Add(userMsg);
            ResetSession(currentSession);

            await Send();

            ResetSession(currentSession);
        }

        public void SaveSession()
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
        public void LoadSession()
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
                        HandyControl.Controls.MessageBox.Show("Error: Invalid session file.");
                        return;
                    }
                    ResetSession(loadedSession);
                }
                catch (JsonSerializationException exception)
                {
                    HandyControl.Controls.MessageBox.Show($"Error: Invalid session file: {exception.Message}");
                    return ;
                }
            }
        }
        public void ClearSession()
        {
            ResetSession();
        }
        public void RefreshSession()
        {
            ResetSession(currentSession);
        }
    }
}
