#nullable enable

namespace Gemini
{
    public partial interface IGeminiApi
    {
        /// <summary>
        /// Transfers ownership of the tuned model.<br/>
        /// This is the only way to change ownership of the tuned model.<br/>
        /// The current owner will be downgraded to writer role.
        /// </summary>
        /// <param name="tunedModel"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::Gemini.ApiException"></exception>
        global::System.Threading.Tasks.Task<string> TransferOwnershipAsync(
            string tunedModel,
            global::Gemini.TransferOwnershipRequest request,
            global::System.Threading.CancellationToken cancellationToken = default);

        /// <summary>
        /// Transfers ownership of the tuned model.<br/>
        /// This is the only way to change ownership of the tuned model.<br/>
        /// The current owner will be downgraded to writer role.
        /// </summary>
        /// <param name="tunedModel"></param>
        /// <param name="emailAddress">
        /// Required. The email address of the user to whom the tuned model is being transferred<br/>
        /// to.
        /// </param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::System.InvalidOperationException"></exception>
        global::System.Threading.Tasks.Task<string> TransferOwnershipAsync(
            string tunedModel,
            string emailAddress,
            global::System.Threading.CancellationToken cancellationToken = default);
    }
}