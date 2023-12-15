using System;
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
using System.Diagnostics;

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
                Debug.WriteLine("Bitmap could not be resized");
                return imgToResize;
            }
        }
        public static string GetFileExtensionFromUrl(string url)
        {
            url = url.Split('?')[0];
            url = url.Split('/').Last();
            return url.Contains('.') ? url[url.LastIndexOf('.')..] : "";
        }
    }
}
