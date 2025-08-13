
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Optional. Describes the output from this function in JSON Schema format. The value<br/>
    /// specified by the schema is the response value of the function.<br/>
    /// This field is mutually exclusive with `response`.
    /// </summary>
    public sealed partial class FunctionDeclarationResponseJsonSchema
    {

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();
    }
}