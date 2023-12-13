using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static ChatGptApiClientV2.MainWindow;
using Flurl;

namespace ChatGptApiClientV2.Tools
{
    public class DalleImageGen : IToolCollection
    {
        public List<IToolFunction> Funcs { get; } =
        [
            new DalleImageGenFunc(),
        ];
        public string DisplayName => "DALL-E 图像生成";
    }
    public class DalleImageGenFunc : IToolFunction
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
        public Type ArgsType => typeof(Args);

        private readonly HttpClient apiClient = new();
        private readonly HttpClient downloadClient = new();
        private class Request
        {
            public string Prompt { get; set; } = "";
            public string Model { get; set; } = "dall-e-3";
            public string Quality { get; set; } = "hd";
            public string ResponseFormat { get; set; } = "url";
            public ImageSize Size { get; set; } = ImageSize.Sqaure;
            public string Style { get; set; } = "vivid";

            public string GeneratePostRequest()
            {
                var contractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                };
                var settings = new JsonSerializerSettings
                {
                    ContractResolver = contractResolver,
                    Formatting = Formatting.None,
                    StringEscapeHandling = StringEscapeHandling.Default,
                    NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
                };
                var result = JsonConvert.SerializeObject(this, settings);
                return result;
            }
        }
        public async Task<ToolMessage> Action(ConfigType config, NetStatusType netstatus, string argstr)
        {
            var msgContents = new List<IMessage.TextContent>();
            var msg = new ToolMessage { Content = msgContents };
            msgContents.Add(new() { Text = "" });

            var args_json = JToken.Parse(argstr);
            var args_reader = new JTokenReader(args_json);
            var args_serializer = new JsonSerializer();
            Args args;
            try
            {
                var parsedArgs = args_serializer.Deserialize<Args>(args_reader);
                if (parsedArgs is null)
                {
                    msgContents[0].Text += $"Failed to parse arguments for image generation. The args are: {argstr}\n\n";
                    return msg;
                }
                args = parsedArgs;
            }
            catch (JsonSerializationException e)
            {
                msgContents[0].Text += $"Failed to parse arguments for image generation. The args are: {argstr}\n\n";
                msgContents[0].Text += $"Exception: {e.Message}\n\n";
                return msg;
            }

            apiClient.DefaultRequestHeaders.Authorization = config.ServiceProvider switch
            {
                ConfigType.ServiceProviderType.Azure => null,
                _ => new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", config.API_KEY),
            };

            Console.WriteLine($"Generating image with prompt: {args.Prompts}");
            Console.WriteLine();

            var requestbody = new Request
            {
                Prompt = args.Prompts,
                Size = args.Size,
            };
            var requestStr = requestbody.GeneratePostRequest();
            var postContent = new StringContent(requestStr, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(config.DalleImageGenServiceURL),
                Content = postContent
            };
            if (config.ServiceProvider == ConfigType.ServiceProviderType.Azure)
            {
                request.Headers.Add("api-key", config.AzureAPIKey);
            }
            netstatus.Status = NetStatusType.StatusEnum.Sending;
            var response = await apiClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            netstatus.Status = NetStatusType.StatusEnum.Receiving;

            if (!response.IsSuccessStatusCode)
            {
                using var errorResponseStream = await response.Content.ReadAsStreamAsync();
                using var errorReader = new StreamReader(errorResponseStream);
                var errorResponse = await errorReader.ReadToEndAsync();
                var errorJson = JToken.Parse(errorResponse);
                if (errorJson?["error"] is not null)
                {
                    msgContents[0].Text += $"Error: {errorJson?["error"]?["message"]?.ToString()}\n\n";
                }
                else
                {
                    msgContents[0].Text += $"Error: {errorResponse}\n\n";
                }
                netstatus.Status = NetStatusType.StatusEnum.Idle;
                return msg;
            }

            using var responseStream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(responseStream);
            var ImageResponse = await reader.ReadToEndAsync();
            var responseJson = JToken.Parse(ImageResponse);
            var response_data = responseJson?["data"]?[0];
            var img_download_url = response_data?["url"]?.ToString();

            if (img_download_url is null)
            {
                msgContents[0].Text += "Error: no image download address generated.\n\n";
                netstatus.Status = NetStatusType.StatusEnum.Idle;
                return msg;
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
                download_success = await downloadClient.DownloadAsync(img_download_url, fs, progress);
                Console.WriteLine();
            }

            if (!download_success)
            {
                msgContents[0].Text += $"Error: failed to download image from {img_download_url}\n\n";
                netstatus.Status = NetStatusType.StatusEnum.Idle;
                return msg;
            }

            var image_file_ext = Utils.GetFileExtensionFromUrl(img_download_url);
            var image_name = Path.ChangeExtension(tmp_name, image_file_ext);
            File.Move(tmp_name, image_name);
            var image_url = Utils.ImageFileToBase64(image_name);
            var image_desc =
                @$"Original Prompts: {args.Prompts}

Revised Prompts: {response_data?["revised_prompt"]}

Download URL: {img_download_url}";
            msg.GeneratedImages.Add(new() { ImageBase64Url = image_url, Description = image_desc });
            File.Delete(image_name);
            msgContents[0].Text += $"Image generated successfully and is now displaying on the screen.\n\n";
            netstatus.Status = NetStatusType.StatusEnum.Idle;
            return msg;
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
