
#nullable enable

namespace Gemini
{
    public partial class GeminiApi
    {
        partial void PrepareUpdateChunkArguments(
            global::System.Net.Http.HttpClient httpClient,
            ref string corpus,
            ref string document,
            ref string chunk,
            ref string updateMask,
            global::Gemini.Chunk request);
        partial void PrepareUpdateChunkRequest(
            global::System.Net.Http.HttpClient httpClient,
            global::System.Net.Http.HttpRequestMessage httpRequestMessage,
            string corpus,
            string document,
            string chunk,
            string updateMask,
            global::Gemini.Chunk request);
        partial void ProcessUpdateChunkResponse(
            global::System.Net.Http.HttpClient httpClient,
            global::System.Net.Http.HttpResponseMessage httpResponseMessage);

        partial void ProcessUpdateChunkResponseContent(
            global::System.Net.Http.HttpClient httpClient,
            global::System.Net.Http.HttpResponseMessage httpResponseMessage,
            ref string content);

        /// <summary>
        /// Updates a `Chunk`.
        /// </summary>
        /// <param name="corpus"></param>
        /// <param name="document"></param>
        /// <param name="chunk"></param>
        /// <param name="updateMask"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::Gemini.ApiException"></exception>
        public async global::System.Threading.Tasks.Task<global::Gemini.Chunk> UpdateChunkAsync(
            string corpus,
            string document,
            string chunk,
            string updateMask,
            global::Gemini.Chunk request,
            global::System.Threading.CancellationToken cancellationToken = default)
        {
            request = request ?? throw new global::System.ArgumentNullException(nameof(request));

            PrepareArguments(
                client: HttpClient);
            PrepareUpdateChunkArguments(
                httpClient: HttpClient,
                corpus: ref corpus,
                document: ref document,
                chunk: ref chunk,
                updateMask: ref updateMask,
                request: request);

            var __pathBuilder = new global::Gemini.PathBuilder(
                path: $"/v1beta/corpora/{corpus}/documents/{document}/chunks/{chunk}",
                baseUri: HttpClient.BaseAddress); 
            __pathBuilder 
                .AddRequiredParameter("updateMask", updateMask) 
                ; 
            var __path = __pathBuilder.ToString();
            using var __httpRequest = new global::System.Net.Http.HttpRequestMessage(
                method: new global::System.Net.Http.HttpMethod("PATCH"),
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
            PrepareUpdateChunkRequest(
                httpClient: HttpClient,
                httpRequestMessage: __httpRequest,
                corpus: corpus,
                document: document,
                chunk: chunk,
                updateMask: updateMask,
                request: request);

            using var __response = await HttpClient.SendAsync(
                request: __httpRequest,
                completionOption: global::System.Net.Http.HttpCompletionOption.ResponseContentRead,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            ProcessResponse(
                client: HttpClient,
                response: __response);
            ProcessUpdateChunkResponse(
                httpClient: HttpClient,
                httpResponseMessage: __response);
            // Successful operation
            if (!__response.IsSuccessStatusCode)
            {
                string? __content_default = null;
                global::System.Exception? __exception_default = null;
                global::Gemini.Chunk? __value_default = null;
                try
                {
                    if (ReadResponseAsString)
                    {
                        __content_default = await __response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                        __value_default = global::Gemini.Chunk.FromJson(__content_default, JsonSerializerContext);
                    }
                    else
                    {
                        var __contentStream_default = await __response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
                        __value_default = await global::Gemini.Chunk.FromJsonStreamAsync(__contentStream_default, JsonSerializerContext).ConfigureAwait(false);
                    }
                }
                catch (global::System.Exception __ex)
                {
                    __exception_default = __ex;
                }

                throw new global::Gemini.ApiException<global::Gemini.Chunk>(
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
                ProcessUpdateChunkResponseContent(
                    httpClient: HttpClient,
                    httpResponseMessage: __response,
                    content: ref __content);

                try
                {
                    __response.EnsureSuccessStatusCode();

                    return
                        global::Gemini.Chunk.FromJson(__content, JsonSerializerContext) ??
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
                        await global::Gemini.Chunk.FromJsonStreamAsync(__content, JsonSerializerContext).ConfigureAwait(false) ??
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
        /// Updates a `Chunk`.
        /// </summary>
        /// <param name="corpus"></param>
        /// <param name="document"></param>
        /// <param name="chunk"></param>
        /// <param name="updateMask"></param>
        /// <param name="name">
        /// Immutable. Identifier. The `Chunk` resource name. The ID (name excluding the<br/>
        /// "corpora/*/documents/*/chunks/" prefix) can contain up to 40 characters<br/>
        /// that are lowercase alphanumeric or dashes (-). The ID cannot start or end<br/>
        /// with a dash. If the name is empty on create, a random 12-character unique<br/>
        /// ID will be generated.<br/>
        /// Example: `corpora/{corpus_id}/documents/{document_id}/chunks/123a456b789c`
        /// </param>
        /// <param name="data">
        /// Required. The content for the `Chunk`, such as the text string.<br/>
        /// The maximum number of tokens per chunk is 2043.
        /// </param>
        /// <param name="customMetadata">
        /// Optional. User provided custom metadata stored as key-value pairs.<br/>
        /// The maximum number of `CustomMetadata` per chunk is 20.
        /// </param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::System.InvalidOperationException"></exception>
        public async global::System.Threading.Tasks.Task<global::Gemini.Chunk> UpdateChunkAsync(
            string corpus,
            string document,
            string chunk,
            string updateMask,
            global::Gemini.ChunkData data,
            string? name = default,
            global::System.Collections.Generic.IList<global::Gemini.CustomMetadata>? customMetadata = default,
            global::System.Threading.CancellationToken cancellationToken = default)
        {
            var __request = new global::Gemini.Chunk
            {
                Name = name,
                Data = data,
                CustomMetadata = customMetadata,
            };

            return await UpdateChunkAsync(
                corpus: corpus,
                document: document,
                chunk: chunk,
                updateMask: updateMask,
                request: __request,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}