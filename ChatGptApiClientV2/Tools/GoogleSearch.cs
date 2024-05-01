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
using PuppeteerSharp;
using System.Windows.Documents;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.ComponentModel;
using static ChatGptApiClientV2.Tools.IToolFunction;
using System.Threading;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace ChatGptApiClientV2.Tools;

public class GoogleSearch : IToolCollection
{
    public List<IToolFunction> Funcs =>
    [
        new GoogleSearchFunc(),
        new WebsiteAccessFunc(),
        new WebsiteNextPageFunc()
    ];

    public string DisplayName => "Google 搜索";
}

public class GoogleSearchFunc : IToolFunction
{
    public class Args
    {
        [Description("The search query.")] public string Query { get; set; } = "";

        [Description(
            "The index of the first result to return. The default number of results per page is 10, so StartIndex=11 would start at the top of the second page of results. Default to be 1, i.e. the first page of results.")]
        public uint StartIndex { get; set; } = 1;
    }

    public string Description =>
        "Look for info on the Internet using Google search. If you need detailed info from searched results, feel free to call 'website_access' to access links in results.";

    public string Name => "google_search";
    public Type ArgsType => typeof(Args);

    private static readonly HttpClient HttpClient;

    static GoogleSearchFunc()
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
                msgContents[0].Text += $"Failed to parse arguments for Google search. The args are: {argstr}\n\n";
                return result;
            }

            args = parsedArgs;
        }
        catch (JsonSerializationException e)
        {
            msgContents[0].Text += $"Failed to parse arguments for Google search. The args are: {argstr}\n\n";
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
            { "cx", state.Config.GoogleSearchEngineID },
            { "key", state.Config.GoogleSearchAPIKey },
            { "q", args.Query },
            { "start", args.StartIndex.ToString() },
            { "fields", "queries(request(count,startIndex),nextPage(count,startIndex)),items(title,link,snippet)" }
        };

        const string serviceUrl = "https://www.googleapis.com/customsearch/v1";

        var query = string.Join("&",
            from p in parameters
            where p.Value is not null
            select $"{p.Key}={System.Net.WebUtility.UrlEncode(p.Value)}");

        state.NewMessage(RoleType.Tool, sessionId);
        state.StreamText($"Google Searching: {args.Query}\n\n", sessionId);

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"{serviceUrl}?{query}")
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
        msgContents[0].Text += $"Results: {responseStr}\n\n";
        msg.Hidden = true; // Hide success results from user
        return result;
    }

    public IEnumerable<Block> GetToolcallMessage(SystemState state, Guid sessionId, string argstr, string toolcallId)
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
                return [new Paragraph(new Run("Google 搜索..."))];
            }

            args = parsedArgs;
        }
        catch (JsonSerializationException)
        {
            return [new Paragraph(new Run("Google 搜索..."))];
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
        paragraph.Inlines.Add(new Run("Google 搜索:"));
        paragraph.Inlines.Add(new LineBreak());
        paragraph.Inlines.Add(new LineBreak());
        paragraph.Inlines.Add(new Run($"{args.Query}"));
        paragraph.Inlines.Add(new LineBreak());

        return [paragraph];
    }
}

public class WebsiteAccessFunc : IToolFunction
{
    private const int ContentPageLimit = 4096;

    public class Args
    {
        [Description("The URL of the website to access.")]
        public string Url { get; set; } = "";
    }

    public string Description =>
        "Access a website. Content will be truncated if it's too long. You can call 'website_nextpage' to get the remaining content if needed.";

