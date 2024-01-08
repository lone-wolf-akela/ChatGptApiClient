using System;
using HandyControl.Controls;
using HandyControl.Tools;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Forms;
using System.Windows.Interop;

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

    private void Window_Unloaded(object sender, RoutedEventArgs e)
    {
        ((SettingsViewModel)DataContext).UnLoadViewModel();
    }

    private void BtnShowColorPicker_Click(object sender, RoutedEventArgs _)
    {
        var picker = SingleOpenHelper.CreateControl<ColorPicker>();
        picker.SetResourceReference(BackgroundProperty, "SolidRegionBrush");
        picker.SelectedBrush = ((SettingsViewModel)DataContext).Config.CustomThemeColor;
        var window = new PopupWindow
        {
            PopupElement = picker, 
            Owner = this,
        };

        const bool showBackground = false;
        
        window.Loaded += (_, _) =>
        {
            var targetElement = (Visual)sender;
            // from HandyControl.Tools.ArithmeticHelper.CalSafePoint
            var point = targetElement.PointToScreen(new Point(0, 0));
            var dpi = VisualTreeHelper.GetDpi(this);
            
            var currentScreen = Screen.FromHandle(new WindowInteropHelper(this).Handle);

            var workArea = new Rect(
                currentScreen.WorkingArea.Left / dpi.DpiScaleX,
                currentScreen.WorkingArea.Top / dpi.DpiScaleY,
                currentScreen.WorkingArea.Width / dpi.DpiScaleX,
                currentScreen.WorkingArea.Height / dpi.DpiScaleY);

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
            var maxLeft = workArea.Right - (window.PopupElement.ActualWidth + window.BorderThickness.Left + window.BorderThickness.Right);
            var maxTop = workArea.Bottom - (window.PopupElement.ActualHeight + window.BorderThickness.Top + window.BorderThickness.Bottom);
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
}