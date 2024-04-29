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
using Microsoft.WindowsAPICodePack.Shell;
using static ChatGptApiClientV2.ChatWindowMessage;
using System.ComponentModel;
using System.IO;
using System.Threading;
// ReSharper disable UnusedParameterInPartialMethod
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedMember.Global

namespace ChatGptApiClientV2;

public partial class FileAttachmentInfo : ObservableObject
{
    public FileAttachmentInfo(string path, ICollection<FileAttachmentInfo> owner)
    {
        Path = path;
        Icon = null;
        ownerCollection = owner;

        // we should not extract icon on UI thread, which may cause the UI to stop responding.
        Task.Run(() =>
        {
            // this is fast but cannot show file preview
            var icon = Utils.Get256FileIcon(Path);
            if (icon is not null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Icon = icon;
                    OnPropertyChanged(nameof(Icon));
                });
            }

            // shellFile Thumbnail can be slow for large file, but can show file preview
            if (Utils.IsPathRemote(path))
            {
                // do not extract thumbnail for remote file
                // as it can be very slow
                return;
            }

            var shellFile = ShellFile.FromFilePath(path);
            var thumbnail = shellFile.Thumbnail?.ExtraLargeBitmapSource;
            thumbnail?.Freeze();
            Application.Current.Dispatcher.Invoke(() =>
            {
                Icon = thumbnail ?? Icon;
                OnPropertyChanged(nameof(Icon));
            });
        }).ConfigureAwait(false);
    }

    private readonly ICollection<FileAttachmentInfo> ownerCollection;
    [RelayCommand]
    private void RemoveFile()
    {
        ownerCollection.Remove(this);
    }
    public string Path { get; }
    public ImageSource? Icon { get; private set; }
}
public partial class ChatWindowMessage : ObservableObject
{
    [ObservableProperty]
    private bool isInEditingMode;
    public partial class EditorModeContent(string srcText) : ObservableObject
    {
        [ObservableProperty]
        private string text = srcText; // must use constructor to init, because write to Text will make it dirty
        public bool IsEditable { get; init; }
        partial void OnTextChanged(string value)
        {
            IsDirty = true;
        }
        public bool IsDirty { get; private set; }
        public IRichMessage? SourceMessage { get; init; }
    }
    public ObservableCollection<EditorModeContent> EditorModeContents { get; } = [];
    [RelayCommand]
    private void EnterEditingMode()
    {
        EditorModeContents.Clear();
        foreach(var msg in messageList)
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
        foreach(var content in EditorModeContents)
        {
            if (!content.IsDirty) { continue; }
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
    private bool isWaitingDeleteConfirm;
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

    public enum AssistantType
    {
        ChatGPT,
        Claude
    }
    public AssistantType Assistant { get; init; }

    private readonly BlockUIContainer loadingBar;


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

        loadingBar = new BlockUIContainer(loadingBarCtl);
    }

    public interface IRichMessage : INotifyPropertyChanged
    {
        public IEnumerable<Block> Rendered { get; }
    }

    private partial class TextMessage(string srcStr, bool enableMarkdown, TextContent? sourceContent) : ObservableObject, IRichMessage
    {
        public TextContent? SourceContent { get; } = sourceContent;
        [ObservableProperty]
        private string text = srcStr;
        partial void OnTextChanged(string value)
        {
            cachedRenderResult = null;
        }
        private bool EnableMarkdown { get; } = enableMarkdown;

        private List<Block>? cachedRenderResult;
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
            catch (Exception)
            {
                // render failure, fallback to plain text mode
                return [new Paragraph(new Run(Text))];
            }
        }
        public IEnumerable<Block> Rendered
        {
            get
            {
                if (cachedRenderResult is not null)
                {
                    return cachedRenderResult;
                }
                cachedRenderResult = EnableMarkdown ? RenderMarkdownText() : [new Paragraph(new Run(Text))];
                return cachedRenderResult;
            }
        }
    }

    private class ImageMessage(BitmapImage image, string? filename, string? tooltip) : ObservableObject, IRichMessage
    {
        private BitmapImage Image { get; } = image;
        public string FileName { get; } = filename ?? "";
        private string ImageTooltip { get; } = tooltip ?? "";

        private List<Block>? cachedRenderResult;
        private List<Block> RenderImage()
        {
            var imageGrid = new Controls.ImageDisplayer
            {
                Image = Image,
                FileName = FileName,
                ImageTooltip = ImageTooltip
            };

            return [new BlockUIContainer(imageGrid)];
        }
        public IEnumerable<Block> Rendered
        {
            get
            {
                if (cachedRenderResult is not null)
                {
                    return cachedRenderResult;
                }
                cachedRenderResult = RenderImage();
                return cachedRenderResult;
            }
        }
    }

