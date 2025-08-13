#nullable enable

namespace Gemini
{
    public partial interface IGeminiApi
    {
        /// <summary>
        /// Batch create `Chunk`s.
        /// </summary>
        /// <param name="corpus"></param>
        /// <param name="document"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::Gemini.ApiException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.BatchCreateChunksResponse> BatchCreateChunksAsync(
            string corpus,
            string document,
            global::Gemini.BatchCreateChunksRequest request,
            global::System.Threading.CancellationToken cancellationToken = default);

        /// <summary>
        /// Batch create `Chunk`s.
        /// </summary>
        /// <param name="corpus"></param>
        /// <param name="document"></param>
        /// <param name="requests">
        /// Required. The request messages specifying the `Chunk`s to create.<br/>
        /// A maximum of 100 `Chunk`s can be created in a batch.
        /// </param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::System.InvalidOperationException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.BatchCreateChunksResponse> BatchCreateChunksAsync(
            string corpus,
            string document,
            global::System.Collections.Generic.IList<global::Gemini.CreateChunkRequest> requests,
            global::System.Threading.CancellationToken cancellationToken = default);
    }
}