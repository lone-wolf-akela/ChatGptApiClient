using NJsonSchema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using static ChatGptApiClientV2.MainWindow;
using System.Windows.Threading;

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
        public string Parameters { get; }
        public Task Action(ConfigType config, NetStatusType netstatus, ChatRecord chatrecord, string args);
    }

    public class DalleImageGen : IToolFunction
    {
        public class Args
        {
            [System.ComponentModel.Description("Text to describe the style and content of the image to be generated. The prompt should be around 100 words. It must be in English. Please include the drawing style according to the user's request, before the text that describing the content of the image. If the user does not specify a style, please use the style of Japanese light novel illustration.")]
            public string Prompt { get; set; } = "";
        }

        public string Description => "Call the DALL-E AI service to generate images according to the requirement given by the user. The DALL-E service takes English texts which describes the content of the image.";
        public string Name => "dall_e_image_generation";
        public string DisplayName => "DALL-E Image Generation";
        public string Parameters => JsonSchema.FromType<Args>().ToJson();

        private readonly HttpClient client = new();
        public async Task Action(ConfigType config, NetStatusType netstatus, ChatRecord record, string argstr)
        {
            var args = JsonSerializer.Deserialize<Args>(argstr);
            if (args is null)
            {
                record.Content += $"\nFailed to parse arguments for image generation. The args are: {argstr}";
                return;
            }

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", config.API_KEY);

            var msg = new JsonObject
            {
                ["prompt"] = args.Prompt,
                ["model"] = "dall-e-3",
                ["quality"] = "hd",
                ["response_format"] = "b64_json",
                ["size"] = "1024x1024",
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
            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            netstatus.Status = NetStatusType.StatusEnum.Receiving;
            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                using var reader = new StreamReader(responseStream);
                var ImageResponse = await reader.ReadToEndAsync();
                var responseJson = JsonNode.Parse(ImageResponse);
                var imageb64 = responseJson?["data"]?[0]?["b64_json"]?.ToString();
                if (imageb64 is null)
                {
                    record.Content += "\nError: no image base64 data.";
                }
                else
                {
                    record.AddImageFromBase64(imageb64, false);
                    record.Content += "\nImage generation successful.";
                }
            }
            else
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                using var reader = new StreamReader(responseStream);
                var ImageResponse = await reader.ReadToEndAsync();
                var responseJson = JsonNode.Parse(ImageResponse);
                if (responseJson?["error"] is not null)
                {
                    record.Content += $"\nError: {responseJson?["error"]?["message"]?.ToString()}";
                }
                else
                {
                    record.Content += $"\nError: {ImageResponse}";
                }
            }
            netstatus.Status = NetStatusType.StatusEnum.Idle;
        }
    }
}
