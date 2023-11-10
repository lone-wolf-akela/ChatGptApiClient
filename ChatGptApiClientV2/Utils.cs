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

namespace ChatGptApiClientV2
{
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
        public static BitmapImage Base64ToBitmap(string base64)
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
    }
}
