#nullable enable

namespace Gemini
{
    public partial interface IGeminiApi
    {
        /// <summary>
        /// Gets information about a specific TunedModel.
        /// </summary>
        /// <param name="tunedModel"></param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::Gemini.ApiException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.TunedModel> GetTunedModelAsync(
            string tunedModel,
            global::System.Threading.CancellationToken cancellationToken = default);
    }
}