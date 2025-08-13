
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Safety feedback for an entire request.<br/>
    /// This field is populated if content in the input and/or response is blocked<br/>
    /// due to safety settings. SafetyFeedback may not exist for every HarmCategory.<br/>
    /// Each SafetyFeedback will return the safety settings used by the request as<br/>
    /// well as the lowest HarmProbability that should be allowed in order to return<br/>
    /// a result.
    /// </summary>
    public sealed partial class SafetyFeedback
    {
        /// <summary>
        /// Safety rating evaluated from content.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("rating")]
        public global::Gemini.SafetyRating? Rating { get; set; }

        /// <summary>
        /// Safety settings applied to the request.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("setting")]
        public global::Gemini.SafetySetting? Setting { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SafetyFeedback" /> class.
        /// </summary>
        /// <param name="rating">
        /// Safety rating evaluated from content.
        /// </param>
        /// <param name="setting">
        /// Safety settings applied to the request.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public SafetyFeedback(
            global::Gemini.SafetyRating? rating,
            global::Gemini.SafetySetting? setting)
        {
            this.Rating = rating;
            this.Setting = setting;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SafetyFeedback" /> class.
        /// </summary>
        public SafetyFeedback()
        {
        }
    }
}