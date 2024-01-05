using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Markdig;
using Markdig.Wpf.ColorCode;
using System.Diagnostics;
using System.Windows.Controls.Primitives;
using static ChatGptApiClientV2.IMessage;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Interop;
using Microsoft.Win32;
using CommunityToolkit.Mvvm.Input;
using SharpVectors.Converters;
using System.IO;
using System.Threading.Tasks;

namespace ChatGptApiClientV2;

public class FileAttachmentInfo
{
    public FileAttachmentInfo(string path)
    {
        Path = path;
        var icon = System.Drawing.Icon.ExtractAssociatedIcon(Path);
        if (icon is not null)
        {
            Icon = Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                new Int32Rect(0, 0, icon.Width, icon.Height),
                BitmapSizeOptions.FromEmptyOptions()
            );
        }
        else
        {
            Icon = null;
        }
    }
    public string Path { get; }
    public ImageSource? Icon { get; }
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
            Blocks
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
        public List<Block> Rendered
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
                                ImageTooltip = ImageTooltip ?? "",
                                ImageMaxHeight = 300
                            };

                            cachedRenderResult = [new BlockUIContainer(imageGrid)];
                            return cachedRenderResult;
                        }
                    case RichMessageType.Blocks:
                        {
                            cachedRenderResult = Blocks!.ToList();
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
        await Task.Run(()=>
        {
            messageList.Add(new RichMessage { Type = RichMessage.RichMessageType.Text, Text = text, EnableMarkdown = enableMarkdown });
        });
        OnPropertyChanged(nameof(RenderedMessage));
    }
    public async Task AddImage(BitmapImage image, string? tooltip)
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
            if (msg is AssistantMessage assistantMsg)
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
[ObservableObject]
public partial class ChatWindow
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

    private void ScrollToParentProcecssor(object? sender, MouseWheelEventArgs e)
    {
        if (e.Handled || sender is not DependencyObject senderObj)
        {
            return;
        }
        e.Handled = true; // 防止事件再次触发

        // 创建一个新的MouseWheel事件，基于原来的事件
        var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
        {
            RoutedEvent = MouseWheelEvent,
            Source = senderObj
        };

        // 找到 ScrollViewer 控件并将事件手动传递给它
        var parent = VisualTreeHelper.GetParent(senderObj);
        while (parent is not null && parent is not ScrollViewer)
        {
            parent = VisualTreeHelper.GetParent(parent);
        }
        // 确认找到 ScrollViewer 控件并触发事件
        var parentUi = parent as UIElement;
        parentUi?.RaiseEvent(eventArg);
    }
    public ChatWindow()
    {
        InitializeComponent();
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

        var sendKeyBinding = new KeyBinding
        {
            Key = Key.Enter,
            Modifiers = ModifierKeys.Control,
            Command = new RelayCommand(() => BtnSend_Click(this, new RoutedEventArgs(ButtonBase.ClickEvent)))
        };
        InputBindings.Add(sendKeyBinding);        
    }

    private static ScrollViewer? GetScrollViewer(DependencyObject o)
    {
        // Return the DependencyObject if it is a ScrollViewer
        if (o is ScrollViewer s)
        {
            return s;
        }
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(o); i++)
        {
            var child = VisualTreeHelper.GetChild(o, i);
            var result = GetScrollViewer(child);
            if (result is not null)
            {
                return result;
            }
        }
        return null;
    }
    private void ScrollToEnd()
    {
        var scrollViewer = GetScrollViewer(LstMsg);
        scrollViewer?.UpdateLayout();
        scrollViewer?.ScrollToEnd();
    }

    private async Task SyncChatSession(ChatCompletionRequest session, bool enableMarkdown)
    {
        await MessageList.SyncChatSession(session, State, enableMarkdown);
        ScrollToEnd();
    }
    private void AddStreamText(string text)
    {
        MessageList.AddStreamText(text);
        ScrollToEnd();
    }
    private void AddMessage(RoleType role)
    {
        MessageList.AddMessage(role);
        ScrollToEnd();
    }

    private void SetStreamProgress(double progress, string text)
    {
        MessageList.SetStreamProgress(progress, text);
        ScrollToEnd();
    }

    private void OpenHyperlink(object sender, ExecutedRoutedEventArgs e)
    {
        var link = e.Parameter.ToString();
        if (link is not null)
        {
            Process.Start(new ProcessStartInfo(link) { UseShellExecute = true });
        }
    }

    private async void BtnSend_Click(object sender, RoutedEventArgs e)
    {
        var input = TxtInput.Text;
        var files = (from fileinfo in FileAttachments select fileinfo.Path).ToList();
        TxtInput.Text = "";
        FileAttachments.Clear();
        await State.UserSendText(input, files);
    }

    private async void BtnReset_Click(object sender, RoutedEventArgs e)
    {
        await State.ClearSession();
    }

    private async void BtnPrint_Click(object sender, RoutedEventArgs e)
    {
        PrintDialog printDialog = new();
        if (printDialog.ShowDialog() != true)
        {
            return;
        }

        if (State.CurrentSession is null)
        {
            return;
        }

        ChatWindowMessageList tempMessages = new();
        await tempMessages.SyncChatSession(State.CurrentSession, State, State.Config.EnableMarkdown);
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

    private class RenderScrollViewerResult(RenderTargetBitmap bitmap, int offsetY)
    {
        public readonly RenderTargetBitmap Bitmap = bitmap;
        public readonly int OffsetY = offsetY;
    }
    private static RenderScrollViewerResult RenderScrollViewer(ScrollViewer viewer, double dpiScaleX, double dpiScaleY)
    {
        // rounding to integer pixels to make sure sharp font rendering
        var offsetY = Math.Floor(viewer.VerticalOffset * dpiScaleY);
        viewer.ScrollToVerticalOffset(offsetY / dpiScaleY);
        viewer.UpdateLayout();

        var renderTargetBitmap = new RenderTargetBitmap(
            (int)(viewer.ActualWidth * dpiScaleX),
            (int)(viewer.ActualHeight * dpiScaleY),
            96 * dpiScaleX,
            96 * dpiScaleY,
            PixelFormats.Pbgra32
        );
        renderTargetBitmap.Render(viewer);
        return new RenderScrollViewerResult(renderTargetBitmap, (int)offsetY);
    }

    private const string SaveScreenshotDialogGuid =  "49CEC7E0-C84B-4B69-8238-B6EFB608D7DC";
    private async void BtnScreenshot_Click(object sender, RoutedEventArgs e)
    {
        var scrollViewer = GetScrollViewer(LstMsg);
        if (scrollViewer is null) { return; }

        var dpi = VisualTreeHelper.GetDpi(this);

        var dlg = new SaveFileDialog
        {
            FileName = "screenshot",
            DefaultExt = ".png",
            Filter = "PNG images|*.png",
            ClientGuid = new Guid(SaveScreenshotDialogGuid)
        };
        if (dlg.ShowDialog() != true) { return; }

        var oldScrollOffset = scrollViewer.VerticalOffset;

        scrollViewer.ScrollToTop();
        scrollViewer.UpdateLayout();

        List<RenderScrollViewerResult> bitmaps = [];

        // scroll 3/4 of a page a time
        var pageHeight = scrollViewer.ActualHeight * 3 / 4;
        bitmaps.Add(RenderScrollViewer(scrollViewer, dpi.DpiScaleX, dpi.DpiScaleY));
        var lastOffsetY = -1;
        while(true)
        {
            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + pageHeight);
            scrollViewer.UpdateLayout();
            var newScreenShot = RenderScrollViewer(scrollViewer, dpi.DpiScaleX, dpi.DpiScaleY);
            if(newScreenShot.OffsetY == lastOffsetY)
            {
                // we have reached the end and cannot scroll further down
                break;
            }
            bitmaps.Add(newScreenShot);
            lastOffsetY = newScreenShot.OffsetY;
        }

        bitmaps.Add(RenderScrollViewer(scrollViewer, dpi.DpiScaleX, dpi.DpiScaleY));

        // concat bitmaps
        var fullBitmap = new RenderTargetBitmap(
            (int)(bitmaps.Last().Bitmap.Width * dpi.DpiScaleX),
            (int)(bitmaps.Last().OffsetY + bitmaps.Last().Bitmap.Height * dpi.DpiScaleY),
            96 * dpi.DpiScaleX,
            96 * dpi.DpiScaleY,
            PixelFormats.Pbgra32
        );
        var drawingVisual = new DrawingVisual();
        using (var drawingContext = drawingVisual.RenderOpen())
        {
            foreach (var renderResult in bitmaps)
            {
                drawingContext.DrawImage(renderResult.Bitmap,
                    new Rect(0, renderResult.OffsetY / dpi.DpiScaleY,
                    renderResult.Bitmap.Width, renderResult.Bitmap.Height)
                );
            }
        }
        fullBitmap.Render(drawingVisual);
        // save as png
        PngBitmapEncoder png = new();
        png.Frames.Add(BitmapFrame.Create(fullBitmap));
        await using Stream stm = File.Create(dlg.FileName);
        png.Save(stm);

        scrollViewer.ScrollToVerticalOffset(oldScrollOffset);
        scrollViewer.UpdateLayout();
    }

    private async void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        await State.SaveSession();
    }

    private async void BtnLoad_Click(object sender, RoutedEventArgs e)
    {
        await State.LoadSession();
    }

    private const string OpenFileAttachmentDialogGuid = "B8F42507-693B-4713-8671-A76F02ED5ADB";
    private void BtnAddfile_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new OpenFileDialog
        {
            Filter = "Any Files|*.*",
            ClientGuid = new Guid(OpenFileAttachmentDialogGuid)
        };
        if (dlg.ShowDialog() == true)
        {
            FileAttachments.Add(new FileAttachmentInfo(dlg.FileName));
        }
    }

    private void BtnRemovefile_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        var fileAttachmentInfo = (FileAttachmentInfo)((ContentControl)sender).DataContext;
        FileAttachments.Remove(fileAttachmentInfo);
    }

    private void ClosePromptPopup(object sender, MouseButtonEventArgs e)
    {
        TgglPrompt.IsChecked = false;
    }

    private async void ChkMarkdown_Click(object sender, RoutedEventArgs e)
    {
        await State.RefreshSession();
    }

    private void BtnSettings_Click(object sender, RoutedEventArgs e)
    {
        var settingsDialog = new Settings(State.Config)
        {
            Owner = this
        };
        settingsDialog.ShowDialog();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        State.Config.RefreshTheme();
    }
}