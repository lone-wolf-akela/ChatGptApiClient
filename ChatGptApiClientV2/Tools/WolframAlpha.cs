using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System.Windows.Documents;

namespace ChatGptApiClientV2.Tools;

public class WolframAlpha : IToolCollection
{
    public List<IToolFunction> Funcs { get; } =
    [
        new WolframAlphaFunc()
    ];
    public string DisplayName => "Wolfram|Alpha";
}
public class WolframAlphaFunc : IToolFunction
{
    public class Args
    {
        [System.ComponentModel.Description("Your question. The query must be in English. Make your question clear and accurate, preventing any possible confusion. For example, when talking about year, using 'the year of 2023', instead of just a plain number '2023'.")]
        public string Query { get; set; } = "";
    }
    public string Description => 
        """
        Let WolframAlpha answer your questions. It can provide solutions on math, science, history, language, and many other fields.
        It can only answer one question at a time. If you have a complex question, think it step by step, making it several simpler quetions and ask them one by one.
        """;
    public string Name => "wolfram_alpha";
    public Type ArgsType => typeof(Args);

    private readonly HttpClient httpClient = new();
    public async Task<ToolMessage> Action(SystemState state, string argstr)
    {
        var msgContents = new List<IMessage.TextContent>();
        var msg = new ToolMessage { Content = msgContents };
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
                msgContents[0].Text += $"Failed to parse arguments for WolframAlpha. The args are: {argstr}\n\n";
                return msg;
            }
            args = parsedArgs;
        }
        catch (JsonSerializationException e)
        {
            msgContents[0].Text += $"Failed to parse arguments for WolframAlpha. The args are: {argstr}\n\n";
            msgContents[0].Text += $"Exception: {e.Message}\n\n";
            return msg;
        }

        var parameters = new Dictionary<string, string?>
        {
            {"input",   args.Query },
            {"format",  "plaintext"},
            {"output",  "JSON"},
            {"appid",   state.Config.WolframAlphaAppid}
        };

        const string serviceUrl = "https://api.wolframalpha.com/v2/query";

        var query = string.Join("&", 
            from p in parameters 
            where p.Value is not null 
            select $"{p.Key}={System.Net.WebUtility.UrlEncode(p.Value)}");

        state.NewMessage(RoleType.Tool);
        state.StreamText($"Wolfram|Alpha: {args.Query}\n\n");

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"{serviceUrl}?{query}")
        };

        state.NetStatus.Status = NetStatus.StatusEnum.Sending;
        var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        state.NetStatus.Status = NetStatus.StatusEnum.Receiving;

        if (!response.IsSuccessStatusCode)
        {
            await using var errorResponseStream = await response.Content.ReadAsStreamAsync();
            using var errorReader = new StreamReader(errorResponseStream);
            var errorResponse = await errorReader.ReadToEndAsync();
            msgContents[0].Text += $"Error: {errorResponse}\n\n";
            state.NetStatus.Status = NetStatus.StatusEnum.Idle;
            return msg;
        }

        await using var responseStream = await response.Content.ReadAsStreamAsync();
        using var reader = new StreamReader(responseStream);
        var responseStr = await reader.ReadToEndAsync();
        var responseJson = JToken.Parse(responseStr);
        var queryresult = responseJson["queryresult"];
        var success = queryresult?["success"]?.Value<bool>();
        if (success != true)
        {
            msgContents[0].Text += $"Error: {responseStr}\n\n";
            state.NetStatus.Status = NetStatus.StatusEnum.Idle;
            return msg;
        }
        var pods = queryresult?["pods"];
        if (pods is null)
        {
            msgContents[0].Text += $"Error: {responseStr}\n\n";
            state.NetStatus.Status = NetStatus.StatusEnum.Idle;
            return msg;
        }

        msgContents[0].Text += $"Results: {pods.ToString(Formatting.Indented)}\n\n";
        state.NetStatus.Status = NetStatus.StatusEnum.Idle;
        msg.Hidden = true; // Hide success results from user
        return msg;
    }

    public IEnumerable<Block> GetToolcallMessage(SystemState state, string argstr, string toolcallId)
    {
        var argsJson = JToken.Parse(argstr);
        var argsReader = new JTokenReader(argsJson);
        var argsSerializer = new JsonSerializer();
        Args args;
        try
        {
            var parsedArgs = argsSerializer.Deserialize<Args>(argsReader);
            if (parsedArgs is null)
            {
                return [new Paragraph(new Run("Asking Wolfram|Alpha..."))];
            }
            args = parsedArgs;
        }
        catch (JsonSerializationException)
        {
            return [new Paragraph(new Run("Asking Wolfram|Alpha..."))];
        }

        List<string> stickers = [
            "艾尔海森-动动脑.png",
            "本-疯狂计算.png",
            "丹恒 思考.png",
            "姬子 计算.png",
            "卡维-挠头.png"
        ];

        var floater = Utils.CreateStickerFloater(stickers, toolcallId);

        var paragraph = new Paragraph();
        paragraph.Inlines.Add(floater);
        paragraph.Inlines.Add(new Run("Asking Wolfram|Alpha:"));
        paragraph.Inlines.Add(new LineBreak());
        paragraph.Inlines.Add(new LineBreak());
        paragraph.Inlines.Add(new Run($"{args.Query}"));
        paragraph.Inlines.Add(new LineBreak());

        return [paragraph];
    }
}