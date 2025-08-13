
#nullable enable

namespace Gemini
{
    /// <summary>
    /// A set of the feedback metadata the prompt specified in<br/>
    /// `GenerateContentRequest.content`.
    /// </summary>
    public sealed partial class PromptFeedback
    {
        /// <summary>
        /// Optional. If set, the prompt was blocked and no candidates are returned.<br/>
        /// Rephrase the prompt.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("blockReason")]
        [global::System.Text.Json.Serialization.JsonConverter(typeof(global::Gemini.JsonConverters.PromptFeedbackBlockReasonJsonConverter))]
        public global::Gemini.PromptFeedbackBlockReason? BlockReason { get; set; }

        /// <summary>
        /// Ratings for safety of the prompt.<br/>
        /// There is at most one rating per category.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("safetyRatings")]
        public global::System.Collections.Generic.IList<global::Gemini.SafetyRating>? SafetyRatings { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PromptFeedback" /> class.
        /// </summary>
        /// <param name="blockReason">
        /// Optional. If set, the prompt was blocked and no candidates are returned.<br/>
        /// Rephrase the prompt.
        /// </param>
        /// <param name="safetyRatings">
        /// Ratings for safety of the prompt.<br/>
        /// There is at most one rating per category.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public PromptFeedback(
            global::Gemini.PromptFeedbackBlockReason? blockReason,
            global::System.Collections.Generic.IList<global::Gemini.SafetyRating>? safetyRatings)
        {
            this.BlockReason = blockReason;
            this.SafetyRatings = safetyRatings;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PromptFeedback" /> class.
        /// </summary>
        public PromptFeedback()
        {
        }
    }
}