
#nullable enable

namespace Gemini
{
    public partial class GeminiApi
    {
        partial void PrepareUpdateCachedContentArguments(
            global::System.Net.Http.HttpClient httpClient,
            ref string id,
            ref string? updateMask,
            global::Gemini.CachedContent request);
        partial void PrepareUpdateCachedContentRequest(
            global::System.Net.Http.HttpClient httpClient,
            global::System.Net.Http.HttpRequestMessage httpRequestMessage,
            string id,
            string? updateMask,
            global::Gemini.CachedContent request);
        partial void ProcessUpdateCachedContentResponse(
            global::System.Net.Http.HttpClient httpClient,
            global::System.Net.Http.HttpResponseMessage httpResponseMessage);

        partial void ProcessUpdateCachedContentResponseContent(
            global::System.Net.Http.HttpClient httpClient,
            global::System.Net.Http.HttpResponseMessage httpResponseMessage,
            ref string content);

        /// <summary>
        /// Updates CachedContent resource (only expiration is updatable).
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateMask"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::Gemini.ApiException"></exception>
        public async global::System.Threading.Tasks.Task<global::Gemini.CachedContent> UpdateCachedContentAsync(
            string id,
            global::Gemini.CachedContent request,
            string? updateMask = default,
            global::System.Threading.CancellationToken cancellationToken = default)
        {
            request = request ?? throw new global::System.ArgumentNullException(nameof(request));

            PrepareArguments(
                client: HttpClient);
            PrepareUpdateCachedContentArguments(
                httpClient: HttpClient,
                id: ref id,
                updateMask: ref updateMask,
                request: request);

            var __pathBuilder = new global::Gemini.PathBuilder(
                path: $"/v1beta/cachedContents/{id}",
                baseUri: HttpClient.BaseAddress); 
            __pathBuilder 
                .AddOptionalParameter("updateMask", updateMask) 
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
            PrepareUpdateCachedContentRequest(
                httpClient: HttpClient,
                httpRequestMessage: __httpRequest,
                id: id,
                updateMask: updateMask,
                request: request);

            using var __response = await HttpClient.SendAsync(
                request: __httpRequest,
                completionOption: global::System.Net.Http.HttpCompletionOption.ResponseContentRead,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            ProcessResponse(
                client: HttpClient,
                response: __response);
            ProcessUpdateCachedContentResponse(
                httpClient: HttpClient,
                httpResponseMessage: __response);
            // Successful operation
            if (!__response.IsSuccessStatusCode)
            {
                string? __content_default = null;
                global::System.Exception? __exception_default = null;
                global::Gemini.CachedContent? __value_default = null;
                try
                {
                    if (ReadResponseAsString)
                    {
                        __content_default = await __response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                        __value_default = global::Gemini.CachedContent.FromJson(__content_default, JsonSerializerContext);
                    }
                    else
                    {
                        var __contentStream_default = await __response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
                        __value_default = await global::Gemini.CachedContent.FromJsonStreamAsync(__contentStream_default, JsonSerializerContext).ConfigureAwait(false);
                    }
                }
                catch (global::System.Exception __ex)
                {
                    __exception_default = __ex;
                }

                throw new global::Gemini.ApiException<global::Gemini.CachedContent>(
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
                ProcessUpdateCachedContentResponseContent(
                    httpClient: HttpClient,
                    httpResponseMessage: __response,
                    content: ref __content);

                try
                {
                    __response.EnsureSuccessStatusCode();

                    return
                        global::Gemini.CachedContent.FromJson(__content, JsonSerializerContext) ??
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
                        await global::Gemini.CachedContent.FromJsonStreamAsync(__content, JsonSerializerContext).ConfigureAwait(false) ??
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
        /// Updates CachedContent resource (only expiration is updatable).
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateMask"></param>
        /// <param name="expireTime">
        /// Timestamp in UTC of when this resource is considered expired.<br/>
        /// This is *always* provided on output, regardless of what was sent<br/>
        /// on input.
        /// </param>
        /// <param name="ttl">
        /// Input only. New TTL for this resource, input only.<br/>
        /// Included only in requests
        /// </param>
        /// <param name="displayName">
        /// Optional. Immutable. The user-generated meaningful display name of the cached content. Maximum<br/>
        /// 128 Unicode characters.
        /// </param>
        /// <param name="model">
        /// Required. Immutable. The name of the `Model` to use for cached content<br/>
        /// Format: `models/{model}`
        /// </param>
        /// <param name="systemInstruction">
        /// Optional. Input only. Immutable. Developer set system instruction. Currently text only.<br/>
        /// Included only in requests
        /// </param>
        /// <param name="contents">
        /// Optional. Input only. Immutable. The content to cache.<br/>
        /// Included only in requests
        /// </param>
        /// <param name="tools">
        /// Optional. Input only. Immutable. A list of `Tools` the model may use to generate the next response<br/>
        /// Included only in requests
        /// </param>
        /// <param name="toolConfig">
        /// Optional. Input only. Immutable. Tool config. This config is shared for all tools.<br/>
        /// Included only in requests
        /// </param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::System.InvalidOperationException"></exception>
        public async global::System.Threading.Tasks.Task<global::Gemini.CachedContent> UpdateCachedContentAsync(
            string id,
            string ttl,
            string model,
            global::Gemini.Content systemInstruction,
            global::System.Collections.Generic.IList<global::Gemini.Content> contents,
            global::System.Collections.Generic.IList<global::Gemini.Tool> tools,
            global::Gemini.ToolConfig toolConfig,
            string? updateMask = default,
            global::System.DateTime? expireTime = default,
            string? displayName = default,
            global::System.Threading.CancellationToken cancellationToken = default)
        {
            var __request = new global::Gemini.CachedContent
            {
                ExpireTime = expireTime,
                Ttl = ttl,
                DisplayName = displayName,
                Model = model,
                SystemInstruction = systemInstruction,
                Contents = contents,
                Tools = tools,
                ToolConfig = toolConfig,
            };

            return await UpdateCachedContentAsync(
                id: id,
                updateMask: updateMask,
                request: __request,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}