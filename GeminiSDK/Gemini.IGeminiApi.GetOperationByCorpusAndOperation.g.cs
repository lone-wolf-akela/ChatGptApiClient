#nullable enable

namespace Gemini
{
    public partial interface IGeminiApi
    {
        /// <summary>
        /// Gets the latest state of a long-running operation.  Clients can use this<br/>
        /// method to poll the operation result at intervals as recommended by the API<br/>
        /// service.
        /// </summary>
        /// <param name="corpus"></param>
        /// <param name="operation"></param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::Gemini.ApiException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.Operation> GetOperationByCorpusAndOperationAsync(
            string corpus,
            string operation,
            global::System.Threading.CancellationToken cancellationToken = default);
    }
}