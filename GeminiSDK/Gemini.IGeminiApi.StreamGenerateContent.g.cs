#nullable enable

namespace Gemini
{
    public partial interface IGeminiApi
    {
        /// <summary>
        /// Generates a [streamed<br/>
        /// response](https://ai.google.dev/gemini-api/docs/text-generation?lang=python#generate-a-text-stream)<br/>
        /// from the model given an input `GenerateContentRequest`.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::Gemini.ApiException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.GenerateContentResponse> StreamGenerateContentAsync(
            string model,
            global::Gemini.GenerateContentRequest request,
            global::System.Threading.CancellationToken cancellationToken = default);

        /// <summary>
        /// Generates a [streamed<br/>
        /// response](https://ai.google.dev/gemini-api/docs/text-generation?lang=python#generate-a-text-stream)<br/>
        /// from the model given an input `GenerateContentRequest`.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="requestModel">
        /// Required. The name of the `Model` to use for generating the completion.<br/>
        /// Format: `models/{model}`.
        /// </param>
        /// <param name="systemInstruction">
        /// Optional. Developer set [system<br/>
        /// instruction(s)](https://ai.google.dev/gemini-api/docs/system-instructions).<br/>
        /// Currently, text only.
        /// </param>
        /// <param name="contents">
        /// Required. The content of the current conversation with the model.<br/>
        /// For single-turn queries, this is a single instance. For multi-turn queries<br/>
        /// like [chat](https://ai.google.dev/gemini-api/docs/text-generation#chat),<br/>
        /// this is a repeated field that contains the conversation history and the<br/>
        /// latest request.
        /// </param>
        /// <param name="tools">
        /// Optional. A list of `Tools` the `Model` may use to generate the next response.<br/>
        /// A `Tool` is a piece of code that enables the system to interact with<br/>
        /// external systems to perform an action, or set of actions, outside of<br/>
        /// knowledge and scope of the `Model`. Supported `Tool`s are `Function` and<br/>
        /// `code_execution`. Refer to the [Function<br/>
        /// calling](https://ai.google.dev/gemini-api/docs/function-calling) and the<br/>
        /// [Code execution](https://ai.google.dev/gemini-api/docs/code-execution)<br/>
        /// guides to learn more.
        /// </param>
        /// <param name="toolConfig">
        /// Optional. Tool configuration for any `Tool` specified in the request. Refer to the<br/>
        /// [Function calling<br/>
        /// guide](https://ai.google.dev/gemini-api/docs/function-calling#function_calling_mode)<br/>
        /// for a usage example.
        /// </param>
        /// <param name="safetySettings">
        /// Optional. A list of unique `SafetySetting` instances for blocking unsafe content.<br/>
        /// This will be enforced on the `GenerateContentRequest.contents` and<br/>
        /// `GenerateContentResponse.candidates`. There should not be more than one<br/>
        /// setting for each `SafetyCategory` type. The API will block any contents and<br/>
        /// responses that fail to meet the thresholds set by these settings. This list<br/>
        /// overrides the default settings for each `SafetyCategory` specified in the<br/>
        /// safety_settings. If there is no `SafetySetting` for a given<br/>
        /// `SafetyCategory` provided in the list, the API will use the default safety<br/>
        /// setting for that category. Harm categories HARM_CATEGORY_HATE_SPEECH,<br/>
        /// HARM_CATEGORY_SEXUALLY_EXPLICIT, HARM_CATEGORY_DANGEROUS_CONTENT,<br/>
        /// HARM_CATEGORY_HARASSMENT, HARM_CATEGORY_CIVIC_INTEGRITY are supported.<br/>
        /// Refer to the [guide](https://ai.google.dev/gemini-api/docs/safety-settings)<br/>
        /// for detailed information on available safety settings. Also refer to the<br/>
        /// [Safety guidance](https://ai.google.dev/gemini-api/docs/safety-guidance) to<br/>
        /// learn how to incorporate safety considerations in your AI applications.
        /// </param>
        /// <param name="generationConfig">
        /// Optional. Configuration options for model generation and outputs.
        /// </param>
        /// <param name="cachedContent">
        /// Optional. The name of the content<br/>
        /// [cached](https://ai.google.dev/gemini-api/docs/caching) to use as context<br/>
        /// to serve the prediction. Format: `cachedContents/{cachedContent}`
        /// </param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::System.InvalidOperationException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.GenerateContentResponse> StreamGenerateContentAsync(
            string model,
            string requestModel,
            global::System.Collections.Generic.IList<global::Gemini.Content> contents,
            global::Gemini.Content? systemInstruction = default,
            global::System.Collections.Generic.IList<global::Gemini.Tool>? tools = default,
            global::Gemini.ToolConfig? toolConfig = default,
            global::System.Collections.Generic.IList<global::Gemini.SafetySetting>? safetySettings = default,
            global::Gemini.GenerationConfig? generationConfig = default,
            string? cachedContent = default,
            global::System.Threading.CancellationToken cancellationToken = default);
    }
}