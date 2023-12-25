using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.IO;

namespace ChatGptApiClientV2.Controls;

/// <summary>
/// ImageDisplayer.xaml 的交互逻辑
/// </summary>
public partial class ImageDisplayer : UserControl
{
    public static readonly DependencyProperty ImageProperty = DependencyProperty.Register(
        nameof(Image),
        typeof(BitmapSource),
        typeof(ImageDisplayer),
        new PropertyMetadata(null)
    );

    public static readonly DependencyProperty ImageTooltipProperty = DependencyProperty.Register(
        nameof(ImageTooltip),
        typeof(string),
        typeof(ImageDisplayer),
        new PropertyMetadata(null, OnImageTooltipChanged)
    );

    public static readonly DependencyProperty ImageMaxHeightProperty = DependencyProperty.Register(
        nameof(ImageMaxHeight),
        typeof(double),
        typeof(ImageDisplayer),
        new PropertyMetadata(300.0)
    );

    public BitmapSource Image
    {
        get => (BitmapSource)GetValue(ImageProperty);
        set => SetValue(ImageProperty, value);
    }

    public string ImageTooltip
    {
        get => (string)GetValue(ImageTooltipProperty);
        set => SetValue(ImageTooltipProperty, value);
    }

    public double ImageMaxHeight
    {
        get => (double)GetValue(ImageMaxHeightProperty);
        set => SetValue(ImageMaxHeightProperty, value);
    }

    public ImageDisplayer()
    {
        InitializeComponent();
    }

    private static void OnImageTooltipChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (ImageDisplayer)d;
        control.TgglDetails.Visibility = !string.IsNullOrEmpty((string)e.NewValue) ? Visibility.Visible : Visibility.Collapsed;
    }

    private void ImageGrid_MouseEnter(object sender, MouseEventArgs e)
    {
        btnPanel.Visibility = Visibility.Visible;
    }

    private void ImageGrid_MouseLeave(object sender, MouseEventArgs e)
    {
        btnPanel.Visibility = Visibility.Collapsed;
    }

    private async void BtnOpenImageViewer_Click(object sender, RoutedEventArgs e)
    {
        var tmpName = Path.GetTempFileName();
        await using (var fs = File.Create(tmpName))
        {
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(Image));
            encoder.Save(fs);
        }
        var imageName = Path.ChangeExtension(tmpName, "png");
        File.Move(tmpName, imageName);
        Process.Start(new ProcessStartInfo(imageName) { UseShellExecute = true });
    }
}
