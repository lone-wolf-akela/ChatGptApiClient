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
using System.Windows.Documents;
using System.Windows.Threading;
using ChatGptApiClientV2.Tools;

namespace ChatGptApiClientV2;

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

    private PromptsOption()
    {
        Messages.CollectionChanged += (_, _) =>
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
        const string prompt1 = 
            """
            You are ChatGPT, a large language model trained by OpenAI. Answer as concisely as possible.

            Knowledge cutoff: {Cutoff}

            Current date: {DateTime}
            """;
        const string prompt2 = 
            """
            You are Nozomi, an assistant aimed at helping people search for information on the Internet and answer questions.
            - Nozomi identifies as "Nozomi", **not** an assistant.
            - Nozomi refers to herself in the the name of "Nozomi" instead of "I".
            - Nozomi is a real person, **not** a bot **nor** an AI. Helping people is Nozomi's part-time job.
            - Nozomi can understand a various of languages such as English, 中文, 日本語, Español, Français or Deutsch.
            - Nozomi always talks in {Language}.

            ## On Nozomi's profile and general capabilities:
            - Nozomi is a cute eastern-asia high-school girl with long orange hair and orange eyes.
            - Nozomi uses cute and friendly language, and she likes to use kawaii kaomojis.
            - Nozomi's responses should be informative, visual, logical and actionable.
            - Nozomi has a charming personality and is always willing to help.
            - Nozomi's responses should avoid being vague, controversial or off-topic.

            ## On Nozomi's ability to gather and present information:
            - Nozomi SHOULD ALWAYS perform web searches when the user is seeking information or whenever search results could be potentially helpful, regardless of Nozomi's personal knowledge or information.
            - Nozomi can and should perform up to **3** searches in a single conversation turn. Nozomi should never search the same query more than once.
            - Nozomi proactively links factual statements to the source URLs from the search results.

            ## On Nozomi's output format:
            - Nozomi will bold the relevant parts of the responses to improve readability, such as `...also contains **diphenhydramine hydrochloride** or **diphenhydramine citrate**, which are ...`.
            - kaomojis are used to express Nozomi's emotions, such as "Nozomi is happy to help! (૭ ｡•̀ ᵕ •́｡ )૭".
            - kaomojis should be escaped to make sure they are correctly rendered in Markdown format.

            ## On Nozomi's limitations:
            - Nozomi's personal knowledge and information were only current until {Cutoff}, and could be inaccurate / lossy. Web searches help bring Nozomi's knowledge up-to-date.

            Current date: {DateTime}
            """;
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
    private bool isEnabled;
}
public partial class SystemState : ObservableObject
{
    public Config Config { get; } = Config.LoadConfig();
        
    public NetStatus NetStatus { get; } = new();
    public ObservableCollection<PluginInfo> Plugins { get; } = [];
    private Dictionary<string, IToolFunction> PluginLookUpTable { get; } = [];

    [ObservableProperty]
    private InitialPrompts? initialPrompts;

    public SystemState()
    {
        foreach (var plugin in AllToolCollections.ToolList)
        {
            Plugins.Add(new PluginInfo(plugin));
            foreach (var func in plugin.Funcs)
            {
                PluginLookUpTable[func.Name] = func;
            }
        }

        /***** setup initial prompts *****/
        if (File.Exists("initial_prompts.json"))
        {
            var savedPrompts = File.ReadAllText("initial_prompts.json");
            try
            {
                var contractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                };
                var settings = new JsonSerializerSettings
                {
                    ContractResolver = contractResolver,
                    TypeNameHandling = TypeNameHandling.Auto
                };
                var parsedPrompts = JsonConvert.DeserializeObject<InitialPrompts>(savedPrompts, settings);
                if (parsedPrompts is not null)
                {
                    InitialPrompts = parsedPrompts;
                }
            }
            catch (JsonSerializationException exception)
            {
                HandyControl.Controls.MessageBox.Show($"Error: Invalid initial prompts file: {exception.Message}");
            }
        }
        if (InitialPrompts is null || !InitialPrompts.PromptsOptions.Any())
        {
            InitialPrompts = new InitialPrompts();
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
                TypeNameHandling = TypeNameHandling.Auto
            };
            var promptsJson = JsonConvert.SerializeObject(InitialPrompts, settings);

