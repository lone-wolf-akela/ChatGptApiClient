
#nullable enable

namespace Gemini
{
    /// <summary>
    /// 
    /// </summary>
    public sealed partial class AsyncBatchEmbedContentOperationVariant2
    {
        /// <summary>
        /// A resource representing a batch of `EmbedContent` requests.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("metadata")]
        public global::Gemini.EmbedContentBatch? Metadata { get; set; }

        /// <summary>
        /// Response for a `BatchGenerateContent` operation.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("response")]
        public global::Gemini.AsyncBatchEmbedContentResponse? Response { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncBatchEmbedContentOperationVariant2" /> class.
        /// </summary>
        /// <param name="metadata">
        /// A resource representing a batch of `EmbedContent` requests.
        /// </param>
        /// <param name="response">
        /// Response for a `BatchGenerateContent` operation.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public AsyncBatchEmbedContentOperationVariant2(
            global::Gemini.EmbedContentBatch? metadata,
            global::Gemini.AsyncBatchEmbedContentResponse? response)
        {
            this.Metadata = metadata;
            this.Response = response;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncBatchEmbedContentOperationVariant2" /> class.
        /// </summary>
        public AsyncBatchEmbedContentOperationVariant2()
        {
        }
    }
}