using NJsonSchema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;
using NJsonSchema.Generation;
using Newtonsoft.Json.Serialization;
using ChatGptApiClientV2.Tools;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Windows.Controls;

namespace ChatGptApiClientV2
{
    public static class AllToolCollections
    {
        public static readonly List<IToolCollection> ToolList =
        [
            new DalleImageGen(),
            new GoogleSearch(),
            new BingSearch(),
            new WolframAlpha(),
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
                var settings = new JsonSchemaGeneratorSettings();
                var schema = new JsonSchema();
                var resolver = new JsonSchemaResolver(schema, settings);
                var generator = new JsonSchemaGenerator(settings);
                generator.Generate(schema, ArgsType, resolver);
                return schema;
            }
        }
        public Task<ToolMessage> Action(SystemState state, string argstr);
        IEnumerable<Block> GetToolcallMessage(SystemState state, string argstr, string toolcallId);
        public ChatCompletionRequest.ToolType GetToolRequest() => new() 
        {
            Function = new () { Name = Name, Parameters = Parameters, Description = Description }
        };
    }
}
