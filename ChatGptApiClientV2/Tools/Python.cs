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
using System.Windows.Documents;
using Markdig;
using Markdig.Wpf.ColorCode;
using System.ComponentModel;
using System.IO;
using Python.Runtime;
using System.Diagnostics;
using static ChatGptApiClientV2.Tools.IToolFunction;
using ChatGptApiClientV2.Controls;
using System.Threading;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace ChatGptApiClientV2.Tools;

public class PythonTool : IToolCollection
{
    public List<IToolFunction> Funcs =>
    [
        new PythonFunc(),
        new ShowImageFunc()
    ];

    public string DisplayName => "Python 解释器";
}

public class PythonFunc : IToolFunction
{
    public string Description =>
        """
        You can execute a python script.
        - The script must contains a `main()` function. Python will respond with the returned value from that `main()` function, or time out after 60.0 seconds.
        - The python standard library is available. Apart from that, you can only import the third-party libraries listed below:
          1. sympy
          2. numpy
          3. matplotlib
          4. pandas
        - The python environment is reset after each execution. So you have to import the libraries you need in each execution.
        - Do not make external web requests in your python script.
        - You are allowed to perform file I/O operations only within the "sandbox" local directory. Any attempt to read from or write to other directories is strictly prohibited.
        - If you need to create plots using matplotlib, you may save the plot as a PNG file within the "sandbox" directory. Once the file is saved, call the `show_image` function and pass the path of the PNG file to display the image to the user.
        """;

    public string Name => "execute_python_code";

    public class Args
    {
        [Description("Python code to be executed. It must contains a `main()` function.")]
        public string Code { get; set; } = "";
    }

    public Type ArgsType => typeof(Args);

    private static void PreparePython(string pythonDll)
    {
        if (!File.Exists(pythonDll))
        {
            throw new Exception($"Python DLL not found: {pythonDll}");
        }

        if (!PythonEngine.IsInitialized)
        {
            Runtime.PythonDLL = pythonDll;
            PythonEngine.Initialize();
            PythonEngine.BeginAllowThreads();
        }

        Directory.CreateDirectory("sandbox");
    }

    private static async Task InstallPackage(Utils.PythonEnv pythonEnv, string package, string version = "",
        CancellationToken cancellationToken = default)
    {
        if (version != "")
        {
            version = $"=={version}";
        }

        ProcessStartInfo startInfo = new()
        {
            FileName = pythonEnv.ExecutablePath,
            Arguments = $"-m pip install {package}{version}",
            WorkingDirectory = pythonEnv.HomePath,
            UseShellExecute = false,
            RedirectStandardOutput = false,
            RedirectStandardError = false,
            CreateNoWindow = true
        };
        var task = Process.Start(startInfo)?.WaitForExitAsync(cancellationToken);
        if (task is not null)
        {
            await task;
        }
    }

    private static async Task PreparePackage(Utils.PythonEnv pythonEnv, string code)
    {
        string[] packages =
        [
            "sympy",
            "numpy",
            "matplotlib",
            "pandas"
        ];
        foreach (var package in packages)
        {
            if (code.Contains(package))
            {
                await InstallPackage(pythonEnv, package);
            }
        }
    }

    public async Task<ToolResult> Action(SystemState state, int sessionIndex, string toolcallId, string argstr,
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
                msgContents[0].Text += $"Failed to parse arguments for Python Execution. The args are: {argstr}\n\n";
                return result;
            }

