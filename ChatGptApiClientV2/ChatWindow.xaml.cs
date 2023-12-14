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
    public class ChatWindowMessage
    {
        public static bool EnableMarkdown { get; set; } = true;
        static ChatWindowMessage()
        {

        }
        public enum RoleType
        {
            User,
            Assistant,
            System,
            Tool
        }
        public string? Message { get; set; }
        public FlowDocument RenderedMessage
        {
            get
            {
                FlowDocument doc;
                if (EnableMarkdown)
                {
                    doc = Markdig.Wpf.Markdown.ToFlowDocument(Message ?? "", 
                        new MarkdownPipelineBuilder()
                        .UseAdvancedExtensions()
                        .UseColorCodeWpf()
                        .UseTaskLists()
                        .UseGridTables()
                        .UsePipeTables()
                        .UseEmphasisExtras()
                        .Build());
                }
                else
                {
                    doc = new FlowDocument();
                    doc.Blocks.Add(new Paragraph(new Run(Message)));
                }
                doc.Foreground = ForegroundColor;
                return doc;
            }
        }
        public RoleType Role { get; set; }
        public ImageSource Avatar => Role switch
        {
            RoleType.User => new BitmapImage(new Uri("pack://application:,,,/images/chatgpt-icon.png")),
            RoleType.System => new BitmapImage(new Uri("pack://application:,,,/images/chatgpt-icon.png")),
            RoleType.Assistant => new BitmapImage(new Uri("pack://application:,,,/images/chatgpt-icon.png")),
            RoleType.Tool => new BitmapImage(new Uri("pack://application:,,,/images/chatgpt-icon.png")),
            _ => throw new NotImplementedException()
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
    public partial class ChatWindow : Window
    {
        public ObservableCollection<ChatWindowMessage> Messages { get; set; } = [];
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
                while (parent != null && !(parent is ScrollViewer))
                    parent = (UIElement)VisualTreeHelper.GetParent(parent);

                // 确认找到ListView控件并触发事件
                parent?.RaiseEvent(eventArg);
            }
        }
        public ChatWindow()
        {
            InitializeComponent();

            Messages.Add(new ChatWindowMessage
            {
                Message = @"# Header 1

## Header 2 你好

这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字

这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字

### Header 3

AAAA BBBBB 你好

#### Header 4

CCCC

*Italic*

**Bold**

***Bold and Italic***

~~Strikethrough~~

> Blockquote

` Inline code `

```cpp
public class Test
{
    public string TestString { get; set; }
}
```

[Link](https://www.google.com)

- List item 1
    - List item 1.1
    - List item 1.2
- List item 2
- List item 3

1. Numbered list item 1
2. Numbered list item 2
3. Numbered list item 3

",
                Role = ChatWindowMessage.RoleType.System
            });
            Messages.Add(new ChatWindowMessage
            {
                Message = @"# Header 1

## Header 2 你好

这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字

这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字

### Header 3

AAAA BBBBB 你好

#### Header 4

CCCC

*Italic*

**Bold**

***Bold and Italic***

~~Strikethrough~~

> Blockquote

` Inline code `

```cpp
public class Test
{
    public string TestString { get; set; }
}
```

[Link](https://www.google.com)

- List item 1
    - List item 1.1
    - List item 1.2
- List item 2
- List item 3

1. Numbered list item 1
2. Numbered list item 2
3. Numbered list item 3

",
                Role = ChatWindowMessage.RoleType.User
            });
            Messages.Add(new ChatWindowMessage
            {
                Message = @"# Header 1

## Header 2 你好

这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字

这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字

### Header 3

AAAA BBBBB 你好

#### Header 4

CCCC

*Italic*

**Bold**

***Bold and Italic***

~~Strikethrough~~

> Blockquote

` Inline code `

```cpp
public class Test
{
    public string TestString { get; set; }
}
```

[Link](https://www.google.com)

- List item 1
    - List item 1.1
    - List item 1.2
- List item 2
- List item 3

1. Numbered list item 1
2. Numbered list item 2
3. Numbered list item 3

",
                Role = ChatWindowMessage.RoleType.Assistant
            });
            Messages.Add(new ChatWindowMessage
            {
                Message = @"# Header 1

## Header 2 你好

这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字

这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字;这是一段测试文字

### Header 3

AAAA BBBBB 你好

#### Header 4

CCCC

*Italic*

**Bold**

***Bold and Italic***

~~Strikethrough~~

> Blockquote

` Inline code `

```cpp
public class Test
{
    public string TestString { get; set; }
}
```

[Link](https://www.google.com)

- List item 1
    - List item 1.1
    - List item 1.2
- List item 2
- List item 3

1. Numbered list item 1
2. Numbered list item 2
3. Numbered list item 3

",
                Role = ChatWindowMessage.RoleType.Tool
            });
            Messages.Add(new ChatWindowMessage
            {
                Message = "Hello, world! 2",
                Role = ChatWindowMessage.RoleType.User
            });

            Messages.Add(new ChatWindowMessage
            {
                Message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
                Role = ChatWindowMessage.RoleType.Assistant
            });

            Messages.Add(new ChatWindowMessage
            {
                Message = "Hello, world! 4",
                Role = ChatWindowMessage.RoleType.Tool
            });

            Messages.Add(new ChatWindowMessage
            {
                Message = "# Lorem ipsum dolor sit amet \r\nconsectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
                Role = ChatWindowMessage.RoleType.Assistant
            });

            Messages.Add(new ChatWindowMessage
            {
                Message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
                Role = ChatWindowMessage.RoleType.Assistant
            });

            Messages.Add(new ChatWindowMessage
            {
                Message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
                Role = ChatWindowMessage.RoleType.Assistant
            });

            Messages.Add(new ChatWindowMessage
            {
                Message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
                Role = ChatWindowMessage.RoleType.Assistant
            });

            Messages.Add(new ChatWindowMessage
            {
                Message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
                Role = ChatWindowMessage.RoleType.Assistant
            });
        }
    }
}
