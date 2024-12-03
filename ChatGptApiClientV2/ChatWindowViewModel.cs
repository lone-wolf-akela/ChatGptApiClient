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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Markdig;
using Markdig.Wpf.ColorCode;
using static ChatGptApiClientV2.IMessage;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Win32;
using CommunityToolkit.Mvvm.Input;
using SharpVectors.Converters;
using System.Threading.Tasks;
using static ChatGptApiClientV2.ChatWindowMessage;
using System.ComponentModel;
using System.IO;
using System.Threading;
using Windows.Storage;

// ReSharper disable UnusedParameterInPartialMethod
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedMember.Global

namespace ChatGptApiClientV2;

public partial class FileAttachmentInfo : ObservableObject
{
    private FileAttachmentInfo(string path, ICollection<FileAttachmentInfo> owner, ImageSource? icon) 
    {
        Path = path;
        Icon = icon;
        _ownerCollection = owner;
    }
    private static async Task<BitmapImage?> ConvertThumbnailToImageSource(Windows.Storage.FileProperties.StorageItemThumbnail? thumbnail)
    {
        if(thumbnail is null || thumbnail.Size == 0)
        {
            return null;
        }

        BitmapImage bitmapImage = new();
        bitmapImage.BeginInit();

        MemoryStream memoryStream = new();
        await thumbnail.AsStreamForRead().CopyToAsync(memoryStream);
        memoryStream.Seek(0, SeekOrigin.Begin);

        bitmapImage.StreamSource = memoryStream;
        bitmapImage.EndInit();
        bitmapImage.Freeze();

        return bitmapImage;
    }
    public static async Task<FileAttachmentInfo> Create(string path, ICollection<FileAttachmentInfo> owner)
    {
        // we should not extract icon on UI thread, which may cause the UI to stop responding.

        var file = await StorageFile.GetFileFromPathAsync(path);
        var iconSource = await file.GetThumbnailAsync(
            Windows.Storage.FileProperties.ThumbnailMode.SingleItem,
            256, Windows.Storage.FileProperties.ThumbnailOptions.ReturnOnlyIfCached
        );
        var icon = await ConvertThumbnailToImageSource(iconSource);

        var fileInfo = new FileAttachmentInfo(path, owner, icon);

        // for remote files, we only use cached icon, or it may take a long time to generate a new one
        // for any other files, since we only asked for cached icon at above, we may just get a simple icon
        // when in fact we can generate a more detailed one according to file content,
        // but it may take some time, so we fire the generation in background
        var needGenerateNewIcon = !Utils.IsPathRemote(path) &&
            (iconSource is null || iconSource.Type == Windows.Storage.FileProperties.ThumbnailType.Icon);

        if (needGenerateNewIcon)
        {
            // fire and forget
            _ = Task.Run(async () =>
            {
                try
                {
                    var newIconSource = await file.GetThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.SingleItem);
                    var newIcon = await ConvertThumbnailToImageSource(newIconSource);
                    fileInfo.Icon = newIcon;
                }
                catch
                {
                    // ignore
                }
            });
        }

        return fileInfo;
    }

    private readonly ICollection<FileAttachmentInfo> _ownerCollection;

    [RelayCommand]
    private void RemoveFile()
    {
        _ownerCollection.Remove(this);
    }

    [ObservableProperty]
    public partial string Path { get; set; }
    [ObservableProperty]
    public partial ImageSource? Icon { get; set; }
}

public partial class ChatWindowMessage : ObservableObject
{
    [ObservableProperty]
    public partial bool IsInEditingMode { get; set; }

    public partial class EditorModeContent(string srcText) : ObservableObject
    {
        [ObservableProperty]
        public partial string Text { get; set; } = srcText;
        partial void OnTextChanged(string value)
        {
            IsDirty = true;
        }

        public bool IsEditable { get; init; }
        public bool IsDirty { get; private set; }
        public IRichMessage? SourceMessage { get; init; }
    }

    public ObservableCollection<EditorModeContent> EditorModeContents { get; } = [];

