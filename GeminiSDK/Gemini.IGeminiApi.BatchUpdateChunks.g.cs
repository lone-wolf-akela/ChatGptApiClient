#nullable enable

namespace Gemini
{
    public partial interface IGeminiApi
    {
        /// <summary>
        /// Batch update `Chunk`s.
        /// </summary>
        /// <param name="corpus"></param>
        /// <param name="document"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::Gemini.ApiException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.BatchUpdateChunksResponse> BatchUpdateChunksAsync(
            string corpus,
            string document,
            global::Gemini.BatchUpdateChunksRequest request,
            global::System.Threading.CancellationToken cancellationToken = default);

        /// <summary>
        /// Batch update `Chunk`s.
        /// </summary>
        /// <param name="corpus"></param>
        /// <param name="document"></param>
        /// <param name="requests">
        /// Required. The request messages specifying the `Chunk`s to update.<br/>
        /// A maximum of 100 `Chunk`s can be updated in a batch.
        /// </param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::System.InvalidOperationException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.BatchUpdateChunksResponse> BatchUpdateChunksAsync(
            string corpus,
            string document,
            global::System.Collections.Generic.IList<global::Gemini.UpdateChunkRequest> requests,
            global::System.Threading.CancellationToken cancellationToken = default);
    }
}