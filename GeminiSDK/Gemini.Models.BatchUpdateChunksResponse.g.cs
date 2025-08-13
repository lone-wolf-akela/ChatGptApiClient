
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Response from `BatchUpdateChunks` containing a list of updated `Chunk`s.
    /// </summary>
    public sealed partial class BatchUpdateChunksResponse
    {
        /// <summary>
        /// `Chunk`s updated.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("chunks")]
        public global::System.Collections.Generic.IList<global::Gemini.Chunk>? Chunks { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchUpdateChunksResponse" /> class.
        /// </summary>
        /// <param name="chunks">
        /// `Chunk`s updated.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public BatchUpdateChunksResponse(
            global::System.Collections.Generic.IList<global::Gemini.Chunk>? chunks)
        {
            this.Chunks = chunks;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchUpdateChunksResponse" /> class.
        /// </summary>
        public BatchUpdateChunksResponse()
        {
        }
    }
}