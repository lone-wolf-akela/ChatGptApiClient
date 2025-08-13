
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Identifier for the source contributing to this attribution.
    /// </summary>
    public sealed partial class AttributionSourceId
    {
        /// <summary>
        /// Identifier for an inline passage.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("groundingPassage")]
        public global::Gemini.GroundingPassageId? GroundingPassage { get; set; }

        /// <summary>
        /// Identifier for a `Chunk` fetched via Semantic Retriever.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("semanticRetrieverChunk")]
        public global::Gemini.SemanticRetrieverChunk? SemanticRetrieverChunk { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributionSourceId" /> class.
        /// </summary>
        /// <param name="groundingPassage">
        /// Identifier for an inline passage.
        /// </param>
        /// <param name="semanticRetrieverChunk">
        /// Identifier for a `Chunk` fetched via Semantic Retriever.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public AttributionSourceId(
            global::Gemini.GroundingPassageId? groundingPassage,
            global::Gemini.SemanticRetrieverChunk? semanticRetrieverChunk)
        {
            this.GroundingPassage = groundingPassage;
            this.SemanticRetrieverChunk = semanticRetrieverChunk;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributionSourceId" /> class.
        /// </summary>
        public AttributionSourceId()
        {
        }
    }
}