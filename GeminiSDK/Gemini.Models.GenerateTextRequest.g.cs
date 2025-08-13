
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Request to generate a text completion response from the model.
    /// </summary>
    public sealed partial class GenerateTextRequest
    {
        /// <summary>
        /// Required. The free-form input text given to the model as a prompt.<br/>
        /// Given a prompt, the model will generate a TextCompletion response it<br/>
        /// predicts as the completion of the input text.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("prompt")]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required global::Gemini.TextPrompt Prompt { get; set; }

        /// <summary>
        /// Optional. Controls the randomness of the output.<br/>
        /// Note: The default value varies by model, see the `Model.temperature`<br/>
        /// attribute of the `Model` returned the `getModel` function.<br/>
        /// Values can range from [0.0,1.0],<br/>
        /// inclusive. A value closer to 1.0 will produce responses that are more<br/>
        /// varied and creative, while a value closer to 0.0 will typically result in<br/>
        /// more straightforward responses from the model.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("temperature")]
        public float? Temperature { get; set; }

        /// <summary>
        /// Optional. Number of generated responses to return.<br/>
        /// This value must be between [1, 8], inclusive. If unset, this will default<br/>
        /// to 1.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("candidateCount")]
        public int? CandidateCount { get; set; }

        /// <summary>
        /// Optional. The maximum number of tokens to include in a candidate.<br/>
        /// If unset, this will default to output_token_limit specified in the `Model`<br/>
        /// specification.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("maxOutputTokens")]
        public int? MaxOutputTokens { get; set; }

        /// <summary>
        /// Optional. The maximum cumulative probability of tokens to consider when sampling.<br/>
        /// The model uses combined Top-k and nucleus sampling.<br/>
        /// Tokens are sorted based on their assigned probabilities so that only the<br/>
        /// most likely tokens are considered. Top-k sampling directly limits the<br/>
        /// maximum number of tokens to consider, while Nucleus sampling limits number<br/>
        /// of tokens based on the cumulative probability.<br/>
        /// Note: The default value varies by model, see the `Model.top_p`<br/>
        /// attribute of the `Model` returned the `getModel` function.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("topP")]
        public float? TopP { get; set; }

        /// <summary>
        /// Optional. The maximum number of tokens to consider when sampling.<br/>
        /// The model uses combined Top-k and nucleus sampling.<br/>
        /// Top-k sampling considers the set of `top_k` most probable tokens.<br/>
        /// Defaults to 40.<br/>
        /// Note: The default value varies by model, see the `Model.top_k`<br/>
        /// attribute of the `Model` returned the `getModel` function.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("topK")]
        public int? TopK { get; set; }

        /// <summary>
        /// Optional. A list of unique `SafetySetting` instances for blocking unsafe content.<br/>
        /// that will be enforced on the `GenerateTextRequest.prompt` and<br/>
        /// `GenerateTextResponse.candidates`. There should not be more than one<br/>
        /// setting for each `SafetyCategory` type. The API will block any prompts and<br/>
        /// responses that fail to meet the thresholds set by these settings. This list<br/>
        /// overrides the default settings for each `SafetyCategory` specified in the<br/>
        /// safety_settings. If there is no `SafetySetting` for a given<br/>
        /// `SafetyCategory` provided in the list, the API will use the default safety<br/>
        /// setting for that category. Harm categories HARM_CATEGORY_DEROGATORY,<br/>
        /// HARM_CATEGORY_TOXICITY, HARM_CATEGORY_VIOLENCE, HARM_CATEGORY_SEXUAL,<br/>
        /// HARM_CATEGORY_MEDICAL, HARM_CATEGORY_DANGEROUS are supported in text<br/>
        /// service.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("safetySettings")]
        public global::System.Collections.Generic.IList<global::Gemini.SafetySetting>? SafetySettings { get; set; }

        /// <summary>
        /// The set of character sequences (up to 5) that will stop output generation.<br/>
        /// If specified, the API will stop at the first appearance of a stop<br/>
        /// sequence. The stop sequence will not be included as part of the response.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("stopSequences")]
        public global::System.Collections.Generic.IList<string>? StopSequences { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateTextRequest" /> class.
        /// </summary>
        /// <param name="prompt">
        /// Required. The free-form input text given to the model as a prompt.<br/>
        /// Given a prompt, the model will generate a TextCompletion response it<br/>
        /// predicts as the completion of the input text.
        /// </param>
        /// <param name="temperature">
        /// Optional. Controls the randomness of the output.<br/>
        /// Note: The default value varies by model, see the `Model.temperature`<br/>
        /// attribute of the `Model` returned the `getModel` function.<br/>
        /// Values can range from [0.0,1.0],<br/>
        /// inclusive. A value closer to 1.0 will produce responses that are more<br/>
        /// varied and creative, while a value closer to 0.0 will typically result in<br/>
        /// more straightforward responses from the model.
        /// </param>
        /// <param name="candidateCount">
        /// Optional. Number of generated responses to return.<br/>
        /// This value must be between [1, 8], inclusive. If unset, this will default<br/>
        /// to 1.
        /// </param>
        /// <param name="maxOutputTokens">
        /// Optional. The maximum number of tokens to include in a candidate.<br/>
        /// If unset, this will default to output_token_limit specified in the `Model`<br/>
        /// specification.
        /// </param>
        /// <param name="topP">
        /// Optional. The maximum cumulative probability of tokens to consider when sampling.<br/>
        /// The model uses combined Top-k and nucleus sampling.<br/>
        /// Tokens are sorted based on their assigned probabilities so that only the<br/>
        /// most likely tokens are considered. Top-k sampling directly limits the<br/>
        /// maximum number of tokens to consider, while Nucleus sampling limits number<br/>
        /// of tokens based on the cumulative probability.<br/>
        /// Note: The default value varies by model, see the `Model.top_p`<br/>
        /// attribute of the `Model` returned the `getModel` function.
        /// </param>
        /// <param name="topK">
        /// Optional. The maximum number of tokens to consider when sampling.<br/>
        /// The model uses combined Top-k and nucleus sampling.<br/>
        /// Top-k sampling considers the set of `top_k` most probable tokens.<br/>
        /// Defaults to 40.<br/>
        /// Note: The default value varies by model, see the `Model.top_k`<br/>
        /// attribute of the `Model` returned the `getModel` function.
        /// </param>
        /// <param name="safetySettings">
        /// Optional. A list of unique `SafetySetting` instances for blocking unsafe content.<br/>
        /// that will be enforced on the `GenerateTextRequest.prompt` and<br/>
        /// `GenerateTextResponse.candidates`. There should not be more than one<br/>
        /// setting for each `SafetyCategory` type. The API will block any prompts and<br/>
        /// responses that fail to meet the thresholds set by these settings. This list<br/>
        /// overrides the default settings for each `SafetyCategory` specified in the<br/>
        /// safety_settings. If there is no `SafetySetting` for a given<br/>
        /// `SafetyCategory` provided in the list, the API will use the default safety<br/>
        /// setting for that category. Harm categories HARM_CATEGORY_DEROGATORY,<br/>
        /// HARM_CATEGORY_TOXICITY, HARM_CATEGORY_VIOLENCE, HARM_CATEGORY_SEXUAL,<br/>
        /// HARM_CATEGORY_MEDICAL, HARM_CATEGORY_DANGEROUS are supported in text<br/>
        /// service.
        /// </param>
        /// <param name="stopSequences">
        /// The set of character sequences (up to 5) that will stop output generation.<br/>
        /// If specified, the API will stop at the first appearance of a stop<br/>
        /// sequence. The stop sequence will not be included as part of the response.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public GenerateTextRequest(
            global::Gemini.TextPrompt prompt,
            float? temperature,
            int? candidateCount,
            int? maxOutputTokens,
            float? topP,
            int? topK,
            global::System.Collections.Generic.IList<global::Gemini.SafetySetting>? safetySettings,
            global::System.Collections.Generic.IList<string>? stopSequences)
        {
            this.Prompt = prompt ?? throw new global::System.ArgumentNullException(nameof(prompt));
            this.Temperature = temperature;
            this.CandidateCount = candidateCount;
            this.MaxOutputTokens = maxOutputTokens;
            this.TopP = topP;
            this.TopK = topK;
            this.SafetySettings = safetySettings;
            this.StopSequences = stopSequences;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateTextRequest" /> class.
        /// </summary>
        public GenerateTextRequest()
        {
        }
    }
}