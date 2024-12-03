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

using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System.Windows.Documents;
using System.ComponentModel;
using static ChatGptApiClientV2.Tools.IToolFunction;
using System.Threading;
using System.ComponentModel.DataAnnotations;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace ChatGptApiClientV2.Tools;

public class BingSearch : IToolCollection
{
    public List<IToolFunction> Funcs =>
    [
        new BingSearchFunc(),
        new WebsiteAccessFunc(),
        new WebsiteNextPageFunc()
    ];

    public string DisplayName => "Bing 搜索";
}

public class BingSearchFunc : IToolFunction
{
    public class Args
    {
        [Description("The search query.")]
        [Required]
        public string Query { get; set; } = "";

        [Description(
            "The offset of the index of the first result to return. The default number of results per page is 10, so StartOffset=10 would start at the top of the second page of results. Default to be 0, i.e. the first page of results.")]
        public uint StartOffset { get; set; } = 0;
    }

    public string Description =>
        "Look for info on the Internet using Bing search. If you need detailed info from searched results, feel free to call 'website_access' to access links in results.";

    public string Name => "bing_search";
    public Type ArgsType => typeof(Args);

    private static readonly HttpClient HttpClient;

    static BingSearchFunc()
    {
        var httpClientHandler = new HttpClientHandler
        {
            AutomaticDecompression = System.Net.DecompressionMethods.All
        };
        HttpClient = new HttpClient(httpClientHandler);
    }

    public async Task<ToolResult> Action(SystemState state, Guid sessionId, string toolcallId, string argstr,
        CancellationToken cancellationToken = default)
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
                msgContents[0].Text += $"Failed to parse arguments for Bing search. The args are: {argstr}\n\n";
                return result;
            }

            args = parsedArgs;
        }
        catch (JsonSerializationException e)
        {
            msgContents[0].Text += $"Failed to parse arguments for Bing search. The args are: {argstr}\n\n";
            msgContents[0].Text += $"Exception: {e.Message}\n\n";
            return result;
        }

        args.Query = args.Query.Trim();
        if (string.IsNullOrEmpty(args.Query))
        {
            msgContents[0].Text += "Error: Empty query.\n\n";
            return result;
        }

        var parameters = new Dictionary<string, string?>
        {
            { "q", args.Query },
            { "offset", args.StartOffset.ToString() },
            { "responseFilter", "Webpages" }
        };

        const string serviceUrl = "https://api.bing.microsoft.com/v7.0/search";

        var query = string.Join("&",
            from p in parameters
            where p.Value is not null
            select $"{p.Key}={System.Net.WebUtility.UrlEncode(p.Value)}");

        state.NewMessage(RoleType.Tool, sessionId);
        state.StreamText($"Bing Searching: {args.Query}\n\n", sessionId);

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"{serviceUrl}?{query}"),
            Headers =
            {
                { "Ocp-Apim-Subscription-Key", state.Config.BingSearchAPIKey }
            }
        };

        state.NetStatus.Status = NetStatus.StatusEnum.Sending;
        var response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        state.NetStatus.Status = NetStatus.StatusEnum.Receiving;

        if (!response.IsSuccessStatusCode)
        {
            await using var errorResponseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var errorReader = new StreamReader(errorResponseStream);
            var errorResponse = await errorReader.ReadToEndAsync(cancellationToken);
            msgContents[0].Text += $"Error: {errorResponse}\n\n";
            return result;
        }

        await using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var reader = new StreamReader(responseStream);
        var responseStr = await reader.ReadToEndAsync(cancellationToken);
        var responseJson = JToken.Parse(responseStr);
        var responsePages = responseJson["webPages"]?["value"];
        if (responsePages is null)
        {
            msgContents[0].Text += $"Error: {responseStr}\n\n";
            return result;
        }

        JObject filteredResponse = [];
        JArray filteredPages = [];
        foreach (var page in responsePages)
        {
            JObject filteredPage = new()
            {
                ["name"] = page["name"],
                ["url"] = page["url"],
                ["snippet"] = page["snippet"]
            };
            filteredPages.Add(filteredPage);
        }

        filteredResponse["webPages"] = filteredPages;

        msgContents[0].Text += $"Results: {filteredResponse.ToString(Formatting.Indented)}\n\n";
        msg.Hidden = true; // Hide success results from user
        return result;
    }

    public ToolCallMessage GetToolcallMessage(SystemState state, Guid sessionId, string argstr, string toolcallId)
    {
        Args args;
        try
        {
            var parsedArgs = JsonConvert.DeserializeObject<Args>(argstr);
            if (parsedArgs is null)
            {
                return new ToolCallMessage("Bing 搜索...");
            }

            args = parsedArgs;
        }
        catch (JsonSerializationException)
        {
            return new ToolCallMessage("Bing 搜索...");
        }

        args.Query = args.Query.Trim();

        List<string> stickers =
        [
            "非常疑惑.png",
            "菲谢尔-这是什么？.png",
            "刻晴-疑问.png",
            "雷电将军-纳闷.png",
            "帕姆_好奇.png",
            "帕姆_疑惑.png",
            "砂糖-疑问.png",
            "申鹤-疑惑.png",
            "烟绯-疑惑.png"
        ];

        var floater = Utils.CreateStickerFloater(stickers, toolcallId);

        var paragraph = new Paragraph();
        paragraph.Inlines.Add(floater);
        paragraph.Inlines.Add(new Run("Bing 搜索:"));
        paragraph.Inlines.Add(new LineBreak());
        paragraph.Inlines.Add(new LineBreak());
        paragraph.Inlines.Add(new Run($"{args.Query}"));
        paragraph.Inlines.Add(new LineBreak());

        return new ToolCallMessage($"Bing 搜索:\n{args.Query}", [paragraph]);
    }
}