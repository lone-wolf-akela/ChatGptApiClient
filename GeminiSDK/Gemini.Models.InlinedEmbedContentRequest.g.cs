
#nullable enable

namespace Gemini
{
    /// <summary>
    /// The request to be processed in the batch.
    /// </summary>
    public sealed partial class InlinedEmbedContentRequest
    {
        /// <summary>
        /// Required. The request to be processed in the batch.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("request")]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required global::Gemini.EmbedContentRequest Request { get; set; }

        /// <summary>
        /// Optional. The metadata to be associated with the request.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("metadata")]
        public object? Metadata { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="InlinedEmbedContentRequest" /> class.
        /// </summary>
        /// <param name="request">
        /// Required. The request to be processed in the batch.
        /// </param>
        /// <param name="metadata">
        /// Optional. The metadata to be associated with the request.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public InlinedEmbedContentRequest(
            global::Gemini.EmbedContentRequest request,
            object? metadata)
        {
            this.Request = request ?? throw new global::System.ArgumentNullException(nameof(request));
            this.Metadata = metadata;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InlinedEmbedContentRequest" /> class.
        /// </summary>
        public InlinedEmbedContentRequest()
        {
        }
    }
}