
#nullable enable

namespace Gemini
{
    /// <summary>
    /// A response candidate generated from the model.
    /// </summary>
    public sealed partial class Candidate
    {
        /// <summary>
        /// Output only. Index of the candidate in the list of response candidates.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("index")]
        public int? Index { get; set; }

        /// <summary>
        /// Output only. Generated content returned from the model.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("content")]
        public global::Gemini.Content? Content { get; set; }

        /// <summary>
        /// Optional. Output only. The reason why the model stopped generating tokens.<br/>
        /// If empty, the model has not stopped generating tokens.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("finishReason")]
        [global::System.Text.Json.Serialization.JsonConverter(typeof(global::Gemini.JsonConverters.CandidateFinishReasonJsonConverter))]
        public global::Gemini.CandidateFinishReason? FinishReason { get; set; }

        /// <summary>
        /// List of ratings for the safety of a response candidate.<br/>
        /// There is at most one rating per category.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("safetyRatings")]
        public global::System.Collections.Generic.IList<global::Gemini.SafetyRating>? SafetyRatings { get; set; }

        /// <summary>
        /// Output only. Citation information for model-generated candidate.<br/>
        /// This field may be populated with recitation information for any text<br/>
        /// included in the `content`. These are passages that are "recited" from<br/>
        /// copyrighted material in the foundational LLM's training data.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("citationMetadata")]
        public global::Gemini.CitationMetadata? CitationMetadata { get; set; }

        /// <summary>
        /// Output only. Token count for this candidate.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("tokenCount")]
        public int? TokenCount { get; set; }

        /// <summary>
        /// Output only. Attribution information for sources that contributed to a grounded answer.<br/>
        /// This field is populated for `GenerateAnswer` calls.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("groundingAttributions")]
        public global::System.Collections.Generic.IList<global::Gemini.GroundingAttribution>? GroundingAttributions { get; set; }

        /// <summary>
        /// Output only. Grounding metadata for the candidate.<br/>
        /// This field is populated for `GenerateContent` calls.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("groundingMetadata")]
        public global::Gemini.GroundingMetadata? GroundingMetadata { get; set; }

        /// <summary>
        /// Output only. Average log probability score of the candidate.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("avgLogprobs")]
        public double? AvgLogprobs { get; set; }

        /// <summary>
        /// Output only. Log-likelihood scores for the response tokens and top tokens<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("logprobsResult")]
        public global::Gemini.LogprobsResult? LogprobsResult { get; set; }

        /// <summary>
        /// Output only. Metadata related to url context retrieval tool.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("urlContextMetadata")]
        public global::Gemini.UrlContextMetadata? UrlContextMetadata { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Candidate" /> class.
        /// </summary>
        /// <param name="index">
        /// Output only. Index of the candidate in the list of response candidates.<br/>
        /// Included only in responses
        /// </param>
        /// <param name="content">
        /// Output only. Generated content returned from the model.<br/>
        /// Included only in responses
        /// </param>
        /// <param name="finishReason">
        /// Optional. Output only. The reason why the model stopped generating tokens.<br/>
        /// If empty, the model has not stopped generating tokens.<br/>
        /// Included only in responses
        /// </param>
        /// <param name="safetyRatings">
        /// List of ratings for the safety of a response candidate.<br/>
        /// There is at most one rating per category.
        /// </param>
        /// <param name="citationMetadata">
        /// Output only. Citation information for model-generated candidate.<br/>
        /// This field may be populated with recitation information for any text<br/>
        /// included in the `content`. These are passages that are "recited" from<br/>
        /// copyrighted material in the foundational LLM's training data.<br/>
        /// Included only in responses
        /// </param>
        /// <param name="tokenCount">
        /// Output only. Token count for this candidate.<br/>
        /// Included only in responses
        /// </param>
        /// <param name="groundingAttributions">
        /// Output only. Attribution information for sources that contributed to a grounded answer.<br/>
        /// This field is populated for `GenerateAnswer` calls.<br/>
        /// Included only in responses
        /// </param>
        /// <param name="groundingMetadata">
        /// Output only. Grounding metadata for the candidate.<br/>
        /// This field is populated for `GenerateContent` calls.<br/>
        /// Included only in responses
        /// </param>
        /// <param name="avgLogprobs">
        /// Output only. Average log probability score of the candidate.<br/>
        /// Included only in responses
        /// </param>
        /// <param name="logprobsResult">
        /// Output only. Log-likelihood scores for the response tokens and top tokens<br/>
        /// Included only in responses
        /// </param>
        /// <param name="urlContextMetadata">
        /// Output only. Metadata related to url context retrieval tool.<br/>
        /// Included only in responses
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public Candidate(
            int? index,
            global::Gemini.Content? content,
            global::Gemini.CandidateFinishReason? finishReason,
            global::System.Collections.Generic.IList<global::Gemini.SafetyRating>? safetyRatings,
            global::Gemini.CitationMetadata? citationMetadata,
            int? tokenCount,
            global::System.Collections.Generic.IList<global::Gemini.GroundingAttribution>? groundingAttributions,
            global::Gemini.GroundingMetadata? groundingMetadata,
            double? avgLogprobs,
            global::Gemini.LogprobsResult? logprobsResult,
            global::Gemini.UrlContextMetadata? urlContextMetadata)
        {
            this.Index = index;
            this.Content = content;
            this.FinishReason = finishReason;
            this.SafetyRatings = safetyRatings;
            this.CitationMetadata = citationMetadata;
            this.TokenCount = tokenCount;
            this.GroundingAttributions = groundingAttributions;
            this.GroundingMetadata = groundingMetadata;
            this.AvgLogprobs = avgLogprobs;
            this.LogprobsResult = logprobsResult;
            this.UrlContextMetadata = urlContextMetadata;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Candidate" /> class.
        /// </summary>
        public Candidate()
        {
        }
    }
}