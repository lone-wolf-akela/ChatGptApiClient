
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Information about a Generative Language Model.
    /// </summary>
    public sealed partial class Model
    {
        /// <summary>
        /// Required. The resource name of the `Model`. Refer to [Model<br/>
        /// variants](https://ai.google.dev/gemini-api/docs/models/gemini#model-variations)<br/>
        /// for all allowed values.<br/>
        /// Format: `models/{model}` with a `{model}` naming convention of:<br/>
        /// * "{base_model_id}-{version}"<br/>
        /// Examples:<br/>
        /// * `models/gemini-1.5-flash-001`
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("name")]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required string Name { get; set; }

        /// <summary>
        /// Required. The name of the base model, pass this to the generation request.<br/>
        /// Examples:<br/>
        /// * `gemini-1.5-flash`
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("baseModelId")]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required string BaseModelId { get; set; }

        /// <summary>
        /// Required. The version number of the model.<br/>
        /// This represents the major version (`1.0` or `1.5`)
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("version")]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required string Version { get; set; }

        /// <summary>
        /// The human-readable name of the model. E.g. "Gemini 1.5 Flash".<br/>
        /// The name can be up to 128 characters long and can consist of any UTF-8<br/>
        /// characters.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("displayName")]
        public string? DisplayName { get; set; }

        /// <summary>
        /// A short description of the model.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("description")]
        public string? Description { get; set; }

        /// <summary>
        /// Maximum number of input tokens allowed for this model.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("inputTokenLimit")]
        public int? InputTokenLimit { get; set; }

        /// <summary>
        /// Maximum number of output tokens available for this model.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("outputTokenLimit")]
        public int? OutputTokenLimit { get; set; }

        /// <summary>
        /// The model's supported generation methods.<br/>
        /// The corresponding API method names are defined as Pascal case<br/>
        /// strings, such as `generateMessage` and `generateContent`.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("supportedGenerationMethods")]
        public global::System.Collections.Generic.IList<string>? SupportedGenerationMethods { get; set; }

        /// <summary>
        /// Controls the randomness of the output.<br/>
        /// Values can range over `[0.0,max_temperature]`, inclusive. A higher value<br/>
        /// will produce responses that are more varied, while a value closer to `0.0`<br/>
        /// will typically result in less surprising responses from the model.<br/>
        /// This value specifies default to be used by the backend while making the<br/>
        /// call to the model.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("temperature")]
        public float? Temperature { get; set; }

        /// <summary>
        /// The maximum temperature this model can use.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("maxTemperature")]
        public float? MaxTemperature { get; set; }

        /// <summary>
        /// For [Nucleus<br/>
        /// sampling](https://ai.google.dev/gemini-api/docs/prompting-strategies#top-p).<br/>
        /// Nucleus sampling considers the smallest set of tokens whose probability<br/>
        /// sum is at least `top_p`.<br/>
        /// This value specifies default to be used by the backend while making the<br/>
        /// call to the model.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("topP")]
        public float? TopP { get; set; }

        /// <summary>
        /// For Top-k sampling.<br/>
        /// Top-k sampling considers the set of `top_k` most probable tokens.<br/>
        /// This value specifies default to be used by the backend while making the<br/>
        /// call to the model.<br/>
        /// If empty, indicates the model doesn't use top-k sampling, and `top_k` isn't<br/>
        /// allowed as a generation parameter.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("topK")]
        public int? TopK { get; set; }

        /// <summary>
        /// Whether the model supports thinking.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("thinking")]
        public bool? Thinking { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Model" /> class.
        /// </summary>
        /// <param name="name">
        /// Required. The resource name of the `Model`. Refer to [Model<br/>
        /// variants](https://ai.google.dev/gemini-api/docs/models/gemini#model-variations)<br/>
        /// for all allowed values.<br/>
        /// Format: `models/{model}` with a `{model}` naming convention of:<br/>
        /// * "{base_model_id}-{version}"<br/>
        /// Examples:<br/>
        /// * `models/gemini-1.5-flash-001`
        /// </param>
        /// <param name="baseModelId">
        /// Required. The name of the base model, pass this to the generation request.<br/>
        /// Examples:<br/>
        /// * `gemini-1.5-flash`
        /// </param>
        /// <param name="version">
        /// Required. The version number of the model.<br/>
        /// This represents the major version (`1.0` or `1.5`)
        /// </param>
        /// <param name="displayName">
        /// The human-readable name of the model. E.g. "Gemini 1.5 Flash".<br/>
        /// The name can be up to 128 characters long and can consist of any UTF-8<br/>
        /// characters.
        /// </param>
        /// <param name="description">
        /// A short description of the model.
        /// </param>
        /// <param name="inputTokenLimit">
        /// Maximum number of input tokens allowed for this model.
        /// </param>
        /// <param name="outputTokenLimit">
        /// Maximum number of output tokens available for this model.
        /// </param>
        /// <param name="supportedGenerationMethods">
        /// The model's supported generation methods.<br/>
        /// The corresponding API method names are defined as Pascal case<br/>
        /// strings, such as `generateMessage` and `generateContent`.
        /// </param>
        /// <param name="temperature">
        /// Controls the randomness of the output.<br/>
        /// Values can range over `[0.0,max_temperature]`, inclusive. A higher value<br/>
        /// will produce responses that are more varied, while a value closer to `0.0`<br/>
        /// will typically result in less surprising responses from the model.<br/>
        /// This value specifies default to be used by the backend while making the<br/>
        /// call to the model.
        /// </param>
        /// <param name="maxTemperature">
        /// The maximum temperature this model can use.
        /// </param>
        /// <param name="topP">
        /// For [Nucleus<br/>
        /// sampling](https://ai.google.dev/gemini-api/docs/prompting-strategies#top-p).<br/>
        /// Nucleus sampling considers the smallest set of tokens whose probability<br/>
        /// sum is at least `top_p`.<br/>
        /// This value specifies default to be used by the backend while making the<br/>
        /// call to the model.
        /// </param>
        /// <param name="topK">
        /// For Top-k sampling.<br/>
        /// Top-k sampling considers the set of `top_k` most probable tokens.<br/>
        /// This value specifies default to be used by the backend while making the<br/>
        /// call to the model.<br/>
        /// If empty, indicates the model doesn't use top-k sampling, and `top_k` isn't<br/>
        /// allowed as a generation parameter.
        /// </param>
        /// <param name="thinking">
        /// Whether the model supports thinking.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public Model(
            string name,
            string baseModelId,
            string version,
            string? displayName,
            string? description,
            int? inputTokenLimit,
            int? outputTokenLimit,
            global::System.Collections.Generic.IList<string>? supportedGenerationMethods,
            float? temperature,
            float? maxTemperature,
            float? topP,
            int? topK,
            bool? thinking)
        {
            this.Name = name ?? throw new global::System.ArgumentNullException(nameof(name));
            this.BaseModelId = baseModelId ?? throw new global::System.ArgumentNullException(nameof(baseModelId));
            this.Version = version ?? throw new global::System.ArgumentNullException(nameof(version));
            this.DisplayName = displayName;
            this.Description = description;
            this.InputTokenLimit = inputTokenLimit;
            this.OutputTokenLimit = outputTokenLimit;
            this.SupportedGenerationMethods = supportedGenerationMethods;
            this.Temperature = temperature;
            this.MaxTemperature = maxTemperature;
            this.TopP = topP;
            this.TopK = topK;
            this.Thinking = thinking;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Model" /> class.
        /// </summary>
        public Model()
        {
        }
    }
}