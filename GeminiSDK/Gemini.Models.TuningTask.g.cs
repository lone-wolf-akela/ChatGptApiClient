
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Tuning tasks that create tuned models.
    /// </summary>
    public sealed partial class TuningTask
    {
        /// <summary>
        /// Output only. The timestamp when tuning this model started.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("startTime")]
        public global::System.DateTime? StartTime { get; set; }

        /// <summary>
        /// Output only. The timestamp when tuning this model completed.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("completeTime")]
        public global::System.DateTime? CompleteTime { get; set; }

        /// <summary>
        /// Output only. Metrics collected during tuning.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("snapshots")]
        public global::System.Collections.Generic.IList<global::Gemini.TuningSnapshot>? Snapshots { get; set; }

        /// <summary>
        /// Required. Input only. Immutable. The model training data.<br/>
        /// Included only in requests
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("trainingData")]
        public global::Gemini.Dataset? TrainingData { get; set; }

        /// <summary>
        /// Immutable. Hyperparameters controlling the tuning process. If not provided, default<br/>
        /// values will be used.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("hyperparameters")]
        public global::Gemini.Hyperparameters? Hyperparameters { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TuningTask" /> class.
        /// </summary>
        /// <param name="startTime">
        /// Output only. The timestamp when tuning this model started.<br/>
        /// Included only in responses
        /// </param>
        /// <param name="completeTime">
        /// Output only. The timestamp when tuning this model completed.<br/>
        /// Included only in responses
        /// </param>
        /// <param name="snapshots">
        /// Output only. Metrics collected during tuning.<br/>
        /// Included only in responses
        /// </param>
        /// <param name="trainingData">
        /// Required. Input only. Immutable. The model training data.<br/>
        /// Included only in requests
        /// </param>
        /// <param name="hyperparameters">
        /// Immutable. Hyperparameters controlling the tuning process. If not provided, default<br/>
        /// values will be used.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public TuningTask(
            global::System.DateTime? startTime,
            global::System.DateTime? completeTime,
            global::System.Collections.Generic.IList<global::Gemini.TuningSnapshot>? snapshots,
            global::Gemini.Dataset? trainingData,
            global::Gemini.Hyperparameters? hyperparameters)
        {
            this.StartTime = startTime;
            this.CompleteTime = completeTime;
            this.Snapshots = snapshots;
            this.TrainingData = trainingData;
            this.Hyperparameters = hyperparameters;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TuningTask" /> class.
        /// </summary>
        public TuningTask()
        {
        }
    }
}