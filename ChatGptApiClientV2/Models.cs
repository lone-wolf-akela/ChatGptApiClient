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
        [EnumMember(Value = "deepseek")] DeepSeek,
        [EnumMember(Value = "other_oai_compat")] OtherOpenAICompat
    }
    public string Name { get; init; } = "";
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string Description { get; init; } = "";
    public ProviderEnum Provider { get; init; }
    public int DisplayPriority { get; init; }

    public static readonly ImmutableArray<ModelInfo> ModelList =
    [
        new()
        {
            Name = "gpt-4o", Description = "gpt-4o (128k tokens)", Provider = ProviderEnum.OpenAI,
            DisplayPriority = 100
        },
        new()
        {
            Name = "gpt-4o-mini", Description = "gpt-4o mini (128k tokens)", Provider = ProviderEnum.OpenAI,
            DisplayPriority = 150
        },
        new()
        {
            Name = "claude-3.5-sonnet", Description = "claude-3.5 Sonnet (200k tokens)", Provider = ProviderEnum.Anthropic,
            DisplayPriority = 200
        },
        new()
        {
            Name = "claude-3-opus", Description = "claude-3 Opus (200k tokens)", Provider = ProviderEnum.Anthropic,
            DisplayPriority = 210
        },
        new()
        {
            Name = "claude-3-sonnet", Description = "claude-3 Sonnet (200k tokens)", Provider = ProviderEnum.Anthropic,
            DisplayPriority = 220
        },
        new()
        {
            Name = "claude-3-haiku", Description = "claude-3 Haiku (200k tokens)", Provider = ProviderEnum.Anthropic,
            DisplayPriority = 230
        },
        new()
        {
            Name = "deepseek", Description = "DeepSeek", Provider = ProviderEnum.DeepSeek,
            DisplayPriority = 250
        },
        new()
        {
            Name = "ThirdParty", Description = "第三方聊天服务", Provider = ProviderEnum.OtherOpenAICompat,
            DisplayPriority = 300
        },
        new()
        {
            Name = "oai-o1-preview", Description = "OpenAI o1 preview", Provider = ProviderEnum.OpenAI,
            DisplayPriority = 320
        },
        new()
        {
            Name = "oai-o1-mini", Description = "OpenAI o1 mini", Provider = ProviderEnum.OpenAI,
            DisplayPriority = 330
        },
        new()
        {
            Name = "gpt-4-128k", Description = "gpt-4 turbo (128k tokens)", Provider = ProviderEnum.OpenAI,
            DisplayPriority = 350
        },
        new()
        {
            Name = "gpt-3.5-16k", Description = "gpt-3.5 turbo (16k tokens)", Provider = ProviderEnum.OpenAI,
            DisplayPriority = 400
        },

        new()
        {
            Name = "gpt-3.5-4k", Description = "gpt-3.5 turbo (4k tokens, 已弃用)", Provider = ProviderEnum.OpenAI,
            DisplayPriority = 10000
        },
        new()
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
    public string? SiliconFlowName { get; set; }
    public string? NvidiaName { get; set; }
    public string Description { get; set; } = "";
    public DateTime KnowledgeCutoff { get; set; } = DateTime.MinValue;
    public bool FunctionCallSupported { get; set; }
    public TokenizerEnum Tokenizer { get; init; }

    public bool IsO1 { get; init; } = false;

    public static readonly ImmutableArray<ModelVersionInfo> VersionList =
    [
        new()
        {
            ModelType = "gpt-3.5-4k", Name = "gpt-3.5-turbo-0613", Description = "2023-06-13 (已弃用)",
            KnowledgeCutoff = new DateTime(2021, 9, 1), FunctionCallSupported = true,
            Tokenizer = TokenizerEnum.Cl100KBase
        },
        new()
        {
            ModelType = "gpt-3.5-4k", Name = "gpt-3.5-turbo-0301", Description = "2023-03-01 (已弃用)",
            KnowledgeCutoff = new DateTime(2021, 9, 1), FunctionCallSupported = false,
            Tokenizer = TokenizerEnum.Cl100KBase
        },

        new()
        {
            ModelType = "gpt-3.5-16k", Name = "gpt-3.5-turbo", Description = "最新 (2024-01-25)",
            KnowledgeCutoff = new DateTime(2021, 9, 1), FunctionCallSupported = true,
            Tokenizer = TokenizerEnum.Cl100KBase
        },
        new()
        {
            ModelType = "gpt-3.5-16k", Name = "gpt-3.5-turbo-0125", Description = "2024-01-25",
            KnowledgeCutoff = new DateTime(2021, 9, 1), FunctionCallSupported = true,
            Tokenizer = TokenizerEnum.Cl100KBase
        },
        new()
        {
            ModelType = "gpt-3.5-16k", Name = "gpt-3.5-turbo-1106", Description = "2023-11-06",
            KnowledgeCutoff = new DateTime(2021, 9, 1), FunctionCallSupported = true,
            Tokenizer = TokenizerEnum.Cl100KBase
        },
        new()
        {
            ModelType = "gpt-3.5-16k", Name = "gpt-3.5-turbo-16k-0613", Description = "2023-06-13 (已弃用)",
            KnowledgeCutoff = new DateTime(2021, 9, 1), FunctionCallSupported = true,
            Tokenizer = TokenizerEnum.Cl100KBase
        },

        new()
        {
            ModelType = "gpt-4-128k", Name = "gpt-4-turbo", Description = "最新 (2024-04-09)",
            KnowledgeCutoff = new DateTime(2023, 12, 1), FunctionCallSupported = true,
            Tokenizer = TokenizerEnum.Cl100KBase
        },
        new()
        {
            ModelType = "gpt-4-128k", Name = "gpt-4-turbo-2024-04-09", Description = "2024-04-09",
            KnowledgeCutoff = new DateTime(2023, 12, 1), FunctionCallSupported = true,
            Tokenizer = TokenizerEnum.Cl100KBase
        },
        new()
        {
            ModelType = "gpt-4-128k", Name = "gpt-4-0125-preview", Description = "2024-01-25",
            KnowledgeCutoff = new DateTime(2023, 12, 1), FunctionCallSupported = true,
            Tokenizer = TokenizerEnum.Cl100KBase
        },
        new()
        {
            ModelType = "gpt-4-128k", Name = "gpt-4-1106-preview", Description = "2023-11-06",
            KnowledgeCutoff = new DateTime(2023, 4, 1), FunctionCallSupported = true,
            Tokenizer = TokenizerEnum.Cl100KBase
        },
        new()
        {
            ModelType = "gpt-4-128k", Name = "gpt-4-1106-vision-preview", Description = "2023-11-06 w/ vision",
            KnowledgeCutoff = new DateTime(2023, 4, 1), FunctionCallSupported = false,
            Tokenizer = TokenizerEnum.Cl100KBase
        },

        new()
        {
            ModelType = "gpt-4-8k", Name = "gpt-4", Description = "最新 (2023-06-13)",
            KnowledgeCutoff = new DateTime(2021, 9, 1), FunctionCallSupported = true,
            Tokenizer = TokenizerEnum.Cl100KBase
        },
        new()
        {
            ModelType = "gpt-4-8k", Name = "gpt-4-0613", Description = "2023-06-13",
            KnowledgeCutoff = new DateTime(2021, 9, 1), FunctionCallSupported = true,
            Tokenizer = TokenizerEnum.Cl100KBase
        },
        new()
        {
            ModelType = "gpt-4-8k", Name = "gpt-4-0314", Description = "2023-03-14 (已弃用)",
            KnowledgeCutoff = new DateTime(2021, 9, 1), FunctionCallSupported = false,
            Tokenizer = TokenizerEnum.Cl100KBase
        },

        new()
        {
            ModelType = "claude-3-haiku", Name = "claude-3-haiku-20240307", Description = "2024-03-07",
            KnowledgeCutoff = new DateTime(2023, 8, 1), FunctionCallSupported = true,
            Tokenizer = TokenizerEnum.Cl100KBase
        },
        new()
        {
            ModelType = "claude-3-sonnet", Name = "claude-3-sonnet-20240229", Description = "2024-02-29",
            KnowledgeCutoff = new DateTime(2023, 8, 1), FunctionCallSupported = true,
            Tokenizer = TokenizerEnum.Cl100KBase
        },
        new()
        {
            ModelType = "claude-3-opus", Name = "claude-3-opus-latest", Description = "最新 (2024-02-29)",
            KnowledgeCutoff = new DateTime(2023, 8, 1), FunctionCallSupported = true,
            Tokenizer = TokenizerEnum.Cl100KBase
        },
        new()
        {
            ModelType = "claude-3-opus", Name = "claude-3-opus-20240229", Description = "2024-02-29",
            KnowledgeCutoff = new DateTime(2023, 8, 1), FunctionCallSupported = true,
            Tokenizer = TokenizerEnum.Cl100KBase
        },
        new()
        {
            ModelType = "claude-3.5-sonnet", Name = "claude-3-5-sonnet-latest", Description = "最新 (2024-10-22)",
            KnowledgeCutoff = new DateTime(2024, 4, 1), FunctionCallSupported = true,
            Tokenizer = TokenizerEnum.Cl100KBase
        },
        new()
        {
            ModelType = "claude-3.5-sonnet", Name = "claude-3-5-sonnet-20241022", Description = "2024-10-22",
            KnowledgeCutoff = new DateTime(2024, 4, 1), FunctionCallSupported = true,
            Tokenizer = TokenizerEnum.Cl100KBase
        },
        new()
        {
            ModelType = "claude-3.5-sonnet", Name = "claude-3-5-sonnet-20240620", Description = "2024-06-20",
            KnowledgeCutoff = new DateTime(2024, 4, 1), FunctionCallSupported = true,
            Tokenizer = TokenizerEnum.Cl100KBase
        },
        new()
        {
            ModelType = "gpt-4o", Name = "gpt-4o", Description = "稳定版 (2024-08-06)",
            KnowledgeCutoff = new DateTime(2023, 10, 1), FunctionCallSupported = true,
            Tokenizer = TokenizerEnum.O200KBase
        },
        new()
        {
            ModelType = "gpt-4o", Name = "gpt-4o-2024-11-20", Description = "2024-11-20",
            KnowledgeCutoff = new DateTime(2023, 10, 1), FunctionCallSupported = true,
            Tokenizer = TokenizerEnum.O200KBase
        },
        new()
        {
            ModelType = "gpt-4o", Name = "gpt-4o-2024-08-06", Description = "2024-08-06",
            KnowledgeCutoff = new DateTime(2023, 10, 1), FunctionCallSupported = true,
            Tokenizer = TokenizerEnum.O200KBase
        },
        new()
        {
            ModelType = "gpt-4o", Name = "gpt-4o-2024-05-13", Description = "2024-05-13",
            KnowledgeCutoff = new DateTime(2023, 10, 1), FunctionCallSupported = true,
            Tokenizer = TokenizerEnum.O200KBase
        },
        new()
        {
            ModelType = "gpt-4o", Name = "chatgpt-4o-latest", Description = "ChatGPT-4o Latest",
            KnowledgeCutoff = new DateTime(2023, 10, 1), FunctionCallSupported = false,
            Tokenizer = TokenizerEnum.O200KBase
        },
        new()
        {
            ModelType = "gpt-4o-mini", Name = "gpt-4o-mini", Description = "最新 (2024-07-18)",
            KnowledgeCutoff = new DateTime(2023, 10, 1), FunctionCallSupported = true,
            Tokenizer = TokenizerEnum.O200KBase
        },
        new()
        {
            ModelType = "gpt-4o-mini", Name = "gpt-4o-mini-2024-07-18", Description = "2024-07-18",
            KnowledgeCutoff = new DateTime(2023, 10, 1), FunctionCallSupported = true,
            Tokenizer = TokenizerEnum.O200KBase
        },
        new()
        {
            ModelType = "oai-o1-preview", Name = "o1-preview", Description = "最新 (2024-09-12)",
            KnowledgeCutoff = new DateTime(2023, 10, 1), FunctionCallSupported = false,
            Tokenizer = TokenizerEnum.O200KBase,
            IsO1 = true
        },
        new()
        {
            ModelType = "oai-o1-preview", Name = "o1-preview-2024-09-12", Description = "2024-09-12",
            KnowledgeCutoff = new DateTime(2023, 10, 1), FunctionCallSupported = false,
            Tokenizer = TokenizerEnum.O200KBase,
            IsO1 = true
        },
        new()
        {
            ModelType = "oai-o1-mini", Name = "o1-mini", Description = "最新 (2024-09-12)",
            KnowledgeCutoff = new DateTime(2023, 10, 1), FunctionCallSupported = false,
            Tokenizer = TokenizerEnum.O200KBase,
            IsO1 = true
        },
        new()
        {
            ModelType = "oai-o1-mini", Name = "o1-mini-2024-09-12", Description = "2024-09-12",
            KnowledgeCutoff = new DateTime(2023, 10, 1), FunctionCallSupported = false,
            Tokenizer = TokenizerEnum.O200KBase,
            IsO1 = true
        },
        new()
        {
            ModelType = "deepseek", Name= "deepseek-chat", Description = "DeepSeek Chat",
            KnowledgeCutoff = new DateTime(2024, 1, 1), FunctionCallSupported = true,
            Tokenizer = TokenizerEnum.Cl100KBase,
            SiliconFlowName = "deepseek-ai/DeepSeek-V3"
        },
        new()
        {
            ModelType = "deepseek", Name= "deepseek-reasoner", Description = "DeepSeek Reasoner",
            KnowledgeCutoff = new DateTime(2024, 1, 1), FunctionCallSupported = false,
            Tokenizer = TokenizerEnum.Cl100KBase,
            SiliconFlowName = "deepseek-ai/DeepSeek-R1",
            NvidiaName = "deepseek-ai/deepseek-r1"
        }
    ];
}