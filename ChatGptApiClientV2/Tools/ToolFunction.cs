using System;
using System.Collections.Generic;
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
    public Task<ToolMessage> Action(SystemState state, string argstr);
    IEnumerable<Block> GetToolcallMessage(SystemState state, string argstr, string toolcallId);
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