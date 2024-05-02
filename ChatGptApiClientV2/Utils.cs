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
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;
using System.Globalization;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows.Markup;
using System.Windows;
using System.Windows.Documents;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using System.Threading;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.ML.Tokenizers;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace ChatGptApiClientV2;

public static class TabControlHelper
{
    public static readonly DependencyProperty AddTabCommandProperty = DependencyProperty.RegisterAttached(
        "AddTabCommand",
        typeof(ICommand),
        typeof(TabControlHelper),
        new PropertyMetadata(null));

    public static void SetAddTabCommand(UIElement element, ICommand? value)
    {
        element.SetValue(AddTabCommandProperty, value);
    }

    public static ICommand? GetAddTabCommand(UIElement element)
    {
        return element.GetValue(AddTabCommandProperty) as ICommand;
    }

    public static readonly DependencyProperty AddTabBtnTooltip = DependencyProperty.RegisterAttached(
        "AddTabBtnTooltip",
        typeof(object),
        typeof(TabControlHelper),
        new PropertyMetadata(null));

    public static void SetAddTabBtnTooltip(UIElement element, object? value)
    {
        element.SetValue(AddTabBtnTooltip, value);
    }

    public static object? GetAddTabBtnTooltip(UIElement element)
    {
        return element.GetValue(AddTabBtnTooltip);
    }
}

public class BottomCornerRadiusConverter : MarkupExtension, IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            double d => new CornerRadius(0, 0, d, d),
            CornerRadius r => new CornerRadius(0, 0, r.BottomRight, r.BottomLeft),
            _ => new CornerRadius(0)
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => value;
    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}

// from https://stackoverflow.com/questions/5649875/how-to-make-the-border-trim-the-child-elements
public class BorderClipConverter : MarkupExtension, IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values is not [double width, double height, CornerRadius radius])
        {
            return DependencyProperty.UnsetValue;
        }

        if (width < double.Epsilon || height < double.Epsilon)
        {
            return Geometry.Empty;
        }

        var clip = Utils.CreateRoundedRectangleGeometry(width, height, radius, new Thickness(0));
        clip.Freeze();

        return clip;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }

    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}

public class Int2BoolConverter : MarkupExtension, IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int i)
        {
            return i != 0;
        }

        return false;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b)
        {
            return b ? 1 : 0;
        }

        return 0;
    }

    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}

public class Log2DoubleConverter : MarkupExtension, IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double d)
        {
            return Math.Log2(d);
        }

        return 0.0;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double d)
        {
            return Math.Pow(2, d);
        }

        return 0.0;
    }

    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}

// from https://stackoverflow.com/questions/397556/how-to-bind-radiobuttons-to-an-enum
public class ComparisonConverter : MarkupExtension, IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value?.Equals(parameter) ?? false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value?.Equals(true) == true ? parameter : Binding.DoNothing;
    }

    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}

public class LargerToVisibilityConverter : MarkupExtension, IMultiValueConverter
{
    public object Convert(object?[] values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values is [double d1, double d2])
        {
            return d1 > d2 ? Visibility.Visible : Visibility.Collapsed;
        }

        return Visibility.Collapsed;
    }

    public object?[] ConvertBack(object? value, Type[] targetType, object? parameter, CultureInfo culture)
    {
        throw new InvalidOperationException();
    }

    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}

// from https://stackoverflow.com/questions/6145888/how-to-bind-an-enum-to-a-combobox-control-in-wpf
public class EnumValueDescription(Enum value, string desc)
{
    public Enum Value { get; set; } = value;
    public string Description { get; set; } = desc;
}

