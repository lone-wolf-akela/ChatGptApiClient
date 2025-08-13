
#nullable enable

namespace Gemini
{
    /// <summary>
    /// A response from `CountTextTokens`.<br/>
    /// It returns the model's `token_count` for the `prompt`.
    /// </summary>
    public sealed partial class CountTextTokensResponse
    {
        /// <summary>
        /// The number of tokens that the `model` tokenizes the `prompt` into.<br/>
        /// Always non-negative.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("tokenCount")]
        public int? TokenCount { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CountTextTokensResponse" /> class.
        /// </summary>
        /// <param name="tokenCount">
        /// The number of tokens that the `model` tokenizes the `prompt` into.<br/>
        /// Always non-negative.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public CountTextTokensResponse(
            int? tokenCount)
        {
            this.TokenCount = tokenCount;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CountTextTokensResponse" /> class.
        /// </summary>
        public CountTextTokensResponse()
        {
        }
    }
}