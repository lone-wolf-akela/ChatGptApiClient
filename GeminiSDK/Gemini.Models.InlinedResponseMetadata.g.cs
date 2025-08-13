
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Output only. The metadata associated with the request.<br/>
    /// Included only in responses
    /// </summary>
    public sealed partial class InlinedResponseMetadata
    {

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();
    }
}