#nullable enable

namespace Gemini
{
    public partial interface IGeminiApi
    {
        /// <summary>
        /// Same as Predict but returns an LRO.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::Gemini.ApiException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.PredictLongRunningOperation> PredictLongRunningAsync(
            string model,
            global::Gemini.PredictLongRunningRequest request,
            global::System.Threading.CancellationToken cancellationToken = default);

        /// <summary>
        /// Same as Predict but returns an LRO.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="instances">
        /// Required. The instances that are the input to the prediction call.
        /// </param>
        /// <param name="parameters">
        /// Optional. The parameters that govern the prediction call.
        /// </param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::System.InvalidOperationException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.PredictLongRunningOperation> PredictLongRunningAsync(
            string model,
            global::System.Collections.Generic.IList<object> instances,
            object? parameters = default,
            global::System.Threading.CancellationToken cancellationToken = default);
    }
}