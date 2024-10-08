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

using Newtonsoft.Json.Converters;
using System;
using System.Collections.Immutable;
using Newtonsoft.Json;
using System.Runtime.Serialization;

// ReSharper disable MemberCanBePrivate.Global

namespace ChatGptApiClientV2;

public class ModelInfo
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ProviderEnum
    {
        [EnumMember(Value = "openai")] OpenAI,
        [EnumMember(Value = "anthropic")] Anthropic,
        [EnumMember(Value = "other_oai_compat")] OtherOpenAICompat
    }
    public string Name { get; init; } = "";
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string Description { get; init; } = "";
    public ProviderEnum Provider { get; init; }
    public int DisplayPriority { get; init; }

    public static readonly ImmutableArray<ModelInfo> ModelList =
    [
        new ModelInfo
        {
            Name = "gpt-4o", Description = "gpt-4o (128k tokens)", Provider = ProviderEnum.OpenAI,
            DisplayPriority = 100
        },
        new ModelInfo
        {
            Name = "gpt-4o-mini", Description = "gpt-4o mini (128k tokens)", Provider = ProviderEnum.OpenAI,
            DisplayPriority = 150
        },
        new ModelInfo
        {
            Name = "claude-3.5", Description = "claude-3.5 (200k tokens)", Provider = ProviderEnum.Anthropic,
            DisplayPriority = 200
        },
        new ModelInfo
        {
            Name = "Local", Description = "本地部署端口", Provider = ProviderEnum.OtherOpenAICompat,
            DisplayPriority = 250
        },
        new ModelInfo
        {
            Name = "claude-3", Description = "claude-3 (200k tokens)", Provider = ProviderEnum.Anthropic,
            DisplayPriority = 300
        },
        new ModelInfo
        {
            Name = "gpt-4-128k", Description = "gpt-4 turbo (128k tokens)", Provider = ProviderEnum.OpenAI,
            DisplayPriority = 350
        },
        new ModelInfo
        {
            Name = "gpt-3.5-16k", Description = "gpt-3.5 turbo (16k tokens)", Provider = ProviderEnum.OpenAI,
            DisplayPriority = 400
        },

        new ModelInfo
        {
            Name = "gpt-3.5-4k", Description = "gpt-3.5 turbo (4k tokens, 已弃用)", Provider = ProviderEnum.OpenAI,
            DisplayPriority = 10000
        },
        new ModelInfo
        {
            Name = "gpt-4-8k", Description = "gpt-4 (8k tokens, 已弃用)", Provider = ProviderEnum.OpenAI,
            DisplayPriority = 10100
        }
    ];
}

public class ModelVersionInfo
{
    public enum TokenizerEnum
    {
        O200KBase,
        Cl100KBase
    }
    public string ModelType { get; init; } = "";
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime KnowledgeCutoff { get; set; } = DateTime.MinValue;
    public bool FunctionCallSupported { get; set; }
    public TokenizerEnum Tokenizer { get; init; }

