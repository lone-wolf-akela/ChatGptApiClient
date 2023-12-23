using HandyControl.Themes;
using HandyControl.Tools;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
        private static void SetupDarkTheme()
        {
            Application.Current.Resources["RegionBrush"] = new SolidColorBrush(Color.FromRgb(0x2b, 0x2b, 0x2b));
            Application.Current.Resources["SecondaryRegionBrush"] = new SolidColorBrush(Color.FromRgb(0x34, 0x34, 0x34));
            ForAllWindows(MicaHelper.ApplyDarkMode);
        }
        private static void SetupLightTheme()
        {
            Application.Current.Resources["RegionBrush"] = new SolidColorBrush(Color.FromRgb(0xfb, 0xfb, 0xfb));
            Application.Current.Resources["SecondaryRegionBrush"] = new SolidColorBrush(Color.FromRgb(0xf6, 0xf6, 0xf6));
            ForAllWindows(MicaHelper.RemoveDarkMode);
        }
        public delegate void ThemeChangedEventHandler();
        public static event ThemeChangedEventHandler? ThemeChanged;
        public static void UpdateTheme(ThemeType theme)
        {
            ThemeManager.Current.AccentColor = ThemeManager.Current.GetAccentColorFromSystem();
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
                    throw new NotImplementedException();
            }
            ThemeChanged?.Invoke();
        }
    }
}