    private class BlocksMessage(IEnumerable<Block> blocks) : ObservableObject, IRichMessage
    {
        private IEnumerable<Block> Blocks { get; } = blocks;

        private List<Block>? cachedRenderResult;
        public IEnumerable<Block> Rendered
        {
            get
            {
                if (cachedRenderResult is not null)
                {
                    return cachedRenderResult;
                }
                cachedRenderResult = Blocks.ToList();
                return cachedRenderResult;
            }
        }
    }

    private class TextFileMessage(string content, string? filename) : ObservableObject, IRichMessage
    {
        public string FileName { get; } = filename ?? "";
        private string FileContent { get; } = content;

        private List<Block>? cachedRenderResult;
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
                if (cachedRenderResult is not null)
                {
                    return cachedRenderResult;
                }
                cachedRenderResult = RenderTextFile();
                return cachedRenderResult;
            }
        }
    }

    public async Task AddText(TextContent text, bool enableMarkdown)
    {
        await Task.Run(() =>
        {
            messageList.Add(new TextMessage(text.Text, enableMarkdown, text));
        });
        OnPropertyChanged(nameof(RenderedMessage));
    }

    public async Task AddTextFile(string fileName, string content)
    {
        await Task.Run(() =>
        {
            messageList.Add(new TextFileMessage(content, fileName));
        });
        OnPropertyChanged(nameof(RenderedMessage));
    }
    private async Task AddImage(BitmapImage image, string? filename, string? tooltip)
    {
        await Task.Run(() =>
        {
            messageList.Add(new ImageMessage(image, filename, tooltip));
        });
        OnPropertyChanged(nameof(RenderedMessage));
    }
    public async Task AddImage(string base64Url, string? filename, string? tooltip)
    {
        var bitmap = Utils.Base64ToBitmapImage(base64Url);
        await AddImage(bitmap, filename, tooltip);
    }
    public async Task AddBlocks(IEnumerable<Block> blocks)
    {
        await Task.Run(() =>
        {
            messageList.Add(new BlocksMessage(blocks));
        });
        OnPropertyChanged(nameof(RenderedMessage));
    }

    private readonly List<IRichMessage> messageList = [];
    /**** stream (temp) data ****/
    public StringBuilder? StreamMessage { get; private set; }

    public void AddStreamText(string text)
    {
        StreamMessage ??= new StringBuilder();
        StreamMessage.Append(text);
        OnPropertyChanged(nameof(RenderedMessage));
    }
    private double? streamProgress;
    private string? streamProgressText;
    public void SetStreamProgress(double progress, string text)
    {
        streamProgress = progress;
        streamProgressText = text;
        OnPropertyChanged(nameof(RenderedMessage));
    }
    /*** end of stream (temp) data ***/
    public FlowDocument RenderedMessage
    {
        get
        {
            var doc = Markdig.Wpf.Markdown.ToFlowDocument(""); // build from Markdown to ensure style

            foreach (var msg in messageList)
            {
                doc.Blocks.AddRange(msg.Rendered);
            }
            if (StreamMessage is not null)
            {
                doc.Blocks.Add(new Paragraph(new Run(StreamMessage.ToString())));
            }
            if (streamProgress is not null)
            {
                var progress = new HandyControl.Controls.CircleProgressBar
                {
                    Value = streamProgress.Value,
                    Maximum = 1,
                    Height = 20,
                    Width = 20,
                    ShowText = false,
                    Margin = new Thickness(0, 0, 10, 0)
                };
                var text = new TextBlock
                {
                    Text = streamProgressText,
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
                doc.Blocks.Add(loadingBar);
            }
            doc.Foreground = ForegroundColor;
            return doc;
        }
    }
    public RoleType Role { get; set; }
    public static string UserAvatarSource => Environment.UserName;

    private const string ChatGPTIcon = "pack://application:,,,/images/chatgpt-icon.svg";
    private const string ClaudeIcon = "pack://application:,,,/images/claude-ai-icon.svg";
    private const string ToolIcon = "pack://application:,,,/images/set-up-svgrepo-com.svg";
    public Uri Avatar => Role switch
    {
        RoleType.Assistant => Assistant == AssistantType.ChatGPT ? new Uri(ChatGPTIcon) : new Uri(ClaudeIcon),
        RoleType.Tool => new Uri(ToolIcon),
        _ => new Uri(ChatGPTIcon)
    };
    public Brush ForegroundColor => Role switch
    {
        RoleType.User => Brushes.Black,
        RoleType.System => Brushes.White,
        RoleType.Assistant => Brushes.Black,
        RoleType.Tool => Brushes.White,
        _ => throw new InvalidOperationException()
    };
    public Brush BackgroundColor => Role switch
    {
        RoleType.User => new SolidColorBrush(Color.FromRgb(60, 209, 125)),
        RoleType.Assistant => new SolidColorBrush(Color.FromRgb(232, 230, 231)),
        RoleType.System => new SolidColorBrush(Color.FromRgb(77, 150, 244)),
        RoleType.Tool => new SolidColorBrush(Color.FromRgb(255, 173, 61)),
        _ => throw new InvalidOperationException()
    };
    public bool ShowLeftAvatar => Role switch
    {
        RoleType.User => false,
        RoleType.Assistant => true,
        RoleType.System => false,
        RoleType.Tool => true,
        _ => throw new InvalidOperationException()
    };
    public bool ShowRightAvatar => Role switch
    {
        RoleType.User => true,
        RoleType.Assistant => false,
        RoleType.System => false,
        RoleType.Tool => false,
        _ => throw new InvalidOperationException()
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
    public static bool IsEmptyTab(ChatWindowMessageTab? tab)
    {
        return tab is null || (tab.Messages.Count == 0 && !tab.UserHasGivenTitle);
    }

    private static int totalTabIndexCount;
    private readonly int assignedTabIndex = ++totalTabIndexCount;
    private string? title;
    public string? Title
    {
        get => title ?? $"对话 {assignedTabIndex}";
        set => SetProperty(ref title, value);
    }
    public bool UserHasGivenTitle => title is not null;
    [ObservableProperty]
    private bool isEditingTitle;
    [RelayCommand]
    private void EnterEditingTitleMode()
    {
        IsEditingTitle = true;
    }
    [RelayCommand]
    private void ConfirmEditingTitleMode()
    {
        IsEditingTitle = false;
        if (syncedSession is not null)
        {
            syncedSession.Title = Title;
        }
    }
    [ObservableProperty]
    private bool isLoading;
    public ObservableCollection<ChatWindowMessage> Messages { get; } = [];
    [RelayCommand]
    private void RemoveMessage(ChatWindowMessage msg)
    {
        Messages.Remove(msg);
        if (msg.SourceMessage is not null)
        {
            syncedSession?.Messages.Remove(msg.SourceMessage);
        }
    }
    public void AddStreamText(string text)
    {
        Messages.Last().AddStreamText(text);
    }
    public int GetCurrentStreamTokenNum()
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
        return Utils.GetStringTokenNum(lastMsg.StreamMessage.ToString());
    }
    public void AddMessage(RoleType role, AssistantType assistantType)
    {
        Messages.Add(new ChatWindowMessage { Role = role, IsStreaming = true, Assistant = assistantType });
    }

    public void SetStreamProgress(double progress, string text)
    {
        Messages.Last().SetStreamProgress(progress, text);
    }
    private ChatCompletionRequest? syncedSession;
    public async Task SyncChatSession(ChatCompletionRequest session, int tabIndex, SystemState state, bool enableMarkdown)
    {
        Messages.Clear();
        syncedSession = session;
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
                Assistant = state.Config.SelectedModelType?.Provider == ModelInfo.ProviderEnum.Anthropic
                    ? AssistantType.Claude
                    : AssistantType.ChatGPT,
                SourceMessage = msg
            };
            foreach (var content in msg.Content)
            {
                if (content is TextContent textContent)
                {
                    await chatMsg.AddText(textContent, enableMarkdown);
                }
                else if (content is ImageContent imgContent)
                {
                    await chatMsg.AddImage(imgContent.ImageUrl.Url, null, null);
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
                            await chatMsg.AddImage(imageFile.ImageBase64Url, imageFile.FileName, null);
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
                    await chatMsg.AddBlocks(state.GetToolcallDescription(toolcall, tabIndex));
                }
            }
            else if (msg is ToolMessage toolMsg)
            {
#pragma warning disable CS0612 // 类型或成员已过时
                foreach (var img in toolMsg.GeneratedImages)
                {
                    await chatMsg.AddImage(img.ImageBase64Url, null, img.Description);
                }
#pragma warning restore CS0612 // 类型或成员已过时
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
        syncedSession = null;
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
    private SystemState state;
    private void SetIsLoading(bool loading, int tabIndex)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            ChatWindowMessageTabs[tabIndex].IsLoading = loading;
        });
    }
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelectedMessageTab))]
    [NotifyPropertyChangedFor(nameof(SessionTokenNum))]
    [NotifyCanExecuteChangedFor(nameof(PrintCommand))]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    [NotifyPropertyChangedFor(nameof(IsSelectedTabValid))]
    private int selectedTabIndex;
    public bool IsSelectedTabValid => SelectedTabIndex >= 0 && SelectedTabIndex < ChatWindowMessageTabs.Count; 
    [RelayCommand]
    private void CreateTab()
    {
        ChatWindowMessageTabs.Add(new ChatWindowMessageTab());
        SelectedTabIndex = ChatWindowMessageTabs.Count - 1;
    }
    [RelayCommand]
    private void CloseTab(ChatWindowMessageTab tabToClose)
    {
        ChatWindowMessageTabs.Remove(tabToClose);
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
            CloseTab(SelectedMessageTab!);
        }
    }
    [RelayCommand]
    private void SelectedTabEnterEditTitleMode()
    {
        SelectedMessageTab?.EnterEditingTitleModeCommand.Execute(null);
    }
    public ObservableCollection<ChatWindowMessageTab> ChatWindowMessageTabs { get; } = [new ChatWindowMessageTab()];
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
        State.ChatSessionChangedEvent += async (session, tabIndex) =>
        {
            await SyncChatSession(session, State.Config.EnableMarkdown, tabIndex);
        };
        State.NewMessageEvent += AddMessage;
        State.StreamTextEvent += AddStreamText;
        State.SetStreamProgressEvent += SetStreamProgress;
        State.SetIsLoadingHandlerEvent += SetIsLoading;

        FileAttachments.CollectionChanged += (_, _) =>
        {
            OnPropertyChanged(nameof(IsFileAttachmentsEmpty));
            OnPropertyChanged(nameof(HasImageFileAttachment));
        };
    }

    public delegate void ScrollToEndHandler(int tabIndex);
    public event ScrollToEndHandler? ScrollToEndEvent;

    private async Task SyncChatSession(ChatCompletionRequest session, bool enableMarkdown, int tabIndex)
    {
        await ChatWindowMessageTabs[tabIndex].SyncChatSession(session, tabIndex, State, enableMarkdown);
        ScrollToEndEvent?.Invoke(tabIndex);

        PrintCommand.NotifyCanExecuteChanged();
        SaveCommand.NotifyCanExecuteChanged();
        OnPropertyChanged(nameof(SessionTokenNum));
    }

    public int SessionTokenNum => State.GetSessionTokens(SelectedTabIndex) + (SelectedMessageTab?.GetCurrentStreamTokenNum() ?? 0);

    private void AddStreamText(string text, int tabIndex)
    {
        ChatWindowMessageTabs[tabIndex].AddStreamText(text);
        ScrollToEndEvent?.Invoke(tabIndex);
        OnPropertyChanged(nameof(SessionTokenNum));
    }
    private void AddMessage(RoleType role, int tabIndex)
    {
        ChatWindowMessageTabs[tabIndex].AddMessage(
            role, 
            State.Config.SelectedModelType?.Provider == ModelInfo.ProviderEnum.Anthropic 
            ? AssistantType.Claude 
            : AssistantType.ChatGPT);
        ScrollToEndEvent?.Invoke(tabIndex);
    }

    private void SetStreamProgress(double progress, string text, int tabIndex)
    {
        ChatWindowMessageTabs[tabIndex].SetStreamProgress(progress, text);
        ScrollToEndEvent?.Invoke(tabIndex);
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TextInputTokenNum))]
    private string textInput = "";

    public int TextInputTokenNum => Utils.GetStringTokenNum(TextInput);


    [RelayCommand(IncludeCancelCommand = true)]
    private async Task SendAsync(CancellationToken cancellationToken)
    {
        try
        {
            var input = TextInput;
            var files = (from fileinfo in FileAttachments select fileinfo.Path).ToList();
            TextInput = "";
            FileAttachments.Clear();
            await State.UserSendText(input, files, SelectedTabIndex, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            // ignore
        }
    }

    [RelayCommand]
    private async Task ResetAsync()
    {
        SelectedMessageTab?.Reset();
        await State.ClearSession(SelectedTabIndex);
    }

    private bool SessionNotNull =>
        IsSelectedTabValid && SelectedTabIndex < State.SessionList.Count && State.SessionList[SelectedTabIndex] is not null;

    [RelayCommand(CanExecute = nameof(SessionNotNull))]
    private async Task PrintAsync()
    {
        PrintDialog printDialog = new();
        if (printDialog.ShowDialog() != true)
        {
            return;
        }

        ChatWindowMessageTab tempMessages = new();
        await tempMessages.SyncChatSession(State.SessionList[SelectedTabIndex]!, SelectedTabIndex, State, State.Config.EnableMarkdown);
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
        await State.SaveSession(SelectedTabIndex);
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
        var loadSuccess = await State.LoadSession(SelectedTabIndex);
        if (!loadSuccess && newTabCreated)
        {
            CloseTab(ChatWindowMessageTabs.Last());
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
            FileAttachments.Add(new FileAttachmentInfo(file, FileAttachments));
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
            await State.RefreshSession(SelectedTabIndex);
        }
    }
}