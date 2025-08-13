
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Structured representation of a function declaration as defined by the<br/>
    /// [OpenAPI 3.03 specification](https://spec.openapis.org/oas/v3.0.3). Included<br/>
    /// in this declaration are the function name and parameters. This<br/>
    /// FunctionDeclaration is a representation of a block of code that can be used<br/>
    /// as a `Tool` by the model and executed by the client.
    /// </summary>
    public sealed partial class FunctionDeclaration
    {
        /// <summary>
        /// Required. The name of the function.<br/>
        /// Must be a-z, A-Z, 0-9, or contain underscores and dashes, with a maximum<br/>
        /// length of 63.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("name")]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required string Name { get; set; }

        /// <summary>
        /// Required. A brief description of the function.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("description")]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required string Description { get; set; }

        /// <summary>
        /// Optional. Describes the parameters to this function. Reflects the Open API 3.03<br/>
        /// Parameter Object string Key: the name of the parameter. Parameter names are<br/>
        /// case sensitive. Schema Value: the Schema defining the type used for the<br/>
        /// parameter.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("parameters")]
        public global::Gemini.Schema? Parameters { get; set; }

        /// <summary>
        /// Optional. Describes the parameters to the function in JSON Schema format. The schema<br/>
        /// must describe an object where the properties are the parameters to the<br/>
        /// function. For example:<br/>
        /// ```<br/>
        /// {<br/>
        ///   "type": "object",<br/>
        ///   "properties": {<br/>
        ///     "name": { "type": "string" },<br/>
        ///     "age": { "type": "integer" }<br/>
        ///   },<br/>
        ///   "additionalProperties": false,<br/>
        ///   "required": ["name", "age"],<br/>
        ///   "propertyOrdering": ["name", "age"]<br/>
        /// }<br/>
        /// ```<br/>
        /// This field is mutually exclusive with `parameters`.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("parametersJsonSchema")]
        public object? ParametersJsonSchema { get; set; }

        /// <summary>
        /// Optional. Describes the output from this function in JSON Schema format. Reflects the<br/>
        /// Open API 3.03 Response Object. The Schema defines the type used for the<br/>
        /// response value of the function.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("response")]
        public global::Gemini.Schema? Response { get; set; }

        /// <summary>
        /// Optional. Describes the output from this function in JSON Schema format. The value<br/>
        /// specified by the schema is the response value of the function.<br/>
        /// This field is mutually exclusive with `response`.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("responseJsonSchema")]
        public object? ResponseJsonSchema { get; set; }

        /// <summary>
        /// Optional. Specifies the function Behavior.<br/>
        /// Currently only supported by the BidiGenerateContent method.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("behavior")]
        [global::System.Text.Json.Serialization.JsonConverter(typeof(global::Gemini.JsonConverters.FunctionDeclarationBehaviorJsonConverter))]
        public global::Gemini.FunctionDeclarationBehavior? Behavior { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionDeclaration" /> class.
        /// </summary>
        /// <param name="name">
        /// Required. The name of the function.<br/>
        /// Must be a-z, A-Z, 0-9, or contain underscores and dashes, with a maximum<br/>
        /// length of 63.
        /// </param>
        /// <param name="description">
        /// Required. A brief description of the function.
        /// </param>
        /// <param name="parameters">
        /// Optional. Describes the parameters to this function. Reflects the Open API 3.03<br/>
        /// Parameter Object string Key: the name of the parameter. Parameter names are<br/>
        /// case sensitive. Schema Value: the Schema defining the type used for the<br/>
        /// parameter.
        /// </param>
        /// <param name="parametersJsonSchema">
        /// Optional. Describes the parameters to the function in JSON Schema format. The schema<br/>
        /// must describe an object where the properties are the parameters to the<br/>
        /// function. For example:<br/>
        /// ```<br/>
        /// {<br/>
        ///   "type": "object",<br/>
        ///   "properties": {<br/>
        ///     "name": { "type": "string" },<br/>
        ///     "age": { "type": "integer" }<br/>
        ///   },<br/>
        ///   "additionalProperties": false,<br/>
        ///   "required": ["name", "age"],<br/>
        ///   "propertyOrdering": ["name", "age"]<br/>
        /// }<br/>
        /// ```<br/>
        /// This field is mutually exclusive with `parameters`.
        /// </param>
        /// <param name="response">
        /// Optional. Describes the output from this function in JSON Schema format. Reflects the<br/>
        /// Open API 3.03 Response Object. The Schema defines the type used for the<br/>
        /// response value of the function.
        /// </param>
        /// <param name="responseJsonSchema">
        /// Optional. Describes the output from this function in JSON Schema format. The value<br/>
        /// specified by the schema is the response value of the function.<br/>
        /// This field is mutually exclusive with `response`.
        /// </param>
        /// <param name="behavior">
        /// Optional. Specifies the function Behavior.<br/>
        /// Currently only supported by the BidiGenerateContent method.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public FunctionDeclaration(
            string name,
            string description,
            global::Gemini.Schema? parameters,
            object? parametersJsonSchema,
            global::Gemini.Schema? response,
            object? responseJsonSchema,
            global::Gemini.FunctionDeclarationBehavior? behavior)
        {
            this.Name = name ?? throw new global::System.ArgumentNullException(nameof(name));
            this.Description = description ?? throw new global::System.ArgumentNullException(nameof(description));
            this.Parameters = parameters;
            this.ParametersJsonSchema = parametersJsonSchema;
            this.Response = response;
            this.ResponseJsonSchema = responseJsonSchema;
            this.Behavior = behavior;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionDeclaration" /> class.
        /// </summary>
        public FunctionDeclaration()
        {
        }
    }
}