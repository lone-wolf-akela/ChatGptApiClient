
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Request to generate a grounded answer from the `Model`.
    /// </summary>
    public sealed partial class GenerateAnswerRequest
    {
        /// <summary>
        /// Passages provided inline with the request.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("inlinePassages")]
        public global::Gemini.GroundingPassages? InlinePassages { get; set; }

        /// <summary>
        /// Content retrieved from resources created via the Semantic Retriever<br/>
        /// API.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("semanticRetriever")]
        public global::Gemini.SemanticRetrieverConfig? SemanticRetriever { get; set; }

        /// <summary>
        /// Required. The content of the current conversation with the `Model`. For single-turn<br/>
        /// queries, this is a single question to answer. For multi-turn queries, this<br/>
        /// is a repeated field that contains conversation history and the last<br/>
        /// `Content` in the list containing the question.<br/>
        /// Note: `GenerateAnswer` only supports queries in English.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("contents")]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required global::System.Collections.Generic.IList<global::Gemini.Content> Contents { get; set; }

        /// <summary>
        /// Required. Style in which answers should be returned.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("answerStyle")]
        [global::System.Text.Json.Serialization.JsonConverter(typeof(global::Gemini.JsonConverters.GenerateAnswerRequestAnswerStyleJsonConverter))]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required global::Gemini.GenerateAnswerRequestAnswerStyle AnswerStyle { get; set; }

        /// <summary>
        /// Optional. A list of unique `SafetySetting` instances for blocking unsafe content.<br/>
        /// This will be enforced on the `GenerateAnswerRequest.contents` and<br/>
        /// `GenerateAnswerResponse.candidate`. There should not be more than one<br/>
        /// setting for each `SafetyCategory` type. The API will block any contents and<br/>
        /// responses that fail to meet the thresholds set by these settings. This list<br/>
        /// overrides the default settings for each `SafetyCategory` specified in the<br/>
        /// safety_settings. If there is no `SafetySetting` for a given<br/>
        /// `SafetyCategory` provided in the list, the API will use the default safety<br/>
        /// setting for that category. Harm categories HARM_CATEGORY_HATE_SPEECH,<br/>
        /// HARM_CATEGORY_SEXUALLY_EXPLICIT, HARM_CATEGORY_DANGEROUS_CONTENT,<br/>
        /// HARM_CATEGORY_HARASSMENT are supported.<br/>
        /// Refer to the<br/>
        /// [guide](https://ai.google.dev/gemini-api/docs/safety-settings)<br/>
        /// for detailed information on available safety settings. Also refer to the<br/>
        /// [Safety guidance](https://ai.google.dev/gemini-api/docs/safety-guidance) to<br/>
        /// learn how to incorporate safety considerations in your AI applications.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("safetySettings")]
        public global::System.Collections.Generic.IList<global::Gemini.SafetySetting>? SafetySettings { get; set; }

        /// <summary>
        /// Optional. Controls the randomness of the output.<br/>
        /// Values can range from [0.0,1.0], inclusive. A value closer to 1.0 will<br/>
        /// produce responses that are more varied and creative, while a value closer<br/>
        /// to 0.0 will typically result in more straightforward responses from the<br/>
        /// model. A low temperature (~0.2) is usually recommended for<br/>
        /// Attributed-Question-Answering use cases.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("temperature")]
        public float? Temperature { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateAnswerRequest" /> class.
        /// </summary>
        /// <param name="inlinePassages">
        /// Passages provided inline with the request.
        /// </param>
        /// <param name="semanticRetriever">
        /// Content retrieved from resources created via the Semantic Retriever<br/>
        /// API.
        /// </param>
        /// <param name="contents">
        /// Required. The content of the current conversation with the `Model`. For single-turn<br/>
        /// queries, this is a single question to answer. For multi-turn queries, this<br/>
        /// is a repeated field that contains conversation history and the last<br/>
        /// `Content` in the list containing the question.<br/>
        /// Note: `GenerateAnswer` only supports queries in English.
        /// </param>
        /// <param name="answerStyle">
        /// Required. Style in which answers should be returned.
        /// </param>
        /// <param name="safetySettings">
        /// Optional. A list of unique `SafetySetting` instances for blocking unsafe content.<br/>
        /// This will be enforced on the `GenerateAnswerRequest.contents` and<br/>
        /// `GenerateAnswerResponse.candidate`. There should not be more than one<br/>
        /// setting for each `SafetyCategory` type. The API will block any contents and<br/>
        /// responses that fail to meet the thresholds set by these settings. This list<br/>
        /// overrides the default settings for each `SafetyCategory` specified in the<br/>
        /// safety_settings. If there is no `SafetySetting` for a given<br/>
        /// `SafetyCategory` provided in the list, the API will use the default safety<br/>
        /// setting for that category. Harm categories HARM_CATEGORY_HATE_SPEECH,<br/>
        /// HARM_CATEGORY_SEXUALLY_EXPLICIT, HARM_CATEGORY_DANGEROUS_CONTENT,<br/>
        /// HARM_CATEGORY_HARASSMENT are supported.<br/>
        /// Refer to the<br/>
        /// [guide](https://ai.google.dev/gemini-api/docs/safety-settings)<br/>
        /// for detailed information on available safety settings. Also refer to the<br/>
        /// [Safety guidance](https://ai.google.dev/gemini-api/docs/safety-guidance) to<br/>
        /// learn how to incorporate safety considerations in your AI applications.
        /// </param>
        /// <param name="temperature">
        /// Optional. Controls the randomness of the output.<br/>
        /// Values can range from [0.0,1.0], inclusive. A value closer to 1.0 will<br/>
        /// produce responses that are more varied and creative, while a value closer<br/>
        /// to 0.0 will typically result in more straightforward responses from the<br/>
        /// model. A low temperature (~0.2) is usually recommended for<br/>
        /// Attributed-Question-Answering use cases.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public GenerateAnswerRequest(
            global::System.Collections.Generic.IList<global::Gemini.Content> contents,
            global::Gemini.GenerateAnswerRequestAnswerStyle answerStyle,
            global::Gemini.GroundingPassages? inlinePassages,
            global::Gemini.SemanticRetrieverConfig? semanticRetriever,
            global::System.Collections.Generic.IList<global::Gemini.SafetySetting>? safetySettings,
            float? temperature)
        {
            this.Contents = contents ?? throw new global::System.ArgumentNullException(nameof(contents));
            this.AnswerStyle = answerStyle;
            this.InlinePassages = inlinePassages;
            this.SemanticRetriever = semanticRetriever;
            this.SafetySettings = safetySettings;
            this.Temperature = temperature;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateAnswerRequest" /> class.
        /// </summary>
        public GenerateAnswerRequest()
        {
        }
    }
}