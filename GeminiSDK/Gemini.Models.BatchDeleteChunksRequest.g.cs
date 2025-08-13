
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Request to batch delete `Chunk`s.
    /// </summary>
    public sealed partial class BatchDeleteChunksRequest
    {
        /// <summary>
        /// Required. The request messages specifying the `Chunk`s to delete.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("requests")]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required global::System.Collections.Generic.IList<global::Gemini.DeleteChunkRequest> Requests { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchDeleteChunksRequest" /> class.
        /// </summary>
        /// <param name="requests">
        /// Required. The request messages specifying the `Chunk`s to delete.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public BatchDeleteChunksRequest(
            global::System.Collections.Generic.IList<global::Gemini.DeleteChunkRequest> requests)
        {
            this.Requests = requests ?? throw new global::System.ArgumentNullException(nameof(requests));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchDeleteChunksRequest" /> class.
        /// </summary>
        public BatchDeleteChunksRequest()
        {
        }
    }
}