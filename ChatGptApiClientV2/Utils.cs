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
using System.Runtime.InteropServices.Marshalling;
using Newtonsoft.Json;
using SharpToken;
using System.Windows.Interop;
using System.Threading;
using System.Windows.Media;
// ReSharper disable UnusedMember.Global

namespace ChatGptApiClientV2;

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

        var clip = Utils.CreateRoundedRectangleGeometry(width, height, radius);
        clip.Freeze();

        return clip;

    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }

    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}

// from https://stackoverflow.com/questions/6145888/how-to-bind-an-enum-to-a-combobox-control-in-wpf
public class Int2VisibilityReConverter : MarkupExtension, IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int i)
        {
            return i == 0 ? Visibility.Visible : Visibility.Collapsed;
        }
        return Visibility.Collapsed;
    }
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Visibility v)
        {
            return v == Visibility.Visible ? 0 : 1;
        }
        return 0;
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

public class EnumValueDescription(Enum value, string desc)
{
    public Enum Value { get; set; } = value;
    public string Description { get; set; } = desc;
}
public static class EnumHelper
{
    private static string Description(this Enum value)
    {
        var attributes = value.GetType().GetField(value.ToString())?.GetCustomAttributes(typeof(DescriptionAttribute), false);
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
    public static Geometry CreateRoundedRectangleGeometry(double width, double height, CornerRadius radius)
    {
        var geometry = new StreamGeometry();

        using var context = geometry.Open();
        context.BeginFigure(new Point(radius.TopLeft, 0), true, true);

        // Top side and top-right corner
        context.LineTo(new Point(width - radius.TopRight, 0), true, false);
        context.ArcTo(new Point(width, radius.TopRight), new Size(radius.TopRight, radius.TopRight), 0, false, SweepDirection.Clockwise, true, false);

        // Right side and bottom-right corner
        context.LineTo(new Point(width, height - radius.BottomRight), true, false);
        context.ArcTo(new Point(width - radius.BottomRight, height), new Size(radius.BottomRight, radius.BottomRight), 0, false, SweepDirection.Clockwise, true, false);

        // Bottom side and bottom-left corner
        context.LineTo(new Point(radius.BottomLeft, height), true, false);
        context.ArcTo(new Point(0, height - radius.BottomLeft), new Size(radius.BottomLeft, radius.BottomLeft), 0, false, SweepDirection.Clockwise, true, false);

        // Left side and top-left corner
        context.LineTo(new Point(0, radius.TopLeft), true, false);
        context.ArcTo(new Point(radius.TopLeft, 0), new Size(radius.TopLeft, radius.TopLeft), 0, false, SweepDirection.Clockwise, true, false);

        return geometry;
    }


    private static GptEncoding? tokenEncoding;
    public static int GetStringTokenNum(string str)
    {
        tokenEncoding ??= GptEncoding.GetEncoding("cl100k_base");
        var token = tokenEncoding.Encode(str);
        return token.Count;
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

    [GeneratedRegex("^data:(?<mime>[a-z]+\\/[a-z]+);base64,(?<data>.+)$")]
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

    public static string OptimizeBase64Png(string base64)
    {
        // optimize the input data
        // used for dall-e generated image, as they are almost not compressed
        // see https://community.openai.com/t/dall-e-3-output-image-file-linked-in-response-url-is-uncompressed/522087/5
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

    private static T RandomSelectByStringHash<T>(string hashsrc, IList<T> list)
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
    // char* poppler_extract_text_from_pdf_file(const char* filename, size_t filename_len = 0);
    // void poppler_free_text(const char* text);
    [LibraryImport("poppler-wrapper.dll", EntryPoint = "poppler_extract_text_from_pdf_file", StringMarshalling = StringMarshalling.Utf8)]
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

    // from https://stackoverflow.com/questions/354477/method-to-determine-if-path-string-is-local-or-remote-machine
    public static bool IsPathRemote(string path)
    {
        DirectoryInfo dir = new(path);
        foreach (var d in DriveInfo.GetDrives())
        {
            if (string.Compare(dir.Root.FullName, d.Name, StringComparison.OrdinalIgnoreCase) == 0)
            {
                return (d.DriveType == DriveType.Network);
            }
        }
        return true;
    }

    // ReSharper disable InconsistentNaming
    // ReSharper disable UnusedMember.Local
    // ReSharper disable UnusedMember.Global
    private const string IID_IImageList = "46EB5926-582E-4017-9FDF-E8998DAA0950";
    private const string IID_IImageList2 = "192B9D83-50FC-457B-90A0-2B82A8B5DAE1";

    private static partial class Shell32
    {
        public const int SHIL_LARGE = 0x0;
        public const int SHIL_SMALL = 0x1;
        public const int SHIL_EXTRALARGE = 0x2;
        public const int SHIL_SYSSMALL = 0x3;
        public const int SHIL_JUMBO = 0x4;
        public const int SHIL_LAST = 0x4;

        public const int ILD_TRANSPARENT = 0x00000001;
        public const int ILD_IMAGE = 0x00000020;

        [LibraryImport("shell32.dll")]
        public static partial int SHGetImageList(int iImageList, ref Guid riid, ref IImageList? ppv);

        [LibraryImport("user32.dll", SetLastError = true)]
        public static partial void DestroyIcon(IntPtr hIcon);

        [DllImport("Shell32.dll")]
        public static extern IntPtr SHGetFileInfo(
            string pszPath,
            uint dwFileAttributes,
            ref SHFILEINFO psfi,
            uint cbFileInfo,
            uint uFlags
        );
    }

    [Flags]
    private enum SHGFI : uint
    {
        /// <summary>get icon</summary>
        Icon = 0x000000100,
        /// <summary>get display name</summary>
        DisplayName = 0x000000200,
        /// <summary>get type name</summary>
        TypeName = 0x000000400,
        /// <summary>get attributes</summary>
        Attributes = 0x000000800,
        /// <summary>get icon location</summary>
        IconLocation = 0x000001000,
        /// <summary>return exe type</summary>
        ExeType = 0x000002000,
        /// <summary>get system icon index</summary>
        SysIconIndex = 0x000004000,
        /// <summary>put a link overlay on icon</summary>
        LinkOverlay = 0x000008000,
        /// <summary>show icon in selected state</summary>
        Selected = 0x000010000,
        /// <summary>get only specified attributes</summary>
        Attr_Specified = 0x000020000,
        /// <summary>get large icon</summary>
        LargeIcon = 0x000000000,
        /// <summary>get small icon</summary>
        SmallIcon = 0x000000001,
        /// <summary>get open icon</summary>
        OpenIcon = 0x000000002,
        /// <summary>get shell size icon</summary>
        ShellIconSize = 0x000000004,
        /// <summary>pszPath is a pidl</summary>
        PIDL = 0x000000008,
        /// <summary>use passed dwFileAttribute</summary>
        UseFileAttributes = 0x000000010,
        /// <summary>apply the appropriate overlays</summary>
        AddOverlays = 0x000000020,
        /// <summary>Get the index of the overlay in the upper 8 bits of the iIcon</summary>
        OverlayIndex = 0x000000040,
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct SHFILEINFO
    {
        public const int NAMESIZE = 80;
        public IntPtr hIcon;
        public int iIcon;
        public uint dwAttributes;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szDisplayName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szTypeName;
    };


    [StructLayout(LayoutKind.Sequential)]
    internal struct RECT
    {
        public int left, top, right, bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct POINT
    {
        int x;
        int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct IMAGELISTDRAWPARAMS
    {
        public int cbSize;
        public IntPtr himl;
        public int i;
        public IntPtr hdcDst;
        public int x;
        public int y;
        public int cx;
        public int cy;
        public int xBitmap;    // x offset from the upper-left of bitmap
        public int yBitmap;    // y offset from the upper-left of bitmap
        public int rgbBk;
        public int rgbFg;
        public int fStyle;
        public int dwRop;
        public int fState;
        public int Frame;
        public int crEffect;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct IMAGEINFO
    {
        public IntPtr hbmImage;
        public IntPtr hbmMask;
        public int Unused1;
        public int Unused2;
        public RECT rcImage;
    }
    [GeneratedComInterface]
    [Guid("46EB5926-582E-4017-9FDF-E8998DAA0950")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal partial interface IImageList
    {
        [PreserveSig]
        int Add(IntPtr hbmImage, IntPtr hbmMask, ref int pi);

        [PreserveSig]
        int ReplaceIcon(int i, IntPtr hicon, ref int pi);

        [PreserveSig]
        int SetOverlayImage(int iImage, int iOverlay);

        [PreserveSig]
        int Replace(int i, IntPtr hbmImage, IntPtr hbmMask);

        [PreserveSig]
        int AddMasked(IntPtr hbmImage, int crMask, ref int pi);

        [PreserveSig]
        int Draw(ref IMAGELISTDRAWPARAMS pimldp);

        [PreserveSig]
        int Remove(int i);

        [PreserveSig]
        int GetIcon(int i, int flags, ref IntPtr picon);

        [PreserveSig]
        int GetImageInfo(int i, ref IMAGEINFO pImageInfo);

        [PreserveSig]
        int Copy(int iDst, IImageList punkSrc, int iSrc, int uFlags);

        [PreserveSig]
        int Merge(int i1, IImageList punk2, int i2, int dx, int dy, ref Guid riid, ref IntPtr ppv);

        [PreserveSig]
        int Clone(ref Guid riid, ref IntPtr ppv);

        [PreserveSig]
        int GetImageRect(int i, ref RECT prc);

        [PreserveSig]
        int GetIconSize(ref int cx, ref int cy);

        [PreserveSig]
        int SetIconSize(int cx, int cy);

        [PreserveSig]
        int GetImageCount(ref int pi);

        [PreserveSig]
        int SetImageCount(int uNewCount);

        [PreserveSig]
        int SetBkColor(int clrBk, ref int pclr);

        [PreserveSig]
        int GetBkColor(ref int pclr);

        [PreserveSig]
        int BeginDrag(int iTrack, int dxHotspot, int dyHotspot);

        [PreserveSig]
        int EndDrag();

        [PreserveSig]
        int DragEnter(IntPtr hwndLock, int x, int y);

        [PreserveSig]
        int DragLeave(IntPtr hwndLock);

        [PreserveSig]
        int DragMove(int x, int y);

        [PreserveSig]
        int SetDragCursorImage(ref IImageList punk, int iDrag, int dxHotspot, int dyHotspot);

        [PreserveSig]
        int DragShowNolock(int fShow);

        [PreserveSig]
        int GetDragImage(ref POINT ppt, ref POINT pptHotspot, ref Guid riid, ref IntPtr ppv);

        [PreserveSig]
        int GetItemFlags(int i, ref int dwFlags);

        [PreserveSig]
        int GetOverlayImage(int iOverlay, ref int piIndex);
    }
    // ReSharper restore UnusedMember.Global
    // ReSharper restore UnusedMember.Local
    // ReSharper restore InconsistentNaming

    private static int GetIconIndex(string pszFile)
    {
        SHFILEINFO sfi = new();
        Shell32.SHGetFileInfo(
            pszFile,
            0,
            ref sfi,
            (uint)Marshal.SizeOf(sfi),
            (uint)(SHGFI.SysIconIndex | SHGFI.LargeIcon | SHGFI.UseFileAttributes)
            );
        return sfi.iIcon;
    }

    private static IntPtr? GetJumboIcon(int iImage)
    {
        IImageList? spiml = null;
        Guid guil = new(IID_IImageList2);//or IID_IImageList
        var result = Shell32.SHGetImageList(Shell32.SHIL_JUMBO, ref guil, ref spiml);
        if (result != 0)
        {
            return null;
        }
        var hIcon = IntPtr.Zero;
        spiml?.GetIcon(iImage, Shell32.ILD_TRANSPARENT | Shell32.ILD_IMAGE, ref hIcon);
        return hIcon;
    }

    // from https://stackoverflow.com/questions/28525925/get-icon-128128-file-type-c-sharp
    public static BitmapSource? Get256FileIcon(string path)
    {
        var hIcon = GetJumboIcon(GetIconIndex(path));
        if (hIcon is null)
        {
            return null;
        }
        // 使用WPF的Imaging类来创建BitmapSource
        var iconSrc = Imaging.CreateBitmapSourceFromHIcon(
            hIcon.Value,
            Int32Rect.Empty,
            BitmapSizeOptions.FromEmptyOptions()
        );
        iconSrc.Freeze();  // 重要：冻结图像，使其可以跨线程使用

        Shell32.DestroyIcon(hIcon.Value); // 清理非托管资源

        return iconSrc;
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