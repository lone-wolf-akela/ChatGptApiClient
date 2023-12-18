using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Markup;
using static ChatGptApiClientV2.EnumHelper;
using System.Windows;
using System.Windows.Documents;
using System.Diagnostics;
using System.Xml;

namespace ChatGptApiClientV2;

// from https://stackoverflow.com/questions/6145888/how-to-bind-an-enum-to-a-combobox-control-in-wpf
public class EnumValueDescription(Enum value, string desc)
{
    public Enum Value { get; set; } = value;
    public string Description { get; set; } = desc;
}
public static class EnumHelper
{
    public static string Description(this Enum value)
    {
        var attributes = value.GetType().GetField(value.ToString())?.GetCustomAttributes(typeof(DescriptionAttribute), false);
        if (attributes is not null && attributes.Length != 0)
        {
            return ((DescriptionAttribute)attributes.First()).Description;
        }
        // If no description is found, the least we can do is replace underscores with spaces
        // You can add your own custom default formatting logic here
        // TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
        // return ti.ToTitleCase(ti.ToLower(value.ToString().Replace("_", " ")));
        return value.ToString();
    }
    public static IEnumerable<EnumValueDescription> GetAllValuesAndDescriptions(Type t)
    {
        if (!t.IsEnum)
        {
            throw new ArgumentException($"{nameof(t)} must be an enum type");
        }
        return Enum.GetValues(t)
            .Cast<Enum>()
            .Select(e => new EnumValueDescription(e, e.Description()))
            .ToList();
    }
}
[ValueConversion(typeof(Enum), typeof(IEnumerable<EnumValueDescription>))]
public class EnumToCollectionConverter : MarkupExtension, IValueConverter
{
    private readonly Dictionary<Type, IEnumerable<EnumValueDescription>> cache = [];
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // => GetAllValuesAndDescriptions(value.GetType());
        if (value is null)
        {
            return new List();
        }
        if(cache.TryGetValue (value.GetType(), out var cached))
        {
            return cached;
        }
        var result = GetAllValuesAndDescriptions(value.GetType());
        cache[value.GetType()] = result;
        return result;
    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => null;
    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}
public partial class RegexValidationRule : ValidationRule
{
    [GeneratedRegex("")]
    private static partial Regex EmptyRegex();
    
    private string pattern = "";
    private Regex regex = EmptyRegex();
    public string ErrorMessage { get; set; } = "";

    public string Pattern
    {
        get => pattern;
        set
        {
            pattern = value;
            regex = new Regex(pattern);
        }
    }

    public override ValidationResult Validate(object? value, CultureInfo ultureInfo)
    {
        return regex.Match(value?.ToString() ?? "").Success 
            ? new ValidationResult(true, null) 
            : new ValidationResult(false, ErrorMessage);
    }
}

public class UrlValidationRule : ValidationRule
{
    public string ErrorMessage { get; set; } = "";

    public override ValidationResult Validate(object? value, CultureInfo ultureInfo)
    {
        var url = value?.ToString() ?? "";
        if (Uri.TryCreate(url, UriKind.Absolute, out var uriResult) 
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
        {
            return new ValidationResult(true, null);
        }
        return new ValidationResult(false, ErrorMessage);

    }
}

internal static partial class Utils
{
    public static UIElement CloneUiElement(UIElement old)
    {
        var str = XamlWriter.Save(old);
        var strReader = new StringReader(str);
        var xmlReader = XmlReader.Create(strReader);
        var clonedElement = (UIElement)XamlReader.Load(xmlReader);
        return clonedElement;
    }
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
            using var g = Graphics.FromImage(b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(imgToResize, 0, 0, size.Width, size.Height);
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

    public static T RandomSelectByStringHash<T>(string hashsrc, IList<T> list)
    {
        var bytes = Encoding.UTF8.GetBytes(hashsrc);
        var hash = System.Security.Cryptography.MD5.HashData(bytes);
        var value = Math.Abs(BitConverter.ToInt32(hash, 0));

        var index = value % list.Count;
        return list[index];
    }

    public static Floater CreateStickerFloater(IList<string> stickers, string hashsrc)
    {
        var selectedSticker = RandomSelectByStringHash(hashsrc, stickers);

        var uri = new Uri($"pack://application:,,,/images/{selectedSticker}");
        var bitmap = new BitmapImage(uri);
        var image = new System.Windows.Controls.Image
        {
            Source = bitmap
        };
        var floater = new Floater(new BlockUIContainer(image))
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            Width = 64,
            Padding = new Thickness(0),
            Margin = new Thickness(0, 0, 10, 10)
        };

        return floater;
    }
}