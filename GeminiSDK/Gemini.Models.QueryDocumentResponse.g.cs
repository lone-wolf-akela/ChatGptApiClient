
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Response from `QueryDocument` containing a list of relevant chunks.
    /// </summary>
    public sealed partial class QueryDocumentResponse
    {
        /// <summary>
        /// The returned relevant chunks.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("relevantChunks")]
        public global::System.Collections.Generic.IList<global::Gemini.RelevantChunk>? RelevantChunks { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryDocumentResponse" /> class.
        /// </summary>
        /// <param name="relevantChunks">
        /// The returned relevant chunks.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public QueryDocumentResponse(
            global::System.Collections.Generic.IList<global::Gemini.RelevantChunk>? relevantChunks)
        {
            this.RelevantChunks = relevantChunks;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryDocumentResponse" /> class.
        /// </summary>
        public QueryDocumentResponse()
        {
        }
    }
}