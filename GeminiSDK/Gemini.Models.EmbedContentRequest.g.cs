
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Request containing the `Content` for the model to embed.
    /// </summary>
    public sealed partial class EmbedContentRequest
    {
        /// <summary>
        /// Required. The model's resource name. This serves as an ID for the Model to use.<br/>
        /// This name should match a model name returned by the `ListModels` method.<br/>
        /// Format: `models/{model}`
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("model")]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required string Model { get; set; }

        /// <summary>
        /// Required. The content to embed. Only the `parts.text` fields will be counted.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("content")]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required global::Gemini.Content Content { get; set; }

        /// <summary>
        /// Optional. Optional task type for which the embeddings will be used. Not supported on<br/>
        /// earlier models (`models/embedding-001`).
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("taskType")]
        [global::System.Text.Json.Serialization.JsonConverter(typeof(global::Gemini.JsonConverters.TaskTypeJsonConverter))]
        public global::Gemini.TaskType? TaskType { get; set; }

        /// <summary>
        /// Optional. An optional title for the text. Only applicable when TaskType is<br/>
        /// `RETRIEVAL_DOCUMENT`.<br/>
        /// Note: Specifying a `title` for `RETRIEVAL_DOCUMENT` provides better quality<br/>
        /// embeddings for retrieval.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("title")]
        public string? Title { get; set; }

        /// <summary>
        /// Optional. Optional reduced dimension for the output embedding. If set, excessive<br/>
        /// values in the output embedding are truncated from the end. Supported by<br/>
        /// newer models since 2024 only. You cannot set this value if using the<br/>
        /// earlier model (`models/embedding-001`).
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("outputDimensionality")]
        public int? OutputDimensionality { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbedContentRequest" /> class.
        /// </summary>
        /// <param name="model">
        /// Required. The model's resource name. This serves as an ID for the Model to use.<br/>
        /// This name should match a model name returned by the `ListModels` method.<br/>
        /// Format: `models/{model}`
        /// </param>
        /// <param name="content">
        /// Required. The content to embed. Only the `parts.text` fields will be counted.
        /// </param>
        /// <param name="taskType">
        /// Optional. Optional task type for which the embeddings will be used. Not supported on<br/>
        /// earlier models (`models/embedding-001`).
        /// </param>
        /// <param name="title">
        /// Optional. An optional title for the text. Only applicable when TaskType is<br/>
        /// `RETRIEVAL_DOCUMENT`.<br/>
        /// Note: Specifying a `title` for `RETRIEVAL_DOCUMENT` provides better quality<br/>
        /// embeddings for retrieval.
        /// </param>
        /// <param name="outputDimensionality">
        /// Optional. Optional reduced dimension for the output embedding. If set, excessive<br/>
        /// values in the output embedding are truncated from the end. Supported by<br/>
        /// newer models since 2024 only. You cannot set this value if using the<br/>
        /// earlier model (`models/embedding-001`).
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public EmbedContentRequest(
            string model,
            global::Gemini.Content content,
            global::Gemini.TaskType? taskType,
            string? title,
            int? outputDimensionality)
        {
            this.Model = model ?? throw new global::System.ArgumentNullException(nameof(model));
            this.Content = content ?? throw new global::System.ArgumentNullException(nameof(content));
            this.TaskType = taskType;
            this.Title = title;
            this.OutputDimensionality = outputDimensionality;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbedContentRequest" /> class.
        /// </summary>
        public EmbedContentRequest()
        {
        }
    }
}