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

using ChatGptApiClientV2.Claude;
using DocumentFormat.OpenXml.Drawing.Charts;
using Flurl;
using HandyControl.Tools.Extension;
using HarmonyLib;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using OpenAI;
using OpenAI.Chat;
using ReflectionMagic;
using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.Media.Protection.PlayReady;
using ChatGptApiClientV2.ServerEndpoint;
using static ChatGptApiClientV2.ChatCompletionRequest;
using static ChatGptApiClientV2.IMessage;

// ReSharper disable PropertyCanBeMadeInitOnly.Global
namespace ChatGptApiClientV2;

public class ServerEndpointOptions
{
    public enum ServiceType
    {
        OpenAI,
        Claude,
        DeepSeek,
        Google,
        OtherOpenAICompat,
        Custom
    }

    public ServiceType Service { get; set; }
    public string Endpoint { get; set; } = "";
    public string Key { get; set; } = "";
    public string Model { get; set; } = "";
    public int? MaxTokens { get; set; }
    public float? PresencePenalty { get; set; }
    public float? Temperature { get; set; }
    public float? TopP { get; set; }
    public IEnumerable<ToolType>? Tools { get; set; }
    public string? UserId { get; set; }
    public IEnumerable<string>? StopSequences { get; set; }
    public bool EnableThinking { get; set; }
    public int ThinkingLength { get; set; }

    public bool SystemPromptNotSupported { get; set; }
    public bool TemperatureSettingNotSupported { get; set; }
    public bool TopPSettingNotSupported { get; set; }
    public bool PenaltySettingNotSupported { get; set; }
    public bool NeedChinesePunctuationNormalization { get; set; }
    public bool StreamingNotSupported { get; set; }
}

public interface IServerEndpoint
{
    public static IServerEndpoint BuildServerEndpoint(ServerEndpointOptions options)
    {
        return options.Service switch
        {
            ServerEndpointOptions.ServiceType.Claude => new ClaudeEndpoint(options),
            ServerEndpointOptions.ServiceType.Google => new GeminiEndpoint(options),
            _ => new OpenAIEndpoint(options)
        };
    }

    public Task BuildSession(ChatCompletionRequest session, CancellationToken cancellationToken = default);
    public IAsyncEnumerable<string> Streaming(CancellationToken cancellationToken = default);
    public string SystemFingerprint { get; }
    public AssistantMessage ResponseMessage { get; }
    public IEnumerable<ToolCallType> ToolCalls { get; }
}




