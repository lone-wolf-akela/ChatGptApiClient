
#nullable enable

namespace Gemini
{
    /// <summary>
    /// The output of a batch request. This is returned in the<br/>
    /// `BatchGenerateContentResponse` or the `GenerateContentBatch.output` field.
    /// </summary>
    public sealed partial class GenerateContentBatchOutput
    {
        /// <summary>
        /// Output only. The file ID of the file containing the responses.<br/>
        /// The file will be a JSONL file with a single response per line.<br/>
        /// The responses will be `GenerateContentResponse` messages formatted as<br/>
        /// JSON.<br/>
        /// The responses will be written in the same order as the input requests.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("responsesFile")]
        public string? ResponsesFile { get; set; }

        /// <summary>
        /// Output only. The responses to the requests in the batch. Returned when the batch was<br/>
        /// built using inlined requests. The responses will be in the same order as<br/>
        /// the input requests.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("inlinedResponses")]
        public global::Gemini.InlinedResponses? InlinedResponses { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateContentBatchOutput" /> class.
        /// </summary>
        /// <param name="responsesFile">
        /// Output only. The file ID of the file containing the responses.<br/>
        /// The file will be a JSONL file with a single response per line.<br/>
        /// The responses will be `GenerateContentResponse` messages formatted as<br/>
        /// JSON.<br/>
        /// The responses will be written in the same order as the input requests.<br/>
        /// Included only in responses
        /// </param>
        /// <param name="inlinedResponses">
        /// Output only. The responses to the requests in the batch. Returned when the batch was<br/>
        /// built using inlined requests. The responses will be in the same order as<br/>
        /// the input requests.<br/>
        /// Included only in responses
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public GenerateContentBatchOutput(
            string? responsesFile,
            global::Gemini.InlinedResponses? inlinedResponses)
        {
            this.ResponsesFile = responsesFile;
            this.InlinedResponses = inlinedResponses;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateContentBatchOutput" /> class.
        /// </summary>
        public GenerateContentBatchOutput()
        {
        }
    }
}