#nullable enable

namespace Gemini
{
    public partial interface IGeminiApi
    {
        /// <summary>
        /// Starts asynchronous cancellation on a long-running operation.  The server<br/>
        /// makes a best effort to cancel the operation, but success is not<br/>
        /// guaranteed.  If the server doesn't support this method, it returns<br/>
        /// `google.rpc.Code.UNIMPLEMENTED`.  Clients can use<br/>
        /// Operations.GetOperation or<br/>
        /// other methods to check whether the cancellation succeeded or whether the<br/>
        /// operation completed despite cancellation. On successful cancellation,<br/>
        /// the operation is not deleted; instead, it becomes an operation with<br/>
        /// an Operation.error value with a google.rpc.Status.code of `1`,<br/>
        /// corresponding to `Code.CANCELLED`.
        /// </summary>
        /// <param name="generateContentBatch"></param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::Gemini.ApiException"></exception>
        global::System.Threading.Tasks.Task<string> CancelOperationAsync(
            string generateContentBatch,
            global::System.Threading.CancellationToken cancellationToken = default);
    }
}