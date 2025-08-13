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
        /// <param name="generatedFile"></param>
        /// <param name="operation"></param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::Gemini.ApiException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.Operation> GetOperationByGeneratedFileAndOperationAsync(
            string generatedFile,
            string operation,
            global::System.Threading.CancellationToken cancellationToken = default);
    }
}