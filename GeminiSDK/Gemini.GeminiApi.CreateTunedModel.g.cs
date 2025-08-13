
#nullable enable

namespace Gemini
{
    public partial class GeminiApi
    {
        partial void PrepareCreateTunedModelArguments(
            global::System.Net.Http.HttpClient httpClient,
            ref string? tunedModelId,
            global::Gemini.TunedModel request);
        partial void PrepareCreateTunedModelRequest(
            global::System.Net.Http.HttpClient httpClient,
            global::System.Net.Http.HttpRequestMessage httpRequestMessage,
            string? tunedModelId,
            global::Gemini.TunedModel request);
        partial void ProcessCreateTunedModelResponse(
            global::System.Net.Http.HttpClient httpClient,
            global::System.Net.Http.HttpResponseMessage httpResponseMessage);

        partial void ProcessCreateTunedModelResponseContent(
            global::System.Net.Http.HttpClient httpClient,
            global::System.Net.Http.HttpResponseMessage httpResponseMessage,
            ref string content);

        /// <summary>
        /// Creates a tuned model.<br/>
        /// Check intermediate tuning progress (if any) through the<br/>
        /// [google.longrunning.Operations] service.<br/>
        /// Access status and results through the Operations service.<br/>
        /// Example:<br/>
        ///   GET /v1/tunedModels/az2mb0bpw6i/operations/000-111-222
        /// </summary>
        /// <param name="tunedModelId"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::Gemini.ApiException"></exception>
        public async global::System.Threading.Tasks.Task<global::Gemini.CreateTunedModelOperation> CreateTunedModelAsync(
            global::Gemini.TunedModel request,
            string? tunedModelId = default,
            global::System.Threading.CancellationToken cancellationToken = default)
        {
            request = request ?? throw new global::System.ArgumentNullException(nameof(request));

            PrepareArguments(
                client: HttpClient);
            PrepareCreateTunedModelArguments(
                httpClient: HttpClient,
                tunedModelId: ref tunedModelId,
                request: request);

            var __pathBuilder = new global::Gemini.PathBuilder(
                path: "/v1beta/tunedModels",
                baseUri: HttpClient.BaseAddress); 
            __pathBuilder 
                .AddOptionalParameter("tunedModelId", tunedModelId) 
                ; 
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
            PrepareCreateTunedModelRequest(
                httpClient: HttpClient,
                httpRequestMessage: __httpRequest,
                tunedModelId: tunedModelId,
                request: request);

            using var __response = await HttpClient.SendAsync(
                request: __httpRequest,
                completionOption: global::System.Net.Http.HttpCompletionOption.ResponseContentRead,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            ProcessResponse(
                client: HttpClient,
                response: __response);
            ProcessCreateTunedModelResponse(
                httpClient: HttpClient,
                httpResponseMessage: __response);
            // Successful operation
            if (!__response.IsSuccessStatusCode)
            {
                string? __content_default = null;
                global::System.Exception? __exception_default = null;
                global::Gemini.CreateTunedModelOperation? __value_default = null;
                try
                {
                    if (ReadResponseAsString)
                    {
                        __content_default = await __response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                        __value_default = global::Gemini.CreateTunedModelOperation.FromJson(__content_default, JsonSerializerContext);
                    }
                    else
                    {
                        var __contentStream_default = await __response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
                        __value_default = await global::Gemini.CreateTunedModelOperation.FromJsonStreamAsync(__contentStream_default, JsonSerializerContext).ConfigureAwait(false);
                    }
                }
                catch (global::System.Exception __ex)
                {
                    __exception_default = __ex;
                }

                throw new global::Gemini.ApiException<global::Gemini.CreateTunedModelOperation?>(
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
                ProcessCreateTunedModelResponseContent(
                    httpClient: HttpClient,
                    httpResponseMessage: __response,
                    content: ref __content);

                try
                {
                    __response.EnsureSuccessStatusCode();

                    return
                        global::Gemini.CreateTunedModelOperation.FromJson(__content, JsonSerializerContext) ??
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
                        await global::Gemini.CreateTunedModelOperation.FromJsonStreamAsync(__content, JsonSerializerContext).ConfigureAwait(false) ??
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
        /// Creates a tuned model.<br/>
        /// Check intermediate tuning progress (if any) through the<br/>
        /// [google.longrunning.Operations] service.<br/>
        /// Access status and results through the Operations service.<br/>
        /// Example:<br/>
        ///   GET /v1/tunedModels/az2mb0bpw6i/operations/000-111-222
        /// </summary>
        /// <param name="tunedModelId"></param>
        /// <param name="tunedModelSource">
        /// Optional. TunedModel to use as the starting point for training the new model.
        /// </param>
        /// <param name="baseModel">
        /// Immutable. The name of the `Model` to tune.<br/>
        /// Example: `models/gemini-1.5-flash-001`
        /// </param>
        /// <param name="displayName">
        /// Optional. The name to display for this model in user interfaces.<br/>
        /// The display name must be up to 40 characters including spaces.
        /// </param>
        /// <param name="description">
        /// Optional. A short description of this model.
        /// </param>
        /// <param name="temperature">
        /// Optional. Controls the randomness of the output.<br/>
        /// Values can range over `[0.0,1.0]`, inclusive. A value closer to `1.0` will<br/>
        /// produce responses that are more varied, while a value closer to `0.0` will<br/>
        /// typically result in less surprising responses from the model.<br/>
        /// This value specifies default to be the one used by the base model while<br/>
        /// creating the model.
        /// </param>
        /// <param name="topP">
        /// Optional. For Nucleus sampling.<br/>
        /// Nucleus sampling considers the smallest set of tokens whose probability<br/>
        /// sum is at least `top_p`.<br/>
        /// This value specifies default to be the one used by the base model while<br/>
        /// creating the model.
        /// </param>
        /// <param name="topK">
        /// Optional. For Top-k sampling.<br/>
        /// Top-k sampling considers the set of `top_k` most probable tokens.<br/>
        /// This value specifies default to be used by the backend while making the<br/>
        /// call to the model.<br/>
        /// This value specifies default to be the one used by the base model while<br/>
        /// creating the model.
        /// </param>
        /// <param name="tuningTask">
        /// Required. The tuning task that creates the tuned model.
        /// </param>
        /// <param name="readerProjectNumbers">
        /// Optional. List of project numbers that have read access to the tuned model.
        /// </param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::System.InvalidOperationException"></exception>
        public async global::System.Threading.Tasks.Task<global::Gemini.CreateTunedModelOperation> CreateTunedModelAsync(
            global::Gemini.TuningTask tuningTask,
            string? tunedModelId = default,
            global::Gemini.TunedModelSource? tunedModelSource = default,
            string? baseModel = default,
            string? displayName = default,
            string? description = default,
            float? temperature = default,
            float? topP = default,
            int? topK = default,
            global::System.Collections.Generic.IList<string>? readerProjectNumbers = default,
            global::System.Threading.CancellationToken cancellationToken = default)
        {
            var __request = new global::Gemini.TunedModel
            {
                TunedModelSource = tunedModelSource,
                BaseModel = baseModel,
                DisplayName = displayName,
                Description = description,
                Temperature = temperature,
                TopP = topP,
                TopK = topK,
                TuningTask = tuningTask,
                ReaderProjectNumbers = readerProjectNumbers,
            };

            return await CreateTunedModelAsync(
                tunedModelId: tunedModelId,
                request: __request,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}