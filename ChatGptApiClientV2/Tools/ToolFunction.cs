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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using NJsonSchema;
using NJsonSchema.Generation;
using NJsonSchema.NewtonsoftJson.Generation;

namespace ChatGptApiClientV2.Tools;

public static class AllToolCollections
{
    public static readonly List<IToolCollection> ToolList =
    [
        new DalleImageGen(),
        new GoogleSearch(),
        new BingSearch(),
        new WolframAlpha(),
        new PythonTool()
    ];
}

public interface IToolCollection
{
    public List<IToolFunction> Funcs { get; }
    public string DisplayName { get; }
}

public interface IToolFunction
{
    public string? Description { get; }
    public string Name { get; }
    public Type ArgsType { get; }

    public JsonSchema Parameters
    {
        get
        {
            var settings = new NewtonsoftJsonSchemaGeneratorSettings();
            var schema = new JsonSchema();
            var resolver = new JsonSchemaResolver(schema, settings);
            var generator = new JsonSchemaGenerator(settings);
            generator.Generate(schema, ArgsType, resolver);
            return schema;
        }
    }

    public class ToolResult(ToolMessage msg, bool responeRequired)
    {
        public ToolMessage Message { get; } = msg;
        public bool ResponeRequired { get; set; } = responeRequired;
    }

    public Task<ToolResult> Action(SystemState state, Guid sessionId, string toolcallId, string argstr,
        CancellationToken cancellationToken = default);

    IEnumerable<Block> GetToolcallMessage(SystemState state, Guid sessionId, string argstr, string toolcallId);

    public ChatCompletionRequest.ToolType GetToolRequest() => new()
    {
        Function = new ChatCompletionRequest.ToolType.FunctionType
        {
            Name = Name,
            Parameters = Parameters,
            Description = Description
        }
    };
}