    public string Name => "website_access";
    public Type ArgsType => typeof(Args);

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
                msgContents[0].Text += $"Failed to parse arguments for Website Access. The args are: {argstr}\n\n";
                return result;
            }

            args = parsedArgs;
        }
        catch (JsonSerializationException e)
        {
            msgContents[0].Text += $"Failed to parse arguments for Website Access. The args are: {argstr}\n\n";
            msgContents[0].Text += $"Exception: {e.Message}\n\n";
            return result;
        }

        state.NewMessage(RoleType.Tool, sessionId);
        state.StreamText($"Accessing: {args.Url}\n\n", sessionId);
        state.NetStatus.Status = NetStatus.StatusEnum.Receiving;

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var installed = await new BrowserFetcher(new BrowserFetcherOptions
            {
                Browser = SupportedBrowser.Chrome,
                Path = "./browser"
            }).DownloadAsync();
            cancellationToken.ThrowIfCancellationRequested();

            var browser = await Puppeteer.LaunchAsync(
                new LaunchOptions
                {
                    Headless = true,
                    Browser = installed.Browser,
                    ExecutablePath = installed.GetExecutablePath()
                });
            cancellationToken.ThrowIfCancellationRequested();

            var page = await browser.NewPageAsync();
            cancellationToken.ThrowIfCancellationRequested();

            await page.GoToAsync(args.Url);
            cancellationToken.ThrowIfCancellationRequested();

            // Ensure at least one element is loaded
            // solves the error "Execution context was destroyed, most likely because of a navigation."
            // which happens when the url will redirect to another page
            // such as when accessing the zh-hans version of wikipedia (which can redirect to the zh-cn version)
            await page.WaitForSelectorAsync("*");
            cancellationToken.ThrowIfCancellationRequested();

            var pageHeaderHandle = await page.QuerySelectorAsync("*");
            cancellationToken.ThrowIfCancellationRequested();

            var innerTextHandle = await pageHeaderHandle.GetPropertyAsync("innerText");
            cancellationToken.ThrowIfCancellationRequested();

            var innerText = await innerTextHandle.JsonValueAsync();
            cancellationToken.ThrowIfCancellationRequested();

            var content = innerText.ToString() ?? "";
            if (content.Length > ContentPageLimit)
            {
                var contentRemained = content[ContentPageLimit..];
                state.SessionDict[sessionId]!.PluginData[$"WebsiteRemained_{args.Url}"] = [contentRemained];
                content = content[..ContentPageLimit];
                content +=
                    $"\n\n[Content truncated due to length limit; {contentRemained.Length} characters remained. Call website_nextpage if you need the remaining content.]";
            }

            msgContents[0].Text += $"Content: {content}";
            await browser.CloseAsync();
        }
        catch (Exception e)
        {
            msgContents[0].Text += $"Error: {e.Message}\n\n";
            return result;
        }

        msg.Hidden = true; // Hide success results from user
        return result;
    }

    public IEnumerable<Block> GetToolcallMessage(SystemState state, Guid sessionId, string argstr, string toolcallId)
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
                return [new Paragraph(new Run("访问网站..."))];
            }

            args = parsedArgs;
        }
        catch (JsonSerializationException)
        {
            return [new Paragraph(new Run("访问网站..."))];
        }

        List<string> stickers =
        [
            "达达利亚-冲浪.png",
            "优菈-让我看看.png",
            "菲米尼-爱好.png",
            "柯莱-学习时间.png",
            "帕姆_好奇.png"
        ];

        var floater = Utils.CreateStickerFloater(stickers, toolcallId);

        var paragraph = new Paragraph();
        paragraph.Inlines.Add(floater);
        paragraph.Inlines.Add(new Run("访问网站:"));
        paragraph.Inlines.Add(new LineBreak());
        paragraph.Inlines.Add(new LineBreak());
        var link = new Hyperlink(new Run($"{args.Url}"))
        {
            NavigateUri = new Uri(args.Url),
            Command = new RelayCommand(() => Process.Start(new ProcessStartInfo(args.Url) { UseShellExecute = true }))
        };
        paragraph.Inlines.Add(link);
        paragraph.Inlines.Add(new LineBreak());

        return [paragraph];
    }
}

public class WebsiteNextPageFunc : IToolFunction
{
    private const int ContentPageLimit = 4096;

