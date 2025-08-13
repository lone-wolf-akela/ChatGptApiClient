#nullable enable

namespace Gemini
{
    public partial interface IGeminiApi
    {
        /// <summary>
        /// Generates a grounded answer from the model given an input<br/>
        /// `GenerateAnswerRequest`.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::Gemini.ApiException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.GenerateAnswerResponse> GenerateAnswerAsync(
            string model,
            global::Gemini.GenerateAnswerRequest request,
            global::System.Threading.CancellationToken cancellationToken = default);

        /// <summary>
        /// Generates a grounded answer from the model given an input<br/>
        /// `GenerateAnswerRequest`.
        /// </summary>
        /// <param name="model"></param>
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
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::System.InvalidOperationException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.GenerateAnswerResponse> GenerateAnswerAsync(
            string model,
            global::System.Collections.Generic.IList<global::Gemini.Content> contents,
            global::Gemini.GenerateAnswerRequestAnswerStyle answerStyle,
            global::Gemini.GroundingPassages? inlinePassages = default,
            global::Gemini.SemanticRetrieverConfig? semanticRetriever = default,
            global::System.Collections.Generic.IList<global::Gemini.SafetySetting>? safetySettings = default,
            float? temperature = default,
            global::System.Threading.CancellationToken cancellationToken = default);
    }
}