
#nullable enable

namespace Gemini
{
    public partial class GeminiApi
    {
        partial void PrepareQueryCorpusArguments(
            global::System.Net.Http.HttpClient httpClient,
            ref string corpus,
            global::Gemini.QueryCorpusRequest request);
        partial void PrepareQueryCorpusRequest(
            global::System.Net.Http.HttpClient httpClient,
            global::System.Net.Http.HttpRequestMessage httpRequestMessage,
            string corpus,
            global::Gemini.QueryCorpusRequest request);
        partial void ProcessQueryCorpusResponse(
            global::System.Net.Http.HttpClient httpClient,
            global::System.Net.Http.HttpResponseMessage httpResponseMessage);

        partial void ProcessQueryCorpusResponseContent(
            global::System.Net.Http.HttpClient httpClient,
            global::System.Net.Http.HttpResponseMessage httpResponseMessage,
            ref string content);

        /// <summary>
        /// Performs semantic search over a `Corpus`.
        /// </summary>
        /// <param name="corpus"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::Gemini.ApiException"></exception>
        public async global::System.Threading.Tasks.Task<global::Gemini.QueryCorpusResponse> QueryCorpusAsync(
            string corpus,
            global::Gemini.QueryCorpusRequest request,
            global::System.Threading.CancellationToken cancellationToken = default)
        {
            request = request ?? throw new global::System.ArgumentNullException(nameof(request));

            PrepareArguments(
                client: HttpClient);
            PrepareQueryCorpusArguments(
                httpClient: HttpClient,
                corpus: ref corpus,
                request: request);

            var __pathBuilder = new global::Gemini.PathBuilder(
                path: $"/v1beta/corpora/{corpus}:query",
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
            PrepareQueryCorpusRequest(
                httpClient: HttpClient,
                httpRequestMessage: __httpRequest,
                corpus: corpus,
                request: request);

            using var __response = await HttpClient.SendAsync(
                request: __httpRequest,
                completionOption: global::System.Net.Http.HttpCompletionOption.ResponseContentRead,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            ProcessResponse(
                client: HttpClient,
                response: __response);
            ProcessQueryCorpusResponse(
                httpClient: HttpClient,
                httpResponseMessage: __response);
            // Successful operation
            if (!__response.IsSuccessStatusCode)
            {
                string? __content_default = null;
                global::System.Exception? __exception_default = null;
                global::Gemini.QueryCorpusResponse? __value_default = null;
                try
                {
                    if (ReadResponseAsString)
                    {
                        __content_default = await __response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                        __value_default = global::Gemini.QueryCorpusResponse.FromJson(__content_default, JsonSerializerContext);
                    }
                    else
                    {
                        var __contentStream_default = await __response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
                        __value_default = await global::Gemini.QueryCorpusResponse.FromJsonStreamAsync(__contentStream_default, JsonSerializerContext).ConfigureAwait(false);
                    }
                }
                catch (global::System.Exception __ex)
                {
                    __exception_default = __ex;
                }

                throw new global::Gemini.ApiException<global::Gemini.QueryCorpusResponse>(
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
                ProcessQueryCorpusResponseContent(
                    httpClient: HttpClient,
                    httpResponseMessage: __response,
                    content: ref __content);

                try
                {
                    __response.EnsureSuccessStatusCode();

                    return
                        global::Gemini.QueryCorpusResponse.FromJson(__content, JsonSerializerContext) ??
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
                        await global::Gemini.QueryCorpusResponse.FromJsonStreamAsync(__content, JsonSerializerContext).ConfigureAwait(false) ??
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
        /// Performs semantic search over a `Corpus`.
        /// </summary>
        /// <param name="corpus"></param>
        /// <param name="query">
        /// Required. Query string to perform semantic search.
        /// </param>
        /// <param name="metadataFilters">
        /// Optional. Filter for `Chunk` and `Document` metadata. Each `MetadataFilter` object<br/>
        /// should correspond to a unique key. Multiple `MetadataFilter` objects are<br/>
        /// joined by logical "AND"s.<br/>
        /// Example query at document level:<br/>
        /// (year &gt;= 2020 OR year &lt; 2010) AND (genre = drama OR genre = action)<br/>
        /// `MetadataFilter` object list:<br/>
        ///  metadata_filters = [<br/>
        ///  {key = "document.custom_metadata.year"<br/>
        ///   conditions = [{int_value = 2020, operation = GREATER_EQUAL},<br/>
        ///                 {int_value = 2010, operation = LESS}]},<br/>
        ///  {key = "document.custom_metadata.year"<br/>
        ///   conditions = [{int_value = 2020, operation = GREATER_EQUAL},<br/>
        ///                 {int_value = 2010, operation = LESS}]},<br/>
        ///  {key = "document.custom_metadata.genre"<br/>
        ///   conditions = [{string_value = "drama", operation = EQUAL},<br/>
        ///                 {string_value = "action", operation = EQUAL}]}]<br/>
        /// Example query at chunk level for a numeric range of values:<br/>
        /// (year &gt; 2015 AND year &lt;= 2020)<br/>
        /// `MetadataFilter` object list:<br/>
        ///  metadata_filters = [<br/>
        ///  {key = "chunk.custom_metadata.year"<br/>
        ///   conditions = [{int_value = 2015, operation = GREATER}]},<br/>
        ///  {key = "chunk.custom_metadata.year"<br/>
        ///   conditions = [{int_value = 2020, operation = LESS_EQUAL}]}]<br/>
        /// Note: "AND"s for the same key are only supported for numeric values. String<br/>
        /// values only support "OR"s for the same key.
        /// </param>
        /// <param name="resultsCount">
        /// Optional. The maximum number of `Chunk`s to return.<br/>
        /// The service may return fewer `Chunk`s.<br/>
        /// If unspecified, at most 10 `Chunk`s will be returned.<br/>
        /// The maximum specified result count is 100.
        /// </param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::System.InvalidOperationException"></exception>
        public async global::System.Threading.Tasks.Task<global::Gemini.QueryCorpusResponse> QueryCorpusAsync(
            string corpus,
            string query,
            global::System.Collections.Generic.IList<global::Gemini.MetadataFilter>? metadataFilters = default,
            int? resultsCount = default,
            global::System.Threading.CancellationToken cancellationToken = default)
        {
            var __request = new global::Gemini.QueryCorpusRequest
            {
                Query = query,
                MetadataFilters = metadataFilters,
                ResultsCount = resultsCount,
            };

            return await QueryCorpusAsync(
                corpus: corpus,
                request: __request,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}