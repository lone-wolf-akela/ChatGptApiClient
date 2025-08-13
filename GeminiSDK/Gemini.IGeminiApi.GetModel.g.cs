#nullable enable

namespace Gemini
{
    public partial interface IGeminiApi
    {
        /// <summary>
        /// Gets information about a specific `Model` such as its version number, token<br/>
        /// limits,<br/>
        /// [parameters](https://ai.google.dev/gemini-api/docs/models/generative-models#model-parameters)<br/>
        /// and other metadata. Refer to the [Gemini models<br/>
        /// guide](https://ai.google.dev/gemini-api/docs/models/gemini) for detailed<br/>
        /// model information.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::Gemini.ApiException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.Model> GetModelAsync(
            string model,
            global::System.Threading.CancellationToken cancellationToken = default);
    }
}