#nullable enable

namespace Gemini
{
    public partial interface IGeminiApi
    {
        /// <summary>
        /// Runs a model's tokenizer on a text and returns the token count.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::Gemini.ApiException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.CountTextTokensResponse> CountTextTokensAsync(
            string model,
            global::Gemini.CountTextTokensRequest request,
            global::System.Threading.CancellationToken cancellationToken = default);

        /// <summary>
        /// Runs a model's tokenizer on a text and returns the token count.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="prompt">
        /// Required. The free-form input text given to the model as a prompt.
        /// </param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::System.InvalidOperationException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.CountTextTokensResponse> CountTextTokensAsync(
            string model,
            global::Gemini.TextPrompt prompt,
            global::System.Threading.CancellationToken cancellationToken = default);
    }
}