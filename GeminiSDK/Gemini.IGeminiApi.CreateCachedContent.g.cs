#nullable enable

namespace Gemini
{
    public partial interface IGeminiApi
    {
        /// <summary>
        /// Creates CachedContent resource.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::Gemini.ApiException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.CachedContent> CreateCachedContentAsync(
            global::Gemini.CachedContent request,
            global::System.Threading.CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates CachedContent resource.
        /// </summary>
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
        global::System.Threading.Tasks.Task<global::Gemini.CachedContent> CreateCachedContentAsync(
            string ttl,
            string model,
            global::Gemini.Content systemInstruction,
            global::System.Collections.Generic.IList<global::Gemini.Content> contents,
            global::System.Collections.Generic.IList<global::Gemini.Tool> tools,
            global::Gemini.ToolConfig toolConfig,
            global::System.DateTime? expireTime = default,
            string? displayName = default,
            global::System.Threading.CancellationToken cancellationToken = default);
    }
}