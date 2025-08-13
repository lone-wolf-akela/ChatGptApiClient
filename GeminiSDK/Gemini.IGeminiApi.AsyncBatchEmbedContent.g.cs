#nullable enable

namespace Gemini
{
    public partial interface IGeminiApi
    {
        /// <summary>
        /// Enqueues a batch of `EmbedContent` requests for batch processing.<br/>
        /// We have a `BatchEmbedContents` handler in `GenerativeService`, but it was<br/>
        /// synchronized. So we name this one to be `Async` to avoid confusion.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::Gemini.ApiException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.AsyncBatchEmbedContentOperation> AsyncBatchEmbedContentAsync(
            string model,
            global::Gemini.AsyncBatchEmbedContentRequest request,
            global::System.Threading.CancellationToken cancellationToken = default);

        /// <summary>
        /// Enqueues a batch of `EmbedContent` requests for batch processing.<br/>
        /// We have a `BatchEmbedContents` handler in `GenerativeService`, but it was<br/>
        /// synchronized. So we name this one to be `Async` to avoid confusion.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="batch">
        /// Required. The batch to create.
        /// </param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::System.InvalidOperationException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.AsyncBatchEmbedContentOperation> AsyncBatchEmbedContentAsync(
            string model,
            global::Gemini.EmbedContentBatch batch,
            global::System.Threading.CancellationToken cancellationToken = default);
    }
}