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

using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.IO;

namespace ChatGptApiClientV2.Controls;

/// <summary>
/// ImageDisplayer.xaml 的交互逻辑
/// </summary>
public partial class ImageDisplayer
{
    public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(
        nameof(IsExpanded),
        typeof(bool),
        typeof(ImageDisplayer),
        new PropertyMetadata(true)
    );

    public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register(
        nameof(FileName),
        typeof(string),
        typeof(ImageDisplayer),
        new PropertyMetadata("")
    );

    public static readonly DependencyProperty ImageProperty = DependencyProperty.Register(
        nameof(Image),
        typeof(BitmapSource),
        typeof(ImageDisplayer)
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

    public BitmapSource Image
    {
        get => (BitmapSource)GetValue(ImageProperty);
        init => SetValue(ImageProperty, value);
    }

    public string ImageTooltip
    {
        get => (string)GetValue(ImageTooltipProperty);
        init => SetValue(ImageTooltipProperty, value);
    }

    public double ImageMaxHeight
    {
        get => (double)GetValue(ImageMaxHeightProperty);
        init => SetValue(ImageMaxHeightProperty, value);
    }

    public ImageDisplayer()
    {
        InitializeComponent();
    }

    private static void OnImageTooltipChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (ImageDisplayer)d;
        control.TgglDetails.Visibility =
            !string.IsNullOrEmpty((string)e.NewValue) ? Visibility.Visible : Visibility.Collapsed;
    }

    private void ImageGrid_MouseEnter(object sender, MouseEventArgs e)
    {
        BtnPanel.Visibility = Visibility.Visible;
    }

    private void ImageGrid_MouseLeave(object sender, MouseEventArgs e)
    {
        BtnPanel.Visibility = Visibility.Collapsed;
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