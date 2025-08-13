
#nullable enable

namespace Gemini
{
    /// <summary>
    /// 
    /// </summary>
    public sealed partial class BatchGenerateContentOperationVariant2
    {
        /// <summary>
        /// A resource representing a batch of `GenerateContent` requests.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("metadata")]
        public global::Gemini.GenerateContentBatch? Metadata { get; set; }

        /// <summary>
        /// Response for a `BatchGenerateContent` operation.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("response")]
        public global::Gemini.BatchGenerateContentResponse? Response { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchGenerateContentOperationVariant2" /> class.
        /// </summary>
        /// <param name="metadata">
        /// A resource representing a batch of `GenerateContent` requests.
        /// </param>
        /// <param name="response">
        /// Response for a `BatchGenerateContent` operation.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public BatchGenerateContentOperationVariant2(
            global::Gemini.GenerateContentBatch? metadata,
            global::Gemini.BatchGenerateContentResponse? response)
        {
            this.Metadata = metadata;
            this.Response = response;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchGenerateContentOperationVariant2" /> class.
        /// </summary>
        public BatchGenerateContentOperationVariant2()
        {
        }
    }
}