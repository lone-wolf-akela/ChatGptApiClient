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

namespace ChatGptApiClientV2
{
    class Utils
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
        public static Image Base64ToImage(string base64)
        {
            var r = new Regex(@"^data:(?<mime>[a-z]+\/[a-z]+);base64,(?<data>.+)$");
            var match = r.Match(base64);
            if (!match.Success)
            {
                throw new ArgumentException("The string is not a base64 image.");
            }
            var data = match.Groups["data"].Value;
            var bytes = Convert.FromBase64String(data);
            using var ms = new MemoryStream(bytes);
            return Image.FromStream(ms);
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// From https://stackoverflow.com/questions/1922040/how-to-resize-an-image-c-sharp
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(Image image, int width, int height, bool keep_aspect_ratio)
        {
            if (keep_aspect_ratio)
            {
                var ratio = Math.Min((double)width / image.Width, (double)height / image.Height);
                width = (int)(image.Width * ratio);
                height = (int)(image.Height * ratio);
            }

            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}
