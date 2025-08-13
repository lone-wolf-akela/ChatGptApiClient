
#nullable enable

namespace Gemini
{
    /// <summary>
    /// A fine-tuned model created using ModelService.CreateTunedModel.
    /// </summary>
    public sealed partial class TunedModel
    {
        /// <summary>
        /// Optional. TunedModel to use as the starting point for training the new model.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("tunedModelSource")]
        public global::Gemini.TunedModelSource? TunedModelSource { get; set; }

        /// <summary>
        /// Immutable. The name of the `Model` to tune.<br/>
        /// Example: `models/gemini-1.5-flash-001`
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("baseModel")]
        public string? BaseModel { get; set; }

        /// <summary>
        /// Output only. The tuned model name. A unique name will be generated on create.<br/>
        /// Example: `tunedModels/az2mb0bpw6i`<br/>
        /// If display_name is set on create, the id portion of the name will be set<br/>
        /// by concatenating the words of the display_name with hyphens and adding a<br/>
        /// random portion for uniqueness.<br/>
        /// Example:<br/>
        ///  * display_name = `Sentence Translator`<br/>
        ///  * name = `tunedModels/sentence-translator-u3b7m`<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("name")]
        public string? Name { get; set; }

        /// <summary>
        /// Optional. The name to display for this model in user interfaces.<br/>
        /// The display name must be up to 40 characters including spaces.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("displayName")]
        public string? DisplayName { get; set; }

        /// <summary>
        /// Optional. A short description of this model.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("description")]
        public string? Description { get; set; }

        /// <summary>
        /// Optional. Controls the randomness of the output.<br/>
        /// Values can range over `[0.0,1.0]`, inclusive. A value closer to `1.0` will<br/>
        /// produce responses that are more varied, while a value closer to `0.0` will<br/>
        /// typically result in less surprising responses from the model.<br/>
        /// This value specifies default to be the one used by the base model while<br/>
        /// creating the model.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("temperature")]
        public float? Temperature { get; set; }

        /// <summary>
        /// Optional. For Nucleus sampling.<br/>
        /// Nucleus sampling considers the smallest set of tokens whose probability<br/>
        /// sum is at least `top_p`.<br/>
        /// This value specifies default to be the one used by the base model while<br/>
        /// creating the model.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("topP")]
        public float? TopP { get; set; }

        /// <summary>
        /// Optional. For Top-k sampling.<br/>
        /// Top-k sampling considers the set of `top_k` most probable tokens.<br/>
        /// This value specifies default to be used by the backend while making the<br/>
        /// call to the model.<br/>
        /// This value specifies default to be the one used by the base model while<br/>
        /// creating the model.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("topK")]
        public int? TopK { get; set; }

        /// <summary>
        /// Output only. The state of the tuned model.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("state")]
        [global::System.Text.Json.Serialization.JsonConverter(typeof(global::Gemini.JsonConverters.TunedModelStateJsonConverter))]
        public global::Gemini.TunedModelState? State { get; set; }

        /// <summary>
        /// Output only. The timestamp when this model was created.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("createTime")]
        public global::System.DateTime? CreateTime { get; set; }

        /// <summary>
        /// Output only. The timestamp when this model was updated.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("updateTime")]
        public global::System.DateTime? UpdateTime { get; set; }

        /// <summary>
        /// Required. The tuning task that creates the tuned model.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("tuningTask")]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required global::Gemini.TuningTask TuningTask { get; set; }

        /// <summary>
        /// Optional. List of project numbers that have read access to the tuned model.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("readerProjectNumbers")]
        public global::System.Collections.Generic.IList<string>? ReaderProjectNumbers { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TunedModel" /> class.
        /// </summary>
        /// <param name="tunedModelSource">
        /// Optional. TunedModel to use as the starting point for training the new model.
        /// </param>
        /// <param name="baseModel">
        /// Immutable. The name of the `Model` to tune.<br/>
        /// Example: `models/gemini-1.5-flash-001`
        /// </param>
        /// <param name="name">
        /// Output only. The tuned model name. A unique name will be generated on create.<br/>
        /// Example: `tunedModels/az2mb0bpw6i`<br/>
        /// If display_name is set on create, the id portion of the name will be set<br/>
        /// by concatenating the words of the display_name with hyphens and adding a<br/>
        /// random portion for uniqueness.<br/>
        /// Example:<br/>
        ///  * display_name = `Sentence Translator`<br/>
        ///  * name = `tunedModels/sentence-translator-u3b7m`<br/>
        /// Included only in responses
        /// </param>
        /// <param name="displayName">
        /// Optional. The name to display for this model in user interfaces.<br/>
        /// The display name must be up to 40 characters including spaces.
        /// </param>
        /// <param name="description">
        /// Optional. A short description of this model.
        /// </param>
        /// <param name="temperature">
        /// Optional. Controls the randomness of the output.<br/>
        /// Values can range over `[0.0,1.0]`, inclusive. A value closer to `1.0` will<br/>
        /// produce responses that are more varied, while a value closer to `0.0` will<br/>
        /// typically result in less surprising responses from the model.<br/>
        /// This value specifies default to be the one used by the base model while<br/>
        /// creating the model.
        /// </param>
        /// <param name="topP">
        /// Optional. For Nucleus sampling.<br/>
        /// Nucleus sampling considers the smallest set of tokens whose probability<br/>
        /// sum is at least `top_p`.<br/>
        /// This value specifies default to be the one used by the base model while<br/>
        /// creating the model.
        /// </param>
        /// <param name="topK">
        /// Optional. For Top-k sampling.<br/>
        /// Top-k sampling considers the set of `top_k` most probable tokens.<br/>
        /// This value specifies default to be used by the backend while making the<br/>
        /// call to the model.<br/>
        /// This value specifies default to be the one used by the base model while<br/>
        /// creating the model.
        /// </param>
        /// <param name="state">
        /// Output only. The state of the tuned model.<br/>
        /// Included only in responses
        /// </param>
        /// <param name="createTime">
        /// Output only. The timestamp when this model was created.<br/>
        /// Included only in responses
        /// </param>
        /// <param name="updateTime">
        /// Output only. The timestamp when this model was updated.<br/>
        /// Included only in responses
        /// </param>
        /// <param name="tuningTask">
        /// Required. The tuning task that creates the tuned model.
        /// </param>
        /// <param name="readerProjectNumbers">
        /// Optional. List of project numbers that have read access to the tuned model.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public TunedModel(
            global::Gemini.TuningTask tuningTask,
            global::Gemini.TunedModelSource? tunedModelSource,
            string? baseModel,
            string? name,
            string? displayName,
            string? description,
            float? temperature,
            float? topP,
            int? topK,
            global::Gemini.TunedModelState? state,
            global::System.DateTime? createTime,
            global::System.DateTime? updateTime,
            global::System.Collections.Generic.IList<string>? readerProjectNumbers)
        {
            this.TuningTask = tuningTask ?? throw new global::System.ArgumentNullException(nameof(tuningTask));
            this.TunedModelSource = tunedModelSource;
            this.BaseModel = baseModel;
            this.Name = name;
            this.DisplayName = displayName;
            this.Description = description;
            this.Temperature = temperature;
            this.TopP = topP;
            this.TopK = topK;
            this.State = state;
            this.CreateTime = createTime;
            this.UpdateTime = updateTime;
            this.ReaderProjectNumbers = readerProjectNumbers;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TunedModel" /> class.
        /// </summary>
        public TunedModel()
        {
        }
    }
}