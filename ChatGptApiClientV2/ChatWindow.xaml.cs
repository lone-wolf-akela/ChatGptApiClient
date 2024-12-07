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
// ReSharper disable once UnusedType.Global
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

    private static T? FindChild<T>(DependencyObject? parent, string childName) where T : DependencyObject
    {
        if (parent == null)
        {
            return null;
        }

        T? foundChild = null;
        var childrenCount = VisualTreeHelper.GetChildrenCount(parent);

        for (var i = 0; i < childrenCount; i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T childType and FrameworkElement frameworkElement && frameworkElement.Name == childName)
            {
                foundChild = childType;
                break;
            }

            foundChild = FindChild<T>(child, childName);
            if (foundChild != null)
            {
                break;
            }
        }

        return foundChild;
    }

    private static T? FindParent<T>(DependencyObject child) where T : DependencyObject
    {
        while (true)
        {
            var parentObject = VisualTreeHelper.GetParent(child);
            switch (parentObject)
            {
                case null:
                    return null;
                case T parent:
                    return parent;
                default:
                    child = parentObject;
                    break;
            }
        }
    }

    private void ScrollToEnd(Guid tabId)
    {
        var selectedTab = ((ChatWindowViewModel)DataContext).SelectedMessageTab;
        if (selectedTab is null || tabId != selectedTab.TabId)
        {
            return;
        }

        var lstBox = FindChild<ListBox>(TabMsg, "LstMsg");
        if (lstBox is null)
        {
            return;
        }

        var scrollViewer = GetScrollViewer(lstBox);
        scrollViewer?.UpdateLayout();
        scrollViewer?.ScrollToEnd();
    }

    private void ScrollToHome(Guid tabId)
    {
        var selectedTab = ((ChatWindowViewModel)DataContext).SelectedMessageTab;
        if (selectedTab is null || tabId != selectedTab.TabId)
        {
            return;
        }

        var lstBox = FindChild<ListBox>(TabMsg, "LstMsg");
        if (lstBox is null)
        {
            return;
        }

        var scrollViewer = GetScrollViewer(lstBox);
        scrollViewer?.UpdateLayout();
        scrollViewer?.ScrollToHome();
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
        var lstBox = FindChild<ListBox>(TabMsg, "LstMsg");
        if (lstBox is null)
        {
            return;
        }

        var scrollViewer = GetScrollViewer(lstBox);
        if (scrollViewer is null)
        {
            return;
        }

        var dpi = VisualTreeHelper.GetDpi(this);

        var dlg = new SaveFileDialog
        {
            FileName = "screenshot",
            DefaultExt = ".png",
            Filter = "PNG images|*.png",
            ClientGuid = new Guid(SaveScreenshotDialogGuid)
        };
        if (dlg.ShowDialog() != true)
        {
            return;
        }

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

        CommandManager.AddPreviewExecutedHandler(TxtInput, TxtInput_onPreviewExecuted);
        CommandManager.AddPreviewCanExecuteHandler(TxtInput, TxtInput_onPreviewCanExecute);
    }

    private void Window_Unloaded(object sender, RoutedEventArgs e)
    {
        CommandManager.RemovePreviewExecutedHandler(TxtInput, TxtInput_onPreviewExecuted);
        CommandManager.RemovePreviewCanExecuteHandler(TxtInput, TxtInput_onPreviewCanExecute);
    }

    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        MainGird.Focus();
    }

    private void TabTitleEditorTextBox_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is true)
        {
            var textBox = (TextBox)sender;
            textBox.Focus();
            textBox.SelectAll();
        }
    }

    private void Window_DragEnter(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            PanelDropFiles.Visibility = Visibility.Visible;
            e.Handled = true;
        }

        e.Effects = DragDropEffects.None;
    }

    private static bool IsMouseOverElement(UIElement element)
    {
        var mousePos = Mouse.GetPosition(element);
        return mousePos.X >= 0 && mousePos.X <= element.RenderSize.Width &&
               mousePos.Y >= 0 && mousePos.Y <= element.RenderSize.Height;
    }

    private void Window_DragLeave(object sender, DragEventArgs e)
    {
        if (!IsMouseOverElement(PanelDropFiles))
        {
            PanelDropFiles.Visibility = Visibility.Collapsed;
        }
    }

    private void Window_DragOver(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            PanelDropFiles.Visibility = Visibility.Visible;
            e.Handled = true;
        }

        e.Effects = DragDropEffects.None;
    }

    private void PanelDropFiles_DragOver(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
        }
    }

    private async void PanelDropFiles_Drop(object sender, DragEventArgs e)
    {
        if (!e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            return;
        }

        PanelDropFiles.Visibility = Visibility.Collapsed;
        e.Handled = true;

        if (e.Data.GetData(DataFormats.FileDrop) is not string[] files)
        {
            return;
        }

        foreach (var file in files)
        {
            await ((ChatWindowViewModel)DataContext).AddSingleFileAttachment(file);
        }
    }

    private void TxtInput_PreviewDragEnter(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            PanelDropFiles.Visibility = Visibility.Visible;
            e.Handled = true;
        }
    }

    private void TxtInput_PreviewDragOver(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            PanelDropFiles.Visibility = Visibility.Visible;
            e.Handled = true;
        }
    }

    // from https://stackoverflow.com/questions/13736626/allowing-pasting-data-into-a-wpf-textbox
    private void TxtInput_onPreviewCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        if (Equals(e.Command, ApplicationCommands.Paste))
        {
            e.CanExecute = true;
            e.Handled = true;
        }
    }

    private async void TxtInput_onPreviewExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        if (!Equals(e.Command, ApplicationCommands.Paste))
        {
            return;
        }

        if (Clipboard.ContainsImage())
        {
            var image = Clipboard.GetImage();
            if (image is null)
            {
                return;
            }
            var tmpName = Path.GetTempFileName();
            await using (var fs = File.Create(tmpName))
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(fs);
            }

            var imageName = Path.ChangeExtension(tmpName, "png");
            File.Move(tmpName, imageName);
            Utils.TempFileCleaner.AddFile(imageName);

            await ((ChatWindowViewModel)DataContext).AddSingleFileAttachment(imageName);
            e.Handled = true;
        }
        else if (Clipboard.ContainsFileDropList())
        {
            var files = Clipboard.GetFileDropList();
            foreach (var file in files)
            {
                if (file is not null)
                {
                    await ((ChatWindowViewModel)DataContext).AddSingleFileAttachment(file);
                }
            }
            e.Handled = true;
        }
    }

    private bool _tabMsgTabItemIsDragging;
    private Point _tabMsgTabItemDragInitialMousePosition;
    private TabItem? _tabMsgTabItemDragSource;
    private TranslateTransform? _tabMsgTabItemTranslateTransform;
    private void TabMsg_TabItem_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.Source is not TabItem tabItem)
        {
            return;
        }

        if (!_tabMsgTabItemIsDragging && Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed)
        {
            _tabMsgTabItemDragInitialMousePosition = e.GetPosition(null);
            _tabMsgTabItemDragSource = tabItem;
            _tabMsgTabItemIsDragging = true;
            tabItem.CaptureMouse();
        }

        if (!_tabMsgTabItemIsDragging)
        {
            return;
        }
        if (Mouse.PrimaryDevice.LeftButton != MouseButtonState.Pressed)
        {
            // stop following mouse
            _tabMsgTabItemDragSource!.ReleaseMouseCapture();
            if (_tabMsgTabItemDragSource.RenderTransform is TransformGroup tGroup)
            {
                tGroup.Children.Remove(_tabMsgTabItemTranslateTransform);
                _tabMsgTabItemTranslateTransform = null;
            }

            _tabMsgTabItemIsDragging = false;

            // find drag target
            var tabControl = FindParent<TabControl>(_tabMsgTabItemDragSource);
            if (tabControl is null)
            {
                return;
            }
            for (var targetTabItemIndex = 0; targetTabItemIndex < tabControl.Items.Count; targetTabItemIndex++)
            {
                if (tabControl.ItemContainerGenerator.ContainerFromIndex(targetTabItemIndex) is not TabItem targetTabItem
                    || targetTabItem == _tabMsgTabItemDragSource)
                {
                    continue;
                }
                if (!IsMouseOverElement(targetTabItem))
                {
                    continue;
                }
                var viewModel = (ChatWindowViewModel)DataContext;
                var movedTab = (ChatWindowMessageTab)_tabMsgTabItemDragSource.DataContext;
                viewModel.MoveTabToIndex(movedTab.TabId, targetTabItemIndex);
                viewModel.SelectTab(movedTab.TabId);
                return;
            }
        }
        else
        {
            var transformIsNotRegistered = _tabMsgTabItemTranslateTransform is null
                                           || _tabMsgTabItemDragSource!.RenderTransform is not TransformGroup;

            _tabMsgTabItemTranslateTransform ??= new TranslateTransform();
            var currentMousePosition = e.GetPosition(null);
            _tabMsgTabItemTranslateTransform.X = currentMousePosition.X - _tabMsgTabItemDragInitialMousePosition.X;
            _tabMsgTabItemTranslateTransform.Y = currentMousePosition.Y - _tabMsgTabItemDragInitialMousePosition.Y;

            if (!transformIsNotRegistered)
            {
                return;
            }
            if (_tabMsgTabItemDragSource!.RenderTransform is null)
            {
                _tabMsgTabItemDragSource.RenderTransform = new TransformGroup
                {
                    Children = { _tabMsgTabItemTranslateTransform }
                };
            }
            else if (_tabMsgTabItemDragSource.RenderTransform is not TransformGroup tGroup)
            {
                _tabMsgTabItemDragSource.RenderTransform = new TransformGroup
                {
                    Children = { _tabMsgTabItemDragSource.RenderTransform, _tabMsgTabItemTranslateTransform }
                };
            }
            else
            {
                tGroup.Children.Add(_tabMsgTabItemTranslateTransform);
            }
        }
    }

    private void Button_ScrollToHome_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
        {
            return;
        }
        if (button.DataContext is not ChatWindowMessageTab tab)
        {
            return;
        }
        ScrollToHome(tab.TabId);
    }

    private void Button_ScrollToEnd_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
        {
            return;
        }
        if (button.DataContext is not ChatWindowMessageTab tab)
        {
            return;
        }
        ScrollToEnd(tab.TabId);
    }
}