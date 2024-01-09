using HandyControl.Themes;
using HandyControl.Tools;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace ChatGptApiClientV2
{
    public enum ThemeType
    {
        [Description("跟随系统")]
        System,
        [Description("浅色")]
        Light,
        [Description("深色")]
        Dark
    }
    public static class ThemeUpdater
    {
        private static void ForAllWindows(Action<Window> action)
        {
            foreach (Window window in Application.Current.Windows)
            {
                action(window);
            }
        }
        private static void ForAllHandyControlWindows(Action<HandyControl.Controls.Window> action)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is HandyControl.Controls.Window hcWindow)
                {
                    action(hcWindow);
                }
            }
        }
        private static void TryEnableMica(HandyControl.Controls.Window window)
        {
            const BackdropType backdrop = BackdropType.Mica;
            if (!backdrop.IsSupported()) { return; }
            window.SystemBackdropType = backdrop;
        }
        private static void SetupDarkTheme()
        {
            Application.Current.Resources["RegionBrush"] = new SolidColorBrush(Color.FromArgb(0x0f, 0xff, 0xff, 0xff));
            Application.Current.Resources["SecondaryRegionBrush"] = new SolidColorBrush(Color.FromArgb(0x1f, 0xff, 0xff, 0xff));
            Application.Current.Resources["SolidRegionBrush"] = new SolidColorBrush(Color.FromRgb(0x20, 0x20, 0x20));
            Application.Current.Resources["SolidSecondaryRegionBrush"] = new SolidColorBrush(Color.FromRgb(0x34, 0x34, 0x34));
            ForAllHandyControlWindows(TryEnableMica);
            ForAllWindows(MicaHelper.ApplyDarkMode);
        }
        private static void SetupLightTheme()
        {
            Application.Current.Resources["RegionBrush"] = new SolidColorBrush(Color.FromArgb(0xb3, 0xff, 0xff, 0xff));
            Application.Current.Resources["SecondaryRegionBrush"] = new SolidColorBrush(Color.FromArgb(0x80, 0xf9, 0xf9, 0xf9));
            Application.Current.Resources["SolidRegionBrush"] = new SolidColorBrush(Color.FromRgb(0xfa, 0xfa, 0xfa));
            Application.Current.Resources["SolidSecondaryRegionBrush"] = new SolidColorBrush(Color.FromRgb(0xf0, 0xf0, 0xf0));
            ForAllHandyControlWindows(TryEnableMica);
            ForAllWindows(MicaHelper.RemoveDarkMode);
        }
        public delegate void ThemeChangedEventHandler();
        public static event ThemeChangedEventHandler? ThemeChanged;
        public static void UpdateTheme(ThemeType theme, Brush? accentColor)
        {
            ThemeManager.Current.AccentColor = accentColor ?? ThemeManager.Current.GetAccentColorFromSystem();
            switch (theme)
            {
                case ThemeType.System:
                    ThemeManager.Current.ApplicationTheme = ThemeManager.GetSystemTheme(false);
                    if (ThemeManager.Current.ApplicationTheme == ApplicationTheme.Dark)
                    {
                        SetupDarkTheme();
                    }
                    else
                    {
                        SetupLightTheme();
                    }
                    break;
                case ThemeType.Light:
                    ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
                    SetupLightTheme();
                    break;
                case ThemeType.Dark:
                    ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
                    SetupDarkTheme();
                    break;
                default:
                    throw new InvalidOperationException();
            }
            ThemeChanged?.Invoke();
        }
    }
}
