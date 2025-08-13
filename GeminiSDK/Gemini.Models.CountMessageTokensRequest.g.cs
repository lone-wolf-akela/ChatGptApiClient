
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Counts the number of tokens in the `prompt` sent to a model.<br/>
    /// Models may tokenize text differently, so each model may return a different<br/>
    /// `token_count`.
    /// </summary>
    public sealed partial class CountMessageTokensRequest
    {
        /// <summary>
        /// Required. The prompt, whose token count is to be returned.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("prompt")]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required global::Gemini.MessagePrompt Prompt { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CountMessageTokensRequest" /> class.
        /// </summary>
        /// <param name="prompt">
        /// Required. The prompt, whose token count is to be returned.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public CountMessageTokensRequest(
            global::Gemini.MessagePrompt prompt)
        {
            this.Prompt = prompt ?? throw new global::System.ArgumentNullException(nameof(prompt));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CountMessageTokensRequest" /> class.
        /// </summary>
        public CountMessageTokensRequest()
        {
        }
    }
}