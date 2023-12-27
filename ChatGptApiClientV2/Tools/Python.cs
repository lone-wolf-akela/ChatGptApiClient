using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Documents;
using Markdig;
using Markdig.Wpf.ColorCode;
using System.ComponentModel;

namespace ChatGptApiClientV2.Tools;

public class PythonTool : IToolCollection
{
    public List<IToolFunction> Funcs =>
    [
        new PythonFunc()
    ];

    public string DisplayName => "Python 解释器";
}

public class PythonFunc : IToolFunction
{
    public string Description =>
        """
        You can execute a python script. 
        - The script must contains a `main()` function. Python will respond with the returned value from that `main()` function, or time out after 60.0 seconds.
        - The python standard library is available, but you cannot import any third-party libraries.
        - Do not make external web requests in your python script.
        - Do not make file I/O in your python script.
        """;

    public string Name => "execute_python_code";
    public class Args
    {
        [Description("Python code to be executed. It must contains a `main()` function.")]
        public string Code { get; set; } = "";
    }

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
                msgContents[0].Text += $"Failed to parse arguments for Python Execution. The args are: {argstr}\n\n";
                return msg;
            }
            args = parsedArgs;
        }
        catch (JsonSerializationException e)
        {
            msgContents[0].Text += $"Failed to parse arguments for Python Execution. The args are: {argstr}\n\n";
            msgContents[0].Text += $"Exception: {e.Message}\n\n";
            return msg;
        }

        if (string.IsNullOrEmpty(args.Code))
        {
            msgContents[0].Text += "Error: Empty query.\n\n";
            return msg;
        }

        try
        {
            var task = Task.Run(() =>
            {
                var eng = IronPython.Hosting.Python.CreateEngine();
                var scope = eng.CreateScope();
                eng.Execute(args.Code, scope);
                return eng.Execute("str(main())", scope);
            });
            if (await Task.WhenAny(task, Task.Delay(60000)) != task)
            {
                msgContents[0].Text += "Error: Python execution timed out.\n\n";
                return msg;
            }
            var executeResult = await task;
            msgContents[0].Text += $"Python execution result: {executeResult}\n\n";
            return msg;
        }
        catch (Exception e)
        {
            msgContents[0].Text += $"Python error: {e.Message}\n\n";
            return msg;
        }
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
                return [new Paragraph(new Run("运行 Python 代码..."))];
            }
            args = parsedArgs;
        }
        catch (JsonSerializationException)
        {
            return [new Paragraph(new Run("运行 Python 代码..."))];
        }

        List<string> stickers = [
            "艾尔海森-动动脑.png",
            "本-疯狂计算.png",
            "丹恒 思考.png",
            "姬子 计算.png",
            "卡维-挠头.png"
        ];

        var floater = Utils.CreateStickerFloater(stickers, toolcallId);


        List<Block> blocks = [];

        var paragraph = new Paragraph();
        paragraph.Inlines.Add(floater);
        paragraph.Inlines.Add(new Run("运行 Python 代码:"));
        paragraph.Inlines.Add(new LineBreak());
        paragraph.Inlines.Add(new LineBreak());

        blocks.Add(paragraph);

        var codeText =
            $"""
            ```python
            {args.Code}
            ```
            """;

        if (state.Config.EnableMarkdown)
        {
            var doc = Markdig.Wpf.Markdown.ToFlowDocument(
                codeText,
                new MarkdownPipelineBuilder()
                    .UseAdvancedExtensions()
                    .UseColorCodeWpf()
                    .UseTaskLists()
                    .UseGridTables()
                    .UsePipeTables()
                    .UseEmphasisExtras()
                    .UseAutoLinks()
                    .Build());
            blocks.AddRange(doc.Blocks.ToList());
        }
        else
        {
            var paragraph2 = new Paragraph();
            paragraph2.Inlines.Add(new Run(codeText));
            paragraph2.Inlines.Add(new LineBreak());
            paragraph2.Inlines.Add(new LineBreak());
            blocks.Add(paragraph2);
        }
        return blocks;
    }
}