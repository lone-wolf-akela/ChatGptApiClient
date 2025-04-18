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
using System.Threading.Tasks;
using System.Windows.Documents;
using System.ComponentModel;
using static ChatGptApiClientV2.Tools.IToolFunction;
using ChatGptApiClientV2.Controls;
using System.Threading;
using System.ComponentModel.DataAnnotations;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

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
        [EnumMember(Value = "1024x1024")] Sqaure,
        [EnumMember(Value = "1792x1024")] Wide,
        [EnumMember(Value = "1024x1792")] Tall
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
        [Required]
        public string Prompts { get; set; } = "";

        [Description(
            "The size of the requested image. Use 1024x1024 (square) as the default, 1792x1024 if the user requests a wide image, and 1024x1792 for full-body portraits.")]
        [Required]
        public ImageSize Size { get; set; } = ImageSize.Sqaure;
    }

    public string Description =>
        "Call the DALL-E AI service to generate images according to the requirement given by the user.";

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

    public async Task<ToolResult> Action(SystemState state, Guid sessionId, string toolcallId, string argstr,
        CancellationToken cancellationToken = default)
    {
        using var guard = new Utils.ScopeGuard(() => state.NetStatus.Status = NetStatus.StatusEnum.Idle);

        var msgContents = new List<IMessage.TextContent>();
        var msg = new ToolMessage { Content = msgContents };
        var result = new ToolResult(msg, true);
        msgContents.Add(new IMessage.TextContent { Text = "" });

        Args args;
        try
        {
            var parsedArgs = JsonConvert.DeserializeObject<Args>(argstr);

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

        HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", state.Config.API_KEY);

        state.NewMessage(RoleType.Tool, sessionId);
        state.StreamText("生成中...\n\n", sessionId);

        var requestbody = new Request
        {
            Prompt =
                $"Use my prompt as “Revised prompt” without changes; DO NOT add any detail, just use it AS-IS.\r\n\r\nPrompt: {args.Prompts}",
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

        state.NetStatus.Status = NetStatus.StatusEnum.Sending;
        var response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        state.NetStatus.Status = NetStatus.StatusEnum.Receiving;

        if (!response.IsSuccessStatusCode)
        {
            await using var errorResponseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var errorReader = new StreamReader(errorResponseStream);
            var errorResponse = await errorReader.ReadToEndAsync(cancellationToken);
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

        await using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var reader = new StreamReader(responseStream);
        var imageResponse = await reader.ReadToEndAsync(cancellationToken);
        var responseJson = JToken.Parse(imageResponse);
        var responseData = responseJson["data"]?[0];
        var imageBase64Data = responseData?["b64_json"]?.ToString();

        if (imageBase64Data is null)
        {
            msgContents[0].Text += "Error: no image generated.\n\n";
            return result;
        }

        // resave as png to reduce size
        // see https://community.openai.com/t/dall-e-3-output-image-file-linked-in-response-url-is-uncompressed/522087/5
        var imageData = ImageData.CreateFromBase64Str(imageBase64Data).ReSaveAsPng();

        var imageDesc =
            $"""
             Original Prompts: {args.Prompts}

             Revised Prompts: {responseData?["revised_prompt"]}
             """;
        state.SessionDict[sessionId]!.PluginData[$"{Name}_{toolcallId}_imagedata"] = [imageData.ToUri()];
        state.SessionDict[sessionId]!.PluginData[$"{Name}_{toolcallId}_imagedesc"] = [imageDesc];
        msgContents[0].Text += "The generated image is now displayed on the screen.\n\n";
        msg.Hidden = true; // Hide success results from user
        result.ResponeRequired = false;
        return result;
    }

    public ToolCallMessage GetToolcallMessage(SystemState state, Guid sessionId, string argstr, string toolcallId)
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
            return new ToolCallMessage("使用 DALL-E 生成图像...");
        }

        List<string> stickers =
        [
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

        var foundData =
            state.SessionDict[sessionId]!.PluginData.TryGetValue($"{Name}_{toolcallId}_imagedata",
                out var imagedata);
        var foundDesc =
            state.SessionDict[sessionId]!.PluginData.TryGetValue($"{Name}_{toolcallId}_imagedesc",
                out var imagedesc);
        if (!foundData)
        {
            return new ToolCallMessage("使用 DALL-E 生成图像.", [paragraph]);
        }

        var image = new ImageDisplayer
        {
            ImageSource = ImageData.CreateFromUri(imagedata![0]).ToBitmapImage(),
            ImageTooltip = foundDesc ? imagedesc![0] : ""
        };
        return new ToolCallMessage($"使用 DALL-E 生成图像:\n{image.ImageTooltip}", [paragraph, new BlockUIContainer(image)]);
    }
}