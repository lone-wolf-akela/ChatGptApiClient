#nullable enable

namespace Gemini
{
    public partial interface IGeminiApi
    {
        /// <summary>
        /// Batch delete `Chunk`s.
        /// </summary>
        /// <param name="corpus"></param>
        /// <param name="document"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::Gemini.ApiException"></exception>
        global::System.Threading.Tasks.Task<string> BatchDeleteChunksAsync(
            string corpus,
            string document,
            global::Gemini.BatchDeleteChunksRequest request,
            global::System.Threading.CancellationToken cancellationToken = default);

        /// <summary>
        /// Batch delete `Chunk`s.
        /// </summary>
        /// <param name="corpus"></param>
        /// <param name="document"></param>
        /// <param name="requests">
        /// Required. The request messages specifying the `Chunk`s to delete.
        /// </param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::System.InvalidOperationException"></exception>
        global::System.Threading.Tasks.Task<string> BatchDeleteChunksAsync(
            string corpus,
            string document,
            global::System.Collections.Generic.IList<global::Gemini.DeleteChunkRequest> requests,
            global::System.Threading.CancellationToken cancellationToken = default);
    }
}