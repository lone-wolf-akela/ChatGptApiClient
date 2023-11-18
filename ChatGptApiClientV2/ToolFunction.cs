using NJsonSchema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static ChatGptApiClientV2.MainWindow;
using System.Windows.Threading;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;
using NJsonSchema.Generation;

namespace ChatGptApiClientV2
{
    public static class ToolFunction
    {
        public static readonly List<IToolFunction> FunctionList =
        [
            new DalleImageGen(),
        ];
    }
    public interface IToolFunction
    {
        public string? Description { get; }
        public string Name { get; }
        public string DisplayName { get; }
        public Type ArgsType { get; }
        public string Parameters
        {
            get
            {
                var settings = new JsonSchemaGeneratorSettings();
                var schema = new JsonSchema();
                var resolver = new JsonSchemaResolver(schema, settings);
                var generator = new JsonSchemaGenerator(settings);
                generator.Generate(schema, ArgsType, resolver);
                return schema.ToJson().ToString();
            }
        }
        public Task Action(ConfigType config, NetStatusType netstatus, ChatRecord chatrecord, string args);
    }

    public class DalleImageGen : IToolFunction
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public enum ImageSize
        {
            [EnumMember(Value = "1024x1024")]
            Sqaure,
            [EnumMember(Value = "1792x1024")]
            Wide,
            [EnumMember(Value = "1024x1792")]
            Tall
        }
        public static ImageSize StringToImageSize(string size) => size switch
        {
            "1024x1024" => ImageSize.Sqaure,
            "1792x1024" => ImageSize.Wide,
            "1024x1792" => ImageSize.Tall,
            _ => throw new NotImplementedException(),
        };
        public class Args
        {
            [System.ComponentModel.Description(
                @"The user's original image description, potentially modified to abide by the following policies:
1. Prompts must be in English. Translate to English if needed.
2. Always mention the image type (photo, oil painting, watercolor painting, illustration, cartoon, drawing, vector, render, etc.) at the beginning of the prompt. Unless the prompt suggests otherwise, make the image a photo.
3. The prompt must intricately describe every part of the image in concrete, objective detail. 
4. The prompt must be from 100 to 200 words long. If the user give too few details, please add more details to the prompt.
5. If the user requested modifications to previous images, the prompt should not simply be longer, but rather it should be refactored to integrate the suggestions into the prompt.")]
            public string Prompts { get; set; } = "";
            [System.ComponentModel.Description(@"The size of the requested image. Use 1024x1024 (square) as the default, 1792x1024 if the user requests a wide image, and 1024x1792 for full-body portraits. Always include this parameter in the request.")]
            public ImageSize Size { get; set; } = ImageSize.Sqaure;
        }

        public string Description => @"Call the DALL-E AI service to generate images according to the requirement given by the user.";
        public string Name => "dall_e_image_generation";
        public string DisplayName => "DALL-E 图像生成";
        public Type ArgsType => typeof(Args);

        private readonly HttpClient api_client = new();
        private readonly HttpClient download_client = new();
        public async Task Action(ConfigType config, NetStatusType netstatus, ChatRecord record, string argstr)
        {
            var args_json = JToken.Parse(argstr);
            var args_reader = new JTokenReader(args_json);
            var args_serializer = new JsonSerializer();
            var args = args_serializer.Deserialize<Args>(args_reader);
            if (args is null)
            {
                record.Content += $"Failed to parse arguments for image generation. The args are: {argstr}\n\n";
                return;
            }

            api_client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", config.API_KEY);

            Console.WriteLine($"Generating image with prompt: {args.Prompts}");
            Console.WriteLine();

            var msg = new JObject
            {
                ["prompt"] = args.Prompts, // $"DO NOT add any detail, just use it AS-IS: {args.Prompts}",
                ["model"] = "dall-e-3",
                ["quality"] = "hd",
                ["response_format"] = "url",
                ["size"] = args_json?["Size"] ?? "1024x1024",
                ["style"] = "vivid", // or "natural"
            };

            var post_content = new StringContent(msg.ToString(), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://api.openai.com/v1/images/generations"),
                Content = post_content
            };
            netstatus.Status = NetStatusType.StatusEnum.Sending;
            var response = await api_client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            netstatus.Status = NetStatusType.StatusEnum.Receiving;

            if (!response.IsSuccessStatusCode)
            {
                using var errorResponseStream = await response.Content.ReadAsStreamAsync();
                using var errorReader = new StreamReader(errorResponseStream);
                var errorResponse = await errorReader.ReadToEndAsync();
                var errorJson = JToken.Parse(errorResponse);
                if (errorJson?["error"] is not null)
                {
                    record.Content += $"Error: {errorJson?["error"]?["message"]?.ToString()}\n\n";
                }
                else
                {
                    record.Content += $"Error: {errorResponse}\n\n";
                }
                netstatus.Status = NetStatusType.StatusEnum.Idle;
                return;
            }

            using var responseStream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(responseStream);
            var ImageResponse = await reader.ReadToEndAsync();
            var responseJson = JToken.Parse(ImageResponse);
            var response_data = responseJson?["data"]?[0];
            var img_download_url = response_data?["url"]?.ToString();

            if (img_download_url is null)
            {
                record.Content += "Error: no image download address generated.\n\n";
                netstatus.Status = NetStatusType.StatusEnum.Idle;
                return;
            }

            Console.WriteLine($"Downloading image from {img_download_url}");
            Console.WriteLine();

            var progress = new Progress<HttpDownloadProgressData>(Progress =>
            {
                Console.Write($"\rDownloading image: {Progress.Percent * 100:0.00}% ({Progress.Current}/{Progress.Total} Bytes)");
            });

            bool download_success;
            var tmp_name = Path.GetTempFileName();
            using (var fs = File.Create(tmp_name))
            {
                download_success = await download_client.DownloadAsync(img_download_url, fs, progress);
            }

            if (!download_success)
            {
                record.Content += $"Error: failed to download image from {img_download_url}\n\n";
                netstatus.Status = NetStatusType.StatusEnum.Idle;
                return;
            }

            var image_file_ext = Utils.GetFileExtensionFromUrl(img_download_url);
            var image_name = Path.ChangeExtension(tmp_name, image_file_ext);
            File.Move(tmp_name, image_name);
            record.AddImageFromFile(image_name, false, $"Original Prompts: {args.Prompts}\n\nRevised Prompts: {response_data?["revised_prompt"]}\n\nDownload URL: {img_download_url}");
            File.Delete(image_name);

            netstatus.Status = NetStatusType.StatusEnum.Idle;
            return;
        }
    }

