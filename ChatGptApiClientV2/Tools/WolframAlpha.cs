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
    public class WolframAlpha : IToolCollection
    {
        public List<IToolFunction> Funcs { get; } =
        [
            new WolframAlphaFunc(),
        ];
        public string DisplayName => "Wolfram|Alpha";
    }
    public class WolframAlphaFunc : IToolFunction
    {
        public class Args
        {
            [System.ComponentModel.Description(@"Your question. The query must be in English. Make your question clear and accurate, preventing any possible confusion. For example, when talking about year, using 'the year of 2023', instead of just a plain number '2023'.")]
            public string Query { get; set; } = "";
        }
        public string Description => @"Let WolframAlpha answer your questions. It can provide solutions on math, science, history, language, and many other fields.
It can only answer one question at a time. If you have a complex question, think it step by step, making it several simpler quetions and ask them one by one.";
        public string Name => "wolfram_alpha";
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
                {"appid",   config.WolframAlphaAppid},
            };

            const string serviceURL = @"https://api.wolframalpha.com/v2/query";

            var query = string.Join("&", 
                from p in parameters 
                where p.Value is not null 
                select $"{p.Key}={System.Net.WebUtility.UrlEncode(p.Value)}");

            Console.WriteLine($"Asking Wolfram|Alpha: {args.Query}");

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{serviceURL}?{query}"),
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
            var queryresult = responseJson["queryresult"];
            var success = queryresult?["success"]?.Value<bool>();
            if (success != true)
            {
                msgContents[0].Text += $"Error: {responseStr}\n\n";
                netstatus.Status = NetStatusType.StatusEnum.Idle;
                return msg;
            }
            var pods = queryresult?["pods"];
            if (pods is null)
            {
                msgContents[0].Text += $"Error: {responseStr}\n\n";
                netstatus.Status = NetStatusType.StatusEnum.Idle;
                return msg;
            }

            msgContents[0].Text += $"Results: {pods.ToString(Formatting.Indented)}\n\n";
            netstatus.Status = NetStatusType.StatusEnum.Idle;
            msg.Hidden = true; // Hide success results from user
            return msg;
        }
    }
}