    [RelayCommand]
    private void EnterEditingMode()
    {
        EditorModeContents.Clear();
        foreach (var msg in _messageList)
        {
            EditorModeContent content;
            if (msg is TextMessage txtMsg)
            {
                content = new EditorModeContent(txtMsg.Text)
                {
                    IsEditable = true,
                    SourceMessage = msg
                };
            }
            else if (msg is ImageMessage imgMsg)
            {
                content = new EditorModeContent(imgMsg.FileName ?? "图像")
                {
                    IsEditable = false,
                    SourceMessage = msg
                };
            }
            else if (msg is BlocksMessage)
            {
                content = new EditorModeContent("不可编辑的UI元素")
                {
                    IsEditable = false,
                    SourceMessage = msg
                };
            }
            else if (msg is TextFileMessage txtFileMsg)
            {
                content = new EditorModeContent(txtFileMsg.FileName ?? "文件")
                {
                    IsEditable = false,
                    SourceMessage = msg
                };
            }
            else
            {
                throw new InvalidOperationException();
            }

            EditorModeContents.Add(content);
        }

        IsInEditingMode = true;
    }

    [RelayCommand]
    private void ConfirmEditingMode()
    {
        foreach (var content in EditorModeContents)
        {
            if (!content.IsDirty)
            {
                continue;
            }

            if (!content.IsEditable || content.SourceMessage is not TextMessage txtMsg)
            {
                throw new InvalidOperationException();
            }

            txtMsg.Text = content.Text;
            if (txtMsg.SourceContent is not null)
            {
                txtMsg.SourceContent.Text = content.Text;
            }
        }

        OnPropertyChanged(nameof(RenderedMessage));
        IsInEditingMode = false;
    }

    [RelayCommand]
    private void CancelEditingMode()
    {
        IsInEditingMode = false;
    }

    public IMessage? SourceMessage { get; init; }
    public bool IsStreaming { get; init; }
    [ObservableProperty]
    public partial bool IsWaitingDeleteConfirm { get; set; }

    [RelayCommand]
    private void TryDeleteMessage()
    {
        IsWaitingDeleteConfirm = true;
    }

    [RelayCommand]
    private void CancelDeleteMessage()
    {
        IsWaitingDeleteConfirm = false;
    }

    [RelayCommand]
    private void CopyMessage()
    {
        var doc = RenderedMessage;
        doc.Foreground = Brushes.Black; // ensure text color is black

        var text = new TextRange(doc.ContentStart, doc.ContentEnd);

        var dataObject = new DataObject();

        using (var msRtf = new MemoryStream())
        {
            text.Save(msRtf, DataFormats.Rtf);
            var rtfText = Encoding.UTF8.GetString(msRtf.ToArray());
            dataObject.SetData(DataFormats.Rtf, rtfText);
        }

        using (var msPlainText = new MemoryStream())
        {
            text.Save(msPlainText, DataFormats.Text);
            var plainText = Encoding.UTF8.GetString(msPlainText.ToArray());
            dataObject.SetData(DataFormats.Text, plainText);
        }

        using (var msXaml = new MemoryStream())
        {
            text.Save(msXaml, DataFormats.Xaml);
            var xamlText = Encoding.UTF8.GetString(msXaml.ToArray());
            dataObject.SetData(DataFormats.Xaml, xamlText);
        }

        using (var msXamlPackage = new MemoryStream())
        {
            text.Save(msXamlPackage, DataFormats.XamlPackage);
            var xamlPackage = msXamlPackage.ToArray();
            dataObject.SetData(DataFormats.XamlPackage, xamlPackage);
        }

        // 将DataObject对象设置到剪贴板
        Clipboard.SetDataObject(dataObject, true);
        OnPropertyChanged(nameof(RenderedMessage)); // need this or the displayed message will be cleared
    }

    public ModelInfo.ProviderEnum? Provider { get; init; }
    public DateTime? DateTime { private get; init; }
    public string FormattedDateTime =>
        IsStreaming ? "生成中..." :
        DateTime is null ? "" :
        // if is the same day as today, only show time
        DateTime.Value.Date == System.DateTime.Now.Date ? DateTime.Value.ToString("HH:mm:ss") :
        // else, also show date
        DateTime.Value.ToString("yyyy-MM-dd HH:mm:ss");

    private readonly Block _loadingBar;

    public ChatWindowMessage()
    {
        var loadingBarCtl = new Controls.ContinuingLoadingLine
        {
            IsRunning = true,
            DotCount = 5,
            DotInterval = 10,
            DotBorderThickness = 0,
            DotDiameter = 6,
            DotSpeed = 4,
            DotDelayTime = 80
        };

        loadingBarCtl.SetResourceReference(Control.ForegroundProperty, "PrimaryBrush");

        _loadingBar = new BlockUIContainer(loadingBarCtl);
    }

    public interface IRichMessage : INotifyPropertyChanged
    {
        public IEnumerable<Block> Rendered { get; }
    }

