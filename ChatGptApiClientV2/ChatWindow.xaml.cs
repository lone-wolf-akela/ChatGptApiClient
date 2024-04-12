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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using Microsoft.Win32;
using System.IO;

namespace ChatGptApiClientV2;

/// <summary>
/// ChatWindow.xaml 的交互逻辑
/// </summary>

public partial class ChatWindow
{
    private void ScrollToParentProcecssor(object? sender, MouseWheelEventArgs e)
    {
        if (e.Handled || sender is not DependencyObject senderObj)
        {
            return;
        }

        if (e.Source is Controls.TextFileDisplayer textFile)
        {
            // pass scroll to parent only if already at the top/bottom
            var scrollViewer = GetScrollViewer(textFile);
            var canSrcScroll = scrollViewer?.ComputedVerticalScrollBarVisibility == Visibility.Visible;
            var isAtTop = scrollViewer?.VerticalOffset <= 0;
            var isAtBottom = scrollViewer?.VerticalOffset >= scrollViewer?.ScrollableHeight;
            var isScrollingUp = e.Delta > 0;
            var isScrollingDown = e.Delta < 0;
            var passToParent = canSrcScroll && ((isAtTop && isScrollingUp) || (isAtBottom && isScrollingDown));

            if (!passToParent)
            {
                return;
            }
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
        DataContext = new ChatWindowViewModel();
        ((ChatWindowViewModel)DataContext).ScrollToEndEvent += ScrollToEnd;
        InitializeComponent();
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

    private void OpenHyperlink(object sender, ExecutedRoutedEventArgs e)
    {
        var link = e.Parameter.ToString();
        if (link is not null)
        {
            Process.Start(new ProcessStartInfo(link) { UseShellExecute = true });
        }
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

    private const string SaveScreenshotDialogGuid = "49CEC7E0-C84B-4B69-8238-B6EFB608D7DC";
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
        while (true)
        {
            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + pageHeight);
            scrollViewer.UpdateLayout();
            var newScreenShot = RenderScrollViewer(scrollViewer, dpi.DpiScaleX, dpi.DpiScaleY);
            if (newScreenShot.OffsetY == lastOffsetY)
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

    private void ClosePromptPopup(object sender, MouseButtonEventArgs e)
    {
        TgglPrompt.IsChecked = false;
    }

    private void BtnSettings_Click(object sender, RoutedEventArgs e)
    {
        var settingsDialog = new Settings(((ChatWindowViewModel)DataContext).State.Config)
        {
            Owner = this
        };
        settingsDialog.ShowDialog();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        ((ChatWindowViewModel)DataContext).State.Config.RefreshTheme();
    }
}