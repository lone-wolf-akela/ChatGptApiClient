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
    public partial ObservableCollection<IMessage> Messages { get; set; } = [];

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
    [ObservableProperty]
    public partial PromptsOption? SelectedOption { get; set; }

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
            You are {ProductName}, a large language model trained by {ModelProvider}.

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
    public partial IToolCollection Plugin { get; set; } = p;

    public string Name => Plugin.DisplayName;
    [ObservableProperty]
    public partial bool IsEnabled { get; set; }
}

public partial class SystemState : ObservableObject
{
    public Config Config { get; } = Config.LoadConfig();

    public NetStatus NetStatus { get; } = new();
    // ReSharper disable once MemberCanBePrivate.Global
    public ObservableCollection<PluginInfo> Plugins { get; } = [];
    private Dictionary<string, IToolFunction> PluginLookUpTable { get; } = [];

    [ObservableProperty]
    public partial InitialPrompts? InitialPrompts { get; set; }

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

    public Dictionary<Guid, ChatCompletionRequest?> SessionDict { get; } = [];

    public delegate Task ChatSessionChangedHandler(ChatCompletionRequest session, Guid sessionId);

    public event ChatSessionChangedHandler? ChatSessionChangedEvent;

    private async Task<ChatCompletionRequest> ResetSession(
        Guid sessionId,
        ChatCompletionRequest? loadedSession = null,
        bool saveSession = true)
    {
        var productName = Config.SelectedModelType?.Provider switch
        {
            ModelInfo.ProviderEnum.OpenAI => "ChatGPT",
            ModelInfo.ProviderEnum.Anthropic => "Claude",
            ModelInfo.ProviderEnum.DeepSeek => Config.SelectedModel?.Description ?? "DeepSeek Chat",
            ModelInfo.ProviderEnum.Google => "Gemini",
            ModelInfo.ProviderEnum.Qwen => "Qwen",
            ModelInfo.ProviderEnum.OtherOpenAICompat => Config.OtherOpenAICompatModelDisplayName,
            _ => throw new InvalidOperationException()
        };

        var providerName = Config.SelectedModelType?.Provider switch
        {
            ModelInfo.ProviderEnum.OpenAI => "OpenAI",
            ModelInfo.ProviderEnum.Anthropic => "Anthropic",
            ModelInfo.ProviderEnum.DeepSeek => "DeepSeek",
            ModelInfo.ProviderEnum.Google => "Google",
            ModelInfo.ProviderEnum.Qwen => "Alibaba",
            ModelInfo.ProviderEnum.OtherOpenAICompat => Config.OtherOpenAICompatModelProviderName,
            _ => throw new InvalidOperationException()
        };

        loadedSession ??= BuildFromInitPrompts(
            InitialPrompts?.SelectedOption?.Messages,
            Config.SelectedModel?.KnowledgeCutoff ?? DateTime.Now,
            productName,
            providerName
        );

        SessionDict[sessionId] = loadedSession;

        if (ChatSessionChangedEvent is not null)
        {
            await ChatSessionChangedEvent.Invoke(loadedSession, sessionId);
        }

        if (saveSession)
        {
            await SaveSessionToPath(sessionId, "./latest_session.json");
        }

        return loadedSession;
    }

    public delegate void NewMessageHandler(RoleType role, Guid sessionId, ModelInfo.ProviderEnum? provider);

    public delegate void StreamTextHandler(string text, Guid sessionId);

    public delegate void SetStreamProgressHandler(double progress, string text, Guid sessionId);

    public event NewMessageHandler? NewMessageEvent;
    public event StreamTextHandler? StreamTextEvent;
    public event SetStreamProgressHandler? SetStreamProgressEvent;

    public void NewMessage(RoleType role, Guid sessionId, ModelInfo.ProviderEnum? provider = null)
    {
        if (provider is null && role == RoleType.Assistant)
        {
            throw new InvalidOperationException("provider should not be null when role is assistant.");
        }
        if (provider is not null && role != RoleType.Assistant)
        {
           throw new InvalidOperationException("provider should be null when role is not assistant.");
        }
        NewMessageEvent?.Invoke(role, sessionId, provider);
    }

