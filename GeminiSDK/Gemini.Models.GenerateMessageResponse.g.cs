
#nullable enable

namespace Gemini
{
    /// <summary>
    /// The response from the model.<br/>
    /// This includes candidate messages and<br/>
    /// conversation history in the form of chronologically-ordered messages.
    /// </summary>
    public sealed partial class GenerateMessageResponse
    {
        /// <summary>
        /// Candidate response messages from the model.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("candidates")]
        public global::System.Collections.Generic.IList<global::Gemini.Message>? Candidates { get; set; }

        /// <summary>
        /// The conversation history used by the model.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("messages")]
        public global::System.Collections.Generic.IList<global::Gemini.Message>? Messages { get; set; }

        /// <summary>
        /// A set of content filtering metadata for the prompt and response<br/>
        /// text.<br/>
        /// This indicates which `SafetyCategory`(s) blocked a<br/>
        /// candidate from this response, the lowest `HarmProbability`<br/>
        /// that triggered a block, and the HarmThreshold setting for that category.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("filters")]
        public global::System.Collections.Generic.IList<global::Gemini.ContentFilter>? Filters { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateMessageResponse" /> class.
        /// </summary>
        /// <param name="candidates">
        /// Candidate response messages from the model.
        /// </param>
        /// <param name="messages">
        /// The conversation history used by the model.
        /// </param>
        /// <param name="filters">
        /// A set of content filtering metadata for the prompt and response<br/>
        /// text.<br/>
        /// This indicates which `SafetyCategory`(s) blocked a<br/>
        /// candidate from this response, the lowest `HarmProbability`<br/>
        /// that triggered a block, and the HarmThreshold setting for that category.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public GenerateMessageResponse(
            global::System.Collections.Generic.IList<global::Gemini.Message>? candidates,
            global::System.Collections.Generic.IList<global::Gemini.Message>? messages,
            global::System.Collections.Generic.IList<global::Gemini.ContentFilter>? filters)
        {
            this.Candidates = candidates;
            this.Messages = messages;
            this.Filters = filters;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateMessageResponse" /> class.
        /// </summary>
        public GenerateMessageResponse()
        {
        }
    }
}