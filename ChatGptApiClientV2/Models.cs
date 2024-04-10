using System;
using System.Collections.Immutable;

namespace ChatGptApiClientV2;

public class ModelInfo
{
    public string Name { get; init; } = "";
    public string Description { get; init; } = "";
    public static readonly ImmutableArray<ModelInfo> ModelList =
    [
        new ModelInfo { Name = "gpt-3.5-16k", Description = "gpt-3.5 turbo (16k tokens)" },
        new ModelInfo { Name = "gpt-4-128k", Description = "gpt-4 turbo (128k tokens)" },
        new ModelInfo { Name = "gpt-3.5-4k", Description = "gpt-3.5 turbo (4k tokens, deprecated)" },
        new ModelInfo { Name = "gpt-4-8k", Description = "gpt-4 (8k tokens, deprecated)" }
        //new ModelInfo{ Name="gpt-4-32k", Description="gpt-4 (32k tokens)" },
    ];
}
public class ModelVersionInfo
{
    public string ModelType { get; init; } = "";
    public string Name { get; init; } = "";
    public string Description { get; init; } = "";
    public DateTime KnowledgeCutoff { get; init; } = DateTime.MinValue;
    public bool FunctionCallSupported { get; init; }
    public static readonly ImmutableArray<ModelVersionInfo> VersionList =
    [
        new ModelVersionInfo { ModelType = "gpt-3.5-16k", Name = "gpt-3.5-turbo-0125", Description = "2024-01-25", KnowledgeCutoff = new DateTime(2021, 9, 1), FunctionCallSupported = true },
        new ModelVersionInfo { ModelType = "gpt-3.5-16k", Name = "gpt-3.5-turbo-1106", Description = "2023-11-06", KnowledgeCutoff = new DateTime(2021, 9, 1), FunctionCallSupported = true },
        new ModelVersionInfo { ModelType = "gpt-3.5-4k", Name = "gpt-3.5-turbo", Description = "current (06-13)", KnowledgeCutoff = new DateTime(2021, 9, 1), FunctionCallSupported = true },
        new ModelVersionInfo { ModelType = "gpt-3.5-16k", Name = "gpt-3.5-turbo-16k", Description = "current (06-13)", KnowledgeCutoff = new DateTime(2021, 9, 1), FunctionCallSupported = true },
        new ModelVersionInfo { ModelType = "gpt-3.5-4k", Name = "gpt-3.5-turbo-0613", Description = "2023-06-13", KnowledgeCutoff = new DateTime(2021, 9, 1), FunctionCallSupported = true },
        new ModelVersionInfo { ModelType = "gpt-3.5-16k", Name = "gpt-3.5-turbo-16k-0613", Description = "2023-06-13", KnowledgeCutoff = new DateTime(2021, 9, 1), FunctionCallSupported = true },
        new ModelVersionInfo { ModelType = "gpt-3.5-4k", Name = "gpt-3.5-turbo-0301", Description = "2023-03-01", KnowledgeCutoff = new DateTime(2021, 9, 1), FunctionCallSupported = false },
        new ModelVersionInfo { ModelType = "gpt-4-128k", Name = "gpt-4-turbo-2024-04-09", Description = "2024-04-09", KnowledgeCutoff = new DateTime(2023, 12, 1), FunctionCallSupported = true },
        new ModelVersionInfo { ModelType = "gpt-4-128k", Name = "gpt-4-0125-preview", Description = "2024-01-25", KnowledgeCutoff = new DateTime(2023, 12, 1), FunctionCallSupported = true },
        new ModelVersionInfo { ModelType = "gpt-4-128k", Name = "gpt-4-1106-preview", Description = "2023-11-06", KnowledgeCutoff = new DateTime(2023, 4, 1), FunctionCallSupported = true },
        new ModelVersionInfo { ModelType = "gpt-4-128k", Name = "gpt-4-vision-preview", Description = "2023-11-06 w/ vision", KnowledgeCutoff = new DateTime(2023, 4, 1), FunctionCallSupported = false },
        new ModelVersionInfo { ModelType = "gpt-4-8k", Name = "gpt-4", Description = "current (06-13)", KnowledgeCutoff = new DateTime(2021, 9, 1), FunctionCallSupported = true },
        //new ModelVersionInfo { ModelType = "gpt-4-32k", Name = "gpt-4-32k", Description = "current (06-13)", KnowledgeCutoff = newDateTime(2021, 9, 1), FunctionCallSupported = true }
        new ModelVersionInfo { ModelType = "gpt-4-8k", Name = "gpt-4-0613", Description = "2023-06-13", KnowledgeCutoff = new DateTime(2021, 9, 1), FunctionCallSupported = true },
        //new ModelVersionInfo { ModelType = "gpt-4-32k", Name = "gpt-4-32k-0613", Description = "2023-06-13", KnowledgeCutoff = newDateTime(2021, 9, 1), FunctionCallSupported = true }
        new ModelVersionInfo { ModelType = "gpt-4-8k", Name = "gpt-4-0314", Description = "2023-03-14", KnowledgeCutoff = new DateTime(2021, 9, 1), FunctionCallSupported = false }
        //new ModelVersionInfo { ModelType = "gpt-4-32k", Name = "gpt-4-32k-0314", Description = "2023-03-14", KnowledgeCutoff = newDateTime(2021, 9, 1), FunctionCallSupported = false }
    ];
}