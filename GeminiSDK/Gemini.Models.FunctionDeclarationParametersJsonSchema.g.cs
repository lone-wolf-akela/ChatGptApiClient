
#nullable enable

namespace Gemini
{
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
    public sealed partial class FunctionDeclarationParametersJsonSchema
    {

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();
    }
}