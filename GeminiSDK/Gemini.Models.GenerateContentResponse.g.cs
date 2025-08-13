
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Response from the model supporting multiple candidate responses.<br/>
    /// Safety ratings and content filtering are reported for both<br/>
    /// prompt in `GenerateContentResponse.prompt_feedback` and for each candidate<br/>
    /// in `finish_reason` and in `safety_ratings`. The API:<br/>
    ///  - Returns either all requested candidates or none of them<br/>
    ///  - Returns no candidates at all only if there was something wrong with the<br/>
    ///    prompt (check `prompt_feedback`)<br/>
    ///  - Reports feedback on each candidate in `finish_reason` and<br/>
    ///    `safety_ratings`.
    /// </summary>
    public sealed partial class GenerateContentResponse
    {
        /// <summary>
        /// Candidate responses from the model.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("candidates")]
        public global::System.Collections.Generic.IList<global::Gemini.Candidate>? Candidates { get; set; }

        /// <summary>
        /// Returns the prompt's feedback related to the content filters.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("promptFeedback")]
        public global::Gemini.PromptFeedback? PromptFeedback { get; set; }

        /// <summary>
        /// Output only. Metadata on the generation requests' token usage.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("usageMetadata")]
        public global::Gemini.UsageMetadata? UsageMetadata { get; set; }

        /// <summary>
        /// Output only. The model version used to generate the response.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("modelVersion")]
        public string? ModelVersion { get; set; }

        /// <summary>
        /// Output only. response_id is used to identify each response.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("responseId")]
        public string? ResponseId { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateContentResponse" /> class.
        /// </summary>
        /// <param name="candidates">
        /// Candidate responses from the model.
        /// </param>
        /// <param name="promptFeedback">
        /// Returns the prompt's feedback related to the content filters.
        /// </param>
        /// <param name="usageMetadata">
        /// Output only. Metadata on the generation requests' token usage.<br/>
        /// Included only in responses
        /// </param>
        /// <param name="modelVersion">
        /// Output only. The model version used to generate the response.<br/>
        /// Included only in responses
        /// </param>
        /// <param name="responseId">
        /// Output only. response_id is used to identify each response.<br/>
        /// Included only in responses
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public GenerateContentResponse(
            global::System.Collections.Generic.IList<global::Gemini.Candidate>? candidates,
            global::Gemini.PromptFeedback? promptFeedback,
            global::Gemini.UsageMetadata? usageMetadata,
            string? modelVersion,
            string? responseId)
        {
            this.Candidates = candidates;
            this.PromptFeedback = promptFeedback;
            this.UsageMetadata = usageMetadata;
            this.ModelVersion = modelVersion;
            this.ResponseId = responseId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateContentResponse" /> class.
        /// </summary>
        public GenerateContentResponse()
        {
        }
    }
}