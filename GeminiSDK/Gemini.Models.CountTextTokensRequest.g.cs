
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Counts the number of tokens in the `prompt` sent to a model.<br/>
    /// Models may tokenize text differently, so each model may return a different<br/>
    /// `token_count`.
    /// </summary>
    public sealed partial class CountTextTokensRequest
    {
        /// <summary>
        /// Required. The free-form input text given to the model as a prompt.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("prompt")]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required global::Gemini.TextPrompt Prompt { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CountTextTokensRequest" /> class.
        /// </summary>
        /// <param name="prompt">
        /// Required. The free-form input text given to the model as a prompt.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public CountTextTokensRequest(
            global::Gemini.TextPrompt prompt)
        {
            this.Prompt = prompt ?? throw new global::System.ArgumentNullException(nameof(prompt));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CountTextTokensRequest" /> class.
        /// </summary>
        public CountTextTokensRequest()
        {
        }
    }
}