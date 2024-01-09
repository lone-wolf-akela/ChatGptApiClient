﻿using System;
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
using System.Windows.Interop;
using Microsoft.Win32;
using CommunityToolkit.Mvvm.Input;
using SharpVectors.Converters;
using System.Threading.Tasks;
using Microsoft.WindowsAPICodePack.Shell;

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
                var iconSrc = Imaging.CreateBitmapSourceFromHIcon(
                    icon.Handle,
                    new Int32Rect(0, 0, icon.Width, icon.Height),
                    BitmapSizeOptions.FromEmptyOptions()
                );
                iconSrc.Freeze();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Icon = iconSrc;
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
            var thumbnail = shellFile?.Thumbnail?.ExtraLargeBitmapSource;
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
public class ChatWindowMessage : ObservableObject
{
    public bool IsStreaming { get; init; }
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

    public string Message
    {
        init
        {
            messageList.Clear();
            messageList.Add(new RichMessage
            {
                Type = RichMessage.RichMessageType.Text,
                Text = value
            });
        }
    }
    internal class RichMessage
    {
        public enum RichMessageType
        {
            Text,
            Image,
            Blocks,
            TextFile,
        }
        public RichMessageType Type { get; init; } = RichMessageType.Text;
        /**** Text Type ****/
        public string? Text { get; init; }
        public bool EnableMarkdown { get; init; }
        /*******************/
        /**** Image Type ****/
        public BitmapImage? Image { get; init; }
        public string? ImageTooltip { get; init; }
        /********************/
        /***** Blocks Type *****/
        public IEnumerable<Block>? Blocks { get; init; }
        /***********************/
        /***** TextFile Type *****/
        public string? FileName { get; init; }
        public string? FileContent { get; init; }

        private List<Block>? cachedRenderResult;
        private List<Block> RenderMarkdownText()
        {
            try
            {
                var doc = Markdig.Wpf.Markdown.ToFlowDocument(Text ?? "",
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

                switch (Type)
                {
                    case RichMessageType.Text when !string.IsNullOrWhiteSpace(Text):
                        {
                            cachedRenderResult = EnableMarkdown ? RenderMarkdownText() : [new Paragraph(new Run(Text))];
                            return cachedRenderResult;
                        }
                    case RichMessageType.Image:
                        {
                            var imageGrid = new Controls.ImageDisplayer
                            {
                                Image = Image!,
                                ImageTooltip = ImageTooltip ?? ""
                            };

                            cachedRenderResult = [new BlockUIContainer(imageGrid)];
                            return cachedRenderResult;
                        }
                    case RichMessageType.Blocks:
                        {
                            cachedRenderResult = Blocks!.ToList();
                            return cachedRenderResult;
                        }
                    case RichMessageType.TextFile:
                        {
                            var textGrid = new Controls.TextFileDisplayer
                            {
                                FileName = FileName!,
                                TextContent = FileContent!
                            };
                            cachedRenderResult = [new BlockUIContainer(textGrid)];
                            return cachedRenderResult;
                        }
                    default:
                        {
                            cachedRenderResult = [];
                            return cachedRenderResult;
                        }
                }
            }
        }
    }
    public async Task AddText(string text, bool enableMarkdown)
    {
        await Task.Run(() =>
        {
            messageList.Add(new RichMessage { Type = RichMessage.RichMessageType.Text, Text = text, EnableMarkdown = enableMarkdown });
        });
        OnPropertyChanged(nameof(RenderedMessage));
    }

    public async Task AddTextFile(string fileName, string content)
    {
        await Task.Run(() =>
        {
            messageList.Add(new RichMessage { Type = RichMessage.RichMessageType.TextFile, FileName = fileName, FileContent = content });
        });
        OnPropertyChanged(nameof(RenderedMessage));
    }
    private async Task AddImage(BitmapImage image, string? tooltip)
    {
        await Task.Run(() =>
        {
            messageList.Add(new RichMessage
            {
                Type = RichMessage.RichMessageType.Image,
                Image = image,
                ImageTooltip = tooltip
            });
        });
        OnPropertyChanged(nameof(RenderedMessage));
    }
    public async Task AddImage(string base64Url, string? tooltip)
    {
        var bitmap = Utils.Base64ToBitmapImage(base64Url);
        await AddImage(bitmap, tooltip);
    }
    public async Task AddBlocks(IEnumerable<Block> blocks)
    {
        await Task.Run(() =>
        {
            messageList.Add(new RichMessage { Type = RichMessage.RichMessageType.Blocks, Blocks = blocks });
        });
        OnPropertyChanged(nameof(RenderedMessage));
    }

