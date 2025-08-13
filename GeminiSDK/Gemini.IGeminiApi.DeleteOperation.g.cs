#nullable enable

namespace Gemini
{
    public partial interface IGeminiApi
    {
        /// <summary>
        /// Deletes a long-running operation. This method indicates that the client is<br/>
        /// no longer interested in the operation result. It does not cancel the<br/>
        /// operation. If the server doesn't support this method, it returns<br/>
        /// `google.rpc.Code.UNIMPLEMENTED`.
        /// </summary>
        /// <param name="generateContentBatch"></param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::Gemini.ApiException"></exception>
        global::System.Threading.Tasks.Task<string> DeleteOperationAsync(
            string generateContentBatch,
            global::System.Threading.CancellationToken cancellationToken = default);
    }
}