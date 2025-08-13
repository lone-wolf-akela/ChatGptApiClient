
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Optional. Default value of the field. Per JSON Schema, this field is intended for<br/>
    /// documentation generators and doesn't affect validation. Thus it's included<br/>
    /// here and ignored so that developers who send schemas with a `default` field<br/>
    /// don't get unknown-field errors.
    /// </summary>
    public sealed partial class SchemaDefault
    {

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();
    }
}