    // from https://stackoverflow.com/questions/20661652/progress-bar-with-httpclient/46497896#46497896
    public struct HttpDownloadProgressData
    {
        public long Current;
        public long Total;
        public readonly float Percent => Total > 0 ? (float)Current / Total : 0;
    }
    public static class HttpClientExtensions
    {
        public static async Task<bool> DownloadAsync(this HttpClient client, string requestUri, Stream destination, IProgress<HttpDownloadProgressData>? progress = null, CancellationToken cancellationToken = default)
        {
            // Get the http headers first to examine the content length
            using var response = await client.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }
            var contentLength = response.Content.Headers.ContentLength;

            using var download = await response.Content.ReadAsStreamAsync(cancellationToken);

            // Ignore progress reporting when no progress reporter was 
            // passed or when the content length is unknown
            if (progress == null || !contentLength.HasValue)
            {
                await download.CopyToAsync(destination, cancellationToken);
                return true;
            }

            // Convert absolute progress (bytes downloaded) into relative progress (0% - 100%)
            var streamprogress = new Progress<long>(totalBytes => progress.Report(new() { Current = totalBytes, Total = contentLength.Value }));
            // Use extension method to report progress while downloading
            await download.CopyToAsync(destination, 1024, streamprogress, cancellationToken);
            progress.Report(new() { Current = contentLength.Value, Total = contentLength.Value });
            return true;
        }
    }

    public static class StreamExtensions
    {
        public static async Task CopyToAsync(this Stream source, Stream destination, int bufferSize, IProgress<long>? progress = null, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(source);
            if (!source.CanRead)
                throw new ArgumentException("Has to be readable", nameof(source));
            ArgumentNullException.ThrowIfNull(destination);
            if (!destination.CanWrite)
                throw new ArgumentException("Has to be writable", nameof(destination));
            ArgumentOutOfRangeException.ThrowIfNegative(bufferSize);

            var buffer = new byte[bufferSize];
            long totalBytesRead = 0;
            int bytesRead;
            while ((bytesRead = await source.ReadAsync(buffer, cancellationToken).ConfigureAwait(false)) != 0)
            {
                await destination.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken).ConfigureAwait(false);
                totalBytesRead += bytesRead;
                progress?.Report(totalBytesRead);
            }
        }
    }
}
