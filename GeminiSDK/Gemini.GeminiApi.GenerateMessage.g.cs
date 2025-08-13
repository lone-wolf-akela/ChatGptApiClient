
#nullable enable

namespace Gemini
{
    public partial class GeminiApi
    {
        partial void PrepareGenerateMessageArguments(
            global::System.Net.Http.HttpClient httpClient,
            ref string model,
            global::Gemini.GenerateMessageRequest request);
        partial void PrepareGenerateMessageRequest(
            global::System.Net.Http.HttpClient httpClient,
            global::System.Net.Http.HttpRequestMessage httpRequestMessage,
            string model,
            global::Gemini.GenerateMessageRequest request);
        partial void ProcessGenerateMessageResponse(
            global::System.Net.Http.HttpClient httpClient,
            global::System.Net.Http.HttpResponseMessage httpResponseMessage);

        partial void ProcessGenerateMessageResponseContent(
            global::System.Net.Http.HttpClient httpClient,
            global::System.Net.Http.HttpResponseMessage httpResponseMessage,
            ref string content);

        /// <summary>
        /// Generates a response from the model given an input `MessagePrompt`.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::Gemini.ApiException"></exception>
        public async global::System.Threading.Tasks.Task<global::Gemini.GenerateMessageResponse> GenerateMessageAsync(
            string model,
            global::Gemini.GenerateMessageRequest request,
            global::System.Threading.CancellationToken cancellationToken = default)
        {
            request = request ?? throw new global::System.ArgumentNullException(nameof(request));

            PrepareArguments(
                client: HttpClient);
            PrepareGenerateMessageArguments(
                httpClient: HttpClient,
                model: ref model,
                request: request);

            var __pathBuilder = new global::Gemini.PathBuilder(
                path: $"/v1beta/models/{model}:generateMessage",
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
            PrepareGenerateMessageRequest(
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
            ProcessGenerateMessageResponse(
                httpClient: HttpClient,
                httpResponseMessage: __response);
            // Successful operation
            if (!__response.IsSuccessStatusCode)
            {
                string? __content_default = null;
                global::System.Exception? __exception_default = null;
                global::Gemini.GenerateMessageResponse? __value_default = null;
                try
                {
                    if (ReadResponseAsString)
                    {
                        __content_default = await __response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                        __value_default = global::Gemini.GenerateMessageResponse.FromJson(__content_default, JsonSerializerContext);
                    }
                    else
                    {
                        var __contentStream_default = await __response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
                        __value_default = await global::Gemini.GenerateMessageResponse.FromJsonStreamAsync(__contentStream_default, JsonSerializerContext).ConfigureAwait(false);
                    }
                }
                catch (global::System.Exception __ex)
                {
                    __exception_default = __ex;
                }

                throw new global::Gemini.ApiException<global::Gemini.GenerateMessageResponse>(
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
                ProcessGenerateMessageResponseContent(
                    httpClient: HttpClient,
                    httpResponseMessage: __response,
                    content: ref __content);

                try
                {
                    __response.EnsureSuccessStatusCode();

                    return
                        global::Gemini.GenerateMessageResponse.FromJson(__content, JsonSerializerContext) ??
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
                        await global::Gemini.GenerateMessageResponse.FromJsonStreamAsync(__content, JsonSerializerContext).ConfigureAwait(false) ??
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
        /// Generates a response from the model given an input `MessagePrompt`.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="prompt">
        /// Required. The structured textual input given to the model as a prompt.<br/>
        /// Given a<br/>
        /// prompt, the model will return what it predicts is the next message in the<br/>
        /// discussion.
        /// </param>
        /// <param name="temperature">
        /// Optional. Controls the randomness of the output.<br/>
        /// Values can range over `[0.0,1.0]`,<br/>
        /// inclusive. A value closer to `1.0` will produce responses that are more<br/>
        /// varied, while a value closer to `0.0` will typically result in<br/>
        /// less surprising responses from the model.
        /// </param>
        /// <param name="candidateCount">
        /// Optional. The number of generated response messages to return.<br/>
        /// This value must be between<br/>
        /// `[1, 8]`, inclusive. If unset, this will default to `1`.
        /// </param>
        /// <param name="topP">
        /// Optional. The maximum cumulative probability of tokens to consider when sampling.<br/>
        /// The model uses combined Top-k and nucleus sampling.<br/>
        /// Nucleus sampling considers the smallest set of tokens whose probability<br/>
        /// sum is at least `top_p`.
        /// </param>
        /// <param name="topK">
        /// Optional. The maximum number of tokens to consider when sampling.<br/>
        /// The model uses combined Top-k and nucleus sampling.<br/>
        /// Top-k sampling considers the set of `top_k` most probable tokens.
        /// </param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::System.InvalidOperationException"></exception>
        public async global::System.Threading.Tasks.Task<global::Gemini.GenerateMessageResponse> GenerateMessageAsync(
            string model,
            global::Gemini.MessagePrompt prompt,
            float? temperature = default,
            int? candidateCount = default,
            float? topP = default,
            int? topK = default,
            global::System.Threading.CancellationToken cancellationToken = default)
        {
            var __request = new global::Gemini.GenerateMessageRequest
            {
                Prompt = prompt,
                Temperature = temperature,
                CandidateCount = candidateCount,
                TopP = topP,
                TopK = topK,
            };

            return await GenerateMessageAsync(
                model: model,
                request: __request,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}