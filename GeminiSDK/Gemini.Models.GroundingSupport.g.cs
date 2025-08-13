
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Grounding support.
    /// </summary>
    public sealed partial class GroundingSupport
    {
        /// <summary>
        /// Segment of the content this support belongs to.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("segment")]
        public global::Gemini.Segment? Segment { get; set; }

        /// <summary>
        /// A list of indices (into 'grounding_chunk') specifying the<br/>
        /// citations associated with the claim. For instance [1,3,4] means<br/>
        /// that grounding_chunk[1], grounding_chunk[3],<br/>
        /// grounding_chunk[4] are the retrieved content attributed to the claim.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("groundingChunkIndices")]
        public global::System.Collections.Generic.IList<int>? GroundingChunkIndices { get; set; }

        /// <summary>
        /// Confidence score of the support references. Ranges from 0 to 1. 1 is the<br/>
        /// most confident. This list must have the same size as the<br/>
        /// grounding_chunk_indices.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("confidenceScores")]
        public global::System.Collections.Generic.IList<float>? ConfidenceScores { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GroundingSupport" /> class.
        /// </summary>
        /// <param name="segment">
        /// Segment of the content this support belongs to.
        /// </param>
        /// <param name="groundingChunkIndices">
        /// A list of indices (into 'grounding_chunk') specifying the<br/>
        /// citations associated with the claim. For instance [1,3,4] means<br/>
        /// that grounding_chunk[1], grounding_chunk[3],<br/>
        /// grounding_chunk[4] are the retrieved content attributed to the claim.
        /// </param>
        /// <param name="confidenceScores">
        /// Confidence score of the support references. Ranges from 0 to 1. 1 is the<br/>
        /// most confident. This list must have the same size as the<br/>
        /// grounding_chunk_indices.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public GroundingSupport(
            global::Gemini.Segment? segment,
            global::System.Collections.Generic.IList<int>? groundingChunkIndices,
            global::System.Collections.Generic.IList<float>? confidenceScores)
        {
            this.Segment = segment;
            this.GroundingChunkIndices = groundingChunkIndices;
            this.ConfidenceScores = confidenceScores;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GroundingSupport" /> class.
        /// </summary>
        public GroundingSupport()
        {
        }
    }
}