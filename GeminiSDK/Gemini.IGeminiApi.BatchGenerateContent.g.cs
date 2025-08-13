#nullable enable

namespace Gemini
{
    public partial interface IGeminiApi
    {
        /// <summary>
        /// Enqueues a batch of `GenerateContent` requests for batch processing.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::Gemini.ApiException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.BatchGenerateContentOperation> BatchGenerateContentAsync(
            string model,
            global::Gemini.BatchGenerateContentRequest request,
            global::System.Threading.CancellationToken cancellationToken = default);

        /// <summary>
        /// Enqueues a batch of `GenerateContent` requests for batch processing.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="batch">
        /// Required. The batch to create.
        /// </param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::System.InvalidOperationException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.BatchGenerateContentOperation> BatchGenerateContentAsync(
            string model,
            global::Gemini.GenerateContentBatch batch,
            global::System.Threading.CancellationToken cancellationToken = default);
    }
}