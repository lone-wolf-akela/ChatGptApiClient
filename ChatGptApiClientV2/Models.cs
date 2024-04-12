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

using System;
using System.Collections.Immutable;
// ReSharper disable MemberCanBePrivate.Global

namespace ChatGptApiClientV2;

public class ModelInfo
{
    public enum ProviderEnum
    {
        OpenAI,
        Anthropic
    }
    public string Name { get; init; } = "";
    public string Description { get; init; } = "";
    public ProviderEnum Provider { get; init; }
    public int DisplayPriority { get; init; }
    public static readonly ImmutableArray<ModelInfo> ModelList =
    [
        new ModelInfo { Name = "gpt-4-128k", Description = "gpt-4 turbo (128k tokens)" , Provider = ProviderEnum.OpenAI, DisplayPriority = 1},
        new ModelInfo { Name = "claude-3", Description = "claude-3 (200k tokens)", Provider = ProviderEnum.Anthropic, DisplayPriority = 2 },

        new ModelInfo { Name = "gpt-3.5-16k", Description = "gpt-3.5 turbo (16k tokens)", Provider = ProviderEnum.OpenAI, DisplayPriority = 3 },

        new ModelInfo { Name = "gpt-3.5-4k", Description = "gpt-3.5 turbo (4k tokens, 已弃用)" , Provider = ProviderEnum.OpenAI, DisplayPriority = 100},
        new ModelInfo { Name = "gpt-4-8k", Description = "gpt-4 (8k tokens, 已弃用)" , Provider = ProviderEnum.OpenAI, DisplayPriority = 101}
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
        new ModelVersionInfo { ModelType = "gpt-3.5-4k", Name = "gpt-3.5-turbo-0613", Description = "2023-06-13 (已弃用)", KnowledgeCutoff = new DateTime(2021, 9, 1), FunctionCallSupported = true },
        new ModelVersionInfo { ModelType = "gpt-3.5-4k", Name = "gpt-3.5-turbo-0301", Description = "2023-03-01 (已弃用)", KnowledgeCutoff = new DateTime(2021, 9, 1), FunctionCallSupported = false },

        new ModelVersionInfo { ModelType = "gpt-3.5-16k", Name = "gpt-3.5-turbo", Description = "最新 (2024-01-25)", KnowledgeCutoff = new DateTime(2021, 9, 1), FunctionCallSupported = true },
        new ModelVersionInfo { ModelType = "gpt-3.5-16k", Name = "gpt-3.5-turbo-0125", Description = "2024-01-25", KnowledgeCutoff = new DateTime(2021, 9, 1), FunctionCallSupported = true },
        new ModelVersionInfo { ModelType = "gpt-3.5-16k", Name = "gpt-3.5-turbo-1106", Description = "2023-11-06", KnowledgeCutoff = new DateTime(2021, 9, 1), FunctionCallSupported = true },
        new ModelVersionInfo { ModelType = "gpt-3.5-16k", Name = "gpt-3.5-turbo-16k-0613", Description = "2023-06-13 (已弃用)", KnowledgeCutoff = new DateTime(2021, 9, 1), FunctionCallSupported = true },

        new ModelVersionInfo { ModelType = "gpt-4-128k", Name = "gpt-4-turbo", Description = "最新 (2024-04-09)", KnowledgeCutoff = new DateTime(2023, 12, 1), FunctionCallSupported = true },
        new ModelVersionInfo { ModelType = "gpt-4-128k", Name = "gpt-4-turbo-2024-04-09", Description = "2024-04-09", KnowledgeCutoff = new DateTime(2023, 12, 1), FunctionCallSupported = true },
        new ModelVersionInfo { ModelType = "gpt-4-128k", Name = "gpt-4-0125-preview", Description = "2024-01-25", KnowledgeCutoff = new DateTime(2023, 12, 1), FunctionCallSupported = true },
        new ModelVersionInfo { ModelType = "gpt-4-128k", Name = "gpt-4-1106-preview", Description = "2023-11-06", KnowledgeCutoff = new DateTime(2023, 4, 1), FunctionCallSupported = true },
        new ModelVersionInfo { ModelType = "gpt-4-128k", Name = "gpt-4-1106-vision-preview", Description = "2023-11-06 w/ vision", KnowledgeCutoff = new DateTime(2023, 4, 1), FunctionCallSupported = false },

        new ModelVersionInfo { ModelType = "gpt-4-8k", Name = "gpt-4", Description = "最新 (2023-06-13)", KnowledgeCutoff = new DateTime(2021, 9, 1), FunctionCallSupported = true },
        new ModelVersionInfo { ModelType = "gpt-4-8k", Name = "gpt-4-0613", Description = "2023-06-13", KnowledgeCutoff = new DateTime(2021, 9, 1), FunctionCallSupported = true },
        new ModelVersionInfo { ModelType = "gpt-4-8k", Name = "gpt-4-0314", Description = "2023-03-14 (已弃用)", KnowledgeCutoff = new DateTime(2021, 9, 1), FunctionCallSupported = false },

        new ModelVersionInfo { ModelType = "claude-3", Name = "claude-3-haiku-20240307", Description = "Haiku (小, 2024-03-07)", KnowledgeCutoff = new DateTime(2023, 8, 1), FunctionCallSupported = true },
        new ModelVersionInfo { ModelType = "claude-3", Name = "claude-3-sonnet-20240229", Description = "Sonnet (中, 2024-02-29)", KnowledgeCutoff = new DateTime(2023, 8, 1), FunctionCallSupported = true },
        new ModelVersionInfo { ModelType = "claude-3", Name = "claude-3-opus-20240229", Description = "Opus (大, 2024-02-29)", KnowledgeCutoff = new DateTime(2023, 8, 1), FunctionCallSupported = true },
    ];
}