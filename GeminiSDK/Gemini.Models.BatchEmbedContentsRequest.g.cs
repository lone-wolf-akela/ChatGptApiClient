
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Batch request to get embeddings from the model for a list of prompts.
    /// </summary>
    public sealed partial class BatchEmbedContentsRequest
    {
        /// <summary>
        /// Required. Embed requests for the batch. The model in each of these requests must<br/>
        /// match the model specified `BatchEmbedContentsRequest.model`.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("requests")]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required global::System.Collections.Generic.IList<global::Gemini.EmbedContentRequest> Requests { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchEmbedContentsRequest" /> class.
        /// </summary>
        /// <param name="requests">
        /// Required. Embed requests for the batch. The model in each of these requests must<br/>
        /// match the model specified `BatchEmbedContentsRequest.model`.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public BatchEmbedContentsRequest(
            global::System.Collections.Generic.IList<global::Gemini.EmbedContentRequest> requests)
        {
            this.Requests = requests ?? throw new global::System.ArgumentNullException(nameof(requests));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchEmbedContentsRequest" /> class.
        /// </summary>
        public BatchEmbedContentsRequest()
        {
        }
    }
}