    public static readonly ImmutableArray<ModelVersionInfo> VersionList =
    [
        new ModelVersionInfo
        {
            ModelType = "gpt-3.5-4k", Name = "gpt-3.5-turbo-0613", Description = "2023-06-13 (已弃用)",
            KnowledgeCutoff = new DateTime(2021, 9, 1), FunctionCallSupported = true,
            Tokenizer = TokenizerEnum.Cl100KBase
        },
        new ModelVersionInfo
        {
            ModelType = "gpt-3.5-4k", Name = "gpt-3.5-turbo-0301", Description = "2023-03-01 (已弃用)",
            KnowledgeCutoff = new DateTime(2021, 9, 1), FunctionCallSupported = false,
            Tokenizer = TokenizerEnum.Cl100KBase
        },

        new ModelVersionInfo
        {
            ModelType = "gpt-3.5-16k", Name = "gpt-3.5-turbo", Description = "最新 (2024-01-25)",
            KnowledgeCutoff = new DateTime(2021, 9, 1), FunctionCallSupported = true,
            Tokenizer = TokenizerEnum.Cl100KBase
        },
        new ModelVersionInfo
        {
            ModelType = "gpt-3.5-16k", Name = "gpt-3.5-turbo-0125", Description = "2024-01-25",
            KnowledgeCutoff = new DateTime(2021, 9, 1), FunctionCallSupported = true,
            Tokenizer = TokenizerEnum.Cl100KBase
        },
        new ModelVersionInfo
        {
            ModelType = "gpt-3.5-16k", Name = "gpt-3.5-turbo-1106", Description = "2023-11-06",
            KnowledgeCutoff = new DateTime(2021, 9, 1), FunctionCallSupported = true,
            Tokenizer = TokenizerEnum.Cl100KBase
        },
        new ModelVersionInfo
        {
            ModelType = "gpt-3.5-16k", Name = "gpt-3.5-turbo-16k-0613", Description = "2023-06-13 (已弃用)",
            KnowledgeCutoff = new DateTime(2021, 9, 1), FunctionCallSupported = true,
            Tokenizer = TokenizerEnum.Cl100KBase
        },

        new ModelVersionInfo
        {
            ModelType = "gpt-4-128k", Name = "gpt-4-turbo", Description = "最新 (2024-04-09)",
            KnowledgeCutoff = new DateTime(2023, 12, 1), FunctionCallSupported = true,
            Tokenizer = TokenizerEnum.Cl100KBase
        },
        new ModelVersionInfo
        {
            ModelType = "gpt-4-128k", Name = "gpt-4-turbo-2024-04-09", Description = "2024-04-09",
            KnowledgeCutoff = new DateTime(2023, 12, 1), FunctionCallSupported = true,
            Tokenizer = TokenizerEnum.Cl100KBase
        },
        new ModelVersionInfo
        {
            ModelType = "gpt-4-128k", Name = "gpt-4-0125-preview", Description = "2024-01-25",
            KnowledgeCutoff = new DateTime(2023, 12, 1), FunctionCallSupported = true,
            Tokenizer = TokenizerEnum.Cl100KBase
        },
        new ModelVersionInfo
        {
            ModelType = "gpt-4-128k", Name = "gpt-4-1106-preview", Description = "2023-11-06",
            KnowledgeCutoff = new DateTime(2023, 4, 1), FunctionCallSupported = true,
            Tokenizer = TokenizerEnum.Cl100KBase
        },
        new ModelVersionInfo
        {
            ModelType = "gpt-4-128k", Name = "gpt-4-1106-vision-preview", Description = "2023-11-06 w/ vision",
            KnowledgeCutoff = new DateTime(2023, 4, 1), FunctionCallSupported = false,
            Tokenizer = TokenizerEnum.Cl100KBase
        },

        new ModelVersionInfo
        {
            ModelType = "gpt-4-8k", Name = "gpt-4", Description = "最新 (2023-06-13)",
            KnowledgeCutoff = new DateTime(2021, 9, 1), FunctionCallSupported = true,
            Tokenizer = TokenizerEnum.Cl100KBase
        },
        new ModelVersionInfo
        {
            ModelType = "gpt-4-8k", Name = "gpt-4-0613", Description = "2023-06-13",
            KnowledgeCutoff = new DateTime(2021, 9, 1), FunctionCallSupported = true,
            Tokenizer = TokenizerEnum.Cl100KBase
        },
        new ModelVersionInfo
        {
            ModelType = "gpt-4-8k", Name = "gpt-4-0314", Description = "2023-03-14 (已弃用)",
            KnowledgeCutoff = new DateTime(2021, 9, 1), FunctionCallSupported = false,
            Tokenizer = TokenizerEnum.Cl100KBase
        },

        new ModelVersionInfo
        {
            ModelType = "claude-3", Name = "claude-3-haiku-20240307", Description = "Haiku (小, 2024-03-07)",
            KnowledgeCutoff = new DateTime(2023, 8, 1), FunctionCallSupported = true,
            Tokenizer = TokenizerEnum.Cl100KBase
        },
        new ModelVersionInfo
        {
            ModelType = "claude-3", Name = "claude-3-sonnet-20240229", Description = "Sonnet (中, 2024-02-29)",
            KnowledgeCutoff = new DateTime(2023, 8, 1), FunctionCallSupported = true,
            Tokenizer = TokenizerEnum.Cl100KBase
        },
        new ModelVersionInfo
        {
            ModelType = "claude-3", Name = "claude-3-opus-20240229", Description = "Opus (大, 2024-02-29)",
            KnowledgeCutoff = new DateTime(2023, 8, 1), FunctionCallSupported = true,
            Tokenizer = TokenizerEnum.Cl100KBase
        },
        new ModelVersionInfo
        {
            ModelType = "claude-3.5", Name = "claude-3-5-sonnet-20240620", Description = "Sonnet (中, 2024-06-20)",
            KnowledgeCutoff = new DateTime(2024, 4, 1), FunctionCallSupported = true,
            Tokenizer = TokenizerEnum.Cl100KBase
        },
        new ModelVersionInfo
        {
            ModelType = "gpt-4o", Name = "chatgpt-4o-latest", Description = "最新 (2024-08-06)",
            KnowledgeCutoff = new DateTime(2023, 10, 1), FunctionCallSupported = true,
            Tokenizer = TokenizerEnum.O200KBase
        },
        new ModelVersionInfo
        {
            ModelType = "gpt-4o", Name = "gpt-4o-2024-08-06", Description = "2024-08-06",
            KnowledgeCutoff = new DateTime(2023, 10, 1), FunctionCallSupported = true,
            Tokenizer = TokenizerEnum.O200KBase
        },
        new ModelVersionInfo
        {
            ModelType = "gpt-4o", Name = "gpt-4o-2024-05-13", Description = "2024-05-13",
            KnowledgeCutoff = new DateTime(2023, 10, 1), FunctionCallSupported = true,
            Tokenizer = TokenizerEnum.O200KBase
        },
        new ModelVersionInfo
        {
            ModelType = "gpt-4o-mini", Name = "gpt-4o-mini", Description = "最新 (2024-07-18)",
            KnowledgeCutoff = new DateTime(2023, 10, 1), FunctionCallSupported = true,
            Tokenizer = TokenizerEnum.O200KBase
        },
        new ModelVersionInfo
        {
            ModelType = "gpt-4o-mini", Name = "gpt-4o-mini-2024-07-18", Description = "2024-07-18",
            KnowledgeCutoff = new DateTime(2023, 10, 1), FunctionCallSupported = true,
            Tokenizer = TokenizerEnum.O200KBase
        },
        new ModelVersionInfo
        {
            ModelType = "Local", Name = "local", Description = "本地",
            KnowledgeCutoff = new DateTime(2023, 10, 1), FunctionCallSupported = false,
            Tokenizer = TokenizerEnum.O200KBase
        }
    ];
}