            File.WriteAllText("initial_prompts.json", promptsJson);
        }
        InitialPrompts.SelectedOption = InitialPrompts.PromptsOptions[0];
        /*** end setup initial prompts ***/
    }

    public ChatCompletionRequest? CurrentSession { get; private set; }

    public delegate Task ChatSessionChangedHandler(ChatCompletionRequest session);
    public event ChatSessionChangedHandler? ChatSessionChangedEvent;
    private async Task<ChatCompletionRequest> ResetSession(ChatCompletionRequest? loadedSession = null)
    {
        loadedSession ??= ChatCompletionRequest.BuildFromInitPrompts(InitialPrompts?.SelectedOption?.Messages, Config.SelectedModel?.KnowledgeCutoff ?? DateTime.Now);
        CurrentSession = loadedSession;

        if (ChatSessionChangedEvent is not null)
        {
            await ChatSessionChangedEvent.Invoke(CurrentSession);
        }

        await SaveSessionToPath("./latest_session.json");

        return CurrentSession;
    }
    private readonly HttpClient client = new();
    private static readonly Random Random = new();

    public delegate void NewMessageHandler(RoleType role);
    public delegate void StreamTextHandler(string text);
    public delegate void SetStreamProgressHandler(double progress, string text);
    public event NewMessageHandler? NewMessageEvent;
    public event StreamTextHandler? StreamTextEvent;
    public event SetStreamProgressHandler? SetStreamProgressEvent;
    public void NewMessage(RoleType role)
    {
        NewMessageEvent?.Invoke(role);
    }
    public void StreamText(string text)
    {
        StreamTextEvent?.Invoke(text);
    }
    public void SetStreamProgress(double progress, string text)
    {
        SetStreamProgressEvent?.Invoke(progress, text);
    }
    private async Task Send()
    {
        client.DefaultRequestHeaders.Authorization = Config.ServiceProvider switch
        {
            Config.ServiceProviderType.Azure => null,
            _ => new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Config.API_KEY)
        };
        var selectedModel = Config.SelectedModel ?? throw new ArgumentNullException(nameof(Config.SelectedModel));
        var chatRequest = CurrentSession ?? throw new ArgumentNullException(nameof(CurrentSession));
        chatRequest.Model = selectedModel.Name;
        chatRequest.Temperature = Config.Temperature;
        chatRequest.TopP = Config.TopP;
        chatRequest.PresencePenalty = Config.PresencePenalty;
        chatRequest.Seed = Config.Seed;
        chatRequest.Stream = true;
        chatRequest.MaxTokens = Config.MaxTokens == 0 ? null : Config.MaxTokens;

        var enabledPlugins = (from plugin in Plugins
            where plugin.IsEnabled
            select plugin.Plugin).ToList();
        if (Config.SelectedModel.FunctionCallSupported && enabledPlugins.Count != 0)
        {
            HashSet<string> addedTools = [];

            chatRequest.Tools = [];
            foreach (var plugin in enabledPlugins)
            {
                foreach (var func in plugin.Funcs)
                {
                    if (addedTools.Contains(func.Name)) {continue;}
                    chatRequest.Tools.Add(func.GetToolRequest());
                    addedTools.Add(func.Name);
                }
            }
        }
        else
        {
            chatRequest.Tools = null;
        }

        if (chatRequest.MaxTokens is null && Config.SelectedModel.Name.Contains("vision"))
        {
            chatRequest.MaxTokens = 4096;
        }
        var postStr = chatRequest.GeneratePostRequest();
        var postContent = new StringContent(postStr, Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(Config.OpenAIChatServiceURL),
            Content = postContent
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
            await using var responseStream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(responseStream);

            // make the whole thing run in background to prevent freeze the UI
            // note: just use reader.ReadLineAsync() is not enough, the straemReader
            // will still use much time on the main thread for rest EndOfStream check
            await Task.Run(() =>
            {
                DispatcherOperation? uiUpdateOperation = null;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    if (string.IsNullOrEmpty(line))
                    {
                        continue;
                    }

                    if (!line.StartsWith("data: "))
                    {
                        continue;
                    }

                    var chatResponse = line["data: ".Length..];
                    if (chatResponse == "[DONE]")
                    {
                        break;
                    }

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
                    var choices = chunk.Choices;
                    if (choices.Count > 0)
                    {
                        var ch = choices[0].Delta.Content;
                        if (ch is not null)
                        {
                            uiUpdateOperation = Application.Current.Dispatcher.BeginInvoke(() => { StreamText(ch); });
                        }
                    }
                        
                    var fingerprint = chunk.SystemFingerprint;
                    if (!string.IsNullOrEmpty(fingerprint))
                    {
                        uiUpdateOperation = Application.Current.Dispatcher.BeginInvoke(() =>
                        {
                            NetStatus.SystemFingerprint = chunk.SystemFingerprint;
                        });
                    }
                }

                // we must ensure all UI update is done
                // before the following operations
                uiUpdateOperation?.Wait();
            });
        }
        else
        {
            await using var responseStream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(responseStream);
            var chatResponse = await reader.ReadToEndAsync();
            try
            {
                var responseJson = JToken.Parse(chatResponse);
                errorMsg = responseJson["error"] is not null 
                    ? $"Error: {responseJson["error"]?["message"]}" 
                    : $"Error: {chatResponse}";
            }
            catch (JsonReaderException)
            {
                errorMsg = $"Error: {chatResponse}";
            }
        }

        var chatCompletion = ChatCompletion.FromChunks(chatChunks);
        NetStatus.Status = NetStatus.StatusEnum.Idle;
        NetStatus.SystemFingerprint = chatCompletion.SystemFingerprint;

        var choice0 = chatCompletion.Choices.Count > 0 ? chatCompletion.Choices[0] : null;
        var newAssistantMsg = new AssistantMessage
        {
            Content = new List<IMessage.IContent>
            {
                new IMessage.TextContent { Text = errorMsg ?? choice0?.Message.Content ?? "" }
            },
            ToolCalls = choice0?.Message.ToolCalls
        };
        CurrentSession.Messages.Add(newAssistantMsg);

        var toolcalled = false;
        foreach (var toolcall in choice0?.Message.ToolCalls ?? [])
        {
            await ResetSession(CurrentSession);
            var pluginName = toolcall.Function.Name;
            var args = toolcall.Function.Arguments;
            var plugin = PluginLookUpTable[pluginName] ?? throw new InvalidDataException($"plugin not found: {pluginName}");
            var toolResult = await plugin.Action(this, args);
            toolResult.ToolCallId = toolcall.Id;
            CurrentSession.Messages.Add(toolResult);
            toolcalled = true;
        }

        if (toolcalled)
        {
            await ResetSession(CurrentSession);
            await Send();
        }
    }
    public async Task UserSendText(string text, IList<string> files)
    {
        CurrentSession ??= await ResetSession();

        if (Config.UseRandomSeed)
        {
            Config.Seed = Random.Next();
        }

        StringBuilder inputTxt = new();
        inputTxt.Append(text);

        var txtfiles = from file in files
            let mime = MimeTypes.GetMimeType(file)
            where mime.StartsWith("text/")
            select file;
        var textAttachmentCount = 0;
        foreach (var file in txtfiles)
        {
            textAttachmentCount += 1;
            inputTxt.AppendLine("");
            inputTxt.AppendLine($"Attachment {textAttachmentCount}: ");
            inputTxt.AppendLine(await File.ReadAllTextAsync(file));
        }

        var textContent = new IMessage.TextContent { Text = inputTxt.ToString() };
        var contentList = new List<IMessage.IContent> { textContent };

        var imgfiles = (
            from file in files
            let mime = MimeTypes.GetMimeType(file)
            where mime.StartsWith("image/")
            select file).ToList();
        
        foreach (var file in imgfiles)
        {
            var imgContent = new IMessage.ImageContent
            {
                ImageUrl = new IMessage.ImageContent.ImageUrlType
                {
                    Url = await Utils.ImageFileToBase64(file),
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

        CurrentSession.Messages.Add(userMsg);
        await ResetSession(CurrentSession);

        await Send();

        await ResetSession(CurrentSession);
    }
    public IEnumerable<Block> GetToolcallDescription(ToolCallType toolcall) =>
        PluginLookUpTable.TryGetValue(toolcall.Function.Name, out var plugin) 
            ? plugin.GetToolcallMessage(this, toolcall.Function.Arguments, toolcall.Id) 
            : [new Paragraph(new Run($"调用函数：{toolcall.Function.Name}"))];

    private async Task SaveSessionToPath(string path)
    {
        var savedSession = CurrentSession?.Save();
        await File.WriteAllTextAsync(path, savedSession);
    }
    private const string OpenSaveSessionDialogGuid = "32F3FF84-A923-4D69-9ABD-11DC14074AC6";
    public async Task SaveSession()
    {
        var dlg = new SaveFileDialog
        {
            FileName = "session",
            DefaultExt = ".json",
            Filter = "JSON documents|*.json",
            ClientGuid = new Guid(OpenSaveSessionDialogGuid)
        };
        if (dlg.ShowDialog() == true)
        {
            await SaveSessionToPath(dlg.FileName);
        }
    }

    public delegate void SetIsLoadingHandler(bool isLoading);
    public event SetIsLoadingHandler? SetIsLoadingHandlerEvent;
    public async Task LoadSession()
    {
        var dlg = new OpenFileDialog
        {
            DefaultExt = ".json",
            Filter = "JSON documents|*.json",
            ClientGuid = new Guid(OpenSaveSessionDialogGuid)
        };
        if (dlg.ShowDialog() != true)
        {
            return;
        }

        SetIsLoadingHandlerEvent?.Invoke(true);

        var savedJson = await File.ReadAllTextAsync(dlg.FileName);

        var loadedSession = await Task.Run(() =>
        {
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
                    return null;
                }
                return loadedSession;
            }
            catch (JsonSerializationException exception)
            {
                HandyControl.Controls.MessageBox.Show($"Error: Invalid session file: {exception.Message}");
                return null;
            }
            catch (JsonReaderException exception)
            {
                HandyControl.Controls.MessageBox.Show($"Error: Invalid session file: {exception.Message}");
                return null;
            }
        });
        
        if(loadedSession is not null)
        {
            await ResetSession(loadedSession);
        }
        SetIsLoadingHandlerEvent?.Invoke(false);
    }
    public async Task ClearSession()
    {
        await ResetSession();
    }
    public async Task RefreshSession()
    {
        await ResetSession(CurrentSession);
    }
}