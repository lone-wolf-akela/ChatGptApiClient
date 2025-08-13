
#nullable enable

namespace Gemini
{
    /// <summary>
    /// A predicted `FunctionCall` returned from the model that contains<br/>
    /// a string representing the `FunctionDeclaration.name` with the<br/>
    /// arguments and their values.
    /// </summary>
    public sealed partial class FunctionCall
    {
        /// <summary>
        /// Optional. The unique id of the function call. If populated, the client to execute the<br/>
        /// `function_call` and return the response with the matching `id`.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("id")]
        public string? Id { get; set; }

        /// <summary>
        /// Required. The name of the function to call.<br/>
        /// Must be a-z, A-Z, 0-9, or contain underscores and dashes, with a maximum<br/>
        /// length of 63.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("name")]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required string Name { get; set; }

        /// <summary>
        /// Optional. The function parameters and values in JSON object format.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("args")]
        public object? Args { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionCall" /> class.
        /// </summary>
        /// <param name="id">
        /// Optional. The unique id of the function call. If populated, the client to execute the<br/>
        /// `function_call` and return the response with the matching `id`.
        /// </param>
        /// <param name="name">
        /// Required. The name of the function to call.<br/>
        /// Must be a-z, A-Z, 0-9, or contain underscores and dashes, with a maximum<br/>
        /// length of 63.
        /// </param>
        /// <param name="args">
        /// Optional. The function parameters and values in JSON object format.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public FunctionCall(
            string name,
            string? id,
            object? args)
        {
            this.Name = name ?? throw new global::System.ArgumentNullException(nameof(name));
            this.Id = id;
            this.Args = args;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionCall" /> class.
        /// </summary>
        public FunctionCall()
        {
        }
    }
}