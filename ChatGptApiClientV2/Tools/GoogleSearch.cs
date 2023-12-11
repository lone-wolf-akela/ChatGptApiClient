using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using static ChatGptApiClientV2.MainWindow;
using System.Net.Http;
using System.IO;
using PuppeteerSharp;

namespace ChatGptApiClientV2.Tools
{
    public class GoogleSearch : IToolCollection
    {
        public List<IToolFunction> Funcs { get; } =
        [
            new GoogleSearchFunc(),
            new WebsiteAccessFunc(),
        ];
        public string DisplayName => "Google 搜索";
    }
    public class GoogleSearchFunc : IToolFunction
    {
        public class Args
        {
            [System.ComponentModel.Description(@"The search query.")]
            public string? Query { get; set; } = null;
            [System.ComponentModel.Description(@"A phrase that all documents in the search results must contain.")]
            public string? ExactTerms { get; set; } = null;
            [System.ComponentModel.Description(@"A word or phrase that should not appear in any documents in the search results.")]
            public string? ExcludeTerms { get; set; } = null;
            [System.ComponentModel.Description(@"The index of the first result to return. The default number of results per page is 10, so StartIndex=11 would start at the top of the second page of results. Default to be 1, i.e. the first page of results.")]
            public uint StartIndex { get; set; } = 1;
        }
        public string Description => @"Look for info on the Internet using Google search. All query or terms fields support non-ascii characters. You MUST NOT use unicode escape sequence such as '\u1234' in them.";
        public string Name => "google_seach";
        public Type ArgsType => typeof(Args);

        private readonly HttpClient httpClient = new();
        public async Task<ToolMessage> Action(ConfigType config, NetStatusType netstatus, string argstr)
        {
            var msgContents = new List<IMessage.TextContent>();
            var msg = new ToolMessage { Content = msgContents, Hidden = true };
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

            var parameters = new Dictionary<string, string?>
            {
                {"cx",              config.GoogleSearchEngineID},
                {"key",             config.GoogleSearchAPIKey},
                {"q",               args.Query },
                {"exactTerms",      args.ExactTerms},
                {"excludeTerms",    args.ExcludeTerms},
                {"start",           args.StartIndex.ToString()},
                {"fields",          @"queries(request(count,startIndex),nextPage(count,startIndex)),items(title,link,snippet)"},
            };

            const string serviceURL = @"https://www.googleapis.com/customsearch/v1";

            var query = string.Join("&", 
                from p in parameters 
                where p.Value is not null 
                select $"{p.Key}={System.Net.WebUtility.UrlEncode(p.Value)}");

            Console.WriteLine("Searching: ");
            Console.WriteLine("\tQuery: " + (args.Query ?? ""));
            Console.WriteLine("\tExactTerms: " + (args.ExactTerms ?? ""));
            Console.WriteLine("\tExcludeTerms: " + (args.ExcludeTerms ?? ""));

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{serviceURL}?{query}")
            };

            netstatus.Status = NetStatusType.StatusEnum.Sending;
            var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            netstatus.Status = NetStatusType.StatusEnum.Receiving;

            if (!response.IsSuccessStatusCode)
            {
                using var errorResponseStream = await response.Content.ReadAsStreamAsync();
                using var errorReader = new StreamReader(errorResponseStream);
                var errorResponse = await errorReader.ReadToEndAsync();
                msgContents[0].Text += $"Error: {errorResponse}\n\n";
                netstatus.Status = NetStatusType.StatusEnum.Idle;
                return msg;
            }

            using var responseStream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(responseStream);
            string responseStr = await reader.ReadToEndAsync();
            msgContents[0].Text += $"Results: {responseStr}\n\n";
            netstatus.Status = NetStatusType.StatusEnum.Idle;
            return msg;
        }
    }
    public class WebsiteAccessFunc : IToolFunction
    {
        public class Args
        {
            [System.ComponentModel.Description(@"The URL of the website to access.")]
            public string URL { get; set; } = "";
        }
        public string Description => "Access a website.";
        public string Name => "website_access";
        public Type ArgsType => typeof(Args);
        public async Task<ToolMessage> Action(ConfigType config, NetStatusType netstatus, string argstr)
        {
            var msgContents = new List<IMessage.TextContent>();
            var msg = new ToolMessage { Content = msgContents, Hidden = true };
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

            Console.WriteLine($"Accessing: {args.URL}");
            netstatus.Status = NetStatusType.StatusEnum.Receiving;

            var installed = await new BrowserFetcher(SupportedBrowser.Firefox).DownloadAsync();
            var browser = await Puppeteer.LaunchAsync(
                new LaunchOptions
                {
                    Headless = true,
                    Browser = installed.Browser,
                    ExecutablePath = installed.GetExecutablePath()
                });
            var page = await browser.NewPageAsync();
            await page.GoToAsync(args.URL);
            var pageHeaderHandle = await page.QuerySelectorAsync("*");
            var innerTextHandle = await pageHeaderHandle.GetPropertyAsync("innerText");
            var innerText = await innerTextHandle.JsonValueAsync();
            var content = innerText.ToString() ?? "";
            // limite content characters
            const int contentLimit = 2048;
            if (content.Length > contentLimit)
            {
                content = content[..contentLimit];
                content += "\n\n[Content truncated]";
            }
            msgContents[0].Text += $"Content: {content}";
            await browser.CloseAsync();

            netstatus.Status = NetStatusType.StatusEnum.Idle;
            return msg;
        }
    }
}
