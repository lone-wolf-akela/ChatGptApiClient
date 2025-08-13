
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Configuration options for model generation and outputs. Not all parameters<br/>
    /// are configurable for every model.
    /// </summary>
    public sealed partial class GenerationConfig
    {
        /// <summary>
        /// Optional. Number of generated responses to return. If unset, this will default<br/>
        /// to 1. Please note that this doesn't work for previous generation<br/>
        /// models (Gemini 1.0 family)
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("candidateCount")]
        public int? CandidateCount { get; set; }

        /// <summary>
        /// Optional. The set of character sequences (up to 5) that will stop output generation.<br/>
        /// If specified, the API will stop at the first appearance of a<br/>
        /// `stop_sequence`. The stop sequence will not be included as part of the<br/>
        /// response.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("stopSequences")]
        public global::System.Collections.Generic.IList<string>? StopSequences { get; set; }

        /// <summary>
        /// Optional. The maximum number of tokens to include in a response candidate.<br/>
        /// Note: The default value varies by model, see the `Model.output_token_limit`<br/>
        /// attribute of the `Model` returned from the `getModel` function.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("maxOutputTokens")]
        public int? MaxOutputTokens { get; set; }

        /// <summary>
        /// Optional. Controls the randomness of the output.<br/>
        /// Note: The default value varies by model, see the `Model.temperature`<br/>
        /// attribute of the `Model` returned from the `getModel` function.<br/>
        /// Values can range from [0.0, 2.0].
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("temperature")]
        public float? Temperature { get; set; }

        /// <summary>
        /// Optional. The maximum cumulative probability of tokens to consider when sampling.<br/>
        /// The model uses combined Top-k and Top-p (nucleus) sampling.<br/>
        /// Tokens are sorted based on their assigned probabilities so that only the<br/>
        /// most likely tokens are considered. Top-k sampling directly limits the<br/>
        /// maximum number of tokens to consider, while Nucleus sampling limits the<br/>
        /// number of tokens based on the cumulative probability.<br/>
        /// Note: The default value varies by `Model` and is specified by<br/>
        /// the`Model.top_p` attribute returned from the `getModel` function. An empty<br/>
        /// `top_k` attribute indicates that the model doesn't apply top-k sampling<br/>
        /// and doesn't allow setting `top_k` on requests.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("topP")]
        public float? TopP { get; set; }

        /// <summary>
        /// Optional. The maximum number of tokens to consider when sampling.<br/>
        /// Gemini models use Top-p (nucleus) sampling or a combination of Top-k and<br/>
        /// nucleus sampling. Top-k sampling considers the set of `top_k` most probable<br/>
        /// tokens. Models running with nucleus sampling don't allow top_k setting.<br/>
        /// Note: The default value varies by `Model` and is specified by<br/>
        /// the`Model.top_p` attribute returned from the `getModel` function. An empty<br/>
        /// `top_k` attribute indicates that the model doesn't apply top-k sampling<br/>
        /// and doesn't allow setting `top_k` on requests.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("topK")]
        public int? TopK { get; set; }

        /// <summary>
        /// Optional. Seed used in decoding. If not set, the request uses a randomly generated<br/>
        /// seed.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("seed")]
        public int? Seed { get; set; }

        /// <summary>
        /// Optional. MIME type of the generated candidate text.<br/>
        /// Supported MIME types are:<br/>
        /// `text/plain`: (default) Text output.<br/>
        /// `application/json`: JSON response in the response candidates.<br/>
        /// `text/x.enum`: ENUM as a string response in the response candidates.<br/>
        /// Refer to the<br/>
        /// [docs](https://ai.google.dev/gemini-api/docs/prompting_with_media#plain_text_formats)<br/>
        /// for a list of all supported text MIME types.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("responseMimeType")]
        public string? ResponseMimeType { get; set; }

        /// <summary>
        /// Optional. Output schema of the generated candidate text. Schemas must be a<br/>
        /// subset of the [OpenAPI schema](https://spec.openapis.org/oas/v3.0.3#schema)<br/>
        /// and can be objects, primitives or arrays.<br/>
        /// If set, a compatible `response_mime_type` must also be set.<br/>
        /// Compatible MIME types:<br/>
        /// `application/json`: Schema for JSON response.<br/>
        /// Refer to the [JSON text generation<br/>
        /// guide](https://ai.google.dev/gemini-api/docs/json-mode) for more details.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("responseSchema")]
        public global::Gemini.Schema? ResponseSchema { get; set; }

        /// <summary>
        /// Optional. Output schema of the generated response. This is an alternative to<br/>
        /// `response_schema` that accepts [JSON Schema](https://json-schema.org/).<br/>
        /// If set, `response_schema` must be omitted, but `response_mime_type` is<br/>
        /// required.<br/>
        /// While the full JSON Schema may be sent, not all features are supported.<br/>
        /// Specifically, only the following properties are supported:<br/>
        /// - `$id`<br/>
        /// - `$defs`<br/>
        /// - `$ref`<br/>
        /// - `$anchor`<br/>
        /// - `type`<br/>
        /// - `format`<br/>
        /// - `title`<br/>
        /// - `description`<br/>
        /// - `enum` (for strings and numbers)<br/>
        /// - `items`<br/>
        /// - `prefixItems`<br/>
        /// - `minItems`<br/>
        /// - `maxItems`<br/>
        /// - `minimum`<br/>
        /// - `maximum`<br/>
        /// - `anyOf`<br/>
        /// - `oneOf` (interpreted the same as `anyOf`)<br/>
        /// - `properties`<br/>
        /// - `additionalProperties`<br/>
        /// - `required`<br/>
        /// The non-standard `propertyOrdering` property may also be set.<br/>
        /// Cyclic references are unrolled to a limited degree and, as such, may only<br/>
        /// be used within non-required properties. (Nullable properties are not<br/>
        /// sufficient.) If `$ref` is set on a sub-schema, no other properties, except<br/>
        /// for than those starting as a `$`, may be set.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("responseJsonSchema")]
        public object? ResponseJsonSchema { get; set; }

        /// <summary>
        /// Optional. Presence penalty applied to the next token's logprobs if the token has<br/>
        /// already been seen in the response.<br/>
        /// This penalty is binary on/off and not dependant on the number of times the<br/>
        /// token is used (after the first). Use<br/>
        /// frequency_penalty<br/>
        /// for a penalty that increases with each use.<br/>
        /// A positive penalty will discourage the use of tokens that have already<br/>
        /// been used in the response, increasing the vocabulary.<br/>
        /// A negative penalty will encourage the use of tokens that have already been<br/>
        /// used in the response, decreasing the vocabulary.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("presencePenalty")]
        public float? PresencePenalty { get; set; }

        /// <summary>
        /// Optional. Frequency penalty applied to the next token's logprobs, multiplied by the<br/>
        /// number of times each token has been seen in the respponse so far.<br/>
        /// A positive penalty will discourage the use of tokens that have already<br/>
        /// been used, proportional to the number of times the token has been used:<br/>
        /// The more a token is used, the more difficult it is for the model to use<br/>
        /// that token again increasing the vocabulary of responses.<br/>
        /// Caution: A _negative_ penalty will encourage the model to reuse tokens<br/>
        /// proportional to the number of times the token has been used. Small<br/>
        /// negative values will reduce the vocabulary of a response. Larger negative<br/>
        /// values will cause the model to start repeating a common token  until it<br/>
        /// hits the max_output_tokens<br/>
        /// limit.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("frequencyPenalty")]
        public float? FrequencyPenalty { get; set; }

        /// <summary>
        /// Optional. If true, export the logprobs results in response.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("responseLogprobs")]
        public bool? ResponseLogprobs { get; set; }

        /// <summary>
        /// Optional. Only valid if response_logprobs=True.<br/>
        /// This sets the number of top logprobs to return at each decoding step in the<br/>
        /// Candidate.logprobs_result.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("logprobs")]
        public int? Logprobs { get; set; }

        /// <summary>
        /// Optional. Enables enhanced civic answers. It may not be available for all models.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("enableEnhancedCivicAnswers")]
        public bool? EnableEnhancedCivicAnswers { get; set; }

        /// <summary>
        /// Optional. The requested modalities of the response. Represents the set of modalities<br/>
        /// that the model can return, and should be expected in the response. This is<br/>
        /// an exact match to the modalities of the response.<br/>
        /// A model may have multiple combinations of supported modalities. If the<br/>
        /// requested modalities do not match any of the supported combinations, an<br/>
        /// error will be returned.<br/>
        /// An empty list is equivalent to requesting only text.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("responseModalities")]
        public global::System.Collections.Generic.IList<global::Gemini.GenerationConfigResponseModalitie>? ResponseModalities { get; set; }

        /// <summary>
        /// Optional. The speech generation config.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("speechConfig")]
        public global::Gemini.SpeechConfig? SpeechConfig { get; set; }

        /// <summary>
        /// Optional. Config for thinking features.<br/>
        /// An error will be returned if this field is set for models that don't<br/>
        /// support thinking.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("thinkingConfig")]
        public global::Gemini.ThinkingConfig? ThinkingConfig { get; set; }

        /// <summary>
        /// Optional. If specified, the media resolution specified will be used.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("mediaResolution")]
        [global::System.Text.Json.Serialization.JsonConverter(typeof(global::Gemini.JsonConverters.GenerationConfigMediaResolutionJsonConverter))]
        public global::Gemini.GenerationConfigMediaResolution? MediaResolution { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerationConfig" /> class.
        /// </summary>
        /// <param name="candidateCount">
        /// Optional. Number of generated responses to return. If unset, this will default<br/>
        /// to 1. Please note that this doesn't work for previous generation<br/>
        /// models (Gemini 1.0 family)
        /// </param>
        /// <param name="stopSequences">
        /// Optional. The set of character sequences (up to 5) that will stop output generation.<br/>
        /// If specified, the API will stop at the first appearance of a<br/>
        /// `stop_sequence`. The stop sequence will not be included as part of the<br/>
        /// response.
        /// </param>
        /// <param name="maxOutputTokens">
        /// Optional. The maximum number of tokens to include in a response candidate.<br/>
        /// Note: The default value varies by model, see the `Model.output_token_limit`<br/>
        /// attribute of the `Model` returned from the `getModel` function.
        /// </param>
        /// <param name="temperature">
        /// Optional. Controls the randomness of the output.<br/>
        /// Note: The default value varies by model, see the `Model.temperature`<br/>
        /// attribute of the `Model` returned from the `getModel` function.<br/>
        /// Values can range from [0.0, 2.0].
        /// </param>
        /// <param name="topP">
        /// Optional. The maximum cumulative probability of tokens to consider when sampling.<br/>
        /// The model uses combined Top-k and Top-p (nucleus) sampling.<br/>
        /// Tokens are sorted based on their assigned probabilities so that only the<br/>
        /// most likely tokens are considered. Top-k sampling directly limits the<br/>
        /// maximum number of tokens to consider, while Nucleus sampling limits the<br/>
        /// number of tokens based on the cumulative probability.<br/>
        /// Note: The default value varies by `Model` and is specified by<br/>
        /// the`Model.top_p` attribute returned from the `getModel` function. An empty<br/>
        /// `top_k` attribute indicates that the model doesn't apply top-k sampling<br/>
        /// and doesn't allow setting `top_k` on requests.
        /// </param>
        /// <param name="topK">
        /// Optional. The maximum number of tokens to consider when sampling.<br/>
        /// Gemini models use Top-p (nucleus) sampling or a combination of Top-k and<br/>
        /// nucleus sampling. Top-k sampling considers the set of `top_k` most probable<br/>
        /// tokens. Models running with nucleus sampling don't allow top_k setting.<br/>
        /// Note: The default value varies by `Model` and is specified by<br/>
        /// the`Model.top_p` attribute returned from the `getModel` function. An empty<br/>
        /// `top_k` attribute indicates that the model doesn't apply top-k sampling<br/>
        /// and doesn't allow setting `top_k` on requests.
        /// </param>
        /// <param name="seed">
        /// Optional. Seed used in decoding. If not set, the request uses a randomly generated<br/>
        /// seed.
        /// </param>
        /// <param name="responseMimeType">
        /// Optional. MIME type of the generated candidate text.<br/>
        /// Supported MIME types are:<br/>
        /// `text/plain`: (default) Text output.<br/>
        /// `application/json`: JSON response in the response candidates.<br/>
        /// `text/x.enum`: ENUM as a string response in the response candidates.<br/>
        /// Refer to the<br/>
        /// [docs](https://ai.google.dev/gemini-api/docs/prompting_with_media#plain_text_formats)<br/>
        /// for a list of all supported text MIME types.
        /// </param>
        /// <param name="responseSchema">
        /// Optional. Output schema of the generated candidate text. Schemas must be a<br/>
        /// subset of the [OpenAPI schema](https://spec.openapis.org/oas/v3.0.3#schema)<br/>
        /// and can be objects, primitives or arrays.<br/>
        /// If set, a compatible `response_mime_type` must also be set.<br/>
        /// Compatible MIME types:<br/>
        /// `application/json`: Schema for JSON response.<br/>
        /// Refer to the [JSON text generation<br/>
        /// guide](https://ai.google.dev/gemini-api/docs/json-mode) for more details.
        /// </param>
        /// <param name="responseJsonSchema">
        /// Optional. Output schema of the generated response. This is an alternative to<br/>
        /// `response_schema` that accepts [JSON Schema](https://json-schema.org/).<br/>
        /// If set, `response_schema` must be omitted, but `response_mime_type` is<br/>
        /// required.<br/>
        /// While the full JSON Schema may be sent, not all features are supported.<br/>
        /// Specifically, only the following properties are supported:<br/>
        /// - `$id`<br/>
        /// - `$defs`<br/>
        /// - `$ref`<br/>
        /// - `$anchor`<br/>
        /// - `type`<br/>
        /// - `format`<br/>
        /// - `title`<br/>
        /// - `description`<br/>
        /// - `enum` (for strings and numbers)<br/>
        /// - `items`<br/>
        /// - `prefixItems`<br/>
        /// - `minItems`<br/>
        /// - `maxItems`<br/>
        /// - `minimum`<br/>
        /// - `maximum`<br/>
        /// - `anyOf`<br/>
        /// - `oneOf` (interpreted the same as `anyOf`)<br/>
        /// - `properties`<br/>
        /// - `additionalProperties`<br/>
        /// - `required`<br/>
        /// The non-standard `propertyOrdering` property may also be set.<br/>
        /// Cyclic references are unrolled to a limited degree and, as such, may only<br/>
        /// be used within non-required properties. (Nullable properties are not<br/>
        /// sufficient.) If `$ref` is set on a sub-schema, no other properties, except<br/>
        /// for than those starting as a `$`, may be set.
        /// </param>
        /// <param name="presencePenalty">
        /// Optional. Presence penalty applied to the next token's logprobs if the token has<br/>
        /// already been seen in the response.<br/>
        /// This penalty is binary on/off and not dependant on the number of times the<br/>
        /// token is used (after the first). Use<br/>
        /// frequency_penalty<br/>
        /// for a penalty that increases with each use.<br/>
        /// A positive penalty will discourage the use of tokens that have already<br/>
        /// been used in the response, increasing the vocabulary.<br/>
        /// A negative penalty will encourage the use of tokens that have already been<br/>
        /// used in the response, decreasing the vocabulary.
        /// </param>
        /// <param name="frequencyPenalty">
        /// Optional. Frequency penalty applied to the next token's logprobs, multiplied by the<br/>
        /// number of times each token has been seen in the respponse so far.<br/>
        /// A positive penalty will discourage the use of tokens that have already<br/>
        /// been used, proportional to the number of times the token has been used:<br/>
        /// The more a token is used, the more difficult it is for the model to use<br/>
        /// that token again increasing the vocabulary of responses.<br/>
        /// Caution: A _negative_ penalty will encourage the model to reuse tokens<br/>
        /// proportional to the number of times the token has been used. Small<br/>
        /// negative values will reduce the vocabulary of a response. Larger negative<br/>
        /// values will cause the model to start repeating a common token  until it<br/>
        /// hits the max_output_tokens<br/>
        /// limit.
        /// </param>
        /// <param name="responseLogprobs">
        /// Optional. If true, export the logprobs results in response.
        /// </param>
        /// <param name="logprobs">
        /// Optional. Only valid if response_logprobs=True.<br/>
        /// This sets the number of top logprobs to return at each decoding step in the<br/>
        /// Candidate.logprobs_result.
        /// </param>
        /// <param name="enableEnhancedCivicAnswers">
        /// Optional. Enables enhanced civic answers. It may not be available for all models.
        /// </param>
        /// <param name="responseModalities">
        /// Optional. The requested modalities of the response. Represents the set of modalities<br/>
        /// that the model can return, and should be expected in the response. This is<br/>
        /// an exact match to the modalities of the response.<br/>
        /// A model may have multiple combinations of supported modalities. If the<br/>
        /// requested modalities do not match any of the supported combinations, an<br/>
        /// error will be returned.<br/>
        /// An empty list is equivalent to requesting only text.
        /// </param>
        /// <param name="speechConfig">
        /// Optional. The speech generation config.
        /// </param>
        /// <param name="thinkingConfig">
        /// Optional. Config for thinking features.<br/>
        /// An error will be returned if this field is set for models that don't<br/>
        /// support thinking.
        /// </param>
        /// <param name="mediaResolution">
        /// Optional. If specified, the media resolution specified will be used.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public GenerationConfig(
            int? candidateCount,
            global::System.Collections.Generic.IList<string>? stopSequences,
            int? maxOutputTokens,
            float? temperature,
            float? topP,
            int? topK,
            int? seed,
            string? responseMimeType,
            global::Gemini.Schema? responseSchema,
            object? responseJsonSchema,
            float? presencePenalty,
            float? frequencyPenalty,
            bool? responseLogprobs,
            int? logprobs,
            bool? enableEnhancedCivicAnswers,
            global::System.Collections.Generic.IList<global::Gemini.GenerationConfigResponseModalitie>? responseModalities,
            global::Gemini.SpeechConfig? speechConfig,
            global::Gemini.ThinkingConfig? thinkingConfig,
            global::Gemini.GenerationConfigMediaResolution? mediaResolution)
        {
            this.CandidateCount = candidateCount;
            this.StopSequences = stopSequences;
            this.MaxOutputTokens = maxOutputTokens;
            this.Temperature = temperature;
            this.TopP = topP;
            this.TopK = topK;
            this.Seed = seed;
            this.ResponseMimeType = responseMimeType;
            this.ResponseSchema = responseSchema;
            this.ResponseJsonSchema = responseJsonSchema;
            this.PresencePenalty = presencePenalty;
            this.FrequencyPenalty = frequencyPenalty;
            this.ResponseLogprobs = responseLogprobs;
            this.Logprobs = logprobs;
            this.EnableEnhancedCivicAnswers = enableEnhancedCivicAnswers;
            this.ResponseModalities = responseModalities;
            this.SpeechConfig = speechConfig;
            this.ThinkingConfig = thinkingConfig;
            this.MediaResolution = mediaResolution;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerationConfig" /> class.
        /// </summary>
        public GenerationConfig()
        {
        }
    }
}