    private partial class TextMessage(string srcStr, bool enableMarkdown, TextContent? sourceContent)
        : ObservableObject, IRichMessage
    {
        public TextContent? SourceContent { get; } = sourceContent;

        [ObservableProperty]
        public partial string Text { get; set; } = srcStr;
        partial void OnTextChanged(string value)
        {
            _cachedRenderResult = null;
        }

        private bool EnableMarkdown { get; } = enableMarkdown;

        private List<Block>? _cachedRenderResult;

        private List<Block> RenderMarkdownText()
        {
            try
            {
                var doc = Markdig.Wpf.Markdown.ToFlowDocument(Text,
                    new MarkdownPipelineBuilder()
                        .UseAdvancedExtensions()
                        .UseColorCodeWpf()
                        .UseTaskLists()
                        .UseGridTables()
                        .UsePipeTables()
                        .UseEmphasisExtras()
                        .UseAutoLinks()
                        .Build());
                return [.. doc.Blocks];
            }
            catch
            {
                // render failure, fallback to plain text mode
                return [new Paragraph(new Run(Text))];
            }
        }

        public IEnumerable<Block> Rendered
        {
            get
            {
                if (_cachedRenderResult is not null)
                {
                    return _cachedRenderResult;
                }

                _cachedRenderResult = EnableMarkdown ? RenderMarkdownText() : [new Paragraph(new Run(Text))];
                return _cachedRenderResult;
            }
        }
    }

    private partial class ImageMessage(BitmapImage image, string? filename, string? tooltip) : ObservableObject, IRichMessage
    {
        private BitmapImage Image { get; } = image;
        public string FileName { get; } = filename ?? "";
        private string ImageTooltip { get; } = tooltip ?? "";

        private List<Block>? _cachedRenderResult;

        private List<Block> RenderImage()
        {
            var imageGrid = new Controls.ImageDisplayer
            {
                ImageSource = Image,
                FileName = FileName,
                ImageTooltip = ImageTooltip
            };

            return [new BlockUIContainer(imageGrid)];
        }

        public IEnumerable<Block> Rendered
        {
            get
            {
                if (_cachedRenderResult is not null)
                {
                    return _cachedRenderResult;
                }

                _cachedRenderResult = RenderImage();
                return _cachedRenderResult;
            }
        }
    }

    private partial class BlocksMessage(IEnumerable<Block> blocks) : ObservableObject, IRichMessage
    {
        private IEnumerable<Block> Blocks { get; } = blocks;

        private List<Block>? _cachedRenderResult;

        public IEnumerable<Block> Rendered
        {
            get
            {
                if (_cachedRenderResult is not null)
                {
                    return _cachedRenderResult;
                }

                _cachedRenderResult = Blocks.ToList();
                return _cachedRenderResult;
            }
        }
    }

    private partial class TextFileMessage(string content, string? filename) : ObservableObject, IRichMessage
    {
        public string FileName { get; } = filename ?? "";
        private string FileContent { get; } = content;

        private List<Block>? _cachedRenderResult;

        private List<Block> RenderTextFile()
        {
            var textGrid = new Controls.TextFileDisplayer
            {
                FileName = FileName,
                TextContent = FileContent
            };
            return [new BlockUIContainer(textGrid)];
        }

        public IEnumerable<Block> Rendered
        {
            get
            {
                if (_cachedRenderResult is not null)
                {
                    return _cachedRenderResult;
                }

                _cachedRenderResult = RenderTextFile();
                return _cachedRenderResult;
            }
        }
    }

    public async Task AddText(TextContent text, bool enableMarkdown)
    {
        await Task.Run(() => { _messageList.Add(new TextMessage(text.Text, enableMarkdown, text)); });
        OnPropertyChanged(nameof(RenderedMessage));
    }

    public async Task AddTextFile(string fileName, string content)
    {
        await Task.Run(() => { _messageList.Add(new TextFileMessage(content, fileName)); });
        OnPropertyChanged(nameof(RenderedMessage));
    }

    private async Task AddImage(BitmapImage image, string? filename, string? tooltip)
    {
        await Task.Run(() => { _messageList.Add(new ImageMessage(image, filename, tooltip)); });
        OnPropertyChanged(nameof(RenderedMessage));
    }

    public async Task AddImage(ImageData imageData, string? filename, string? tooltip)
    {
        await AddImage(imageData.ToBitmapImage(), filename, tooltip);
    }

    public async Task AddBlocks(IEnumerable<Block> blocks)
    {
        await Task.Run(() => { _messageList.Add(new BlocksMessage(blocks)); });
        OnPropertyChanged(nameof(RenderedMessage));
    }

