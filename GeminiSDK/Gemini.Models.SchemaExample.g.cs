
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Optional. Example of the object. Will only populated when the object is the root.
    /// </summary>
    public sealed partial class SchemaExample
    {

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();
    }
}