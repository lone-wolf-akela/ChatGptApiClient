
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Metadata about the state and progress of creating a tuned model returned from<br/>
    /// the long-running operation
    /// </summary>
    public sealed partial class CreateTunedModelMetadata
    {
        /// <summary>
        /// Name of the tuned model associated with the tuning operation.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("tunedModel")]
        public string? TunedModel { get; set; }

        /// <summary>
        /// The total number of tuning steps.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("totalSteps")]
        public int? TotalSteps { get; set; }

        /// <summary>
        /// The number of steps completed.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("completedSteps")]
        public int? CompletedSteps { get; set; }

        /// <summary>
        /// The completed percentage for the tuning operation.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("completedPercent")]
        public float? CompletedPercent { get; set; }

        /// <summary>
        /// Metrics collected during tuning.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("snapshots")]
        public global::System.Collections.Generic.IList<global::Gemini.TuningSnapshot>? Snapshots { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateTunedModelMetadata" /> class.
        /// </summary>
        /// <param name="tunedModel">
        /// Name of the tuned model associated with the tuning operation.
        /// </param>
        /// <param name="totalSteps">
        /// The total number of tuning steps.
        /// </param>
        /// <param name="completedSteps">
        /// The number of steps completed.
        /// </param>
        /// <param name="completedPercent">
        /// The completed percentage for the tuning operation.
        /// </param>
        /// <param name="snapshots">
        /// Metrics collected during tuning.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public CreateTunedModelMetadata(
            string? tunedModel,
            int? totalSteps,
            int? completedSteps,
            float? completedPercent,
            global::System.Collections.Generic.IList<global::Gemini.TuningSnapshot>? snapshots)
        {
            this.TunedModel = tunedModel;
            this.TotalSteps = totalSteps;
            this.CompletedSteps = completedSteps;
            this.CompletedPercent = completedPercent;
            this.Snapshots = snapshots;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateTunedModelMetadata" /> class.
        /// </summary>
        public CreateTunedModelMetadata()
        {
        }
    }
}