
#nullable enable

namespace Gemini
{
    public partial class GeminiApi
    {
        partial void PrepareGenerateContentByDynamicIdArguments(
            global::System.Net.Http.HttpClient httpClient,
            ref string dynamicId,
            global::Gemini.GenerateContentRequest request);
        partial void PrepareGenerateContentByDynamicIdRequest(
            global::System.Net.Http.HttpClient httpClient,
            global::System.Net.Http.HttpRequestMessage httpRequestMessage,
            string dynamicId,
            global::Gemini.GenerateContentRequest request);
        partial void ProcessGenerateContentByDynamicIdResponse(
            global::System.Net.Http.HttpClient httpClient,
            global::System.Net.Http.HttpResponseMessage httpResponseMessage);

        partial void ProcessGenerateContentByDynamicIdResponseContent(
            global::System.Net.Http.HttpClient httpClient,
            global::System.Net.Http.HttpResponseMessage httpResponseMessage,
            ref string content);

        /// <summary>
        /// Generates a model response given an input `GenerateContentRequest`.<br/>
        /// Refer to the [text generation<br/>
        /// guide](https://ai.google.dev/gemini-api/docs/text-generation) for detailed<br/>
        /// usage information. Input capabilities differ between models, including<br/>
        /// tuned models. Refer to the [model<br/>
        /// guide](https://ai.google.dev/gemini-api/docs/models/gemini) and [tuning<br/>
        /// guide](https://ai.google.dev/gemini-api/docs/model-tuning) for details.
        /// </summary>
        /// <param name="dynamicId"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::Gemini.ApiException"></exception>
        public async global::System.Threading.Tasks.Task<global::Gemini.GenerateContentResponse> GenerateContentByDynamicIdAsync(
            string dynamicId,
            global::Gemini.GenerateContentRequest request,
            global::System.Threading.CancellationToken cancellationToken = default)
        {
            request = request ?? throw new global::System.ArgumentNullException(nameof(request));

            PrepareArguments(
                client: HttpClient);
            PrepareGenerateContentByDynamicIdArguments(
                httpClient: HttpClient,
                dynamicId: ref dynamicId,
                request: request);

            var __pathBuilder = new global::Gemini.PathBuilder(
                path: $"/v1beta/dynamic/{dynamicId}:generateContent",
                baseUri: HttpClient.BaseAddress); 
            var __path = __pathBuilder.ToString();
            using var __httpRequest = new global::System.Net.Http.HttpRequestMessage(
                method: global::System.Net.Http.HttpMethod.Post,
                requestUri: new global::System.Uri(__path, global::System.UriKind.RelativeOrAbsolute));
#if NET6_0_OR_GREATER
            __httpRequest.Version = global::System.Net.HttpVersion.Version11;
            __httpRequest.VersionPolicy = global::System.Net.Http.HttpVersionPolicy.RequestVersionOrHigher;
#endif
            var __httpRequestContentBody = request.ToJson(JsonSerializerContext);
            var __httpRequestContent = new global::System.Net.Http.StringContent(
                content: __httpRequestContentBody,
                encoding: global::System.Text.Encoding.UTF8,
                mediaType: "application/json");
            __httpRequest.Content = __httpRequestContent;

            PrepareRequest(
                client: HttpClient,
                request: __httpRequest);
            PrepareGenerateContentByDynamicIdRequest(
                httpClient: HttpClient,
                httpRequestMessage: __httpRequest,
                dynamicId: dynamicId,
                request: request);

            using var __response = await HttpClient.SendAsync(
                request: __httpRequest,
                completionOption: global::System.Net.Http.HttpCompletionOption.ResponseContentRead,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            ProcessResponse(
                client: HttpClient,
                response: __response);
            ProcessGenerateContentByDynamicIdResponse(
                httpClient: HttpClient,
                httpResponseMessage: __response);
            // Successful operation
            if (!__response.IsSuccessStatusCode)
            {
                string? __content_default = null;
                global::System.Exception? __exception_default = null;
                global::Gemini.GenerateContentResponse? __value_default = null;
                try
                {
                    if (ReadResponseAsString)
                    {
                        __content_default = await __response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                        __value_default = global::Gemini.GenerateContentResponse.FromJson(__content_default, JsonSerializerContext);
                    }
                    else
                    {
                        var __contentStream_default = await __response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
                        __value_default = await global::Gemini.GenerateContentResponse.FromJsonStreamAsync(__contentStream_default, JsonSerializerContext).ConfigureAwait(false);
                    }
                }
                catch (global::System.Exception __ex)
                {
                    __exception_default = __ex;
                }

                throw new global::Gemini.ApiException<global::Gemini.GenerateContentResponse>(
                    message: __content_default ?? __response.ReasonPhrase ?? string.Empty,
                    innerException: __exception_default,
                    statusCode: __response.StatusCode)
                {
                    ResponseBody = __content_default,
                    ResponseObject = __value_default,
                    ResponseHeaders = global::System.Linq.Enumerable.ToDictionary(
                        __response.Headers,
                        h => h.Key,
                        h => h.Value),
                };
            }

            if (ReadResponseAsString)
            {
                var __content = await __response.Content.ReadAsStringAsync(
#if NET5_0_OR_GREATER
                    cancellationToken
#endif
                ).ConfigureAwait(false);

                ProcessResponseContent(
                    client: HttpClient,
                    response: __response,
                    content: ref __content);
                ProcessGenerateContentByDynamicIdResponseContent(
                    httpClient: HttpClient,
                    httpResponseMessage: __response,
                    content: ref __content);

                try
                {
                    __response.EnsureSuccessStatusCode();

                    return
                        global::Gemini.GenerateContentResponse.FromJson(__content, JsonSerializerContext) ??
                        throw new global::System.InvalidOperationException($"Response deserialization failed for \"{__content}\" ");
                }
                catch (global::System.Exception __ex)
                {
                    throw new global::Gemini.ApiException(
                        message: __content ?? __response.ReasonPhrase ?? string.Empty,
                        innerException: __ex,
                        statusCode: __response.StatusCode)
                    {
                        ResponseBody = __content,
                        ResponseHeaders = global::System.Linq.Enumerable.ToDictionary(
                            __response.Headers,
                            h => h.Key,
                            h => h.Value),
                    };
                }
            }
            else
            {
                try
                {
                    __response.EnsureSuccessStatusCode();

                    using var __content = await __response.Content.ReadAsStreamAsync(
#if NET5_0_OR_GREATER
                        cancellationToken
#endif
                    ).ConfigureAwait(false);

                    return
                        await global::Gemini.GenerateContentResponse.FromJsonStreamAsync(__content, JsonSerializerContext).ConfigureAwait(false) ??
                        throw new global::System.InvalidOperationException("Response deserialization failed.");
                }
                catch (global::System.Exception __ex)
                {
                    throw new global::Gemini.ApiException(
                        message: __response.ReasonPhrase ?? string.Empty,
                        innerException: __ex,
                        statusCode: __response.StatusCode)
                    {
                        ResponseHeaders = global::System.Linq.Enumerable.ToDictionary(
                            __response.Headers,
                            h => h.Key,
                            h => h.Value),
                    };
                }
            }
        }

        /// <summary>
        /// Generates a model response given an input `GenerateContentRequest`.<br/>
        /// Refer to the [text generation<br/>
        /// guide](https://ai.google.dev/gemini-api/docs/text-generation) for detailed<br/>
        /// usage information. Input capabilities differ between models, including<br/>
        /// tuned models. Refer to the [model<br/>
        /// guide](https://ai.google.dev/gemini-api/docs/models/gemini) and [tuning<br/>
        /// guide](https://ai.google.dev/gemini-api/docs/model-tuning) for details.
        /// </summary>
        /// <param name="dynamicId"></param>
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
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::System.InvalidOperationException"></exception>
        public async global::System.Threading.Tasks.Task<global::Gemini.GenerateContentResponse> GenerateContentByDynamicIdAsync(
            string dynamicId,
            string model,
            global::System.Collections.Generic.IList<global::Gemini.Content> contents,
            global::Gemini.Content? systemInstruction = default,
            global::System.Collections.Generic.IList<global::Gemini.Tool>? tools = default,
            global::Gemini.ToolConfig? toolConfig = default,
            global::System.Collections.Generic.IList<global::Gemini.SafetySetting>? safetySettings = default,
            global::Gemini.GenerationConfig? generationConfig = default,
            string? cachedContent = default,
            global::System.Threading.CancellationToken cancellationToken = default)
        {
            var __request = new global::Gemini.GenerateContentRequest
            {
                Model = model,
                SystemInstruction = systemInstruction,
                Contents = contents,
                Tools = tools,
                ToolConfig = toolConfig,
                SafetySettings = safetySettings,
                GenerationConfig = generationConfig,
                CachedContent = cachedContent,
            };

            return await GenerateContentByDynamicIdAsync(
                dynamicId: dynamicId,
                request: __request,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}