    private readonly List<IRichMessage> _messageList = [];

    /**** stream (temp) data ****/
    public StringBuilder? StreamMessage { get; private set; }

    public void AddStreamText(string text)
    {
        StreamMessage ??= new StringBuilder();
        StreamMessage.Append(text);
        OnPropertyChanged(nameof(RenderedMessage));
    }

    private double? _streamProgress;
    private string? _streamProgressText;

    public void SetStreamProgress(double progress, string text)
    {
        _streamProgress = progress;
        _streamProgressText = text;
        OnPropertyChanged(nameof(RenderedMessage));
    }

    /*** end of stream (temp) data ***/
    public FlowDocument RenderedMessage
    {
        get
        {
            var doc = Markdig.Wpf.Markdown.ToFlowDocument(""); // build from Markdown to ensure style

            foreach (var msg in _messageList)
            {
                doc.Blocks.AddRange(msg.Rendered);
            }

            if (StreamMessage is not null)
            {
                doc.Blocks.Add(new Paragraph(new Run(StreamMessage.ToString())));
            }

            if (_streamProgress is not null)
            {
                var progress = new HandyControl.Controls.CircleProgressBar
                {
                    Value = _streamProgress.Value,
                    Maximum = 1,
                    Height = 20,
                    Width = 20,
                    ShowText = false,
                    Margin = new Thickness(0, 0, 10, 0)
                };
                var text = new TextBlock
                {
                    Text = _streamProgressText,
                    VerticalAlignment = VerticalAlignment.Center
                };
                var stackpanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Children = { progress, text }
                };
                doc.Blocks.Add(new BlockUIContainer(stackpanel));
            }

            if (IsStreaming)
            {
                doc.Blocks.Add(_loadingBar);
            }

            doc.Foreground = ForegroundColor;
            return doc;
        }
    }

    public RoleType Role { get; set; }
    public static string UserAvatarSource => Environment.UserName;

    private const string ChatGPTIcon = "pack://application:,,,/images/chatgpt-icon.svg";
    private const string ClaudeIcon = "pack://application:,,,/images/claude-ai-icon.svg";
    private const string OtherModelIcon = "pack://application:,,,/images/meta-ai-icon.svg";
    private const string ToolIcon = "pack://application:,,,/images/set-up-svgrepo-com.svg";

    public Uri Avatar => Role switch
    {
        RoleType.Assistant => Provider switch
        {
            ModelInfo.ProviderEnum.OpenAI => new Uri(ChatGPTIcon),
            ModelInfo.ProviderEnum.Anthropic => new Uri(ClaudeIcon),
            ModelInfo.ProviderEnum.OtherOpenAICompat => new Uri(OtherModelIcon),
            _ => new Uri(ChatGPTIcon),
        },
        RoleType.Tool => new Uri(ToolIcon),
        _ => new Uri(ChatGPTIcon)
    };

    public Brush ForegroundColor => Role switch
    {
        RoleType.User => Brushes.Black,
        RoleType.System => Brushes.White,
        RoleType.Assistant => Brushes.Black,
        RoleType.Tool => Brushes.White,
        _ => Brushes.Black
    };

    public Brush BackgroundColor => Role switch
    {
        RoleType.User => new SolidColorBrush(Color.FromRgb(60, 209, 125)),
        RoleType.Assistant => new SolidColorBrush(Color.FromRgb(232, 230, 231)),
        RoleType.System => new SolidColorBrush(Color.FromRgb(77, 150, 244)),
        RoleType.Tool => new SolidColorBrush(Color.FromRgb(255, 173, 61)),
        _ => new SolidColorBrush(Color.FromRgb(232, 230, 231)),
    };

    public bool ShowLeftAvatar => Role switch
    {
        RoleType.User => false,
        RoleType.Assistant => true,
        RoleType.System => false,
        RoleType.Tool => true,
        _ => false
    };

    public bool ShowRightAvatar => Role switch
    {
        RoleType.User => true,
        RoleType.Assistant => false,
        RoleType.System => false,
        RoleType.Tool => false,
        _ => false
    };

    public bool ShowLeftBlank => Role switch
    {
        RoleType.System => false,
        _ => !ShowLeftAvatar
    };

    public bool ShowRightBlank => Role switch
    {
        RoleType.System => false,
        _ => !ShowRightAvatar
    };
}

