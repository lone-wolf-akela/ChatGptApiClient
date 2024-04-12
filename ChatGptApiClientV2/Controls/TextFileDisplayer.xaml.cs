using System.Windows;

namespace ChatGptApiClientV2.Controls
{
    /// <summary>
    /// TextFileDisplayer.xaml 的交互逻辑
    /// </summary>
    public partial class TextFileDisplayer
    {
        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(
            nameof(IsExpanded),
            typeof(bool),
            typeof(TextFileDisplayer),
            new PropertyMetadata(false)
        );

        public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register(
            nameof(FileName),
            typeof(string),
            typeof(TextFileDisplayer),
            new PropertyMetadata("")
        );

        public static readonly DependencyProperty TextContentProperty = DependencyProperty.Register(
            nameof(TextContent),
            typeof(string),
            typeof(TextFileDisplayer),
            new PropertyMetadata("")
        );

        public static readonly DependencyProperty TextContentMaxHeightProperty = DependencyProperty.Register(
            nameof(TextContentMaxHeight),
            typeof(double),
            typeof(TextFileDisplayer),
            new PropertyMetadata(300.0)
        );

        public bool IsExpanded
        {
            get => (bool)GetValue(IsExpandedProperty);
            init => SetValue(IsExpandedProperty, value);
        }

        public string FileName
        {
            get => (string)GetValue(FileNameProperty);
            init => SetValue(FileNameProperty, value);
        }

        public string TextContent
        {
            get => (string)GetValue(TextContentProperty);
            init => SetValue(TextContentProperty, value);
        }

        public double TextContentMaxHeight
        {
            get => (double)GetValue(TextContentMaxHeightProperty);
            init => SetValue(TextContentMaxHeightProperty, value);
        }

        public TextFileDisplayer()
        {
            InitializeComponent();
        }
    }
}
