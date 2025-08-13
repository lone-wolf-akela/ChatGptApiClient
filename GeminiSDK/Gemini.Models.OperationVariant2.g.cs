
#nullable enable

namespace Gemini
{
    /// <summary>
    /// 
    /// </summary>
    public sealed partial class OperationVariant2
    {
        /// <summary>
        /// Service-specific metadata associated with the operation.  It typically<br/>
        /// contains progress information and common metadata such as create time.<br/>
        /// Some services might not provide such metadata.  Any method that returns a<br/>
        /// long-running operation should document the metadata type, if any.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("metadata")]
        public object? Metadata { get; set; }

        /// <summary>
        /// The normal, successful response of the operation.  If the original<br/>
        /// method returns no data on success, such as `Delete`, the response is<br/>
        /// `google.protobuf.Empty`.  If the original method is standard<br/>
        /// `Get`/`Create`/`Update`, the response should be the resource.  For other<br/>
        /// methods, the response should have the type `XxxResponse`, where `Xxx`<br/>
        /// is the original method name.  For example, if the original method name<br/>
        /// is `TakeSnapshot()`, the inferred response type is<br/>
        /// `TakeSnapshotResponse`.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("response")]
        public object? Response { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationVariant2" /> class.
        /// </summary>
        /// <param name="metadata">
        /// Service-specific metadata associated with the operation.  It typically<br/>
        /// contains progress information and common metadata such as create time.<br/>
        /// Some services might not provide such metadata.  Any method that returns a<br/>
        /// long-running operation should document the metadata type, if any.
        /// </param>
        /// <param name="response">
        /// The normal, successful response of the operation.  If the original<br/>
        /// method returns no data on success, such as `Delete`, the response is<br/>
        /// `google.protobuf.Empty`.  If the original method is standard<br/>
        /// `Get`/`Create`/`Update`, the response should be the resource.  For other<br/>
        /// methods, the response should have the type `XxxResponse`, where `Xxx`<br/>
        /// is the original method name.  For example, if the original method name<br/>
        /// is `TakeSnapshot()`, the inferred response type is<br/>
        /// `TakeSnapshotResponse`.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public OperationVariant2(
            object? metadata,
            object? response)
        {
            this.Metadata = metadata;
            this.Response = response;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationVariant2" /> class.
        /// </summary>
        public OperationVariant2()
        {
        }
    }
}