using System.Windows.Media.Imaging;

namespace ChatGptApiClientV2;

/// <summary>
/// ImageViewer.xaml 的交互逻辑
/// </summary>
public partial class ImageViewer
{
    public ImageViewer(BitmapSource img)
    {
        InitializeComponent();
        Imgbox.ImageSource = BitmapFrame.Create(img);
    }
}