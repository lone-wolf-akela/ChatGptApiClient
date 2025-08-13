#nullable enable

namespace Gemini
{
    public partial interface IGeminiApi
    {
        /// <summary>
        /// Generates an embedding from the model given an input message.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::Gemini.ApiException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.EmbedTextResponse> EmbedTextAsync(
            string model,
            global::Gemini.EmbedTextRequest request,
            global::System.Threading.CancellationToken cancellationToken = default);

        /// <summary>
        /// Generates an embedding from the model given an input message.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="requestModel">
        /// Required. The model name to use with the format model=models/{model}.
        /// </param>
        /// <param name="text">
        /// Optional. The free-form input text that the model will turn into an embedding.
        /// </param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::System.InvalidOperationException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.EmbedTextResponse> EmbedTextAsync(
            string model,
            string requestModel,
            string? text = default,
            global::System.Threading.CancellationToken cancellationToken = default);
    }
}