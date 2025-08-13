
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Response for a `BatchGenerateContent` operation.
    /// </summary>
    public sealed partial class BatchGenerateContentResponse
    {
        /// <summary>
        /// Output only. The output of the batch request.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("output")]
        public global::Gemini.GenerateContentBatchOutput? Output { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchGenerateContentResponse" /> class.
        /// </summary>
        /// <param name="output">
        /// Output only. The output of the batch request.<br/>
        /// Included only in responses
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public BatchGenerateContentResponse(
            global::Gemini.GenerateContentBatchOutput? output)
        {
            this.Output = output;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchGenerateContentResponse" /> class.
        /// </summary>
        public BatchGenerateContentResponse()
        {
        }
    }
}