    public void StreamText(string text, Guid sessionId)
    {
        StreamTextEvent?.Invoke(text, sessionId);
    }
    // ReSharper disable once UnusedMember.Global
    public void SetStreamProgress(double progress, string text, Guid sessionId)
    {
        SetStreamProgressEvent?.Invoke(progress, text, sessionId);
    }

    enum AlternativeModelName
    {
        Default, SiliconFlow, Nvidia
    }

    private async Task Send(Guid sessionId, CancellationToken cancellationToken = default)
    {
        var selectedModelType = Config.SelectedModelType ??
                                throw new InvalidOperationException($"{nameof(Config.SelectedModelType)} is null.");
        var selectedModel = Config.SelectedModel ??
                            throw new InvalidOperationException($"{nameof(Config.SelectedModel)} is null.");
        var chatRequest = SessionDict[sessionId] ?? throw new InvalidOperationException("selected session is null.");

        ServerEndpointOptions.ServiceType service;
        string endpointUrl;
        string apiKey;
        int? maxTokens;
        float temperature;
        int thinkingLevel = -1;
        bool includeThoughts = true;

        var alternativeModelName = AlternativeModelName.Default;

        switch (selectedModelType.Provider)
        {
            case ModelInfo.ProviderEnum.OpenAI:
            {
                service = Config.ServiceProvider switch
                {
                    Config.ServiceProviderType.OpenAI => ServerEndpointOptions.ServiceType.OpenAI,
                    _ => ServerEndpointOptions.ServiceType.Custom
                };
                endpointUrl = Config.ServiceURL;
                apiKey = Config.API_KEY;
                maxTokens = Config.MaxTokens == 0 ? null : Config.MaxTokens;
                temperature = Config.Temperature;
                break;
            }
            case ModelInfo.ProviderEnum.DeepSeek:
            {
                service = ServerEndpointOptions.ServiceType.DeepSeek;
                endpointUrl = Config.DeepSeekServiceURL;
                apiKey = Config.SelectedDeepSeekAPIKey;
                maxTokens = Config.MaxTokens == 0 ? 8192 : Config.MaxTokens;
                temperature = Config.Temperature;

                alternativeModelName = Config.DeepSeekServiceProvider switch
                {
                    Config.DeepSeekServiceProviderType.SiliconFlow => AlternativeModelName.SiliconFlow,
                    Config.DeepSeekServiceProviderType.Nvidia => AlternativeModelName.Nvidia,
                    _ => AlternativeModelName.Default
                };

                break;
            }
            case ModelInfo.ProviderEnum.Qwen:
            {
                service = ServerEndpointOptions.ServiceType.Ali;
                endpointUrl = Config.AliQwenServiceURL;
                apiKey = Config.AliQwenAPIKey;
                maxTokens = Config.MaxTokens == 0 ? null : Config.MaxTokens;
                temperature = Config.Temperature;
                break;
            }
            case ModelInfo.ProviderEnum.Google:
            {
                service = ServerEndpointOptions.ServiceType.Google;
                endpointUrl = Config.GoogleGeminiServiceURL;
                apiKey = Config.GoogleGeminiAPIKey;
                if (selectedModel.MaxOutput is null)
                {
                    throw new InvalidOperationException("The selected Gemini model has no Max Output config.");
                }
                maxTokens = Config.MaxTokens == 0 ? selectedModel.MaxOutput
                                              : Math.Min(Config.MaxTokens, selectedModel.MaxOutput.Value);
                temperature = Config.Temperature;
                thinkingLevel = Config.GoogleGeminiThinkingLevel switch
                {
                    Config.GoogleGeminiThinkingLevelType.Low => 0,
                    Config.GoogleGeminiThinkingLevelType.High => 1,
                    _ => -1
                };
                includeThoughts = Config.GoogleGeminiIncludeThoughts;
                break;
            }
            case ModelInfo.ProviderEnum.OtherOpenAICompat:
            {
                service = ServerEndpointOptions.ServiceType.OtherOpenAICompat;
                endpointUrl = Config.OtherOpenAICompatServiceURL;
                maxTokens = Config.MaxTokens == 0 ? null : Config.MaxTokens;
                temperature = Config.Temperature;
                // this key cannot be empty or the OpenAI SDK will throw an exception
                apiKey = !string.IsNullOrEmpty(Config.OtherOpenAICompatModelAPIKey) ? Config.OtherOpenAICompatModelAPIKey : "SomeKey";
                break;
            }
            case ModelInfo.ProviderEnum.Anthropic:
            default:
            {
                service = ServerEndpointOptions.ServiceType.Claude;
                endpointUrl = Config.AnthropicServiceURL;
                apiKey = Config.AnthropicAPIKey;

                if (selectedModel.MaxOutput is null)
                {
                    throw new InvalidOperationException("The selected Anthropic model has no Max Output config.");
                }

                maxTokens = Config.MaxTokens == 0 ? selectedModel.MaxOutput 
                                                  : Math.Min(Config.MaxTokens, selectedModel.MaxOutput.Value);
                temperature = Config.Temperature / 2.0f; // while openai use 0~2 as temperature range, anthropic use 0~1
                break;
            }
        }

        switch (alternativeModelName)
        {
            case AlternativeModelName.SiliconFlow when selectedModel.SiliconFlowName is null:
                throw new InvalidOperationException("The selected model is not available on SiliconFlow.");
            case AlternativeModelName.Nvidia when selectedModel.NvidiaName is null:
                throw new InvalidOperationException("The selected model is not available on Nvidia.");
        }

        var serverOptions = new ServerEndpointOptions
        {
            Service = service,
            Endpoint = endpointUrl,
            Key = apiKey,
            Model = alternativeModelName switch
            {
                AlternativeModelName.Default => selectedModel.Name,
                AlternativeModelName.SiliconFlow => Config.SiliconFlowUseProModel
                    ? $"Pro/{selectedModel.SiliconFlowName!}"
                    : selectedModel.SiliconFlowName!,
                AlternativeModelName.Nvidia => selectedModel.NvidiaName!,
                _ => throw new InvalidOperationException()
            },
            MaxTokens = maxTokens,
            PresencePenalty = Config.PresencePenalty,
            Temperature = temperature,
            TopP = Config.TopP,
            UserId = Config.UserAdvertisingId,
            StopSequences = Config.StopSequences,
            SystemPromptNotSupported = selectedModel.SystemPromptNotSupported,
            TemperatureSettingNotSupported = selectedModel.TemperatureSettingNotSupported,
            TopPSettingNotSupported = selectedModel.TopPSettingNotSupported,
            PenaltySettingNotSupported = selectedModel.PenaltySettingNotSupported,
            NeedChinesePunctuationNormalization = selectedModel.NeedChinesePunctuationNormalization,
            StreamingNotSupported = selectedModel.StreamingNotSupported,
            EnableThinking = selectedModel.OptionalThinkingAbility && Config.EnableThinking,
            ThinkingLength = Config.ThinkingLength,
            ThinkingLevel = thinkingLevel,
            IncludeThoughts = includeThoughts
        };

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

        NewMessage(RoleType.Assistant, sessionId, Config.SelectedModelType.Provider);

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

                StreamText(sb.ToString(), sessionId);
                sb.Clear();
                NetStatus.SystemFingerprint = endpoint.SystemFingerprint;
                lastUpdateTime = DateTime.Now;
                // need this to make sure the UI is updated
                Application.Current.Dispatcher.Invoke(() => { }, System.Windows.Threading.DispatcherPriority.Render,
                    cancellationToken);
            }

