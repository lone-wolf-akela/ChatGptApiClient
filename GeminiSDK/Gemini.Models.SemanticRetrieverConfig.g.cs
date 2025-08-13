
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Configuration for retrieving grounding content from a `Corpus` or<br/>
    /// `Document` created using the Semantic Retriever API.
    /// </summary>
    public sealed partial class SemanticRetrieverConfig
    {
        /// <summary>
        /// Required. Name of the resource for retrieval. Example: `corpora/123` or<br/>
        /// `corpora/123/documents/abc`.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("source")]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required string Source { get; set; }

        /// <summary>
        /// Required. Query to use for matching `Chunk`s in the given resource by similarity.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("query")]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required global::Gemini.Content Query { get; set; }

        /// <summary>
        /// Optional. Filters for selecting `Document`s and/or `Chunk`s from the resource.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("metadataFilters")]
        public global::System.Collections.Generic.IList<global::Gemini.MetadataFilter>? MetadataFilters { get; set; }

        /// <summary>
        /// Optional. Maximum number of relevant `Chunk`s to retrieve.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("maxChunksCount")]
        public int? MaxChunksCount { get; set; }

        /// <summary>
        /// Optional. Minimum relevance score for retrieved relevant `Chunk`s.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("minimumRelevanceScore")]
        public float? MinimumRelevanceScore { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SemanticRetrieverConfig" /> class.
        /// </summary>
        /// <param name="source">
        /// Required. Name of the resource for retrieval. Example: `corpora/123` or<br/>
        /// `corpora/123/documents/abc`.
        /// </param>
        /// <param name="query">
        /// Required. Query to use for matching `Chunk`s in the given resource by similarity.
        /// </param>
        /// <param name="metadataFilters">
        /// Optional. Filters for selecting `Document`s and/or `Chunk`s from the resource.
        /// </param>
        /// <param name="maxChunksCount">
        /// Optional. Maximum number of relevant `Chunk`s to retrieve.
        /// </param>
        /// <param name="minimumRelevanceScore">
        /// Optional. Minimum relevance score for retrieved relevant `Chunk`s.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public SemanticRetrieverConfig(
            string source,
            global::Gemini.Content query,
            global::System.Collections.Generic.IList<global::Gemini.MetadataFilter>? metadataFilters,
            int? maxChunksCount,
            float? minimumRelevanceScore)
        {
            this.Source = source ?? throw new global::System.ArgumentNullException(nameof(source));
            this.Query = query ?? throw new global::System.ArgumentNullException(nameof(query));
            this.MetadataFilters = metadataFilters;
            this.MaxChunksCount = maxChunksCount;
            this.MinimumRelevanceScore = minimumRelevanceScore;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SemanticRetrieverConfig" /> class.
        /// </summary>
        public SemanticRetrieverConfig()
        {
        }
    }
}