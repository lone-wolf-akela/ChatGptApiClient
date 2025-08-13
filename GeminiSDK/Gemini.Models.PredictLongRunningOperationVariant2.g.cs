
#nullable enable

namespace Gemini
{
    /// <summary>
    /// 
    /// </summary>
    public sealed partial class PredictLongRunningOperationVariant2
    {
        /// <summary>
        /// Metadata for PredictLongRunning long running operations.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("metadata")]
        public object? Metadata { get; set; }

        /// <summary>
        /// Response message for [PredictionService.PredictLongRunning]
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("response")]
        public global::Gemini.PredictLongRunningResponse? Response { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PredictLongRunningOperationVariant2" /> class.
        /// </summary>
        /// <param name="metadata">
        /// Metadata for PredictLongRunning long running operations.
        /// </param>
        /// <param name="response">
        /// Response message for [PredictionService.PredictLongRunning]
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public PredictLongRunningOperationVariant2(
            object? metadata,
            global::Gemini.PredictLongRunningResponse? response)
        {
            this.Metadata = metadata;
            this.Response = response;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PredictLongRunningOperationVariant2" /> class.
        /// </summary>
        public PredictLongRunningOperationVariant2()
        {
        }
    }
}