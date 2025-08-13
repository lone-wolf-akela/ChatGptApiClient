
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Request to batch update `Chunk`s.
    /// </summary>
    public sealed partial class BatchUpdateChunksRequest
    {
        /// <summary>
        /// Required. The request messages specifying the `Chunk`s to update.<br/>
        /// A maximum of 100 `Chunk`s can be updated in a batch.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("requests")]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required global::System.Collections.Generic.IList<global::Gemini.UpdateChunkRequest> Requests { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchUpdateChunksRequest" /> class.
        /// </summary>
        /// <param name="requests">
        /// Required. The request messages specifying the `Chunk`s to update.<br/>
        /// A maximum of 100 `Chunk`s can be updated in a batch.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public BatchUpdateChunksRequest(
            global::System.Collections.Generic.IList<global::Gemini.UpdateChunkRequest> requests)
        {
            this.Requests = requests ?? throw new global::System.ArgumentNullException(nameof(requests));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchUpdateChunksRequest" /> class.
        /// </summary>
        public BatchUpdateChunksRequest()
        {
        }
    }
}