public partial class ChatWindowMessageTab : ObservableObject
{
    public Guid TabId { get; } = Guid.NewGuid();
    public static bool IsEmptyTab(ChatWindowMessageTab? tab)
    {
        return tab is null || (tab.Messages.Count == 0 && !tab.UserHasGivenTitle);
    }

    private static int _totalTabIndexCount;
    private readonly int _assignedTabIndex = ++_totalTabIndexCount;

    private string? _title;
    public string? Title
    {
        get => _title ?? $"对话 {_assignedTabIndex}";
        set => SetProperty(ref _title, value);
    }

    public bool UserHasGivenTitle => _title is not null;
    [ObservableProperty]
    public partial bool IsEditingTitle { get; set; }

    [RelayCommand]
    private void EnterEditingTitleMode()
    {
        IsEditingTitle = true;
    }

    [RelayCommand]
    private void ConfirmEditingTitleMode()
    {
        IsEditingTitle = false;
        if (_syncedSession is not null)
        {
            _syncedSession.Title = Title;
        }
    }

    [ObservableProperty]
    public partial bool IsLoading { get; set; }
    public ObservableCollection<ChatWindowMessage> Messages { get; } = [];

    [RelayCommand]
    private void RemoveMessage(ChatWindowMessage msg)
    {
        Messages.Remove(msg);
        if (msg.SourceMessage is not null)
        {
            _syncedSession?.Messages.Remove(msg.SourceMessage);
        }
    }

    public void AddStreamText(string text)
    {
        Messages.Last().AddStreamText(text);
    }

    public int GetCurrentStreamTokenNum(ModelVersionInfo.TokenizerEnum tokenizer)
    {
        if (Messages.Count == 0)
        {
            return 0;
        }

        var lastMsg = Messages.Last();
        if (lastMsg.StreamMessage is null)
        {
            return 0;
        }

        return Utils.GetStringTokenCount(lastMsg.StreamMessage.ToString(), tokenizer);
    }

    public void AddMessage(RoleType role, ModelInfo.ProviderEnum? provider)
    {
        Messages.Add(new ChatWindowMessage { Role = role, IsStreaming = true, Provider = provider });
    }

    public void SetStreamProgress(double progress, string text)
    {
        Messages.Last().SetStreamProgress(progress, text);
    }

    private ChatCompletionRequest? _syncedSession;
    public void RemoveLastMessage()
    {
        _syncedSession?.Messages.RemoveAt(_syncedSession.Messages.Count - 1);
        Messages.RemoveAt(Messages.Count - 1);
    }

    public async Task SyncChatSession(ChatCompletionRequest session, Guid tabId, SystemState state,
        Config.MarkdownRenderMode markdownMode)
    {
        Messages.Clear();
        _syncedSession = session;
        if (UserHasGivenTitle)
        {
            session.Title = Title;
        }
        else
        {
            Title = session.Title;
        }

        foreach (var msg in session.Messages)
        {
            if (msg.Hidden)
            {
                continue;
            }

            var chatMsg = new ChatWindowMessage
            {
                Role = msg.Role,
                Provider = msg is AssistantMessage aMsg ? aMsg.Provider : null,
                SourceMessage = msg,
                DateTime = msg.DateTime
            };
            foreach (var content in msg.Content)
            {
                var enableMarkdown = 
                    (msg.Role == RoleType.Assistant && markdownMode == Config.MarkdownRenderMode.EnabledForAssistantMessages)
                    || markdownMode == Config.MarkdownRenderMode.EnabledForAllMessages;

                if (content is TextContent textContent)
                {
                    await chatMsg.AddText(textContent, enableMarkdown);
                }
                else if (content is ImageContent imgContent)
                {
                    var imageData = imgContent.ImageUrl.Url;
                    if (imageData is not null)
                    {
                        await chatMsg.AddImage(imageData, null, null);
                    }
                }
            }

            if (msg is UserMessage userMsg)
            {
                foreach (var file in userMsg.Attachments)
                {
                    switch (file)
                    {
                        case UserMessage.TextAttachmentInfo textFile:
                            await chatMsg.AddTextFile(textFile.FileName, textFile.Content);
                            break;
                        case UserMessage.ImageAttachmentInfo imageFile:
                            {
                                var imageData = imageFile.ImageBase64Url;
                                if (imageData is not null)
                                {
                                    await chatMsg.AddImage(imageData, imageFile.FileName, null);
                                }
                            }
                            break;
                        default:
                            throw new InvalidOperationException();
                    }
                }
            }
            else if (msg is AssistantMessage assistantMsg)
            {
                foreach (var toolcall in assistantMsg.ToolCalls ?? [])
                {
                    await chatMsg.AddBlocks(state.GetToolcallDescription(toolcall, tabId));
                }
            }
            else if (msg is ToolMessage toolMsg)
            {
#pragma warning disable CS0618 // 类型或成员已过时
                foreach (var img in toolMsg.GeneratedImages)
                {
                    var imageData = img.ImageBase64Url;
                    if (imageData is not null)
                    {
                        await chatMsg.AddImage(imageData, null, img.Description);
                    }
                }
#pragma warning restore CS0618 // 类型或成员已过时
            }

            Messages.Add(chatMsg);
        }
    }

