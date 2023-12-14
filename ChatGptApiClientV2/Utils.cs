﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Media.Media3D;
using System.Buffers.Text;
using System.Web;
using System.Net.Http;
using System.Threading;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Markup;
using static ChatGptApiClientV2.EnumHelper;
using System.Windows;
using System.Windows.Documents;

namespace ChatGptApiClientV2
{
    // from https://stackoverflow.com/questions/6145888/how-to-bind-an-enum-to-a-combobox-control-in-wpf
    public static class EnumHelper
    {
        public static string Description(this Enum value)
        {
            var attributes = value?.GetType()?.GetField(value.ToString())?.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes is not null && attributes.Length != 0)
            {
                return ((DescriptionAttribute)attributes.First()).Description;
            }
            // If no description is found, the least we can do is replace underscores with spaces
            // You can add your own custom default formatting logic here
            // TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
            // return ti.ToTitleCase(ti.ToLower(value.ToString().Replace("_", " ")));
            return value?.ToString() ?? "?";
        }
        public class EnumValueDescription(Enum value, string desc)
        {
            public Enum Value { get; set; } = value;
            public string Description { get; set; } = desc;
        }
        public static IEnumerable<EnumValueDescription> GetAllValuesAndDescriptions(Type t)
        {
            if (!t.IsEnum)
            {
                throw new ArgumentException($"{nameof(t)} must be an enum type");
            }
            return Enum.GetValues(t)
                       .Cast<Enum>()
                       .Select((e) => new EnumValueDescription(e, e.Description()))
                       .ToList();
        }
    }
    [ValueConversion(typeof(Enum), typeof(IEnumerable<EnumValueDescription>))]
    public class EnumToCollectionConverter : MarkupExtension, IValueConverter
    {
        private readonly Dictionary<Type, IEnumerable<EnumValueDescription>> cache = [];
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // => GetAllValuesAndDescriptions(value.GetType());
            if(cache.TryGetValue (value.GetType(), out var cached))
            {
                return cached;
            }
            else
            {
                var result = GetAllValuesAndDescriptions(value.GetType());
                cache[value.GetType()] = result;
                return result;
            }
        }
        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
    public class RegexValidationRule : ValidationRule
    {
        private string pattern = "";
        private Regex regex = new("");
        public string ErrorMessage { get; set; } = "";

        public string Pattern
        {
            get => pattern;
            set
            {
                pattern = value;
                regex = new(pattern);
            }
        }

        public override ValidationResult Validate(object value, CultureInfo ultureInfo)
        {
            if (!regex.Match(value.ToString() ?? "").Success)
            {
                return new ValidationResult(false, ErrorMessage);
            }
            else
            {
                return new ValidationResult(true, null);
            }
        }
    }

    public class UrlValidationRule : ValidationRule
    {
        public string ErrorMessage { get; set; } = "";

        public override ValidationResult Validate(object value, CultureInfo ultureInfo)
        {
            var url = value.ToString() ?? "";
            if (Uri.TryCreate(url, UriKind.Absolute, out var uriResult) 
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
            {
                return new ValidationResult(true, null);
            }
            else
            {
                return new ValidationResult(false, ErrorMessage);
            }

        }
    }

    [ValueConversion(typeof(bool), typeof(bool))]
    public class InvertBooleanConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => !(bool)value;
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => !(bool)value;
        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
    [ValueConversion(typeof(bool), typeof(System.Windows.Visibility))]
    public class BoolToVisibilityConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (bool)value ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => (System.Windows.Visibility)value == System.Windows.Visibility.Visible;

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
    partial class Utils
    {
        [Flags]
        public enum ConsoleInputModes : uint
        {
            ENABLE_PROCESSED_INPUT = 0x0001,
            ENABLE_LINE_INPUT = 0x0002,
            ENABLE_ECHO_INPUT = 0x0004,
            ENABLE_WINDOW_INPUT = 0x0008,
            ENABLE_MOUSE_INPUT = 0x0010,
            ENABLE_INSERT_MODE = 0x0020,
            ENABLE_QUICK_EDIT_MODE = 0x0040,
            ENABLE_EXTENDED_FLAGS = 0x0080,
            ENABLE_AUTO_POSITION = 0x0100,
            ENABLE_VIRTUAL_TERMINAL_INPUT = 0x0200
        }

        [Flags]
        public enum ConsoleOutputModes : uint
        {
            ENABLE_PROCESSED_OUTPUT = 0x0001,
            ENABLE_WRAP_AT_EOL_OUTPUT = 0x0002,
            ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004,
            DISABLE_NEWLINE_AUTO_RETURN = 0x0008,
            ENABLE_LVB_GRID_WORLDWIDE = 0x0010
        }

        public enum StdHandle : int
        {
            STD_INPUT_HANDLE = -10,
            STD_OUTPUT_HANDLE = -11,
            STD_ERROR_HANDLE = -12
        }

        public const nint INVALID_HANDLE_VALUE = -1;

        [LibraryImport("kernel32.dll", SetLastError = true)]
        public static partial IntPtr GetStdHandle(int nStdHandle);

        [LibraryImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [LibraryImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        public static string ImageFileToBase64(string filename)
        {
            var mime = MimeTypes.GetMimeType(filename);
            if (!mime.StartsWith("image/"))
            {
                throw new ArgumentException("The file is not an image.");
            }
            var base64 = Convert.ToBase64String(File.ReadAllBytes(filename));
            return $"data:{mime};base64,{base64}";
        }

        [GeneratedRegex("^data:(?<mime>[a-z]+\\/[a-z]+);base64,(?<data>.+)$")]
        private static partial Regex Base64UrlExtract();
        public static string AssertIsBase64Url(string s)
        {
            var r = Base64UrlExtract();
            var match = r.Match(s);
            if (!match.Success)
            {
                throw new ArgumentException("The string is not a base64 image.");
            }
            return s;
        }
        public static string ExtractBase64FromUrl(string url)
        {
            var r = Base64UrlExtract();
            var match = r.Match(url);
            if (!match.Success)
            {
                throw new ArgumentException("The string is not a base64 image.");
            }
            var data = match.Groups["data"].Value;
            return data;
        }
        public static BitmapImage Base64ToBitmapImage(string base64)
        {
            var data = ExtractBase64FromUrl(base64);
            var bytes = Convert.FromBase64String(data);
            using var ms = new MemoryStream(bytes);
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = ms;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            bitmapImage.Freeze();
            return bitmapImage;
        }

        public static Bitmap Base64ToBitmap(string base64)
        {
            var data = ExtractBase64FromUrl(base64);
            var bytes = Convert.FromBase64String(data);
            using var ms = new MemoryStream(bytes);
            var bitmap = new Bitmap(ms);
            return bitmap;
        }

        public static void ConsoleEnableVTSeq()
        {
            var handle = GetStdHandle((int)StdHandle.STD_OUTPUT_HANDLE);
            if (handle == INVALID_HANDLE_VALUE)
            {
                var error = Marshal.GetLastPInvokeErrorMessage();
                throw new Exception($"GetStdHandle failed: {error}");
            }
            if (!GetConsoleMode(handle, out uint mode))
            {
                var error = Marshal.GetLastPInvokeErrorMessage();
                throw new Exception($"GetConsoleMode failed: {error}");
            }
            var new_mode = (ConsoleOutputModes)mode | ConsoleOutputModes.ENABLE_VIRTUAL_TERMINAL_PROCESSING;
            if (!SetConsoleMode(handle, (uint)new_mode))
            {
                var error = Marshal.GetLastPInvokeErrorMessage();
                throw new Exception($"SetConsoleMode failed: {error}");
            }
        }
        public static void ConsoleDisableWrap()
        {
            var handle = GetStdHandle((int)StdHandle.STD_OUTPUT_HANDLE);
            if (handle == INVALID_HANDLE_VALUE)
            {
                var error = Marshal.GetLastPInvokeErrorMessage();
                throw new Exception($"GetStdHandle failed: {error}");
            }
            if (!GetConsoleMode(handle, out uint mode))
            {
                var error = Marshal.GetLastPInvokeErrorMessage();
                throw new Exception($"GetConsoleMode failed: {error}");
            }
            var new_mode = (ConsoleOutputModes)mode & ~ConsoleOutputModes.ENABLE_WRAP_AT_EOL_OUTPUT;
            if (!SetConsoleMode(handle, (uint)new_mode))
            {
                var error = Marshal.GetLastPInvokeErrorMessage();
                throw new Exception($"SetConsoleMode failed: {error}");
            }
        }
        public static void ConsoleEnableWrap()
        {
            var handle = GetStdHandle((int)StdHandle.STD_OUTPUT_HANDLE);
            if (handle == INVALID_HANDLE_VALUE)
            {
                var error = Marshal.GetLastPInvokeErrorMessage();
                throw new Exception($"GetStdHandle failed: {error}");
            }
            if (!GetConsoleMode(handle, out uint mode))
            {
                var error = Marshal.GetLastPInvokeErrorMessage();
                throw new Exception($"GetConsoleMode failed: {error}");
            }
            var new_mode = (ConsoleOutputModes)mode | ConsoleOutputModes.ENABLE_WRAP_AT_EOL_OUTPUT;
            if (!SetConsoleMode(handle, (uint)new_mode))
            {
                var error = Marshal.GetLastPInvokeErrorMessage();
                throw new Exception($"SetConsoleMode failed: {error}");
            }
        }
        public static Bitmap ResizeImage(Bitmap imgToResize, System.Drawing.Size size, PixelFormat format)
        {
            try
            {
                Bitmap b = new(size.Width, size.Height, format);
                using (Graphics g = Graphics.FromImage(b))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(imgToResize, 0, 0, size.Width, size.Height);
                }
                return b;
            }
            catch
            {
                Console.WriteLine("Bitmap could not be resized");
                return imgToResize;
            }
        }
        public static string ConvertImageToConsoleSeq(Bitmap img)
        {
            float float_width = 240;
            float float_height = img.Height * (float_width / img.Width);
            // make sure width is multiple of 4 and height is multiple of 8
            int width = (int)Math.Ceiling(float_width / 4) * 4;
            int height = (int)Math.Ceiling(float_height / 8) * 8;

            var resized = ResizeImage(img, new System.Drawing.Size(width, height), PixelFormat.Format24bppRgb);
            string converted = Derasterize.ConvertImageToString(resized);
            return converted;
        }
        public static void ConsolePrintImage(string img)
        {
            ConsoleEnableVTSeq();
            ConsoleDisableWrap();
            Console.Write(img);
            ConsoleEnableWrap();
        }
        public static string GetFileExtensionFromUrl(string url)
        {
            url = url.Split('?')[0];
            url = url.Split('/').Last();
            return url.Contains('.') ? url[url.LastIndexOf('.')..] : "";
        }
    }
}
