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
        static abstract List<ChatRecord> InitialPrompt { get; }
        bool DetectUsage(string bot_data);
        string ProcessData(string bot_data);
    }

    class PythonPlugin : Plugins
    {
        public static string Name { get => "Python"; }
        public static List<ChatRecord> InitialPrompt
        {
            get 
            {
                return new List<ChatRecord>
                {
                    new ChatRecord(ChatRecord.ChatType.System, @"When a required task can be performed using a python script, you can run python code as the following example shows:

> User: Can you tell me the result of 123*123+5564/238?

> Assistant: 
CALL_PYTHON
def main():
    return 123*123+5564/238
END_PYTHON

The code should return the final answer as the returned value of the main function.
Please remember, you do not need to give answer to the question if you choose to use Python. The system will automatically run the python code and give the result as the answer.
And do not be confident with math question. When you are in doubt, use python script to get the correct answer.
Remember, DO NOT give final answer if you write the Python code.
", hidden:true),
                    new ChatRecord(ChatRecord.ChatType.User, "请问，123/1234是多少？", hidden:true),
                    new ChatRecord(ChatRecord.ChatType.Bot, @"CALL_PYTHON
def main():
    return 123/1234
END_PYTHON", hidden:true),
                    new ChatRecord(ChatRecord.ChatType.System, "python result: 0.099675850891410053", hidden:true),
                    new ChatRecord(ChatRecord.ChatType.Bot, @"答案是0.099675850891410053。", hidden:true),
                    new ChatRecord(ChatRecord.ChatType.User, "它的四次方是多少呢？", hidden:true),
                    new ChatRecord(ChatRecord.ChatType.Bot, @"CALL_PYTHON
def main():
    return (123/1234)**4
END_PYTHON", hidden:true),
                    new ChatRecord(ChatRecord.ChatType.System, "python result: 9.8709694311674824e-05", hidden:true),
                    new ChatRecord(ChatRecord.ChatType.Bot, @"(123/1234)的四次方是9.8709694311674824e-05。", hidden:true),
                };
            }
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
            try
            {
                pythonEngine.Execute(code_block, scope);
                var result = pythonEngine.Execute("str(main())", scope);
                return $"python result: {result?.ToString() ?? "null"}";
            }
            catch (Exception e)
            {
                return $"Python error: {e.Message}";
            }
        }
    }
}
