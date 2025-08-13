
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Metadata related to retrieval in the grounding flow.
    /// </summary>
    public sealed partial class RetrievalMetadata
    {
        /// <summary>
        /// Optional. Score indicating how likely information from google search could help<br/>
        /// answer the prompt. The score is in the range [0, 1], where 0 is the least<br/>
        /// likely and 1 is the most likely. This score is only populated when<br/>
        /// google search grounding and dynamic retrieval is enabled. It will be<br/>
        /// compared to the threshold to determine whether to trigger google search.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("googleSearchDynamicRetrievalScore")]
        public float? GoogleSearchDynamicRetrievalScore { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RetrievalMetadata" /> class.
        /// </summary>
        /// <param name="googleSearchDynamicRetrievalScore">
        /// Optional. Score indicating how likely information from google search could help<br/>
        /// answer the prompt. The score is in the range [0, 1], where 0 is the least<br/>
        /// likely and 1 is the most likely. This score is only populated when<br/>
        /// google search grounding and dynamic retrieval is enabled. It will be<br/>
        /// compared to the threshold to determine whether to trigger google search.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public RetrievalMetadata(
            float? googleSearchDynamicRetrievalScore)
        {
            this.GoogleSearchDynamicRetrievalScore = googleSearchDynamicRetrievalScore;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RetrievalMetadata" /> class.
        /// </summary>
        public RetrievalMetadata()
        {
        }
    }
}