            args = parsedArgs;
        }
        catch (JsonSerializationException e)
        {
            msgContents[0].Text += $"Failed to parse arguments for Python Execution. The args are: {argstr}\n\n";
            msgContents[0].Text += $"Exception: {e.Message}\n\n";
            return result;
        }

        if (string.IsNullOrEmpty(args.Code))
        {
            msgContents[0].Text += "Error: Empty query.\n\n";
            return result;
        }

        state.NetStatus.Status = NetStatus.StatusEnum.Processing;

        try
        {
            PreparePython(state.Config.PythonDllPath);
            await PreparePackage(state.Config.PythonEnv!, args.Code);
            var task = Task.Run(() =>
            {
                using (Py.GIL())
                {
                    using var scope = Py.CreateScope();
                    scope.Exec(args.Code);
                    var main = scope.Get("main") ?? throw new Exception("code must contains a `main()` function.");
                    var r = main.Invoke();
                    // ReSharper disable once SpecifyACultureInStringConversionExplicitly
                    return r.ToString();
                }
            }, cancellationToken);
            if (await Task.WhenAny(task, Task.Delay(60000, cancellationToken)) != task)
            {
                throw new Exception("execution timed out.");
            }

            var executeResult = await task;
            msgContents[0].Text += $"Python execution result: {executeResult}\n\n";
            return result;
        }
        catch (Exception e)
        {
            msgContents[0].Text += $"Python error: {e.Message}\n\n";
            return result;
        }
    }

    public IEnumerable<Block> GetToolcallMessage(SystemState state, int sessionIndex, string argstr, string toolcallId)
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

        List<string> stickers =
        [
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

public class ShowImageFunc : IToolFunction
{
    public string Description => """
                                 Display the specified image to the user.
                                 The image must be in the local "sandbox" directory.
                                 The image file must be a PNG or JPEG file.
                                 """;

    public string Name => "show_image";

    public class Args
    {
        [Description(
            "The name of the image file that is in the \"sandbox\" directory. You should only provide the filename here, do not include the path.")]
        public string FileName { get; set; } = "";
    }

    public Type ArgsType => typeof(Args);

    public async Task<ToolResult> Action(SystemState state, int sessionIndex, string toolcallId, string argstr,
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
                msgContents[0].Text += $"Failed to parse arguments for Image Displayer. The args are: {argstr}\n\n";
                return result;
            }

            args = parsedArgs;
        }
        catch (JsonSerializationException e)
        {
            msgContents[0].Text += $"Failed to parse arguments for Image Displayer. The args are: {argstr}\n\n";
            msgContents[0].Text += $"Exception: {e.Message}\n\n";
            return result;
        }

        if (string.IsNullOrEmpty(args.FileName))
        {
            msgContents[0].Text += "Error: Empty filename.\n\n";
            return result;
        }

        var filePath = Path.Combine("sandbox", args.FileName);
        if (!File.Exists(filePath))
        {
            msgContents[0].Text += $"Error: File not found: {filePath}\n\n";
            return result;
        }

        if (!filePath.EndsWith(".png") && !filePath.EndsWith(".jpg") && !filePath.EndsWith(".jpeg"))
        {
            msgContents[0].Text += $"Error: File must be a PNG or JPEG file: {filePath}\n\n";
            return result;
        }

        msgContents[0].Text += "Image successfully displayed.";
        var imageUrl = await Utils.ImageFileToBase64(filePath, cancellationToken);
        state.SessionList[sessionIndex]!.PluginData[$"{Name}_{toolcallId}_imageurl"] = [imageUrl];
        state.SessionList[sessionIndex]!.PluginData[$"{Name}_{toolcallId}_filename"] = [args.FileName];
        result.ResponeRequired = false;
        return result;
    }

    public IEnumerable<Block> GetToolcallMessage(SystemState state, int sessionIndex, string argstr, string toolcallId)
    {
        var paragraph = new Paragraph();
        paragraph.Inlines.Add(new Run("显示图片..."));
        yield return paragraph;

        BlockUIContainer? imageBlock = null;
        try
        {
            var imageUrl = state.SessionList[sessionIndex]!.PluginData[$"{Name}_{toolcallId}_imageurl"][0];
            var imageFileName = state.SessionList[sessionIndex]!.PluginData[$"{Name}_{toolcallId}_filename"][0];
            var image = new ImageDisplayer
            {
                FileName = imageFileName,
                Image = Utils.Base64ToBitmapImage(imageUrl)
            };
            imageBlock = new BlockUIContainer(image);
        }
        catch (KeyNotFoundException)
        {
            // do nothing, the toolcall may have failed
        }

        if (imageBlock is not null)
        {
            yield return imageBlock;
        }
    }
}