            StreamText(sb.ToString(), sessionId);
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
            await ResetSession(sessionId, chatRequest);
            var pluginName = toolcall.Function.Name;
            var args = toolcall.Function.Arguments;
            var plugin = PluginLookUpTable[pluginName] ??
                         throw new InvalidDataException($"plugin not found: {pluginName}");
            try
            {
                var toolResult = await plugin.Action(this, sessionId, toolcall.Id, args, cancellationToken);
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
            await ResetSession(sessionId, chatRequest);
            await Send(sessionId, cancellationToken);
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
            var isText = mime.StartsWith("text/") ||
                mime is "application/json" ||
                mime is "application/json5" ||
                mime is "application/xml" ||
                mime is "application/xaml+xml" ||
                mime is "application/toml";
            return Task.FromResult(isText);
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

            return new ImageAttachmentInfo
            {
                FileName = Path.GetFileName(file),
                ImageBase64Url = await ImageData.CreateFromFile(file, cancellationToken),
                HighResMode = state.Config.UploadHiresImage,
            };
        }
    }

    private class ConvertibleImageFileAttachmentReader : IFileAttachmentReader
    {
        private string? _filePathCache;
        private DateTime? _fileModifiedTimeCache;
        private ImageData? _pngCache;

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

            if (_filePathCache == file
                && _fileModifiedTimeCache == GetFileModifiedTime(file)
                && _pngCache is not null)
            {
                return true;
            }

            try
            {
                var imageDataOriginal = await ImageData.CreateFromFile(file, cancellationToken);
                _pngCache = imageDataOriginal.ReSaveAsPng();
                _filePathCache = file;
                _fileModifiedTimeCache = GetFileModifiedTime(file);
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

            return new ImageAttachmentInfo
            {
                FileName = Path.GetFileName(file),
                ImageBase64Url = _pngCache,
                HighResMode = state.Config.UploadHiresImage,
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

    public async Task UserSendText(string? text, IEnumerable<string>? files, Guid sessionId,
        CancellationToken cancellationToken = default)
    {
        SessionDict.TryAdd(sessionId, null);
        SessionDict[sessionId] ??= await ResetSession(sessionId);
        var selectedSession = SessionDict[sessionId];

        if (text is not null)
        {
            List<IAttachmentInfo> attachments = [];
            foreach (var file in files ?? [])
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
        }

        await ResetSession(sessionId, selectedSession);

        await Send(sessionId, cancellationToken);

        await ResetSession(sessionId, selectedSession);
    }

    public ToolCallMessage GetToolcallDescription(ToolCallType toolcall, Guid sessionId) =>
        PluginLookUpTable.TryGetValue(toolcall.Function.Name, out var plugin)
            ? plugin.GetToolcallMessage(this, sessionId, toolcall.Function.Arguments, toolcall.Id)
            : new ToolCallMessage($"调用函数：{toolcall.Function.Name}");

    private async Task SaveSessionToPath(Guid sessionId, string path)
    {
        var savedSession = SessionDict[sessionId]?.Save();
        await File.WriteAllTextAsync(path, savedSession);
    }

    private const string OpenSaveSessionDialogGuid = "32F3FF84-A923-4D69-9ABD-11DC14074AC6";

    public async Task SaveSession(Guid sessionId)
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
            await SaveSessionToPath(sessionId, dlg.FileName);
        }
    }

    public delegate void SetIsLoadingHandler(bool isLoading, Guid sessionId);

    public event SetIsLoadingHandler? SetIsLoadingHandlerEvent;

    public async Task<bool> LoadSession(Guid sessionId)
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

        SetIsLoadingHandlerEvent?.Invoke(true, sessionId);

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
            await ResetSession(sessionId, loadedSession, false);
        }

        SetIsLoadingHandlerEvent?.Invoke(false, sessionId);
        return true;
    }

    public async Task ClearSession(Guid sessionId)
    {
        await ResetSession(sessionId);
    }

    public async Task RefreshSession(Guid sessionId)
    {
        await ResetSession(sessionId, SessionDict[sessionId]);
    }

    public int GetSessionTokens(Guid sessionId)
    {
        if(!SessionDict.TryGetValue(sessionId, out var session))
        {
            return 0;
        }

        var count = session?.CountTokens(Config.SelectedModel?.Tokenizer ?? ModelVersionInfo.TokenizerEnum.Cl100KBase) 
            ?? 0;
        return count;
    }
}