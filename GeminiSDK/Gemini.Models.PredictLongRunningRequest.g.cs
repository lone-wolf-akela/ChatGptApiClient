
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Request message for [PredictionService.PredictLongRunning].
    /// </summary>
    public sealed partial class PredictLongRunningRequest
    {
        /// <summary>
        /// Required. The instances that are the input to the prediction call.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("instances")]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required global::System.Collections.Generic.IList<object> Instances { get; set; }

        /// <summary>
        /// Optional. The parameters that govern the prediction call.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("parameters")]
        public object? Parameters { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PredictLongRunningRequest" /> class.
        /// </summary>
        /// <param name="instances">
        /// Required. The instances that are the input to the prediction call.
        /// </param>
        /// <param name="parameters">
        /// Optional. The parameters that govern the prediction call.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public PredictLongRunningRequest(
            global::System.Collections.Generic.IList<object> instances,
            object? parameters)
        {
            this.Instances = instances ?? throw new global::System.ArgumentNullException(nameof(instances));
            this.Parameters = parameters;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PredictLongRunningRequest" /> class.
        /// </summary>
        public PredictLongRunningRequest()
        {
        }
    }
}