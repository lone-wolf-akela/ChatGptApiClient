#nullable enable

namespace Gemini
{
    public partial interface IGeminiApi
    {
        /// <summary>
        /// Generates multiple embedding vectors from the input `Content` which<br/>
        /// consists of a batch of strings represented as `EmbedContentRequest`<br/>
        /// objects.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::Gemini.ApiException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.BatchEmbedContentsResponse> BatchEmbedContentsAsync(
            string model,
            global::Gemini.BatchEmbedContentsRequest request,
            global::System.Threading.CancellationToken cancellationToken = default);

        /// <summary>
        /// Generates multiple embedding vectors from the input `Content` which<br/>
        /// consists of a batch of strings represented as `EmbedContentRequest`<br/>
        /// objects.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="requests">
        /// Required. Embed requests for the batch. The model in each of these requests must<br/>
        /// match the model specified `BatchEmbedContentsRequest.model`.
        /// </param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::System.InvalidOperationException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.BatchEmbedContentsResponse> BatchEmbedContentsAsync(
            string model,
            global::System.Collections.Generic.IList<global::Gemini.EmbedContentRequest> requests,
            global::System.Threading.CancellationToken cancellationToken = default);
    }
}