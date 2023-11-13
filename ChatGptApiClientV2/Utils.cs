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

namespace ChatGptApiClientV2
{
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
        public static BitmapImage Base64ToBitmapImage(string base64)
        {
            var r = Base64UrlExtract();
            var match = r.Match(base64);
            if (!match.Success)
            {
                throw new ArgumentException("The string is not a base64 image.");
            }
            var data = match.Groups["data"].Value;
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
            var r = Base64UrlExtract();
            var match = r.Match(base64);
            if (!match.Success)
            {
                throw new ArgumentException("The string is not a base64 image.");
            }
            var data = match.Groups["data"].Value;
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
        public static Bitmap ResizeImage(Bitmap imgToResize, Size size, PixelFormat format)
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

            var resized = ResizeImage(img, new Size(width, height), PixelFormat.Format24bppRgb);
            string converted = Derasterize.ConvertImageToString(resized);
            return converted;
        }
        public static void ConsolePrintImage(string img)
        {
            ConsoleEnableVTSeq();
            ConsoleDisableWrap();
            Console.WriteLine(img);
            ConsoleEnableWrap();
        }
    }
}
