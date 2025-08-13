
#nullable enable

namespace Gemini
{
    public partial class GeminiApi
    {
        partial void PrepareCancelOperationArguments(
            global::System.Net.Http.HttpClient httpClient,
            ref string generateContentBatch);
        partial void PrepareCancelOperationRequest(
            global::System.Net.Http.HttpClient httpClient,
            global::System.Net.Http.HttpRequestMessage httpRequestMessage,
            string generateContentBatch);
        partial void ProcessCancelOperationResponse(
            global::System.Net.Http.HttpClient httpClient,
            global::System.Net.Http.HttpResponseMessage httpResponseMessage);

        partial void ProcessCancelOperationResponseContent(
            global::System.Net.Http.HttpClient httpClient,
            global::System.Net.Http.HttpResponseMessage httpResponseMessage,
            ref string content);

        /// <summary>
        /// Starts asynchronous cancellation on a long-running operation.  The server<br/>
        /// makes a best effort to cancel the operation, but success is not<br/>
        /// guaranteed.  If the server doesn't support this method, it returns<br/>
        /// `google.rpc.Code.UNIMPLEMENTED`.  Clients can use<br/>
        /// Operations.GetOperation or<br/>
        /// other methods to check whether the cancellation succeeded or whether the<br/>
        /// operation completed despite cancellation. On successful cancellation,<br/>
        /// the operation is not deleted; instead, it becomes an operation with<br/>
        /// an Operation.error value with a google.rpc.Status.code of `1`,<br/>
        /// corresponding to `Code.CANCELLED`.
        /// </summary>
        /// <param name="generateContentBatch"></param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::Gemini.ApiException"></exception>
        public async global::System.Threading.Tasks.Task<string> CancelOperationAsync(
            string generateContentBatch,
            global::System.Threading.CancellationToken cancellationToken = default)
        {
            PrepareArguments(
                client: HttpClient);
            PrepareCancelOperationArguments(
                httpClient: HttpClient,
                generateContentBatch: ref generateContentBatch);

            var __pathBuilder = new global::Gemini.PathBuilder(
                path: $"/v1beta/batches/{generateContentBatch}:cancel",
                baseUri: HttpClient.BaseAddress); 
            var __path = __pathBuilder.ToString();
            using var __httpRequest = new global::System.Net.Http.HttpRequestMessage(
                method: global::System.Net.Http.HttpMethod.Post,
                requestUri: new global::System.Uri(__path, global::System.UriKind.RelativeOrAbsolute));
#if NET6_0_OR_GREATER
            __httpRequest.Version = global::System.Net.HttpVersion.Version11;
            __httpRequest.VersionPolicy = global::System.Net.Http.HttpVersionPolicy.RequestVersionOrHigher;
#endif

            PrepareRequest(
                client: HttpClient,
                request: __httpRequest);
            PrepareCancelOperationRequest(
                httpClient: HttpClient,
                httpRequestMessage: __httpRequest,
                generateContentBatch: generateContentBatch);

            using var __response = await HttpClient.SendAsync(
                request: __httpRequest,
                completionOption: global::System.Net.Http.HttpCompletionOption.ResponseContentRead,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            ProcessResponse(
                client: HttpClient,
                response: __response);
            ProcessCancelOperationResponse(
                httpClient: HttpClient,
                httpResponseMessage: __response);
            // Successful operation
            if (!__response.IsSuccessStatusCode)
            {
                string? __content_default = null;
                global::System.Exception? __exception_default = null;
                string? __value_default = null;
                try
                {
                    if (ReadResponseAsString)
                    {
                        __content_default = await __response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                        __value_default = global::System.Text.Json.JsonSerializer.Deserialize(__content_default, typeof(string), JsonSerializerContext) as string;
                    }
                    else
                    {
                        var __contentStream_default = await __response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
                        __value_default = await global::System.Text.Json.JsonSerializer.DeserializeAsync(__contentStream_default, typeof(string), JsonSerializerContext).ConfigureAwait(false) as string;
                    }
                }
                catch (global::System.Exception __ex)
                {
                    __exception_default = __ex;
                }

                throw new global::Gemini.ApiException<string>(
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
                ProcessCancelOperationResponseContent(
                    httpClient: HttpClient,
                    httpResponseMessage: __response,
                    content: ref __content);

                try
                {
                    __response.EnsureSuccessStatusCode();

                    return __content;
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

                    var __content = await __response.Content.ReadAsStringAsync(
#if NET5_0_OR_GREATER
                        cancellationToken
#endif
                    ).ConfigureAwait(false);

                    return __content;
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
    }
}