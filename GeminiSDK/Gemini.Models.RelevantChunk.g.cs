
#nullable enable

namespace Gemini
{
    /// <summary>
    /// The information for a chunk relevant to a query.
    /// </summary>
    public sealed partial class RelevantChunk
    {
        /// <summary>
        /// `Chunk` relevance to the query.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("chunkRelevanceScore")]
        public float? ChunkRelevanceScore { get; set; }

        /// <summary>
        /// `Chunk` associated with the query.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("chunk")]
        public global::Gemini.Chunk? Chunk { get; set; }

        /// <summary>
        /// `Document` associated with the chunk.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("document")]
        public global::Gemini.Document? Document { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RelevantChunk" /> class.
        /// </summary>
        /// <param name="chunkRelevanceScore">
        /// `Chunk` relevance to the query.
        /// </param>
        /// <param name="chunk">
        /// `Chunk` associated with the query.
        /// </param>
        /// <param name="document">
        /// `Document` associated with the chunk.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public RelevantChunk(
            float? chunkRelevanceScore,
            global::Gemini.Chunk? chunk,
            global::Gemini.Document? document)
        {
            this.ChunkRelevanceScore = chunkRelevanceScore;
            this.Chunk = chunk;
            this.Document = document;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelevantChunk" /> class.
        /// </summary>
        public RelevantChunk()
        {
        }
    }
}