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

namespace ChatGptApiClientV2.Tools;

public class GoogleSearch : IToolCollection
{
    public List<IToolFunction> Funcs { get; } =
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
        [System.ComponentModel.Description("The search query.")]
        public string Query { get; set; } = "";
        [System.ComponentModel.Description("The index of the first result to return. The default number of results per page is 10, so StartIndex=11 would start at the top of the second page of results. Default to be 1, i.e. the first page of results.")]
        public uint StartIndex { get; set; } = 1;
    }
    public string Description => "Look for info on the Internet using Google search. If you need detailed info from searched results, feel free to call 'website_access' to access links in results.";
    public string Name => "google_search";
    public Type ArgsType => typeof(Args);

    private readonly HttpClient httpClient = new();
    public async Task<ToolMessage> Action(SystemState state, string argstr)
    {
        var msgContents = new List<IMessage.TextContent>();
        var msg = new ToolMessage { Content = msgContents};
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
                return msg;
            }
            args = parsedArgs;
        }
        catch (JsonSerializationException e)
        {
            msgContents[0].Text += $"Failed to parse arguments for Google search. The args are: {argstr}\n\n";
            msgContents[0].Text += $"Exception: {e.Message}\n\n";
            return msg;
        }

        args.Query = args.Query.Trim();
        if (string.IsNullOrEmpty(args.Query))
        {
            msgContents[0].Text += "Error: Empty query.\n\n";
            return msg;
        }

        var parameters = new Dictionary<string, string?>
        {
            {"cx",              state.Config.GoogleSearchEngineID},
            {"key",             state.Config.GoogleSearchAPIKey},
            {"q",               args.Query },
            {"start",           args.StartIndex.ToString()},
            {"fields",          "queries(request(count,startIndex),nextPage(count,startIndex)),items(title,link,snippet)"}
        };

        const string serviceUrl = "https://www.googleapis.com/customsearch/v1";

        var query = string.Join("&", 
            from p in parameters 
            where p.Value is not null 
            select $"{p.Key}={System.Net.WebUtility.UrlEncode(p.Value)}");

        state.NewMessage(RoleType.Tool);
        state.StreamText($"Google Searching: {args.Query}\n\n");

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
        msgContents[0].Text += $"Results: {responseStr}\n\n";
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
                return [new Paragraph(new Run("Google 搜索..."))];
            }
            args = parsedArgs;
        }
        catch (JsonSerializationException)
        {
            return [new Paragraph(new Run("Google 搜索..."))];
        }
        args.Query = args.Query.Trim();

        List<string> stickers = [
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
    public static readonly Dictionary<string, string> ContentRemained = [];
    private const int ContentPageLimit = 2048;
    public class Args
    {
        [System.ComponentModel.Description("The URL of the website to access.")]
        public string Url { get; set; } = "";
    }
    public string Description => "Access a website. Content will be truncated if it's too long. You can call 'website_nextpage' to get the remaining content if needed.";
    public string Name => "website_access";
    public Type ArgsType => typeof(Args);
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
                msgContents[0].Text += $"Failed to parse arguments for Website Access. The args are: {argstr}\n\n";
                return msg;
            }
            args = parsedArgs;
        }
        catch (JsonSerializationException e)
        {
            msgContents[0].Text += $"Failed to parse arguments for Website Access. The args are: {argstr}\n\n";
            msgContents[0].Text += $"Exception: {e.Message}\n\n";
            return msg;
        }

        state.NewMessage(RoleType.Tool);
        state.StreamText($"Accessing: {args.Url}\n\n");
        state.NetStatus.Status = NetStatus.StatusEnum.Receiving;

        try
        {
            var installed = await new BrowserFetcher(new BrowserFetcherOptions
            {
                Browser = SupportedBrowser.Chrome,
                Path = "./browser"
            }).DownloadAsync();
            var browser = await Puppeteer.LaunchAsync(
                new LaunchOptions
                {
                    Headless = true,
                    Browser = installed.Browser,
                    ExecutablePath = installed.GetExecutablePath()
                });
            var page = await browser.NewPageAsync();
            await page.GoToAsync(args.Url);
            var pageHeaderHandle = await page.QuerySelectorAsync("*");
            var innerTextHandle = await pageHeaderHandle.GetPropertyAsync("innerText");
            var innerText = await innerTextHandle.JsonValueAsync();
            var content = innerText.ToString() ?? "";
            if (content.Length > ContentPageLimit)
            {
                ContentRemained[args.Url] = content[ContentPageLimit..];
                content = content[..ContentPageLimit];
                content += $"\n\n[Content truncated due to length limit; {ContentRemained[args.Url].Length} characters remained. Call website_nextpage if you need the remaining content.]";
            }
            msgContents[0].Text += $"Content: {content}";
            await browser.CloseAsync();
        }
        catch(Exception e)
        {
            msgContents[0].Text += $"Error: {e.Message}\n\n";
            state.NetStatus.Status = NetStatus.StatusEnum.Idle;
            return msg;
        }

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
                return [new Paragraph(new Run("访问网站..."))];
            }
            args = parsedArgs;
        }
        catch (JsonSerializationException)
        {
            return [new Paragraph(new Run("访问网站..."))];
        }

        List<string> stickers = [
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
    private const int ContentPageLimit = 2048;
    public string Description => "Get the next page of content from the past call to website_access. The content will be truncated if it's too long. If so, call website_nextpage again to get the remaining content when needed.";

    public string Name => "website_nextpage";
    public class Args
    {
        [System.ComponentModel.Description("The URL of the website to access. Must be a URL that has been called with website_access previously.")]
        public string Url { get; set; } = "";
    }
    public Type ArgsType => typeof(Args);

    public Task<ToolMessage> Action(SystemState state, string argstr)
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
                msgContents[0].Text += $"Failed to parse arguments for Website Access. The args are: {argstr}\n\n";
                return Task.FromResult(msg);
            }
            args = parsedArgs;
        }
        catch (JsonSerializationException e)
        {
            msgContents[0].Text += $"Failed to parse arguments for Website Access. The args are: {argstr}\n\n";
            msgContents[0].Text += $"Exception: {e.Message}\n\n";
            return Task.FromResult(msg);
        }

        state.NewMessage(RoleType.Tool);
        state.StreamText("Accessing the next page...\n\n");

        var foundCache = WebsiteAccessFunc.ContentRemained.TryGetValue(args.Url, out var content);
        if (!foundCache)
        {
            msgContents[0].Text += $"Error: {args.Url} has not been visited. You must first call website_access to get the content.\n\n";
            return Task.FromResult(msg);
        }

        if (string.IsNullOrEmpty(content))
        {
            msgContents[0].Text += $"Error: No content remained for {args.Url}\n\n";
            return Task.FromResult(msg);
        }

        if (content.Length > ContentPageLimit)
        {
            WebsiteAccessFunc.ContentRemained[args.Url] = content[ContentPageLimit..];
            content = content[..ContentPageLimit];
            content += $"\n\n[Content truncated due to length limit; {WebsiteAccessFunc.ContentRemained[args.Url].Length} characters remained. Call website_nextpage if you need the remaining content.]";
        }
        else
        {
            WebsiteAccessFunc.ContentRemained[args.Url] = "";
        }
        msgContents[0].Text += $"Content: {content}";
        msg.Hidden = true; // Hide success results from user
        return Task.FromResult(msg);
    }

    public IEnumerable<Block> GetToolcallMessage(SystemState state, string argstr, string toolcallId)
    {
        List<string> stickers = [
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