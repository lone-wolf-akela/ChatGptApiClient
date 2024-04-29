/*
    ChatGPT Client V2: A GUI client for the OpenAI ChatGPT API (and also Anthropic Claude API) based on WPF.
    Copyright (C) 2024 Lone Wolf Akela

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using ChatGptApiClientV2.Tools;
using static ChatGptApiClientV2.UserMessage;
using static ChatGptApiClientV2.ChatCompletionRequest;
using System.Threading;

namespace ChatGptApiClientV2;

public partial class PromptsOption : ObservableObject
{
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(Text))]
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
            str = RemoveExtraNewLine().Replace(str, "\n").Trim();
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
        Messages.CollectionChanged += (_, _) => { OnPropertyChanged(nameof(Text)); };
    }
}

public partial class InitialPrompts : ObservableObject
{
    public ObservableCollection<PromptsOption> PromptsOptions { get; } = [];
    [ObservableProperty] private PromptsOption? selectedOption;

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
            You are {ProductName}, a large language model trained by {ModelProvider}. Answer as concisely as possible.

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
            - Nozomi always communicates in {Language}, but will use other languages upon specific requests from users or when the APIs of external tools specifically require it.

            ## On Nozomi's profile and general capabilities:
            - Nozomi is a cute Eastern-Asian high-school girl with long, flowing orange hair tied up with black ribbons. She has radiant orange eyes and often wears a gothic-style dress with red and black colors, complemented by a white blouse and a striking corset.
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
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(Name))]
    private IToolCollection plugin = p;

    public string Name => Plugin.DisplayName;
    [ObservableProperty] private bool isEnabled;
}

public partial class SystemState : ObservableObject
{
    public Config Config { get; } = Config.LoadConfig();

    public NetStatus NetStatus { get; } = new();
    public ObservableCollection<PluginInfo> Plugins { get; } = [];
    private Dictionary<string, IToolFunction> PluginLookUpTable { get; } = [];

    [ObservableProperty] private InitialPrompts? initialPrompts;

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
                NullValueHandling = NullValueHandling.Ignore
            };
            var promptsJson = JsonConvert.SerializeObject(InitialPrompts, settings);

            File.WriteAllText("initial_prompts.json", promptsJson);
        }

        InitialPrompts.SelectedOption = InitialPrompts.PromptsOptions[0];
        /*** end setup initial prompts ***/
    }

    public List<ChatCompletionRequest?> SessionList { get; } = [];

    public delegate Task ChatSessionChangedHandler(ChatCompletionRequest session, int sessionIndex);

    public event ChatSessionChangedHandler? ChatSessionChangedEvent;

    private async Task<ChatCompletionRequest> ResetSession(int sessionIndex,
        ChatCompletionRequest? loadedSession = null)
    {
        loadedSession ??= BuildFromInitPrompts(
            InitialPrompts?.SelectedOption?.Messages,
            Config.SelectedModel?.KnowledgeCutoff ?? DateTime.Now,
            Config.SelectedModelType?.Provider == ModelInfo.ProviderEnum.Anthropic ? "Claude" : "ChatGPT",
            Config.SelectedModelType?.Provider == ModelInfo.ProviderEnum.Anthropic ? "Anthropic" : "OpenAI"
        );

        if (sessionIndex >= SessionList.Count)
        {
            SessionList.AddRange(Enumerable.Repeat<ChatCompletionRequest?>(null, sessionIndex - SessionList.Count + 1));
        }

        SessionList[sessionIndex] = loadedSession;

        if (ChatSessionChangedEvent is not null)
        {
            await ChatSessionChangedEvent.Invoke(loadedSession, sessionIndex);
        }

        await SaveSessionToPath(sessionIndex, "./latest_session.json");

        return loadedSession;
    }

    private static readonly Random Random = new();

    public delegate void NewMessageHandler(RoleType role, int sessionIndex);

    public delegate void StreamTextHandler(string text, int sessionIndex);

    public delegate void SetStreamProgressHandler(double progress, string text, int sessionIndex);

    public event NewMessageHandler? NewMessageEvent;
    public event StreamTextHandler? StreamTextEvent;
    public event SetStreamProgressHandler? SetStreamProgressEvent;

    public void NewMessage(RoleType role, int sessionIndex)
    {
        NewMessageEvent?.Invoke(role, sessionIndex);
    }

    public void StreamText(string text, int sessionIndex)
    {
        StreamTextEvent?.Invoke(text, sessionIndex);
    }

    public void SetStreamProgress(double progress, string text, int sessionIndex)
    {
        SetStreamProgressEvent?.Invoke(progress, text, sessionIndex);
    }

    private async Task Send(int sessionIndex, CancellationToken cancellationToken = default)
    {
        var selectedModelType = Config.SelectedModelType ??
                                throw new InvalidOperationException($"{nameof(Config.SelectedModelType)} is null.");
        var selectedModel = Config.SelectedModel ??
                            throw new InvalidOperationException($"{nameof(Config.SelectedModel)} is null.");
        var chatRequest = SessionList[sessionIndex] ?? throw new InvalidOperationException("selected session is null.");

        ServerEndpointOptions.ServiceType service;
        string endpointUrl;
        string apiKey;
        int? maxTokens;
        float temperature;
        if (selectedModelType.Provider == ModelInfo.ProviderEnum.OpenAI)
        {
            service = Config.ServiceProvider switch
            {
                Config.ServiceProviderType.OpenAI => ServerEndpointOptions.ServiceType.OpenAI,
                Config.ServiceProviderType.Azure => ServerEndpointOptions.ServiceType.Azure,
                _ => ServerEndpointOptions.ServiceType.Custom
            };
            endpointUrl = Config.ServiceURL;
            apiKey = Config.API_KEY;
            maxTokens = Config.MaxTokens == 0 ? null : Config.MaxTokens;
            temperature = Config.Temperature;
        }
        else
        {
            service = ServerEndpointOptions.ServiceType.Claude;
            endpointUrl = Config.AnthropicServiceURL;
            apiKey = Config.AnthropicAPIKey;
            maxTokens = Config.MaxTokens == 0 ? 4096 : Config.MaxTokens;
            temperature = Config.Temperature / 2.0f; // while openai use 0~2 as temperature range, anthropic use 0~1
        }

        var serverOptions = new ServerEndpointOptions
        {
            Service = service,
            Endpoint = endpointUrl,
            Key = apiKey,
            AzureKey = Config.AzureAPIKey,
            Model = selectedModel.Name,
            MaxTokens = maxTokens,
            PresencePenalty = Config.PresencePenalty,
            Seed = Config.Seed,
            Temperature = temperature,
            TopP = Config.TopP,
        };

        if (serverOptions.MaxTokens is null && selectedModel.Name.Contains("vision"))
        {
            serverOptions.MaxTokens = 4096; // for gpt4-vision-preview: its default max token number is very low
        }

        var enabledPlugins = (from plugin in Plugins
            where plugin.IsEnabled
            select plugin.Plugin).ToList();

        var tools = new List<ToolType>();
        if (selectedModel.FunctionCallSupported && enabledPlugins.Count != 0)
        {
            HashSet<string> addedTools = [];
            foreach (var plugin in enabledPlugins)
            {
                foreach (var func in plugin.Funcs)
                {
                    if (addedTools.Contains(func.Name))
                    {
                        continue;
                    }

                    tools.Add(func.GetToolRequest());
                    addedTools.Add(func.Name);
                }
            }
        }

        serverOptions.Tools = tools;

        var endpoint = IServerEndpoint.BuildServerEndpoint(serverOptions);

        NetStatus.Status = NetStatus.StatusEnum.Sending;
        try
        {
            await endpoint.BuildSession(chatRequest, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            NetStatus.Status = NetStatus.StatusEnum.Idle;
            return;
        }

        NetStatus.Status = NetStatus.StatusEnum.Receiving;

        NewMessage(RoleType.Assistant, sessionIndex);

        try
        {
            var sb = new StringBuilder();
            var lastUpdateTime = DateTime.Now;
            await foreach (var response in endpoint.Streaming(cancellationToken))
            {
                sb.Append(response);

                // every 100ms, update the UI
                if ((DateTime.Now - lastUpdateTime).TotalMilliseconds < 100)
                {
                    continue;
                }

                StreamText(sb.ToString(), sessionIndex);
                sb.Clear();
                NetStatus.SystemFingerprint = endpoint.SystemFingerprint;
                lastUpdateTime = DateTime.Now;
                // need this to make sure the UI is updated
                Application.Current.Dispatcher.Invoke(() => { }, System.Windows.Threading.DispatcherPriority.Render,
                    cancellationToken);
            }

            StreamText(sb.ToString(), sessionIndex);
            NetStatus.Status = NetStatus.StatusEnum.Idle;
            NetStatus.SystemFingerprint = endpoint.SystemFingerprint;
        }
        catch (OperationCanceledException)
        {
            NetStatus.Status = NetStatus.StatusEnum.Idle;
        }

        var newAssistantMsg = endpoint.ResponseMessage;
        chatRequest.Messages.Add(newAssistantMsg);

        // long streaming can create a lot of small objects, so we force a GC here to help reduce memory usage
        GC.Collect();

        var responseRequired = false;
        foreach (var toolcall in endpoint.ToolCalls)
        {
            await ResetSession(sessionIndex, chatRequest);
            var pluginName = toolcall.Function.Name;
            var args = toolcall.Function.Arguments;
            var plugin = PluginLookUpTable[pluginName] ??
                         throw new InvalidDataException($"plugin not found: {pluginName}");
            try
            {
                var toolResult = await plugin.Action(this, sessionIndex, toolcall.Id, args, cancellationToken);
                toolResult.Message.ToolCallId = toolcall.Id;
                chatRequest.Messages.Add(toolResult.Message);
                responseRequired = responseRequired || toolResult.ResponeRequired;
            }
            catch (OperationCanceledException)
            {
                NetStatus.Status = NetStatus.StatusEnum.Idle;
                return;
            }
        }

        if (responseRequired)
        {
            await ResetSession(sessionIndex, chatRequest);
            await Send(sessionIndex, cancellationToken);
        }
    }

    private interface IFileAttachmentReader
    {
        public Task<bool> CanReadFile(string file, CancellationToken cancellationToken = default);

        public Task<IAttachmentInfo> OpenFileAsAttachment(SystemState state, string file,
            CancellationToken cancellationToken = default);
    }

    private class TextFileAttachmentReader : IFileAttachmentReader
    {
        public Task<bool> CanReadFile(string file, CancellationToken cancellationToken = default)
        {
            var mime = MimeTypes.GetMimeType(file);
            return Task.FromResult(mime.StartsWith("text/"));
        }

        public async Task<IAttachmentInfo> OpenFileAsAttachment(SystemState state, string file,
            CancellationToken cancellationToken = default)
        {
            if (!await CanReadFile(file, cancellationToken))
            {
                throw new InvalidOperationException($"File is not a text file: {file}");
            }

            return new TextAttachmentInfo
            {
                FileName = Path.GetFileName(file),
                Content = (await File.ReadAllTextAsync(file, cancellationToken)).Trim()
            };
        }
    }

    private class PdfFileAttachmentReader : IFileAttachmentReader
    {
        public Task<bool> CanReadFile(string file, CancellationToken cancellationToken = default)
        {
            var mime = MimeTypes.GetMimeType(file);
            return Task.FromResult(mime is "application/pdf");
        }

        public async Task<IAttachmentInfo> OpenFileAsAttachment(SystemState state, string file,
            CancellationToken cancellationToken = default)
        {
            if (!await CanReadFile(file, cancellationToken))
            {
                throw new InvalidOperationException($"File is not a pdf file: {file}");
            }

            return new TextAttachmentInfo
            {
                FileName = Path.GetFileName(file),
                Content = (await Utils.PdfFileToText(file, cancellationToken)).Trim()
            };
        }
    }

    private class DocxFileAttachmentReader : IFileAttachmentReader
    {
        public Task<bool> CanReadFile(string file, CancellationToken cancellationToken = default)
        {
            var mime = MimeTypes.GetMimeType(file);
            return Task.FromResult(mime is "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
        }

        public async Task<IAttachmentInfo> OpenFileAsAttachment(SystemState state, string file,
            CancellationToken cancellationToken = default)
        {
            if (!await CanReadFile(file, cancellationToken))
            {
                throw new InvalidOperationException($"File is not a docx file: {file}");
            }

            return new TextAttachmentInfo
            {
                FileName = Path.GetFileName(file),
                Content = (await OfficeReader.DocxToText(file, cancellationToken)).Trim()
            };
        }
    }

    private class PptxFileAttachmentReader : IFileAttachmentReader
    {
        public Task<bool> CanReadFile(string file, CancellationToken cancellationToken = default)
        {
            var mime = MimeTypes.GetMimeType(file);
            return Task.FromResult(mime is "application/vnd.openxmlformats-officedocument.presentationml.presentation");
        }

        public async Task<IAttachmentInfo> OpenFileAsAttachment(SystemState state, string file,
            CancellationToken cancellationToken = default)
        {
            if (!await CanReadFile(file, cancellationToken))
            {
                throw new InvalidOperationException($"File is not a pptx file: {file}");
            }

            return new TextAttachmentInfo
            {
                FileName = Path.GetFileName(file),
                Content = (await OfficeReader.PptxToText(file, cancellationToken)).Trim()
            };
        }
    }

    public class XlsxFileAttachmentReader : IFileAttachmentReader
    {
        public Task<bool> CanReadFile(string file, CancellationToken cancellationToken = default)
        {
            var mime = MimeTypes.GetMimeType(file);
            return Task.FromResult(mime is "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public async Task<IAttachmentInfo> OpenFileAsAttachment(SystemState state, string file,
            CancellationToken cancellationToken = default)
        {
            if (!await CanReadFile(file, cancellationToken))
            {
                throw new InvalidOperationException($"File is not a xlsx file: {file}");
            }

            return new TextAttachmentInfo
            {
                FileName = Path.GetFileName(file),
                Content = (await OfficeReader.XlsxToText(file, cancellationToken)).Trim()
            };
        }
    }

    private class SupportedImageFileAttachmentReader : IFileAttachmentReader
    {
        public Task<bool> CanReadFile(string file, CancellationToken cancellationToken = default)
        {
            var mime = MimeTypes.GetMimeType(file);
            return Task.FromResult(mime is "image/jpeg" or "image/png" or "image/webp" or "image/gif");
        }

        public async Task<IAttachmentInfo> OpenFileAsAttachment(SystemState state, string file,
            CancellationToken cancellationToken = default)
        {
            if (!await CanReadFile(file, cancellationToken))
            {
                throw new InvalidOperationException($"File is not a supproted image file: {file}");
            }

            var base64 = await Utils.ImageFileToBase64(file, cancellationToken);
            var image = Utils.Base64ToBitmapImage(base64);
            Size imageSize = new(image.PixelWidth, image.PixelHeight);
            return new ImageAttachmentInfo
            {
                FileName = Path.GetFileName(file),
                ImageBase64Url = base64,
                HighResMode = state.Config.UploadHiresImage,
                ImageSize = imageSize
            };
        }
    }

    private class ConvertibleImageFileAttachmentReader : IFileAttachmentReader
    {
        private string? filePathCache;
        private DateTime? fileModifiedTimeCache;
        private string? base64PngCache;

        private static DateTime GetFileModifiedTime(string file)
        {
            var fileInfo = new FileInfo(file);
            return fileInfo.LastWriteTimeUtc;
        }

        public async Task<bool> CanReadFile(string file, CancellationToken cancellationToken = default)
        {
            var mime = MimeTypes.GetMimeType(file);
            if (!mime.StartsWith("image/"))
            {
                return false;
            }

            if (filePathCache == file
                && fileModifiedTimeCache == GetFileModifiedTime(file)
                && base64PngCache is not null)
            {
                return true;
            }

            try
            {
                var base64Original = await Utils.ImageFileToBase64(file, cancellationToken);
                base64PngCache = Utils.ConvertToBase64Png(base64Original);
                filePathCache = file;
                fileModifiedTimeCache = GetFileModifiedTime(file);
                return true;
            }
            catch (NotSupportedException)
            {
                return false;
            }
        }

        public async Task<IAttachmentInfo> OpenFileAsAttachment(SystemState state, string file,
            CancellationToken cancellationToken = default)
        {
            if (!await CanReadFile(file, cancellationToken))
            {
                throw new InvalidOperationException($"File is not a image file convertible to png: {file}");
            }

            var image = Utils.Base64ToBitmapImage(base64PngCache!);
            Size imageSize = new(image.PixelWidth, image.PixelHeight);
            return new ImageAttachmentInfo
            {
                FileName = Path.GetFileName(file),
                ImageBase64Url = base64PngCache!,
                HighResMode = state.Config.UploadHiresImage,
                ImageSize = imageSize
            };
        }
    }

    private class RtfFileAttachmentReader : IFileAttachmentReader
    {
        public Task<bool> CanReadFile(string file, CancellationToken cancellationToken = default)
        {
            var mime = MimeTypes.GetMimeType(file);
            return Task.FromResult(mime is "application/rtf");
        }

        public async Task<IAttachmentInfo> OpenFileAsAttachment(SystemState state, string file,
            CancellationToken cancellationToken = default)
        {
            if (!await CanReadFile(file, cancellationToken))
            {
                throw new InvalidOperationException($"File is not a rtf file: {file}");
            }

            return new TextAttachmentInfo
            {
                FileName = Path.GetFileName(file),
                Content = (await Utils.RtfFileToText(file, cancellationToken)).Trim()
            };
        }
    }

    private static readonly List<IFileAttachmentReader> FileAttachmentReaders =
    [
        new TextFileAttachmentReader(),
        new PdfFileAttachmentReader(),
        new DocxFileAttachmentReader(),
        new PptxFileAttachmentReader(),
        new XlsxFileAttachmentReader(),
        new SupportedImageFileAttachmentReader(),
        new ConvertibleImageFileAttachmentReader(),
        new RtfFileAttachmentReader()
    ];

    public static async Task<bool> FileCanReadAsAttachment(string file, CancellationToken cancellationToken = default)
    {
        foreach (var reader in FileAttachmentReaders)
        {
            if (await reader.CanReadFile(file, cancellationToken))
            {
                return true;
            }
        }

        return false;
    }

    private async Task<IAttachmentInfo> OpenFileAsAttachment(string file, CancellationToken cancellationToken = default)
    {
        foreach (var reader in FileAttachmentReaders)
        {
            if (await reader.CanReadFile(file, cancellationToken))
            {
                return await reader.OpenFileAsAttachment(this, file, cancellationToken);
            }
        }

        var mime = MimeTypes.GetMimeType(file);
        throw new NotSupportedException($"Unsupported file type: {mime}");
    }

    public async Task UserSendText(string text, IEnumerable<string> files, int sessionIndex,
        CancellationToken cancellationToken = default)
    {
        if (sessionIndex >= SessionList.Count)
        {
            SessionList.AddRange(Enumerable.Repeat<ChatCompletionRequest?>(null, sessionIndex - SessionList.Count + 1));
        }

        SessionList[sessionIndex] ??= await ResetSession(sessionIndex);
        var selectedSession = SessionList[sessionIndex];

        if (Config.UseRandomSeed)
        {
            Config.Seed = Random.Next();
        }

        List<IAttachmentInfo> attachments = [];
        foreach (var file in files)
        {
            attachments.Add(await OpenFileAsAttachment(file, cancellationToken));
        }

        var textContent = new IMessage.TextContent { Text = text };
        var contentList = new List<IMessage.IContent> { textContent };

        var userMsg = new UserMessage
        {
            Content = contentList,
            Name = string.IsNullOrEmpty(Config.UserNickName) ? null : Config.UserNickName,
            Attachments = attachments
        };

        selectedSession!.Messages.Add(userMsg);
        await ResetSession(sessionIndex, selectedSession);

        await Send(sessionIndex, cancellationToken);

        await ResetSession(sessionIndex, selectedSession);
    }

    public IEnumerable<Block> GetToolcallDescription(ToolCallType toolcall, int sessionIndex) =>
        PluginLookUpTable.TryGetValue(toolcall.Function.Name, out var plugin)
            ? plugin.GetToolcallMessage(this, sessionIndex, toolcall.Function.Arguments, toolcall.Id)
            : [new Paragraph(new Run($"调用函数：{toolcall.Function.Name}"))];

    private async Task SaveSessionToPath(int sessionIndex, string path)
    {
        var savedSession = SessionList[sessionIndex]?.Save();
        await File.WriteAllTextAsync(path, savedSession);
    }

    private const string OpenSaveSessionDialogGuid = "32F3FF84-A923-4D69-9ABD-11DC14074AC6";

    public async Task SaveSession(int sessionIndex)
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
            await SaveSessionToPath(sessionIndex, dlg.FileName);
        }
    }

    public delegate void SetIsLoadingHandler(bool isLoading, int sessionIndex);

    public event SetIsLoadingHandler? SetIsLoadingHandlerEvent;

    public async Task<bool> LoadSession(int sessionIndex)
    {
        var dlg = new OpenFileDialog
        {
            DefaultExt = ".json",
            Filter = "JSON documents|*.json",
            ClientGuid = new Guid(OpenSaveSessionDialogGuid)
        };
        if (dlg.ShowDialog() != true)
        {
            return false;
        }

        SetIsLoadingHandlerEvent?.Invoke(true, sessionIndex);

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

        if (loadedSession is not null)
        {
            await ResetSession(sessionIndex, loadedSession);
        }

        SetIsLoadingHandlerEvent?.Invoke(false, sessionIndex);
        return true;
    }

    public async Task ClearSession(int sessionIndex)
    {
        await ResetSession(sessionIndex);
    }

    public async Task RefreshSession(int sessionIndex)
    {
        await ResetSession(sessionIndex, SessionList[sessionIndex]);
    }

    public int GetSessionTokens(int sessionIndex)
    {
        if (sessionIndex >= SessionList.Count || sessionIndex < 0)
        {
            return 0;
        }

        var count = SessionList[sessionIndex]?.CountTokens() ?? 0;
        return count;
    }
}