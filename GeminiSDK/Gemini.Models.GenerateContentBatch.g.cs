
#nullable enable

namespace Gemini
{
    /// <summary>
    /// A resource representing a batch of `GenerateContent` requests.
    /// </summary>
    public sealed partial class GenerateContentBatch
    {
        /// <summary>
        /// Required. The name of the `Model` to use for generating the completion.<br/>
        /// Format: `models/{model}`.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("model")]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required string Model { get; set; }

        /// <summary>
        /// Output only. Identifier. Resource name of the batch.<br/>
        /// Format: `batches/{batch_id}`.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("name")]
        public string? Name { get; set; }

        /// <summary>
        /// Required. The user-defined name of this batch.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("displayName")]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required string DisplayName { get; set; }

        /// <summary>
        /// Required. Input configuration of the instances on which batch processing<br/>
        /// are performed.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("inputConfig")]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required global::Gemini.InputConfig InputConfig { get; set; }

        /// <summary>
        /// Output only. The output of the batch request.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("output")]
        public global::Gemini.GenerateContentBatchOutput? Output { get; set; }

        /// <summary>
        /// Output only. The time at which the batch was created.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("createTime")]
        public global::System.DateTime? CreateTime { get; set; }

        /// <summary>
        /// Output only. The time at which the batch processing completed.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("endTime")]
        public global::System.DateTime? EndTime { get; set; }

        /// <summary>
        /// Output only. The time at which the batch was last updated.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("updateTime")]
        public global::System.DateTime? UpdateTime { get; set; }

        /// <summary>
        /// Output only. Stats about the batch.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("batchStats")]
        public global::Gemini.BatchStats? BatchStats { get; set; }

        /// <summary>
        /// Output only. The state of the batch.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("state")]
        [global::System.Text.Json.Serialization.JsonConverter(typeof(global::Gemini.JsonConverters.BatchStateJsonConverter))]
        public global::Gemini.BatchState? State { get; set; }

        /// <summary>
        /// Optional. The priority of the batch. Batches with a higher priority value will be<br/>
        /// processed before batches with a lower priority value. Negative values are<br/>
        /// allowed. Default is 0.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("priority")]
        public string? Priority { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateContentBatch" /> class.
        /// </summary>
        /// <param name="model">
        /// Required. The name of the `Model` to use for generating the completion.<br/>
        /// Format: `models/{model}`.
        /// </param>
        /// <param name="name">
        /// Output only. Identifier. Resource name of the batch.<br/>
        /// Format: `batches/{batch_id}`.<br/>
        /// Included only in responses
        /// </param>
        /// <param name="displayName">
        /// Required. The user-defined name of this batch.
        /// </param>
        /// <param name="inputConfig">
        /// Required. Input configuration of the instances on which batch processing<br/>
        /// are performed.
        /// </param>
        /// <param name="output">
        /// Output only. The output of the batch request.<br/>
        /// Included only in responses
        /// </param>
        /// <param name="createTime">
        /// Output only. The time at which the batch was created.<br/>
        /// Included only in responses
        /// </param>
        /// <param name="endTime">
        /// Output only. The time at which the batch processing completed.<br/>
        /// Included only in responses
        /// </param>
        /// <param name="updateTime">
        /// Output only. The time at which the batch was last updated.<br/>
        /// Included only in responses
        /// </param>
        /// <param name="batchStats">
        /// Output only. Stats about the batch.<br/>
        /// Included only in responses
        /// </param>
        /// <param name="state">
        /// Output only. The state of the batch.<br/>
        /// Included only in responses
        /// </param>
        /// <param name="priority">
        /// Optional. The priority of the batch. Batches with a higher priority value will be<br/>
        /// processed before batches with a lower priority value. Negative values are<br/>
        /// allowed. Default is 0.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public GenerateContentBatch(
            string model,
            string displayName,
            global::Gemini.InputConfig inputConfig,
            string? name,
            global::Gemini.GenerateContentBatchOutput? output,
            global::System.DateTime? createTime,
            global::System.DateTime? endTime,
            global::System.DateTime? updateTime,
            global::Gemini.BatchStats? batchStats,
            global::Gemini.BatchState? state,
            string? priority)
        {
            this.Model = model ?? throw new global::System.ArgumentNullException(nameof(model));
            this.DisplayName = displayName ?? throw new global::System.ArgumentNullException(nameof(displayName));
            this.InputConfig = inputConfig ?? throw new global::System.ArgumentNullException(nameof(inputConfig));
            this.Name = name;
            this.Output = output;
            this.CreateTime = createTime;
            this.EndTime = endTime;
            this.UpdateTime = updateTime;
            this.BatchStats = batchStats;
            this.State = state;
            this.Priority = priority;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateContentBatch" /> class.
        /// </summary>
        public GenerateContentBatch()
        {
        }
    }
}