public static class EnumHelper
{
    private static string Description(this Enum value)
    {
        var attributes = value.GetType().GetField(value.ToString())
            ?.GetCustomAttributes(typeof(DescriptionAttribute), false);
        if (attributes is not null && attributes.Length != 0)
        {
            return ((DescriptionAttribute)attributes.First()).Description;
        }

        // If no description is found
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

        if (cache.TryGetValue(value.GetType(), out var cached))
        {
            return cached;
        }

        var result = EnumHelper.GetAllValuesAndDescriptions(value.GetType());
        cache[value.GetType()] = result;
        return result;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => null;
    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}

public static partial class Utils
{
    public static Geometry CreateRoundedRectangleGeometry(double width, double height, CornerRadius radius,
        Thickness thickness)
    {
        var geometry = new StreamGeometry();

        using var context = geometry.Open();
        context.BeginFigure(new Point(radius.TopLeft + thickness.Left, thickness.Top), true, true);

        // Top side and top-right corner
        context.LineTo(new Point(width - radius.TopRight - thickness.Right, 0), true, false);
        context.ArcTo(new Point(width - thickness.Right, radius.TopRight + thickness.Top),
            new Size(radius.TopRight, radius.TopRight), 0, false, SweepDirection.Clockwise, true, false);

        // Right side and bottom-right corner
        context.LineTo(new Point(width - thickness.Right, height - radius.BottomRight - thickness.Bottom), true, false);
        context.ArcTo(new Point(width - radius.BottomRight - thickness.Right, height - thickness.Bottom),
            new Size(radius.BottomRight, radius.BottomRight), 0, false, SweepDirection.Clockwise, true, false);

        // Bottom side and bottom-left corner
        context.LineTo(new Point(radius.BottomLeft + thickness.Left, height - thickness.Bottom), true, false);
        context.ArcTo(new Point(thickness.Left, height - radius.BottomLeft - thickness.Bottom),
            new Size(radius.BottomLeft, radius.BottomLeft), 0, false, SweepDirection.Clockwise, true, false);

        // Left side and top-left corner
        context.LineTo(new Point(thickness.Left, radius.TopLeft + thickness.Top), true, false);
        context.ArcTo(new Point(radius.TopLeft + thickness.Left, thickness.Top),
            new Size(radius.TopLeft, radius.TopLeft), 0, false, SweepDirection.Clockwise, true, false);

        return geometry;
    }


    private static Tokenizer? gpt4Tokenizer;

    public static int GetStringTokenCount(string str)
    {
        gpt4Tokenizer ??= Tokenizer.CreateTiktokenForModel("gpt-4");
        var tokenCount = gpt4Tokenizer.CountTokens(str);
        return tokenCount;
    }

    public static async Task<string> ImageFileToBase64(string filename, CancellationToken cancellationToken = default)
    {
        var mime = MimeTypes.GetMimeType(filename);
        if (!mime.StartsWith("image/"))
        {
            throw new ArgumentException("The file is not an image.");
        }

        var base64 = Convert.ToBase64String(await File.ReadAllBytesAsync(filename, cancellationToken));
        return $"data:{mime};base64,{base64}";
    }

    [GeneratedRegex(@"^data:(?<mime>[a-z\-\+\.]+\/[a-z\-\+\.]+);base64,(?<data>.+)$")]
    private static partial Regex Base64UrlExtract();

    public static string ExtractBase64FromUrl(string urlOrData)
    {
        var r = Base64UrlExtract();
        var match = r.Match(urlOrData);
        if (!match.Success)
        {
            return urlOrData;
        }

        var data = match.Groups["data"].Value;
        return data;
    }

    public static string ExtractMimeFromUrl(string url)
    {
        var r = Base64UrlExtract();
        var match = r.Match(url);
        if (!match.Success)
        {
            throw new ArgumentException("The input is not a base64 data URL.");
        }

        var mime = match.Groups["mime"].Value;
        return mime;
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

    public static string ConvertToBase64Png(string base64)
    {
        var data = ExtractBase64FromUrl(base64);
        var bytes = Convert.FromBase64String(data);

        using var ms = new MemoryStream(bytes);
        var imageSource = BitmapFrame.Create(ms, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);

        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(imageSource));

        using var msOutput = new MemoryStream();
        encoder.Save(msOutput);
        var outBase64 = Convert.ToBase64String(msOutput.ToArray());

        return $"data:image/png;base64,{outBase64}";
    }

    public static string OptimizeBase64Png(string base64)
    {
        // optimize the input data
        // used for dall-e generated image, as they are almost not compressed
        // see https://community.openai.com/t/dall-e-3-output-image-file-linked-in-response-url-is-uncompressed/522087/5
        return ConvertToBase64Png(base64);
    }

    private static T RandomSelectByStringHash<T>(string hashSrc, IList<T> list)
    {
        var bytes = Encoding.UTF8.GetBytes(hashSrc);
        var hash = System.Security.Cryptography.MD5.HashData(bytes);
        var value = Math.Abs(BitConverter.ToInt32(hash, 0));

        var index = value % list.Count;
        return list[index];
    }

