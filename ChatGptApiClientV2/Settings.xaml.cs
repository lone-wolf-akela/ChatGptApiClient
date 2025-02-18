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
using HandyControl.Controls;
using HandyControl.Tools;
using System.Windows;
using System.Windows.Media;
using System.Windows.Interop;
using System.Windows.Navigation;
using System.Diagnostics;

namespace ChatGptApiClientV2;

/// <summary>
/// Settings.xaml 的交互逻辑
/// </summary>
public partial class Settings
{
    public Settings(Config conf)
    {
        DataContext = new SettingsViewModel(conf);
        InitializeComponent();
    }

    private void BtnShowColorPicker_Click(object sender, RoutedEventArgs _)
    {
        var picker = SingleOpenHelper.CreateControl<ColorPicker>();
        picker.SetResourceReference(BackgroundProperty, "SolidRegionBrush");
        picker.SelectedBrush = ((SettingsViewModel)DataContext).Config.CustomThemeColor;
        var window = new PopupWindow
        {
            PopupElement = picker,
            Owner = this
        };

        const bool showBackground = false;

        window.Loaded += (_, _) =>
        {
            var targetElement = (Visual)sender;
            // from HandyControl.Tools.ArithmeticHelper.CalSafePoint
            var point = targetElement.PointToScreen(new Point(0, 0));
            var dpi = VisualTreeHelper.GetDpi(this);

            var hWnd = new WindowInteropHelper(this).Handle;
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            var displayArea = Microsoft.UI.Windowing.DisplayArea.GetFromWindowId(
                windowId, Microsoft.UI.Windowing.DisplayAreaFallback.Nearest);

            var workArea = new Rect(
                displayArea.WorkArea.X / dpi.DpiScaleX,
                displayArea.WorkArea.Y / dpi.DpiScaleY,
                displayArea.WorkArea.Width / dpi.DpiScaleX,
                displayArea.WorkArea.Height / dpi.DpiScaleY);

            point.X /= dpi.DpiScaleX;
            point.Y /= dpi.DpiScaleY;

            // I'm not entirely sure why this is needed,
            // but we have to use this 2 steps method to move the window to the target position
            // or, sometimes the window does not show when the target position is at the bottom right of my 3rd monitor

            // firstly, move to target monitor
            window.Left = workArea.Left + 10;
            window.Top = workArea.Top + 10;

            window.UpdateLayout();

            // then, move to target position, clamp to working area
            var maxLeft = workArea.Right - (window.PopupElement.ActualWidth + window.BorderThickness.Left +
                                            window.BorderThickness.Right);
            var maxTop = workArea.Bottom - (window.PopupElement.ActualHeight + window.BorderThickness.Top +
                                            window.BorderThickness.Bottom);
            point.X = Math.Clamp(point.X, workArea.Left, maxLeft);
            point.Y = Math.Clamp(point.Y, workArea.Top, maxTop);
            window.Left = point.X;
            window.Top = point.Y;
        };

        picker.Confirmed += (_, _) =>
        {
            ((SettingsViewModel)DataContext).Config.CustomThemeColor = picker.SelectedBrush;
            window.Close();
        };
        picker.Canceled += (_, _) => window.Close();
        window.ShowDialog((FrameworkElement)sender, showBackground);
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        ((SettingsViewModel)DataContext).Config.RefreshTheme();
    }

    private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
        e.Handled = true;
    }
}