    public FlowDocument GeneratePrintableDocument()
    {
        var doc = Markdig.Wpf.Markdown.ToFlowDocument(""); // build from Markdown to ensure style
        const double avatarSize = 32;
        const double avatarMargin = 10;
        const double sectionMargin = 20;

        foreach (var msg in Messages)
        {
            switch (msg.Role)
            {
                case RoleType.User:
                    var avatarId = UserAvatarSource;
                    HandyControl.Controls.Gravatar gravatar = new()
                    {
                        Id = avatarId,
                        Width = avatarSize,
                        Height = avatarSize,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Margin = new Thickness(0, 0, 0, avatarMargin)
                    };
                    doc.Blocks.Add(new BlockUIContainer(gravatar));
                    break;
                case RoleType.Assistant:
                case RoleType.Tool:
                    var avatarUri = msg.Avatar;
                    var svg = new SvgViewbox
                    {
                        Source = avatarUri,
                        Width = avatarSize,
                        Height = avatarSize,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Margin = new Thickness(0, 0, 0, avatarMargin)
                    };
                    doc.Blocks.Add(new BlockUIContainer(svg));
                    break;
                case RoleType.System:
                    // no avatar
                    break;
                default:
                    throw new InvalidOperationException();
            }

            var rendered = msg.RenderedMessage;
            var blocks = rendered.Blocks.ToList();
            foreach (var block in blocks)
            {
                block.Foreground = msg.ForegroundColor;
                block.Background = msg.BackgroundColor;
                block.Margin = new Thickness(0);
            }

            blocks.Last().Margin = new Thickness(0, 0, 0, sectionMargin);
            doc.Blocks.AddRange(blocks);
        }

        return doc;
    }

    public void Reset()
    {
        _syncedSession = null;
        Title = null;
        IsEditingTitle = false;
        IsLoading = false;
        Messages.Clear();
    }
}

/// <summary>
/// ChatWindow.xaml 的交互逻辑
/// </summary>
public partial class ChatWindowViewModel : ObservableObject
{
    [ObservableProperty]
    public partial SystemState State { get; set; }

