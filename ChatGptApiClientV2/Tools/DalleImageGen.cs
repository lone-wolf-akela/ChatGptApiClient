using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.ComponentModel;
using static ChatGptApiClientV2.Tools.IToolFunction;
using ChatGptApiClientV2.Controls;

namespace ChatGptApiClientV2.Tools;

public class DalleImageGen : IToolCollection
{
    public List<IToolFunction> Funcs =>
    [
        new DalleImageGenFunc()
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
    public class Args
    {
        [Description(
            """
            The user's original image description, potentially modified to abide by the following policies:
            1. Prompts must be in English. Translate to English if needed.
            2. Always mention the image type (photo, oil painting, watercolor painting, illustration, cartoon, drawing, vector, render, etc.) at the beginning of the prompt. Unless the prompt suggests otherwise, make the image a photo.
            3. The prompt must intricately describe every part of the image in concrete, objective detail.
            4. The prompt must be from 100 to 200 words long. If the user give too few details, please add more details to the prompt.
            5. If the user requested modifications to previous images, the prompt should not simply be longer, but rather it should be refactored to integrate the suggestions into the prompt.
            """)]
        public string Prompts { get; set; } = "";
        [Description("The size of the requested image. Use 1024x1024 (square) as the default, 1792x1024 if the user requests a wide image, and 1024x1792 for full-body portraits. Always include this parameter in the request.")]
        public ImageSize Size { get; set; } = ImageSize.Sqaure;
    }

    public string Description => "Call the DALL-E AI service to generate images according to the requirement given by the user.";
    public string Name => "dall_e_image_generation";
    public Type ArgsType => typeof(Args);

    private static readonly HttpClient HttpClient;
    static DalleImageGenFunc()
    {
        var httpClientHandler = new HttpClientHandler
        {
            AutomaticDecompression = System.Net.DecompressionMethods.All
        };
        HttpClient = new HttpClient(httpClientHandler);
    }
    private class Request
    {
        public string Prompt { get; init; } = "";
        public string Model { get; init; } = "dall-e-3";
        public string Quality { get; init; } = "hd";
        public string ResponseFormat { get; init; } = "b64_json";
        public ImageSize Size { get; init; } = ImageSize.Sqaure;
        public string Style { get; init; } = "vivid";

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
                NullValueHandling = NullValueHandling.Ignore
            };
            var result = JsonConvert.SerializeObject(this, settings);
            return result;
        }
    }
    public async Task<ToolResult> Action(SystemState state, string toolcallId, string argstr)
    {
        using var guard = new Utils.ScopeGuard(() => state.NetStatus.Status = NetStatus.StatusEnum.Idle);

        var msgContents = new List<IMessage.TextContent>();
        var msg = new ToolMessage { Content = msgContents };
        var result = new ToolResult(msg, true);
        msgContents.Add(new IMessage.TextContent { Text = "" });

        var argsJson = JToken.Parse(argstr);
        var argsReader = new JTokenReader(argsJson);
        var argsSerializer = new JsonSerializer();
        Args args;
        try
        {
            var parsedArgs = argsSerializer.Deserialize<Args>(argsReader);
            if (parsedArgs is null)
            {
                msgContents[0].Text += $"Failed to parse arguments for image generation. The args are: {argstr}\n\n";
                return result;
            }
            args = parsedArgs;
        }
        catch (JsonSerializationException e)
        {
            msgContents[0].Text += $"Failed to parse arguments for image generation. The args are: {argstr}\n\n";
            msgContents[0].Text += $"Exception: {e.Message}\n\n";
            return result;
        }

        HttpClient.DefaultRequestHeaders.Authorization = state.Config.ServiceProvider switch
        {
            Config.ServiceProviderType.Azure => null,
            _ => new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", state.Config.API_KEY)
        };

        state.NewMessage(RoleType.Tool);
        state.StreamText("生成中...\n\n");

        var requestbody = new Request
        {
            Prompt = $"Use my prompt as “Revised prompt” without changes; DO NOT add any detail, just use it AS-IS.\r\n\r\nPrompt: {args.Prompts}",
            Size = args.Size
        };
        var requestStr = requestbody.GeneratePostRequest();
        var postContent = new StringContent(requestStr, Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(state.Config.DalleImageGenServiceURL),
            Content = postContent
        };
        if (state.Config.ServiceProvider == Config.ServiceProviderType.Azure)
        {
            request.Headers.Add("api-key", state.Config.AzureAPIKey);
        }
        state.NetStatus.Status = NetStatus.StatusEnum.Sending;
        var response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        state.NetStatus.Status = NetStatus.StatusEnum.Receiving;

        if (!response.IsSuccessStatusCode)
        {
            await using var errorResponseStream = await response.Content.ReadAsStreamAsync();
            using var errorReader = new StreamReader(errorResponseStream);
            var errorResponse = await errorReader.ReadToEndAsync();
            var errorJson = JToken.Parse(errorResponse);
            if (errorJson["error"] is not null)
            {
                msgContents[0].Text += $"Error: {errorJson["error"]?["message"]}\n\n";
            }
            else
            {
                msgContents[0].Text += $"Error: {errorResponse}\n\n";
            }
            return result;
        }

        await using var responseStream = await response.Content.ReadAsStreamAsync();
        using var reader = new StreamReader(responseStream);
        var imageResponse = await reader.ReadToEndAsync();
        var responseJson = JToken.Parse(imageResponse);
        var responseData = responseJson["data"]?[0];
        var imageBase64Data = responseData?["b64_json"]?.ToString();

        if (imageBase64Data is null)
        {
            msgContents[0].Text += "Error: no image generated.\n\n";
            return result;
        }

        // see https://community.openai.com/t/dall-e-3-output-image-file-linked-in-response-url-is-uncompressed/522087/5
        imageBase64Data = Utils.OptimizeBase64Png(imageBase64Data);

        var imageDesc =
            $"""
             Original Prompts: {args.Prompts}

             Revised Prompts: {responseData?["revised_prompt"]}
             """;
        state.CurrentSession!.PluginData[$"{Name}_{toolcallId}_imagedata"] = [imageBase64Data];
        state.CurrentSession!.PluginData[$"{Name}_{toolcallId}_imagedesc"] = [imageDesc];
        msgContents[0].Text += "The generated image is now displayed on the screen.\n\n";
        msg.Hidden = true; // Hide success results from user
        result.ResponeRequired = false;
        return result;
    }

    public IEnumerable<Block> GetToolcallMessage(SystemState state, string argstr, string toolcallId)
    {
        var argsJson = JToken.Parse(argstr);
        var argsReader = new JTokenReader(argsJson);
        var argsSerializer = new JsonSerializer();
        var isFailed = false;
        try
        {
            var parsedArgs = argsSerializer.Deserialize<Args>(argsReader);
            if (parsedArgs is null)
            {
                isFailed = true;
            }
        }
        catch (JsonSerializationException)
        {
            isFailed = true;
        }
        if (isFailed)
        {
            yield return new Paragraph(new Run("使用 DALL-E 生成图像..."));
            yield break;
        }

        List<string> stickers = [
            "格蕾修_在做了.png",
            "胡桃-交给我吧.png",
            "卡维-开工.png",
            "赛诺-艺术！.png"
        ];

        var floater = Utils.CreateStickerFloater(stickers, toolcallId);

        var paragraph = new Paragraph();
        paragraph.Inlines.Add(floater);
        paragraph.Inlines.Add(new Run("使用 DALL-E 生成图像:"));
        paragraph.Inlines.Add(new LineBreak());
        paragraph.Inlines.Add(new LineBreak());

        yield return paragraph;

        var imagedata = state.CurrentSession!.PluginData[$"{Name}_{toolcallId}_imagedata"][0];
        var imagedesc = state.CurrentSession!.PluginData[$"{Name}_{toolcallId}_imagedesc"][0];
        var image = new ImageDisplayer
        {
            Image = Utils.Base64ToBitmapImage(imagedata),
            ImageTooltip = imagedesc
        };
        yield return new BlockUIContainer(image);
    }
}
