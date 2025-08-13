#nullable enable

namespace Gemini
{
    public partial interface IGeminiApi
    {
        /// <summary>
        /// Generates multiple embeddings from the model given input text in a<br/>
        /// synchronous call.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::Gemini.ApiException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.BatchEmbedTextResponse> BatchEmbedTextAsync(
            string model,
            global::Gemini.BatchEmbedTextRequest request,
            global::System.Threading.CancellationToken cancellationToken = default);

        /// <summary>
        /// Generates multiple embeddings from the model given input text in a<br/>
        /// synchronous call.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="texts">
        /// Optional. The free-form input texts that the model will turn into an embedding. The<br/>
        /// current limit is 100 texts, over which an error will be thrown.
        /// </param>
        /// <param name="requests">
        /// Optional. Embed requests for the batch. Only one of `texts` or `requests` can be set.
        /// </param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::System.InvalidOperationException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.BatchEmbedTextResponse> BatchEmbedTextAsync(
            string model,
            global::System.Collections.Generic.IList<string>? texts = default,
            global::System.Collections.Generic.IList<global::Gemini.EmbedTextRequest>? requests = default,
            global::System.Threading.CancellationToken cancellationToken = default);
    }
}