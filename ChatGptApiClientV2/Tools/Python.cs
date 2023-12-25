using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace ChatGptApiClientV2.Tools;

public class Python : IToolCollection
{
    public List<IToolFunction> Funcs =>
    [
        new PythonFunc()
    ];

    public string DisplayName => "Python 解释器";
}

public class PythonFunc : IToolFunction
{
    public string? Description => throw new NotImplementedException();

    public string Name => "execute_python_code";
    public class Args
    {
        [System.ComponentModel.Description("Python code to be executed.")]
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
            var executeResult = await Task.Run(() =>
            {
                var eng = IronPython.Hosting.Python.CreateEngine();
                var scope = eng.CreateScope();
                return eng.Execute(args.Code, scope);
            });
        }
        catch (Exception e)
        {
            msgContents[0].Text += $"Python error: {e.Message}\n\n";
            return msg;
        }

        throw new NotImplementedException();
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

        var paragraph = new Paragraph();
        paragraph.Inlines.Add(floater);
        paragraph.Inlines.Add(new Run("Bing 搜索:"));
        paragraph.Inlines.Add(new LineBreak());
        paragraph.Inlines.Add(new LineBreak());
        paragraph.Inlines.Add(new Run($"{args.Code}"));
        paragraph.Inlines.Add(new LineBreak());

        return [paragraph];
    }
}