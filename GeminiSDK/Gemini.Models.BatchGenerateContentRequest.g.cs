
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Request for a `BatchGenerateContent` operation.
    /// </summary>
    public sealed partial class BatchGenerateContentRequest
    {
        /// <summary>
        /// Required. The batch to create.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("batch")]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required global::Gemini.GenerateContentBatch Batch { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchGenerateContentRequest" /> class.
        /// </summary>
        /// <param name="batch">
        /// Required. The batch to create.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public BatchGenerateContentRequest(
            global::Gemini.GenerateContentBatch batch)
        {
            this.Batch = batch ?? throw new global::System.ArgumentNullException(nameof(batch));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchGenerateContentRequest" /> class.
        /// </summary>
        public BatchGenerateContentRequest()
        {
        }
    }
}