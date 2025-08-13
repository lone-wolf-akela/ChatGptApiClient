
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Request to generate a completion from the model.
    /// </summary>
    public sealed partial class GenerateContentRequest
    {
        /// <summary>
        /// Required. The name of the `Model` to use for generating the completion.<br/>
        /// Format: `models/{model}`.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("model")]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required string Model { get; set; }

        /// <summary>
        /// Optional. Developer set [system<br/>
        /// instruction(s)](https://ai.google.dev/gemini-api/docs/system-instructions).<br/>
        /// Currently, text only.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("systemInstruction")]
        public global::Gemini.Content? SystemInstruction { get; set; }

        /// <summary>
        /// Required. The content of the current conversation with the model.<br/>
        /// For single-turn queries, this is a single instance. For multi-turn queries<br/>
        /// like [chat](https://ai.google.dev/gemini-api/docs/text-generation#chat),<br/>
        /// this is a repeated field that contains the conversation history and the<br/>
        /// latest request.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("contents")]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required global::System.Collections.Generic.IList<global::Gemini.Content> Contents { get; set; }

        /// <summary>
        /// Optional. A list of `Tools` the `Model` may use to generate the next response.<br/>
        /// A `Tool` is a piece of code that enables the system to interact with<br/>
        /// external systems to perform an action, or set of actions, outside of<br/>
        /// knowledge and scope of the `Model`. Supported `Tool`s are `Function` and<br/>
        /// `code_execution`. Refer to the [Function<br/>
        /// calling](https://ai.google.dev/gemini-api/docs/function-calling) and the<br/>
        /// [Code execution](https://ai.google.dev/gemini-api/docs/code-execution)<br/>
        /// guides to learn more.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("tools")]
        public global::System.Collections.Generic.IList<global::Gemini.Tool>? Tools { get; set; }

        /// <summary>
        /// Optional. Tool configuration for any `Tool` specified in the request. Refer to the<br/>
        /// [Function calling<br/>
        /// guide](https://ai.google.dev/gemini-api/docs/function-calling#function_calling_mode)<br/>
        /// for a usage example.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("toolConfig")]
        public global::Gemini.ToolConfig? ToolConfig { get; set; }

        /// <summary>
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
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("safetySettings")]
        public global::System.Collections.Generic.IList<global::Gemini.SafetySetting>? SafetySettings { get; set; }

        /// <summary>
        /// Optional. Configuration options for model generation and outputs.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("generationConfig")]
        public global::Gemini.GenerationConfig? GenerationConfig { get; set; }

        /// <summary>
        /// Optional. The name of the content<br/>
        /// [cached](https://ai.google.dev/gemini-api/docs/caching) to use as context<br/>
        /// to serve the prediction. Format: `cachedContents/{cachedContent}`
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("cachedContent")]
        public string? CachedContent { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateContentRequest" /> class.
        /// </summary>
        /// <param name="model">
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
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public GenerateContentRequest(
            string model,
            global::System.Collections.Generic.IList<global::Gemini.Content> contents,
            global::Gemini.Content? systemInstruction,
            global::System.Collections.Generic.IList<global::Gemini.Tool>? tools,
            global::Gemini.ToolConfig? toolConfig,
            global::System.Collections.Generic.IList<global::Gemini.SafetySetting>? safetySettings,
            global::Gemini.GenerationConfig? generationConfig,
            string? cachedContent)
        {
            this.Model = model ?? throw new global::System.ArgumentNullException(nameof(model));
            this.Contents = contents ?? throw new global::System.ArgumentNullException(nameof(contents));
            this.SystemInstruction = systemInstruction;
            this.Tools = tools;
            this.ToolConfig = toolConfig;
            this.SafetySettings = safetySettings;
            this.GenerationConfig = generationConfig;
            this.CachedContent = cachedContent;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateContentRequest" /> class.
        /// </summary>
        public GenerateContentRequest()
        {
        }
    }
}