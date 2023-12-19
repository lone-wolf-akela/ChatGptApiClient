using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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
using System.Windows.Media.Effects;

namespace ChatGptApiClientV2;

public class ContainerWidthConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // 减去一些像素来容纳滚动条等
        var actualWidth = (double?)value;
        return actualWidth - 30;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
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
                            if (EnableMarkdown)
                            {
                                var newDoc = Markdig.Wpf.Markdown.ToFlowDocument(Text ?? "",
                                    new MarkdownPipelineBuilder()
                                        .UseAdvancedExtensions()
                                        .UseColorCodeWpf()
                                        .UseTaskLists()
                                        .UseGridTables()
                                        .UsePipeTables()
                                        .UseEmphasisExtras()
                                        .UseAutoLinks()
                                        .Build());
                                cachedRenderResult = newDoc.Blocks.ToList();
                                return cachedRenderResult;
                            }
                            cachedRenderResult = [new Paragraph(new Run(Text))];
                            return cachedRenderResult;
                        }
                    case RichMessageType.Image:
                        {
                            var btnOpenImageViewer = new Button
                            {
                                Content = "在单独窗口中打开",
                                Margin = new Thickness(0, 0, 5, 0)
                            };
                            btnOpenImageViewer.Click += (_, _) =>
                            {
                                var viewer = new ImageViewer(Image!);
                                viewer.ShowDialog();
                            };
                            var btnShowImageDetails = new Button
                            {
                                Content = "查看图片信息",
                            };
                            var btnPanel = new StackPanel
                            {
                                Orientation = Orientation.Horizontal,
                                VerticalAlignment = VerticalAlignment.Bottom,
                                HorizontalAlignment = HorizontalAlignment.Right,
                                Margin = new Thickness(0, 0, 10, 10),
                                Children = { btnOpenImageViewer, btnShowImageDetails },
                                Visibility = Visibility.Collapsed
                            };
                            if (ImageTooltip is not null)
                            {
                                var imageTooltip = new TextBox
                                {
                                    Text = ImageTooltip,
                                    MaxWidth = 600,
                                    TextWrapping = TextWrapping.Wrap,
                                    IsReadOnly = true
                                };
                                var popUp = new Popup
                                {
                                    Placement = PlacementMode.Mouse,
                                    AllowsTransparency = true,
                                    StaysOpen = false,
                                    Child = imageTooltip
                                };
                                btnShowImageDetails.Click += (_, _) =>
                                {
                                    popUp.IsOpen = true;
                                };
                            }
                            else
                            {
                                btnShowImageDetails.Visibility = Visibility.Collapsed;
                            }

                            var image = new Image
                            {
                                Source = Image,
                                Stretch = Stretch.Uniform,
                                MaxHeight = 300
                            };
                            var imageGrid = new Grid
                            {
                                HorizontalAlignment = HorizontalAlignment.Left,
                                Children = { image, btnPanel }
                            };

                            imageGrid.MouseEnter += (s, e) =>
                            {
                                btnPanel.Visibility = Visibility.Visible;
                            };

                            imageGrid.MouseLeave += (s, e) =>
                            {
                                btnPanel.Visibility = Visibility.Collapsed;
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
    public void AddText(string text, bool enableMarkdown)
    {
        messageList.Add(new RichMessage { Type = RichMessage.RichMessageType.Text, Text = text, EnableMarkdown = enableMarkdown });
        OnPropertyChanged(nameof(RenderedMessage));
    }
    public void AddImage(BitmapImage image, string? tooltip)
    {
        messageList.Add(new RichMessage
        {
            Type = RichMessage.RichMessageType.Image,
            Image = image,
            ImageTooltip = tooltip
        });
        OnPropertyChanged(nameof(RenderedMessage));
    }
    public void AddImage(string base64Url, string? tooltip)
    {
        var bitmap = Utils.Base64ToBitmapImage(base64Url);
        AddImage(bitmap, tooltip);
    }

    public void AddBlocks(IEnumerable<Block> blocks)
    {
        messageList.Add(new RichMessage { Type = RichMessage.RichMessageType.Blocks, Blocks = blocks });
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

    public Uri Avatar => Role switch
    {
        RoleType.Assistant => new Uri("pack://application:,,,/images/chatgpt-icon.svg"),
        RoleType.Tool => new Uri("pack://application:,,,/images/set-up-svgrepo-com.svg"),
        _ => new Uri("pack://application:,,,/images/chatgpt-icon.svg")
    };
    public Brush ForegroundColor => Role switch
    {
        RoleType.User => new SolidColorBrush(Color.FromRgb(0, 0, 0)),
        RoleType.System => new SolidColorBrush(Color.FromRgb(255, 255, 255)),
        RoleType.Assistant => new SolidColorBrush(Color.FromRgb(0, 0, 0)),
        RoleType.Tool => new SolidColorBrush(Color.FromRgb(255, 255, 255)),
        _ => throw new NotImplementedException()
    };
    public Brush BackgroundColor => Role switch
    {
        RoleType.User => new SolidColorBrush(Color.FromRgb(60, 209, 125)),
        RoleType.Assistant => new SolidColorBrush(Color.FromRgb(232, 230, 231)),
        RoleType.System => new SolidColorBrush(Color.FromRgb(77, 150, 244)),
        RoleType.Tool => new SolidColorBrush(Color.FromRgb(255, 173, 61)),
        _ => throw new NotImplementedException()
    };
    public bool ShowLeftAvatar => Role switch
    {
        RoleType.User => false,
        RoleType.Assistant => true,
        RoleType.System => false,
        RoleType.Tool => true,
        _ => throw new NotImplementedException()
    };
    public bool ShowRightAvatar => Role switch
    {
        RoleType.User => true,
        RoleType.Assistant => false,
        RoleType.System => false,
        RoleType.Tool => false,
        _ => throw new NotImplementedException()
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

public partial class ChatWindowMessageList : ObservableObject
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
    public void SyncChatSession(ChatCompletionRequest session, SystemState state, bool enableMarkdown)
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
                    chatMsg.AddText(textContent.Text, enableMarkdown);
                }
                else if (content is ImageContent imgContent)
                {
                    chatMsg.AddImage(imgContent.ImageUrl.Url, null);
                }
            }
            if (msg is AssistantMessage assistantMsg)
            {
                foreach (var toolcall in assistantMsg.ToolCalls ?? [])
                {
                    chatMsg.AddBlocks(state.GetToolcallDescription(toolcall));
                }
            }
            else if (msg is ToolMessage toolMsg)
            {
                foreach (var img in toolMsg.GeneratedImages)
                {
                    chatMsg.AddImage(img.ImageBase64Url, img.Description);
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
                    throw new NotImplementedException();
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

    public ChatWindowMessageList MessageList { get; } = new();
    public ObservableCollection<FileAttachmentInfo> FileAttachments { get; } = [];
    public bool IsFileAttachmentsEmpty => FileAttachments.Count == 0;

    private void SmoothScrollProcecssor(object? sender, MouseWheelEventArgs e)
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
        State.ChatSessionChangedEvent += session =>
        {
            SyncChatSession(session, State.Config.EnableMarkdown);
        };
        State.NewMessageEvent += AddMessage;
        State.StreamTextEvent += AddStreamText;
        State.SetStreamProgressEvent += SetStreamProgress;

        FileAttachments.CollectionChanged += (_, _) =>
        {
            OnPropertyChanged(nameof(IsFileAttachmentsEmpty));
        };

        var sendKeyBinding = new KeyBinding
        {
            Key = Key.Enter,
            Modifiers = ModifierKeys.Control,
            Command = new RelayCommand(() => btn_send_Click(this, new RoutedEventArgs(ButtonBase.ClickEvent)))
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
        scrollViewer?.Dispatcher.Invoke(() =>
        {
            scrollViewer.ScrollToEnd();
        });
    }

    private void SyncChatSession(ChatCompletionRequest session, bool enableMarkdown)
    {
        MessageList.SyncChatSession(session, State, enableMarkdown);
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

    private async void btn_send_Click(object sender, RoutedEventArgs e)
    {
        var input = TxtInput.Text;
        var files = (from fileinfo in FileAttachments select fileinfo.Path).ToList();
        TxtInput.Text = "";
        FileAttachments.Clear();
        await State.UserSendText(input, files);
    }

    private void btn_reset_Click(object sender, RoutedEventArgs e)
    {
        State.ClearSession();
    }

    private void btn_print_Click(object sender, RoutedEventArgs e)
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
        tempMessages.SyncChatSession(State.CurrentSession, State, State.Config.EnableMarkdown);
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

    private void btn_save_Click(object sender, RoutedEventArgs e)
    {
        State.SaveSession();
    }

    private void btn_load_Click(object sender, RoutedEventArgs e)
    {
        State.LoadSession();
    }

    private void btn_addfile_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new OpenFileDialog
        {
            Filter = "Any Files|*.*",
            ClientGuid = new Guid("B8F42507-693B-4713-8671-A76F02ED5ADB")
        };
        if (dlg.ShowDialog() == true)
        {
            FileAttachments.Add(new FileAttachmentInfo(dlg.FileName));
        }
    }

    private void btn_removefile_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        var fileAttachmentInfo = (FileAttachmentInfo)((ContentControl)sender).DataContext;
        FileAttachments.Remove(fileAttachmentInfo);
    }

    private void ClosePromptPopup(object sender, MouseButtonEventArgs e)
    {
        TgglPrompt.IsChecked = false;
    }

    private void chk_markdown_Click(object sender, RoutedEventArgs e)
    {
        State.RefreshSession();
    }

    private void btn_settings_Click(object sender, RoutedEventArgs e)
    {
        var settingsDialog = new Settings(State.Config);
        settingsDialog.ShowDialog();
    }
}