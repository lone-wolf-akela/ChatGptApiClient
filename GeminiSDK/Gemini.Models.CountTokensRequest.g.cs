
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Counts the number of tokens in the `prompt` sent to a model.<br/>
    /// Models may tokenize text differently, so each model may return a different<br/>
    /// `token_count`.
    /// </summary>
    public sealed partial class CountTokensRequest
    {
        /// <summary>
        /// Optional. The input given to the model as a prompt. This field is ignored when<br/>
        /// `generate_content_request` is set.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("contents")]
        public global::System.Collections.Generic.IList<global::Gemini.Content>? Contents { get; set; }

        /// <summary>
        /// Optional. The overall input given to the `Model`. This includes the prompt as well as<br/>
        /// other model steering information like [system<br/>
        /// instructions](https://ai.google.dev/gemini-api/docs/system-instructions),<br/>
        /// and/or function declarations for [function<br/>
        /// calling](https://ai.google.dev/gemini-api/docs/function-calling).<br/>
        /// `Model`s/`Content`s and `generate_content_request`s are mutually<br/>
        /// exclusive. You can either send `Model` + `Content`s or a<br/>
        /// `generate_content_request`, but never both.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("generateContentRequest")]
        public global::Gemini.GenerateContentRequest? GenerateContentRequest { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CountTokensRequest" /> class.
        /// </summary>
        /// <param name="contents">
        /// Optional. The input given to the model as a prompt. This field is ignored when<br/>
        /// `generate_content_request` is set.
        /// </param>
        /// <param name="generateContentRequest">
        /// Optional. The overall input given to the `Model`. This includes the prompt as well as<br/>
        /// other model steering information like [system<br/>
        /// instructions](https://ai.google.dev/gemini-api/docs/system-instructions),<br/>
        /// and/or function declarations for [function<br/>
        /// calling](https://ai.google.dev/gemini-api/docs/function-calling).<br/>
        /// `Model`s/`Content`s and `generate_content_request`s are mutually<br/>
        /// exclusive. You can either send `Model` + `Content`s or a<br/>
        /// `generate_content_request`, but never both.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public CountTokensRequest(
            global::System.Collections.Generic.IList<global::Gemini.Content>? contents,
            global::Gemini.GenerateContentRequest? generateContentRequest)
        {
            this.Contents = contents;
            this.GenerateContentRequest = generateContentRequest;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CountTokensRequest" /> class.
        /// </summary>
        public CountTokensRequest()
        {
        }
    }
}