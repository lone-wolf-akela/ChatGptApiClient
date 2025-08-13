
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Service-specific metadata associated with the operation.  It typically<br/>
    /// contains progress information and common metadata such as create time.<br/>
    /// Some services might not provide such metadata.  Any method that returns a<br/>
    /// long-running operation should document the metadata type, if any.
    /// </summary>
    public sealed partial class OperationVariant2Metadata
    {

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();
    }
}