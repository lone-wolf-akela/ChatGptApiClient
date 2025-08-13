
#nullable enable

namespace Gemini
{
    /// <summary>
    /// The response to a single request in the batch.
    /// </summary>
    public sealed partial class InlinedResponse
    {
        /// <summary>
        /// Output only. The error encountered while processing the request.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("error")]
        public global::Gemini.Status? Error { get; set; }

        /// <summary>
        /// Output only. The response to the request.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("response")]
        public global::Gemini.GenerateContentResponse? Response { get; set; }

        /// <summary>
        /// Output only. The metadata associated with the request.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("metadata")]
        public object? Metadata { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="InlinedResponse" /> class.
        /// </summary>
        /// <param name="error">
        /// Output only. The error encountered while processing the request.<br/>
        /// Included only in responses
        /// </param>
        /// <param name="response">
        /// Output only. The response to the request.<br/>
        /// Included only in responses
        /// </param>
        /// <param name="metadata">
        /// Output only. The metadata associated with the request.<br/>
        /// Included only in responses
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public InlinedResponse(
            global::Gemini.Status? error,
            global::Gemini.GenerateContentResponse? response,
            object? metadata)
        {
            this.Error = error;
            this.Response = response;
            this.Metadata = metadata;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InlinedResponse" /> class.
        /// </summary>
        public InlinedResponse()
        {
        }
    }
}