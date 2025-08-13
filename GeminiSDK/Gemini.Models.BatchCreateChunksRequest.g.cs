
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Request to batch create `Chunk`s.
    /// </summary>
    public sealed partial class BatchCreateChunksRequest
    {
        /// <summary>
        /// Required. The request messages specifying the `Chunk`s to create.<br/>
        /// A maximum of 100 `Chunk`s can be created in a batch.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("requests")]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required global::System.Collections.Generic.IList<global::Gemini.CreateChunkRequest> Requests { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchCreateChunksRequest" /> class.
        /// </summary>
        /// <param name="requests">
        /// Required. The request messages specifying the `Chunk`s to create.<br/>
        /// A maximum of 100 `Chunk`s can be created in a batch.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public BatchCreateChunksRequest(
            global::System.Collections.Generic.IList<global::Gemini.CreateChunkRequest> requests)
        {
            this.Requests = requests ?? throw new global::System.ArgumentNullException(nameof(requests));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchCreateChunksRequest" /> class.
        /// </summary>
        public BatchCreateChunksRequest()
        {
        }
    }
}