    /**** stream (temp) data ****/
    private readonly List<RichMessage> messageList = [];
    private StringBuilder? streamMessage;
    public void AddStreamText(string text)
    {
        streamMessage ??= new StringBuilder();
        streamMessage.Append(text);
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
            if (streamMessage is not null)
            {
                doc.Blocks.Add(new Paragraph(new Run(streamMessage.ToString())));
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
    private const string ToolIcon = "pack://application:,,,/images/set-up-svgrepo-com.svg";
    public Uri Avatar => Role switch
    {
        RoleType.Assistant => new Uri(ChatGPTIcon),
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

public class ChatWindowMessageList : ObservableObject
{
    public ObservableCollection<ChatWindowMessage> Messages { get; } = [];
    public void AddStreamText(string text)
    {
        Messages.Last().AddStreamText(text);
    }
    public void AddMessage(RoleType role)
    {
        Messages.Add(new ChatWindowMessage { Role = role, IsStreaming = true });
    }

    public void SetStreamProgress(double progress, string text)
    {
        Messages.Last().SetStreamProgress(progress, text);
    }
    public async Task SyncChatSession(ChatCompletionRequest session, SystemState state, bool enableMarkdown)
    {
        Messages.Clear();

        foreach (var msg in session.Messages)
        {
            if (msg.Hidden)
            {
                continue;
            }

            var chatMsg = new ChatWindowMessage
            {
                Role = msg.Role
            };
            foreach (var content in msg.Content)
            {
                if (content is TextContent textContent)
                {
                    await chatMsg.AddText(textContent.Text, enableMarkdown);
                }
                else if (content is ImageContent imgContent)
                {
                    await chatMsg.AddImage(imgContent.ImageUrl.Url, null);
                }
            }
            if (msg is UserMessage userMsg)
            {
                foreach(var file in userMsg.Attachments)
                {
                    await chatMsg.AddTextFile(file.FileName, file.Content);
                }
            }
            else if (msg is AssistantMessage assistantMsg)
            {
                foreach (var toolcall in assistantMsg.ToolCalls ?? [])
                {
                    await chatMsg.AddBlocks(state.GetToolcallDescription(toolcall));
                }
            }
            else if (msg is ToolMessage toolMsg)
            {
                foreach (var img in toolMsg.GeneratedImages)
                {
                    await chatMsg.AddImage(img.ImageBase64Url, img.Description);
                }
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
                    var avatarId = ChatWindowMessage.UserAvatarSource;
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
}

/// <summary>
/// ChatWindow.xaml 的交互逻辑
/// </summary>
public partial class ChatWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private SystemState state;
    [ObservableProperty]
    private bool isLoading;
    private void SetIsLoading(bool loading)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            IsLoading = loading;
        });
    }

    public ChatWindowMessageList MessageList { get; } = new();
    public ObservableCollection<FileAttachmentInfo> FileAttachments { get; } = [];
    public bool IsFileAttachmentsEmpty => FileAttachments.Count == 0;

    public bool HasImageFileAttachment => FileAttachments.Select(file => MimeTypes.GetMimeType(file.Path))
                                                         .Any(mime => mime.StartsWith("image/"));
    public ChatWindowViewModel()
    {
        State = new SystemState();
        State.ChatSessionChangedEvent += async session =>
        {
            await SyncChatSession(session, State.Config.EnableMarkdown);
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

    public delegate void ScrollToEndHandler();
    public event ScrollToEndHandler? ScrollToEndEvent;

    private async Task SyncChatSession(ChatCompletionRequest session, bool enableMarkdown)
    {
        await MessageList.SyncChatSession(session, State, enableMarkdown);
        ScrollToEndEvent?.Invoke();

        PrintCommand.NotifyCanExecuteChanged();
        SaveCommand.NotifyCanExecuteChanged();
    }
    private void AddStreamText(string text)
    {
        MessageList.AddStreamText(text);
        ScrollToEndEvent?.Invoke();
    }
    private void AddMessage(RoleType role)
    {
        MessageList.AddMessage(role);
        ScrollToEndEvent?.Invoke();
    }

    private void SetStreamProgress(double progress, string text)
    {
        MessageList.SetStreamProgress(progress, text);
        ScrollToEndEvent?.Invoke();
    }

    [ObservableProperty]
    private string textInput = "";
    [RelayCommand]
    private async Task SendAsync()
    {
        var input = TextInput;
        var files = (from fileinfo in FileAttachments select fileinfo.Path).ToList();
        TextInput = "";
        FileAttachments.Clear();
        await State.UserSendText(input, files);
    }

    [RelayCommand]
    private async Task ResetAsync()
    {
        await State.ClearSession();
    }

    private bool SessionNotNull => State.CurrentSession is not null;

    [RelayCommand(CanExecute = nameof(SessionNotNull))]
    private async Task PrintAsync()
    {
        PrintDialog printDialog = new();
        if (printDialog.ShowDialog() != true)
        {
            return;
        }

        ChatWindowMessageList tempMessages = new();
        await tempMessages.SyncChatSession(State.CurrentSession!, State, State.Config.EnableMarkdown);
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
        await State.SaveSession();
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        await State.LoadSession();
    }

    private const string OpenFileAttachmentDialogGuid = "B8F42507-693B-4713-8671-A76F02ED5ADB";

    [RelayCommand]
    private void Addfile()
    {
        var dlg = new OpenFileDialog
        {
            Filter = "Any Files|*.*",
            ClientGuid = new Guid(OpenFileAttachmentDialogGuid)
        };
        if (dlg.ShowDialog() == true)
        {
            FileAttachments.Add(new FileAttachmentInfo(dlg.FileName, FileAttachments));
        }
    }

    [RelayCommand]
    private async Task SwitchMarkdownRenderingAsync()
    {
        if (SessionNotNull)
        {
            await State.RefreshSession();
        }
    }
}