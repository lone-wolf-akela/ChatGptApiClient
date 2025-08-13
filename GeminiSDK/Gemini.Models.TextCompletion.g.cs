
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Output text returned from a model.
    /// </summary>
    public sealed partial class TextCompletion
    {
        /// <summary>
        /// Output only. The generated text returned from the model.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("output")]
        public string? Output { get; set; }

        /// <summary>
        /// Ratings for the safety of a response.<br/>
        /// There is at most one rating per category.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("safetyRatings")]
        public global::System.Collections.Generic.IList<global::Gemini.SafetyRating>? SafetyRatings { get; set; }

        /// <summary>
        /// Output only. Citation information for model-generated `output` in this<br/>
        /// `TextCompletion`.<br/>
        /// This field may be populated with attribution information for any text<br/>
        /// included in the `output`.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("citationMetadata")]
        public global::Gemini.CitationMetadata? CitationMetadata { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TextCompletion" /> class.
        /// </summary>
        /// <param name="output">
        /// Output only. The generated text returned from the model.<br/>
        /// Included only in responses
        /// </param>
        /// <param name="safetyRatings">
        /// Ratings for the safety of a response.<br/>
        /// There is at most one rating per category.
        /// </param>
        /// <param name="citationMetadata">
        /// Output only. Citation information for model-generated `output` in this<br/>
        /// `TextCompletion`.<br/>
        /// This field may be populated with attribution information for any text<br/>
        /// included in the `output`.<br/>
        /// Included only in responses
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public TextCompletion(
            string? output,
            global::System.Collections.Generic.IList<global::Gemini.SafetyRating>? safetyRatings,
            global::Gemini.CitationMetadata? citationMetadata)
        {
            this.Output = output;
            this.SafetyRatings = safetyRatings;
            this.CitationMetadata = citationMetadata;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextCompletion" /> class.
        /// </summary>
        public TextCompletion()
        {
        }
    }
}