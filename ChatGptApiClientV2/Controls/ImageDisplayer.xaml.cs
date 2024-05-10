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

using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Documents;

namespace ChatGptApiClientV2.Controls;

/// <summary>
/// ImageDisplayer.xaml 的交互逻辑
/// </summary>
public partial class ImageDisplayer
{
    public static readonly DependencyProperty IsExpandedProperty = Gen.IsExpanded(true);
    public static readonly DependencyProperty FileNameProperty = Gen.FileName("");
    public static readonly DependencyProperty ImageSourceProperty = Gen.ImageSource<BitmapSource>();
    public static readonly DependencyProperty ImageMaxHeightProperty = Gen.ImageMaxHeight(300.0);
    public static readonly DependencyProperty ImageTooltipProperty = Gen.ImageTooltip<string?>(null);
    private static void OnImageTooltipChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (ImageDisplayer)d;
        control.TgglDetails.Visibility =
            !string.IsNullOrEmpty((string)e.NewValue) ? Visibility.Visible : Visibility.Collapsed;
    }

    public ImageDisplayer()
    {
        InitializeComponent();
    }


    private void ImageGrid_MouseEnter(object sender, MouseEventArgs e)
    {
        BtnPanel.Visibility = Visibility.Visible;
    }

    private void ImageGrid_MouseLeave(object sender, MouseEventArgs e)
    {
        BtnPanel.Visibility = Visibility.Collapsed;
    }
    private class TempFileCleanerClass
    {
        private readonly List<string> files = [];
        public void AddFile(string file)
        {
            files.Add(file);
        }
        private void CleanFiles()
        {
            foreach (var file in files)
            {
                try
                {
                    File.Delete(file);
                }
                catch
                {
                    // ignored
                }
            }
        }
        public TempFileCleanerClass()
        {
            Application.Current.Exit += (sender, args) => CleanFiles();
        }
    }
    private static readonly TempFileCleanerClass TempFileCleaner = new();
    private async void BtnOpenImageViewer_Click(object sender, RoutedEventArgs e)
    {
        var tmpName = Path.GetTempFileName();
        await using (var fs = File.Create(tmpName))
        {
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(ImageSource));
            encoder.Save(fs);
        }

        var imageName = Path.ChangeExtension(tmpName, "png");
        File.Move(tmpName, imageName);
        TempFileCleaner.AddFile(imageName);
        Process.Start(new ProcessStartInfo(imageName) { UseShellExecute = true });
    }
}