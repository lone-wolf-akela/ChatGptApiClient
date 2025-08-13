
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Response from `QueryCorpus` containing a list of relevant chunks.
    /// </summary>
    public sealed partial class QueryCorpusResponse
    {
        /// <summary>
        /// The relevant chunks.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("relevantChunks")]
        public global::System.Collections.Generic.IList<global::Gemini.RelevantChunk>? RelevantChunks { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryCorpusResponse" /> class.
        /// </summary>
        /// <param name="relevantChunks">
        /// The relevant chunks.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public QueryCorpusResponse(
            global::System.Collections.Generic.IList<global::Gemini.RelevantChunk>? relevantChunks)
        {
            this.RelevantChunks = relevantChunks;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryCorpusResponse" /> class.
        /// </summary>
        public QueryCorpusResponse()
        {
        }
    }
}