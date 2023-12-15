using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Markdig;
using Markdig.Wpf;
using Markdig.Wpf.ColorCode;
using System.Diagnostics;
using HandyControl.Data;
using static ChatGptApiClientV2.IMessage;
using CommunityToolkit.Mvvm.ComponentModel;
using Jdenticon;
using System.Security.Principal;
using SharpVectors;
using System.Windows.Interop;
using Microsoft.Win32;
using System.Windows.Resources;

namespace ChatGptApiClientV2
{
    public class ContainerWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 假设我们要减去一些像素来容纳滚动条等
            double actualWidth = (double)value;
            return actualWidth - 30;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
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
        public string Path { get; private set; }
        public ImageSource? Icon { get; private set; }
    }
    public class ChatWindowMessage : ObservableObject
    {
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
                Image
            }
            public RichMessageType Type { get; set; } = RichMessageType.Text;
            /**** Text Type ****/
            public string? Text { get; set; } = null;
            public bool EnableMarkdown { get; set; } = true;
            /*******************/
            /**** Image Type ****/
            public BitmapImage? Image { get; set; } = null;
            public string? ImageTooltip { get; set; } = null;
            /********************/
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
        public void AddImage(string base64url, string? tooltip)
        {
            var bitmap = Utils.Base64ToBitmapImage(base64url);
            AddImage(bitmap, tooltip);
        }
        private readonly List<RichMessage> messageList = [];
        private StringBuilder? streamMessage = null;
        public void AddStreamText(string text)
        {
            streamMessage ??= new StringBuilder();
            streamMessage.Append(text);
            OnPropertyChanged(nameof(RenderedMessage));
        }
        public FlowDocument RenderedMessage
        {
            get
            {
                FlowDocument doc = Markdig.Wpf.Markdown.ToFlowDocument(""); // build from Markdown to ensure style
                foreach (var msg in messageList)
                {
                    if (msg.Type == RichMessage.RichMessageType.Text)
                    {
                        if (msg.EnableMarkdown)
                        {
                            var newDoc = Markdig.Wpf.Markdown.ToFlowDocument(msg.Text ?? "",
                                new MarkdownPipelineBuilder()
                                .UseAdvancedExtensions()
                                .UseColorCodeWpf()
                                .UseTaskLists()
                                .UseGridTables()
                                .UsePipeTables()
                                .UseEmphasisExtras()
                                .UseAutoLinks()
                                .Build());
                            doc.Blocks.AddRange(newDoc.Blocks.ToList());
                        }
                        else
                        {
                            doc.Blocks.Add(new Paragraph(new Run(msg.Text)));
                        }
                    }
                    else if (msg.Type == RichMessage.RichMessageType.Image)
                    {
                        var image = new Image 
                        { 
                            Source = msg.Image,
                            Stretch = Stretch.Uniform,
                            MaxHeight = 300,
                            Margin = new Thickness(0, 0, 10, 0)
                        };
                        if (msg.ImageTooltip is not null)
                        {
                            image.ToolTip = new ToolTip 
                            { 
                                Content = new TextBlock
                                {
                                    Text = msg.ImageTooltip,
                                    MaxWidth = 600,
                                    TextWrapping = TextWrapping.Wrap
                                }
                            };
                        }
                        var btn = new Button
                        {
                            Content = "在单独窗口中打开",
                            VerticalAlignment = VerticalAlignment.Bottom
                        };
                        btn.Click += (sender, e) =>
                        {
                            var viewer = new ImageViewer(msg.Image!);
                            viewer.ShowDialog();
                        };
                        var stackpanel = new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            Children = { image, btn }
                        };
                        doc.Blocks.Add(new BlockUIContainer(stackpanel));
                    }
                }
                if (streamMessage is not null)
                {
                    doc.Blocks.Add(new Paragraph(new Run(streamMessage.ToString())));
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
            RoleType.User => true,
            RoleType.Assistant => false,
            RoleType.System => true,
            RoleType.Tool => false,
            _ => throw new NotImplementedException()
        };
        public bool ShowRightBlank => Role switch
        {
            RoleType.User => false,
            RoleType.Assistant => true,
            RoleType.System => true,
            RoleType.Tool => true,
            _ => throw new NotImplementedException()
        };
    }
    /// <summary>
    /// ChatWindow.xaml 的交互逻辑
    /// </summary>
    [INotifyPropertyChanged]
    public partial class ChatWindow : Window
    {
        [ObservableProperty]
        private SystemState state;
        public ObservableCollection<ChatWindowMessage> Messages { get; } = [];
        public ObservableCollection<FileAttachmentInfo> FileAttachments { get; } = [];
        public bool IsFileAttachmentsNotEmpty => FileAttachments.Count != 0;

        private bool firstInput = true;
        private void SmoothScrollProcecssor(object sender, MouseWheelEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = true; // 防止事件再次触发

                // 创建一个新的MouseWheel事件，基于原来的事件
                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
                {
                    RoutedEvent = MouseWheelEvent,
                    Source = sender
                };

                // 找到ListView控件并将事件手动传递给其
                var parent = (UIElement)VisualTreeHelper.GetParent((DependencyObject)sender);
                while (parent is not null && parent is not ScrollViewer)
                {
                    parent = (UIElement)VisualTreeHelper.GetParent(parent);
                }
                // 确认找到ListView控件并触发事件
                parent?.RaiseEvent(eventArg);
            }
        }
        public ChatWindow()
        {
            InitializeComponent();
            State = new();
            State.ChatSessionChangedEvent += (session) =>
            {
                SyncChatSession(session, State.Config.EnableMarkdown);
            };
            State.NewMessageEvent += AddMessage;
            State.StreamTextEvent += AddStreamText;

            FileAttachments.CollectionChanged += (sender, e) =>
            {
                OnPropertyChanged(nameof(IsFileAttachmentsNotEmpty));
            };
        }
        private static ScrollViewer? GetScrollViewer(DependencyObject o)
        {
            // Return the DependencyObject if it is a ScrollViewer
            if (o is ScrollViewer s) 
            { 
                return s; 
            }
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(o); i++)
            {
                var child = VisualTreeHelper.GetChild(o, i);
                var result = GetScrollViewer(child);
                if (result is null) 
                { 
                    continue; 
                }
                else 
                { 
                    return result; 
                }
            }
            return null;
        }
        private void ScrollToEnd()
        {
            var scrollViewer = GetScrollViewer(lst_msg);
            scrollViewer?.Dispatcher.Invoke(() =>
                {
                    scrollViewer.ScrollToEnd();
                });
        }

        public void SyncChatSession(ChatCompletionRequest session, bool enableMarkdown)
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
                        chatMsg.AddText($"调用函数：{toolcall.Function.Name}", false);
                    }
                }
                if (msg is ToolMessage toolMsg)
                {
                    foreach (var img in toolMsg.GeneratedImages)
                    {
                        chatMsg.AddImage(img.ImageBase64Url, img.Description);
                    }
                }

                Messages.Add(chatMsg);
                ScrollToEnd();
            }
        }
        public void AddStreamText(string text)
        {
            Messages.Last().AddStreamText(text);
            ScrollToEnd();
        }
        public void AddMessage(RoleType role)
        {
            Messages.Add(new ChatWindowMessage { Role = role });
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
            await State.UserSendText(txt_input.Text, from fileinfo in FileAttachments select fileinfo.Path);
            txt_input.Text = "";
            FileAttachments.Clear();
        }

        private void btn_reset_Click(object sender, RoutedEventArgs e)
        {
            State.ClearSession();
        }

        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            State.SaveSession();
        }

        private void btn_load_Click(object sender, RoutedEventArgs e)
        {
            State.LoadSession();
        }

        private void txt_input_GotFocus(object sender, RoutedEventArgs e)
        {
            if (firstInput)
            {
                txt_input.Text = "";
                firstInput = false;
            }
        }

        private void btn_addfile_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "Any Files|*.*",
                ClientGuid = new Guid("B8F42507-693B-4713-8671-A76F02ED5ADB"),
            };
            if (dlg.ShowDialog() == true)
            {
                FileAttachments.Add(new(dlg.FileName));
            }
        }

        private void btn_removefile_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var fileAttachmentInfo = (FileAttachmentInfo)((ContentControl)sender).DataContext;
            FileAttachments.Remove(fileAttachmentInfo);
        }

        private void ClosePromptPopup(object sender, MouseButtonEventArgs e)
        {
            tggl_prompt.IsChecked = false;
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
}
