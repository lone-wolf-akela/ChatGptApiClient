
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Response from `BatchCreateChunks` containing a list of created `Chunk`s.
    /// </summary>
    public sealed partial class BatchCreateChunksResponse
    {
        /// <summary>
        /// `Chunk`s created.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("chunks")]
        public global::System.Collections.Generic.IList<global::Gemini.Chunk>? Chunks { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchCreateChunksResponse" /> class.
        /// </summary>
        /// <param name="chunks">
        /// `Chunk`s created.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public BatchCreateChunksResponse(
            global::System.Collections.Generic.IList<global::Gemini.Chunk>? chunks)
        {
            this.Chunks = chunks;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchCreateChunksResponse" /> class.
        /// </summary>
        public BatchCreateChunksResponse()
        {
        }
    }
}