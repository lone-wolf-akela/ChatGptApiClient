
#nullable enable

namespace Gemini
{
    public partial class GeminiApi
    {
        partial void PrepareGenerateAnswerArguments(
            global::System.Net.Http.HttpClient httpClient,
            ref string model,
            global::Gemini.GenerateAnswerRequest request);
        partial void PrepareGenerateAnswerRequest(
            global::System.Net.Http.HttpClient httpClient,
            global::System.Net.Http.HttpRequestMessage httpRequestMessage,
            string model,
            global::Gemini.GenerateAnswerRequest request);
        partial void ProcessGenerateAnswerResponse(
            global::System.Net.Http.HttpClient httpClient,
            global::System.Net.Http.HttpResponseMessage httpResponseMessage);

        partial void ProcessGenerateAnswerResponseContent(
            global::System.Net.Http.HttpClient httpClient,
            global::System.Net.Http.HttpResponseMessage httpResponseMessage,
            ref string content);

        /// <summary>
        /// Generates a grounded answer from the model given an input<br/>
        /// `GenerateAnswerRequest`.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::Gemini.ApiException"></exception>
        public async global::System.Threading.Tasks.Task<global::Gemini.GenerateAnswerResponse> GenerateAnswerAsync(
            string model,
            global::Gemini.GenerateAnswerRequest request,
            global::System.Threading.CancellationToken cancellationToken = default)
        {
            request = request ?? throw new global::System.ArgumentNullException(nameof(request));

            PrepareArguments(
                client: HttpClient);
            PrepareGenerateAnswerArguments(
                httpClient: HttpClient,
                model: ref model,
                request: request);

            var __pathBuilder = new global::Gemini.PathBuilder(
                path: $"/v1beta/models/{model}:generateAnswer",
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
            PrepareGenerateAnswerRequest(
                httpClient: HttpClient,
                httpRequestMessage: __httpRequest,
                model: model,
                request: request);

            using var __response = await HttpClient.SendAsync(
                request: __httpRequest,
                completionOption: global::System.Net.Http.HttpCompletionOption.ResponseContentRead,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            ProcessResponse(
                client: HttpClient,
                response: __response);
            ProcessGenerateAnswerResponse(
                httpClient: HttpClient,
                httpResponseMessage: __response);
            // Successful operation
            if (!__response.IsSuccessStatusCode)
            {
                string? __content_default = null;
                global::System.Exception? __exception_default = null;
                global::Gemini.GenerateAnswerResponse? __value_default = null;
                try
                {
                    if (ReadResponseAsString)
                    {
                        __content_default = await __response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                        __value_default = global::Gemini.GenerateAnswerResponse.FromJson(__content_default, JsonSerializerContext);
                    }
                    else
                    {
                        var __contentStream_default = await __response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
                        __value_default = await global::Gemini.GenerateAnswerResponse.FromJsonStreamAsync(__contentStream_default, JsonSerializerContext).ConfigureAwait(false);
                    }
                }
                catch (global::System.Exception __ex)
                {
                    __exception_default = __ex;
                }

                throw new global::Gemini.ApiException<global::Gemini.GenerateAnswerResponse>(
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
                ProcessGenerateAnswerResponseContent(
                    httpClient: HttpClient,
                    httpResponseMessage: __response,
                    content: ref __content);

                try
                {
                    __response.EnsureSuccessStatusCode();

                    return
                        global::Gemini.GenerateAnswerResponse.FromJson(__content, JsonSerializerContext) ??
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
                        await global::Gemini.GenerateAnswerResponse.FromJsonStreamAsync(__content, JsonSerializerContext).ConfigureAwait(false) ??
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
        public async global::System.Threading.Tasks.Task<global::Gemini.GenerateAnswerResponse> GenerateAnswerAsync(
            string model,
            global::System.Collections.Generic.IList<global::Gemini.Content> contents,
            global::Gemini.GenerateAnswerRequestAnswerStyle answerStyle,
            global::Gemini.GroundingPassages? inlinePassages = default,
            global::Gemini.SemanticRetrieverConfig? semanticRetriever = default,
            global::System.Collections.Generic.IList<global::Gemini.SafetySetting>? safetySettings = default,
            float? temperature = default,
            global::System.Threading.CancellationToken cancellationToken = default)
        {
            var __request = new global::Gemini.GenerateAnswerRequest
            {
                InlinePassages = inlinePassages,
                SemanticRetriever = semanticRetriever,
                Contents = contents,
                AnswerStyle = answerStyle,
                SafetySettings = safetySettings,
                Temperature = temperature,
            };

            return await GenerateAnswerAsync(
                model: model,
                request: __request,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}