    public string Description =>
        "Get the next page of content from the past call to website_access. The content will be truncated if it's too long. If so, call website_nextpage again to get the remaining content when needed.";

    public string Name => "website_nextpage";

    public class Args
    {
        [Description(
            "The URL of the website to access. Must be a URL that has been called with website_access previously.")]
        public string Url { get; set; } = "";
    }

    public Type ArgsType => typeof(Args);

    public Task<ToolResult> Action(SystemState state, Guid sessionId, string toolcallId, string argstr,
        CancellationToken cancellationToken = default)
    {
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
                msgContents[0].Text += $"Failed to parse arguments for Website Access. The args are: {argstr}\n\n";
                return Task.FromResult(result);
            }

            args = parsedArgs;
        }
        catch (JsonSerializationException e)
        {
            msgContents[0].Text += $"Failed to parse arguments for Website Access. The args are: {argstr}\n\n";
            msgContents[0].Text += $"Exception: {e.Message}\n\n";
            return Task.FromResult(result);
        }

        state.NewMessage(RoleType.Tool, sessionId);
        state.StreamText("Accessing the next page...\n\n", sessionId);

        string? content;
        try
        {
            content = state.SessionDict[sessionId]!.PluginData[$"WebsiteRemained_{args.Url}"][0];
        }
        catch (KeyNotFoundException)
        {
            msgContents[0].Text +=
                $"Error: {args.Url} has not been visited. You must first call website_access to get the content.\n\n";
            return Task.FromResult(result);
        }

        if (string.IsNullOrEmpty(content))
        {
            msgContents[0].Text += $"Error: No content remained for {args.Url}\n\n";
            return Task.FromResult(result);
        }

        if (content.Length > ContentPageLimit)
        {
            var contentRemained = content[ContentPageLimit..];
            state.SessionDict[sessionId]!.PluginData[$"WebsiteRemained_{args.Url}"][0] = contentRemained;
            content = content[..ContentPageLimit];
            content +=
                $"\n\n[Content truncated due to length limit; {contentRemained.Length} characters remained. Call website_nextpage if you need the remaining content.]";
        }
        else
        {
            state.SessionDict[sessionId]!.PluginData[$"WebsiteRemained_{args.Url}"][0] = "";
        }

        msgContents[0].Text += $"Content: {content}";
        msg.Hidden = true; // Hide success results from user
        return Task.FromResult(result);
    }

    public IEnumerable<Block> GetToolcallMessage(SystemState state, Guid sessionId, string argstr, string toolcallId)
    {
        List<string> stickers =
        [
            "紬_急急急.png",
            "柯莱-学习时间.png",
            "夏洛蒂-您继续.png",
            "瑶瑶-急急急.png",
            "优菈-让我看看.png"
        ];

        var argsJson = JToken.Parse(argstr);
        var argsReader = new JTokenReader(argsJson);
        var argsSerializer = new JsonSerializer();
        Args args;
        try
        {
            var parsedArgs = argsSerializer.Deserialize<Args>(argsReader);
            if (parsedArgs is null)
            {
                return [new Paragraph(new Run("访问下一页..."))];
            }

            args = parsedArgs;
        }
        catch (JsonSerializationException)
        {
            return [new Paragraph(new Run("访问下一页..."))];
        }

        var floater = Utils.CreateStickerFloater(stickers, toolcallId);

        var paragraph = new Paragraph();
        paragraph.Inlines.Add(floater);
        paragraph.Inlines.Add(new Run("访问下一页:"));
        paragraph.Inlines.Add(new LineBreak());
        paragraph.Inlines.Add(new LineBreak());
        var link = new Hyperlink(new Run($"{args.Url}"))
        {
            NavigateUri = new Uri(args.Url),
            Command = new RelayCommand(() => Process.Start(new ProcessStartInfo(args.Url) { UseShellExecute = true }))
        };
        paragraph.Inlines.Add(link);
        paragraph.Inlines.Add(new LineBreak());

        return [paragraph];
    }
}