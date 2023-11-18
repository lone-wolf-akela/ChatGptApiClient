using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatGptApiClientV2
{
    public class ModelInfo
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public static readonly ImmutableArray<ModelInfo> ModelList =
        [
            new() { Name = "gpt-3.5-16k", Description = "gpt-3.5 turbo (16k tokens)" },
            new() { Name = "gpt-4-128k", Description = "gpt-4 turbo (128k tokens)" },
            new() { Name = "gpt-3.5-4k", Description = "gpt-3.5 turbo (4k tokens, deprecated)" },
            new() { Name = "gpt-4-8k", Description = "gpt-4 (8k tokens, deprecated)" },
            //new (){ Name="gpt-4-32k", Description="gpt-4 (32k tokens)" },
        ];
    }
    public class ModelVersionInfo
    {
        public string ModelType { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime KnowledgeCutoff { get; set; } = DateTime.MinValue;
        public bool FunctionCallSupported { get; set; } = false;
        public static readonly ImmutableArray<ModelVersionInfo> VersionList =
        [
            new() { ModelType = "gpt-3.5-16k", Name = "gpt-3.5-turbo-1106", Description = "2023-11-06", KnowledgeCutoff = new(2021, 9, 1), FunctionCallSupported = true },
            new() { ModelType = "gpt-3.5-4k", Name = "gpt-3.5-turbo", Description = "current (06-13)", KnowledgeCutoff = new(2021, 9, 1), FunctionCallSupported = true },
            new() { ModelType = "gpt-3.5-16k", Name = "gpt-3.5-turbo-16k", Description = "current (06-13)", KnowledgeCutoff = new(2021, 9, 1), FunctionCallSupported = true },
            new() { ModelType = "gpt-3.5-4k", Name = "gpt-3.5-turbo-0613", Description = "2023-06-13", KnowledgeCutoff = new(2021, 9, 1), FunctionCallSupported = true },
            new() { ModelType = "gpt-3.5-16k", Name = "gpt-3.5-16k-turbo-0613", Description = "2023-06-13", KnowledgeCutoff = new(2021, 9, 1), FunctionCallSupported = true },
            new() { ModelType = "gpt-3.5-4k", Name = "gpt-3.5-turbo-0301", Description = "2023-03-01", KnowledgeCutoff = new(2021, 9, 1), FunctionCallSupported = false },
            new() { ModelType = "gpt-4-128k", Name = "gpt-4-1106-preview", Description = "2023-11-06", KnowledgeCutoff = new(2023, 4, 1), FunctionCallSupported = true },
            new() { ModelType = "gpt-4-128k", Name = "gpt-4-vision-preview", Description = "2023-11-06 w/ vision", KnowledgeCutoff = new(2023, 4, 1), FunctionCallSupported = false },
            new() { ModelType = "gpt-4-8k", Name = "gpt-4", Description = "current (06-13)", KnowledgeCutoff = new(2021, 9, 1), FunctionCallSupported = true },
            //new() { ModelType = "gpt-4-32k", Name = "gpt-4-32k", Description = "current (06-13)", KnowledgeCutoff = new(2021, 9, 1), FunctionCallSupported = true },
            new() { ModelType = "gpt-4-8k", Name = "gpt-4-0613", Description = "2023-06-13", KnowledgeCutoff = new(2021, 9, 1), FunctionCallSupported = true },
            //new() { ModelType = "gpt-4-32k", Name = "gpt-4-32k-0613", Description = "2023-06-13", KnowledgeCutoff = new(2021, 9, 1), FunctionCallSupported = true },
            new() { ModelType = "gpt-4-8k", Name = "gpt-4-0314", Description = "2023-03-14", KnowledgeCutoff = new(2021, 9, 1), FunctionCallSupported = false },
            //new() { ModelType = "gpt-4-32k", Name = "gpt-4-32k-0314", Description = "2023-03-14", KnowledgeCutoff = new(2021, 9, 1), FunctionCallSupported = false },
        ];
    }
}