    public static Floater CreateStickerFloater(IList<string> stickers, string hashSrc)
    {
        var selectedSticker = RandomSelectByStringHash(hashSrc, stickers);

        var uri = new Uri($"pack://application:,,,/images/{selectedSticker}");
        var bitmap = new BitmapImage(uri);
        var image = new Image
        {
            Source = bitmap,
            SnapsToDevicePixels = true
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

    // char* poppler_extract_text_from_pdf_file(const char* filename, size_t filename_len = 0);
    // void poppler_free_text(const char* text);
    [LibraryImport("poppler-wrapper.dll", EntryPoint = "poppler_extract_text_from_pdf_file",
        StringMarshalling = StringMarshalling.Utf8)]
    private static partial IntPtr PopplerExtractTextFromPdfFile(string filename, int filenameLen);

    [LibraryImport("poppler-wrapper.dll", EntryPoint = "poppler_free_text")]
    private static partial void PopplerFreeText(IntPtr text);

    public static async Task<string> PdfFileToText(string filename, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() =>
        {
            var ptr = PopplerExtractTextFromPdfFile(filename, 0);
            if (ptr == IntPtr.Zero)
            {
                throw new InvalidOperationException("Failed to extract text from PDF file.");
            }

            var text = Marshal.PtrToStringUTF8(ptr) ?? "";
            PopplerFreeText(ptr);
            return text;
        }, cancellationToken);
    }

    public static async Task<string> RtfFileToText(string filename, CancellationToken _ = default)
    {
        var doc = new FlowDocument();
        await using var fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
        var range = new TextRange(doc.ContentStart, doc.ContentEnd);
        range.Load(fs, DataFormats.Rtf);
        var text = new TextRange(doc.ContentStart, doc.ContentEnd).Text;
        return text;
    }

    // from https://stackoverflow.com/questions/354477/method-to-determine-if-path-string-is-local-or-remote-machine
    public static bool IsPathRemote(string path)
    {
        DirectoryInfo dir = new(path);
        foreach (var d in DriveInfo.GetDrives())
        {
            if (string.Compare(dir.Root.FullName, d.Name, StringComparison.OrdinalIgnoreCase) == 0)
            {
                return d.DriveType == DriveType.Network;
            }
        }

        return true;
    }

    private static IEnumerable<string> FindFolderContains(string fileName, IEnumerable<string> folders)
    {
        return from folder in folders
            where File.Exists(Path.Combine(folder, fileName))
            select folder;
    }

    private static IEnumerable<string> FindPathContains(string fileName)
    {
        var path = Environment.GetEnvironmentVariable("PATH");
        if (path is null)
        {
            yield break;
        }

        var folders = path.Split(';');
        foreach (var folder in FindFolderContains(fileName, folders))
        {
            yield return folder;
        }
    }

    [method: JsonConstructor]
    public class PythonEnv(string homePath, string executablePath, string dllPath)
    {
        public string HomePath { get; } = homePath;
        public string ExecutablePath { get; } = executablePath;

        // ReSharper disable once UnusedMember.Global
        public string DllPath { get; } = dllPath;
    }

    private static PythonEnv? DetectPythonInFolder(string folder)
    {
        var exe = Path.Combine(folder, "python.exe");
        if (!File.Exists(exe))
        {
            return null;
        }

        var dlls = Directory.GetFiles(folder, "python3*.dll").ToList();
        // remove python3.dll
        dlls.RemoveAll(x => x.EndsWith("python3.dll"));
        if (dlls.Count == 0)
        {
            return null;
        }

        return new PythonEnv(folder, exe, dlls[0]);
    }

    public static IEnumerable<PythonEnv> FindPythonEnvs()
    {
        // system installed python
        foreach (var folder in FindPathContains("python.exe"))
        {
            var env = DetectPythonInFolder(folder);
            if (env is not null)
            {
                yield return env;
            }
        }

        // find conda installed path
        var condaExeBase = FindPathContains("conda.exe");
        var condaBatBase = FindPathContains("conda.bat");
        var condaBases = condaExeBase.Concat(condaBatBase).Distinct();
        var condaEnvsPath = (from path in condaBases
            let envsPath = Path.GetFullPath(Path.Combine(path, "..", "envs"))
            where Directory.Exists(envsPath)
            select envsPath).Distinct();

        foreach (var envsFolder in condaEnvsPath)
        {
            foreach (var pythonHome in Directory.GetDirectories(envsFolder))
            {
                var env = DetectPythonInFolder(pythonHome);
                if (env is not null)
                {
                    yield return env;
                }
            }
        }
    }

    public sealed class ScopeGuard(Action action) : IDisposable
    {
        private bool disposed;

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            action();
            disposed = true;
        }
    }
}