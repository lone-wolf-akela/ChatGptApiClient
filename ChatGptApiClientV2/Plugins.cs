using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronPython.Hosting;

namespace ChatGptApiClientV2.Plugins
{
    interface Plugins
    {
        static abstract string Name { get; }
        static abstract string InitialPrompt { get; }
        bool DetectUsage(string bot_data);
        string ProcessData(string bot_data);
    }

    class PythonPlugin : Plugins
    {
        public static string Name { get => "Python"; }
        public static string InitialPrompt
        {
            get =>
                @"When a required task can be performed using a python script, you can run python code as the following example shows:

> User: Can you tell me the result of 123*123+5564/238?

> Assistant: 
CALL_PYTHON
def main():
    return 123*123+5564/238
END_PYTHON

Please remember, you do not need to simulate running this code. You just need to give the code to the system, and the system will automatically feedback you the right answer.
And do not be confident with math question. When you are in doubt, use python script to get the correct answer.
";
        }
        public bool DetectUsage(string bot_data)
        {
            return bot_data.Contains("CALL_PYTHON");
        }
        private Microsoft.Scripting.Hosting.ScriptEngine pythonEngine = Python.CreateEngine();
        public string ProcessData(string bot_data)
        {
            string code_block_start = "CALL_PYTHON";
            string code_block_end = "END_PYTHON";
            int start_index = bot_data.IndexOf(code_block_start) + code_block_start.Length;
            int end_index = bot_data.IndexOf(code_block_end, start_index);

            if (start_index == -1 || end_index == -1 || start_index == end_index)
            {
                return "Error: Failed to find code block. Code block should start with 'CALL_PYTHON', and end with 'END_PYTHON'.";
            }

            string code_block = bot_data.Substring(start_index, end_index - start_index);

            var scope = pythonEngine.CreateScope();
            pythonEngine.Execute(code_block, scope);
            var main_func = scope.GetVariable("main");
            var result = main_func();
            return $"python result: {result?.ToString() ?? "null"}";
        }
    }
}
