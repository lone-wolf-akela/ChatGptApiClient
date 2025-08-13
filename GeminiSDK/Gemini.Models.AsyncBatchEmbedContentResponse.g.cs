
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Response for a `BatchGenerateContent` operation.
    /// </summary>
    public sealed partial class AsyncBatchEmbedContentResponse
    {
        /// <summary>
        /// Output only. The output of the batch request.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("output")]
        public global::Gemini.EmbedContentBatchOutput? Output { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncBatchEmbedContentResponse" /> class.
        /// </summary>
        /// <param name="output">
        /// Output only. The output of the batch request.<br/>
        /// Included only in responses
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public AsyncBatchEmbedContentResponse(
            global::Gemini.EmbedContentBatchOutput? output)
        {
            this.Output = output;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncBatchEmbedContentResponse" /> class.
        /// </summary>
        public AsyncBatchEmbedContentResponse()
        {
        }
    }
}