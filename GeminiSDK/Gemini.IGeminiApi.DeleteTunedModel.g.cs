#nullable enable

namespace Gemini
{
    public partial interface IGeminiApi
    {
        /// <summary>
        /// Deletes a tuned model.
        /// </summary>
        /// <param name="tunedModel"></param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::Gemini.ApiException"></exception>
        global::System.Threading.Tasks.Task<string> DeleteTunedModelAsync(
            string tunedModel,
            global::System.Threading.CancellationToken cancellationToken = default);
    }
}