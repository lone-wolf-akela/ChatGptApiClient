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
    public class BingSearch : IToolCollection
    {
        public List<IToolFunction> Funcs { get; } =
        [
            new BingSearchFunc(),
            new WebsiteAccessFunc(),
            new WebsiteNextPageFunc(),
        ];
        public string DisplayName => "Bing 搜索";
    }
    public class BingSearchFunc : IToolFunction
    {
        public class Args
        {
            [System.ComponentModel.Description(@"The search query. Non-ASCII characters are supported. You MUST NOT use unicode escape sequence such as '\uxxxx' in the query.")]
            public string Query { get; set; } = "";
            [System.ComponentModel.Description(@"The offset of the index of the first result to return. The default number of results per page is 10, so StartOffset=10 would start at the top of the second page of results. Default to be 0, i.e. the first page of results.")]
            public uint StartOffset { get; set; } = 0;
        }
        public string Description => @"Look for info on the Internet using Bing search. If you need detailed info from searched results, feel free to call 'website_access' to access links in results.";
        public string Name => "bing_search";
        public Type ArgsType => typeof(Args);

        private readonly HttpClient httpClient = new();
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
                    msgContents[0].Text += $"Failed to parse arguments for Bing search. The args are: {argstr}\n\n";
                    return msg;
                }
                args = parsedArgs;
            }
            catch (JsonSerializationException e)
            {
                msgContents[0].Text += $"Failed to parse arguments for Bing search. The args are: {argstr}\n\n";
                msgContents[0].Text += $"Exception: {e.Message}\n\n";
                return msg;
            }

            var parameters = new Dictionary<string, string?>
            {
                {"q",               args.Query },
                {"offset",          args.StartOffset.ToString()},
                {"responseFilter",  "Webpages"},
            };

            const string serviceURL = @"https://api.bing.microsoft.com/v7.0/search";

            var query = string.Join("&", 
                from p in parameters 
                where p.Value is not null 
                select $"{p.Key}={System.Net.WebUtility.UrlEncode(p.Value)}");

            Console.WriteLine($"Bing Searching: {args.Query}");

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{serviceURL}?{query}"),
                Headers =
                {
                    { "Ocp-Apim-Subscription-Key", config.BingSearchAPIKey },
                },
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
            var responseJson = JToken.Parse(responseStr);
            var responsePages = responseJson?["webPages"]?["value"];
            if (responsePages is null)
            {
                msgContents[0].Text += $"Error: {responseStr}\n\n";
                netstatus.Status = NetStatusType.StatusEnum.Idle;
                return msg;
            }

            JObject filteredResponse = [];
            JArray filteredPages = [];
            foreach(var page in responsePages)
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
            netstatus.Status = NetStatusType.StatusEnum.Idle;
            msg.Hidden = true; // Hide success results from user
            return msg;
        }
    }
}
