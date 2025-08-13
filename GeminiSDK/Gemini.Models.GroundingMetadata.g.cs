
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Metadata returned to client when grounding is enabled.
    /// </summary>
    public sealed partial class GroundingMetadata
    {
        /// <summary>
        /// Optional. Google search entry for the following-up web searches.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("searchEntryPoint")]
        public global::Gemini.SearchEntryPoint? SearchEntryPoint { get; set; }

        /// <summary>
        /// List of supporting references retrieved from specified grounding source.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("groundingChunks")]
        public global::System.Collections.Generic.IList<global::Gemini.GroundingChunk>? GroundingChunks { get; set; }

        /// <summary>
        /// List of grounding support.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("groundingSupports")]
        public global::System.Collections.Generic.IList<global::Gemini.GroundingSupport>? GroundingSupports { get; set; }

        /// <summary>
        /// Metadata related to retrieval in the grounding flow.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("retrievalMetadata")]
        public global::Gemini.RetrievalMetadata? RetrievalMetadata { get; set; }

        /// <summary>
        /// Web search queries for the following-up web search.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("webSearchQueries")]
        public global::System.Collections.Generic.IList<string>? WebSearchQueries { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GroundingMetadata" /> class.
        /// </summary>
        /// <param name="searchEntryPoint">
        /// Optional. Google search entry for the following-up web searches.
        /// </param>
        /// <param name="groundingChunks">
        /// List of supporting references retrieved from specified grounding source.
        /// </param>
        /// <param name="groundingSupports">
        /// List of grounding support.
        /// </param>
        /// <param name="retrievalMetadata">
        /// Metadata related to retrieval in the grounding flow.
        /// </param>
        /// <param name="webSearchQueries">
        /// Web search queries for the following-up web search.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public GroundingMetadata(
            global::Gemini.SearchEntryPoint? searchEntryPoint,
            global::System.Collections.Generic.IList<global::Gemini.GroundingChunk>? groundingChunks,
            global::System.Collections.Generic.IList<global::Gemini.GroundingSupport>? groundingSupports,
            global::Gemini.RetrievalMetadata? retrievalMetadata,
            global::System.Collections.Generic.IList<string>? webSearchQueries)
        {
            this.SearchEntryPoint = searchEntryPoint;
            this.GroundingChunks = groundingChunks;
            this.GroundingSupports = groundingSupports;
            this.RetrievalMetadata = retrievalMetadata;
            this.WebSearchQueries = webSearchQueries;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GroundingMetadata" /> class.
        /// </summary>
        public GroundingMetadata()
        {
        }
    }
}