    private void SetIsLoading(bool loading, Guid tabId)
    {
        GetTabById(tabId).IsLoading = loading;
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelectedMessageTab))]
    [NotifyPropertyChangedFor(nameof(SessionTokenNum))]
    [NotifyCanExecuteChangedFor(nameof(PrintCommand))]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    [NotifyPropertyChangedFor(nameof(IsSelectedTabValid))]
    public partial int SelectedTabIndex { get; set; }

    public bool IsSelectedTabValid => SelectedTabIndex >= 0 && SelectedTabIndex < ChatWindowMessageTabs.Count;
    public void SelectTab(Guid tabId)
    {
        var tab = ChatWindowMessageTabs.First(pair => pair.TabId == tabId);
        var index = ChatWindowMessageTabs.IndexOf(tab);
        SelectedTabIndex = index;
    }
    [RelayCommand]
    private void CreateTab()
    {
        ChatWindowMessageTabs.Add(new ChatWindowMessageTab());
        SelectedTabIndex = ChatWindowMessageTabs.Count - 1;
    }

    [RelayCommand]
    private void CloseTab(Guid tabId)
    {
        ChatWindowMessageTabs.Remove(ChatWindowMessageTabs.First(pair => pair.TabId == tabId));
        State.SessionDict.Remove(tabId);
        if (ChatWindowMessageTabs.Count == 0)
        {
            ChatWindowMessageTabs.Add(new ChatWindowMessageTab());
        }

        if (!IsSelectedTabValid)
        {
            SelectedTabIndex = ChatWindowMessageTabs.Count - 1;
        }
    }

    [RelayCommand]
    private void CloseCurrentTab()
    {
        if (IsSelectedTabValid)
        {
            CloseTab(SelectedMessageTab!.TabId);
        }
    }

    [RelayCommand]
    private void SelectedTabEnterEditTitleMode()
    {
        SelectedMessageTab?.EnterEditingTitleModeCommand.Execute(null);
    }

    public ObservableCollection<ChatWindowMessageTab> ChatWindowMessageTabs { get; } = [new ChatWindowMessageTab()];
    private ChatWindowMessageTab GetTabById(Guid tabId)
    {
        return ChatWindowMessageTabs.First(pair => pair.TabId == tabId);
    }
    public void MoveTabToIndex(Guid tabId, int targetIdx)
    {
        var tab = GetTabById(tabId);
        ChatWindowMessageTabs.Remove(tab);
        ChatWindowMessageTabs.Insert(targetIdx, tab);
    }

    public ChatWindowMessageTab? SelectedMessageTab =>
        IsSelectedTabValid
            ? ChatWindowMessageTabs[SelectedTabIndex]
            : null;

    public ObservableCollection<FileAttachmentInfo> FileAttachments { get; } = [];
    public bool IsFileAttachmentsEmpty => FileAttachments.Count == 0;

    public bool HasImageFileAttachment => FileAttachments.Select(file => MimeTypes.GetMimeType(file.Path))
        .Any(mime => mime.StartsWith("image/"));

    public ChatWindowViewModel()
    {
        State = new SystemState();
        State.ChatSessionChangedEvent += async (session, tabId) =>
        {
            await SyncChatSession(session, State.Config.EnableMarkdown, tabId);
        };
        State.NewMessageEvent += AddMessage;
        State.StreamTextEvent += AddStreamText;
        State.SetStreamProgressEvent += SetStreamProgress;
        State.SetIsLoadingHandlerEvent += SetIsLoading;

        State.Config.SelectedModelChangedEvent += () =>
        {
            OnPropertyChanged(nameof(SessionTokenNum));
            OnPropertyChanged(nameof(TextInputTokenNum));
        };

        FileAttachments.CollectionChanged += (_, _) =>
        {
            OnPropertyChanged(nameof(IsFileAttachmentsEmpty));
            OnPropertyChanged(nameof(HasImageFileAttachment));
        };
    }

    public delegate void ScrollToEndHandler(Guid tabId);

    public event ScrollToEndHandler? ScrollToEndEvent;

    private async Task SyncChatSession(ChatCompletionRequest session, Config.MarkdownRenderMode markdownMode, Guid tabId)
    {
        await GetTabById(tabId).SyncChatSession(session, tabId, State, markdownMode);
        ScrollToEndEvent?.Invoke(tabId);

        PrintCommand.NotifyCanExecuteChanged();
        SaveCommand.NotifyCanExecuteChanged();
        OnPropertyChanged(nameof(SessionTokenNum));
    }

    public int SessionTokenNum =>
        SelectedMessageTab is null ? 0 :
        State.GetSessionTokens(SelectedMessageTab.TabId) + 
        SelectedMessageTab.GetCurrentStreamTokenNum(State.Config.SelectedModel?.Tokenizer ?? ModelVersionInfo.TokenizerEnum.Cl100KBase);

    private void AddStreamText(string text, Guid tabId)
    {
        GetTabById(tabId).AddStreamText(text);
        ScrollToEndEvent?.Invoke(tabId);
        OnPropertyChanged(nameof(SessionTokenNum));
    }

    private void AddMessage(RoleType role, Guid tabId, ModelInfo.ProviderEnum? provider)
    {
        GetTabById(tabId).AddMessage(role, provider);
        ScrollToEndEvent?.Invoke(tabId);
    }

    private void SetStreamProgress(double progress, string text, Guid tabId)
    {
        GetTabById(tabId).SetStreamProgress(progress, text);
        ScrollToEndEvent?.Invoke(tabId);
    }

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(TextInputTokenNum))]
    public partial string TextInput { get; set; } = "";

    public int TextInputTokenNum => Utils.GetStringTokenCount(TextInput, 
        State.Config.SelectedModel?.Tokenizer ?? ModelVersionInfo.TokenizerEnum.Cl100KBase);

    [ObservableProperty]
    public partial bool IsIdle { get; set; } = true;
    partial void OnIsIdleChanged(bool value)
    {
        ReGenerateCommand.NotifyCanExecuteChanged();
        SendCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand(IncludeCancelCommand = true, CanExecute = nameof(IsIdle))]
    private async Task ReGenerateAsync(CancellationToken cancellationToken)
    {
        try
        {
            IsIdle = false;

            if (SelectedMessageTab is null)
            {
                return;
            }
            while (SelectedMessageTab.Messages.Count > 0 &&
                SelectedMessageTab.Messages.Last().Role is RoleType.Assistant or RoleType.Tool)
            {
                SelectedMessageTab.RemoveLastMessage();
            }
            await State.UserSendText(null, null, SelectedMessageTab.TabId, cancellationToken);
        }
        finally
        {
            IsIdle = true;
        }
    }

    [RelayCommand(IncludeCancelCommand = true, CanExecute = nameof(IsIdle))]
    private async Task SendAsync(CancellationToken cancellationToken)
    {
        try
        {
            IsIdle = false;

            if(SelectedMessageTab is null)
            {
                return;
            }
            var input = TextInput;
            var files = (from fileinfo in FileAttachments select fileinfo.Path).ToList();
            TextInput = "";
            FileAttachments.Clear();
            await State.UserSendText(input, files, SelectedMessageTab.TabId, cancellationToken);
        }
        finally
        {
            IsIdle = true;
        }
    }

    [RelayCommand]
    private void CancelSending()
    {
        if (SendCancelCommand.CanExecute(null))
        {
            SendCancelCommand.Execute(null);
        }
        if (ReGenerateCancelCommand.CanExecute(null))
        {
            ReGenerateCancelCommand.Execute(null);
        }
    }

    [RelayCommand]
    private async Task ResetAsync()
    {
        if (SelectedMessageTab is null)
        {
            return;
        }
        SelectedMessageTab.Reset();
        await State.ClearSession(SelectedMessageTab.TabId);
    }

    private bool SessionNotNull
    {
        get
        {
            if (SelectedMessageTab is null)
            {
                return false;
            }
            if(!State.SessionDict.TryGetValue(SelectedMessageTab.TabId, out var session))
            {
                return false;
            }
            return session is not null;
        }
    }

    [RelayCommand(CanExecute = nameof(SessionNotNull))]
    private async Task PrintAsync()
    {
        PrintDialog printDialog = new();
        if (printDialog.ShowDialog() != true)
        {
            return;
        }
        
        ChatWindowMessageTab tempMessages = new();
        await tempMessages.SyncChatSession(State.SessionDict[SelectedMessageTab!.TabId]!, SelectedMessageTab!.TabId, State,
            State.Config.EnableMarkdown);
        var doc = tempMessages.GeneratePrintableDocument();
        // default is 2 columns, uncomment below to use only one column
        /*
        doc.PagePadding = new Thickness(50);
        doc.ColumnGap = 0;
        doc.ColumnWidth = printDialog.PrintableAreaWidth;
        */
        var dps = (IDocumentPaginatorSource)doc;
        var dp = dps.DocumentPaginator;
        printDialog.PrintDocument(dp, "打印聊天记录");
    }


    [RelayCommand(CanExecute = nameof(SessionNotNull))]
    private async Task SaveAsync()
    {
        await State.SaveSession(SelectedMessageTab!.TabId);
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        var newTabCreated = false;
        if (!ChatWindowMessageTab.IsEmptyTab(SelectedMessageTab))
        {
            CreateTab();
            newTabCreated = true;
        }

        var loadSuccess = await State.LoadSession(SelectedMessageTab!.TabId);
        if (!loadSuccess && newTabCreated)
        {
            CloseTab(ChatWindowMessageTabs.Last().TabId);
        }
    }

    private const string OpenFileAttachmentDialogGuid = "B8F42507-693B-4713-8671-A76F02ED5ADB";

    public async Task AddSingleFileAttachment(string file)
    {
        if (!await SystemState.FileCanReadAsAttachment(file))
        {
            MessageBox.Show(
                $"不支持将 {Path.GetFileName(file)} 所属的文件格式作为附件发送！",
                "错误",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }
        else
        {
            FileAttachments.Add(await FileAttachmentInfo.Create(file, FileAttachments));
        }
    }

    [RelayCommand]
    private async Task AddFileAsync()
    {
        var dlg = new OpenFileDialog
        {
            Filter = "Any Files|*.*",
            ClientGuid = new Guid(OpenFileAttachmentDialogGuid)
        };
        if (dlg.ShowDialog() == true)
        {
            await AddSingleFileAttachment(dlg.FileName);
        }
    }

    [RelayCommand]
    private async Task SwitchMarkdownRenderingAsync()
    {
        if (SessionNotNull)
        {
            await State.RefreshSession(SelectedMessageTab!.TabId);
        }
    }
}