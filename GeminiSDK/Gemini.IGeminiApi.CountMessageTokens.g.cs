#nullable enable

namespace Gemini
{
    public partial interface IGeminiApi
    {
        /// <summary>
        /// Runs a model's tokenizer on a string and returns the token count.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::Gemini.ApiException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.CountMessageTokensResponse> CountMessageTokensAsync(
            string model,
            global::Gemini.CountMessageTokensRequest request,
            global::System.Threading.CancellationToken cancellationToken = default);

        /// <summary>
        /// Runs a model's tokenizer on a string and returns the token count.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="prompt">
        /// Required. The prompt, whose token count is to be returned.
        /// </param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::System.InvalidOperationException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.CountMessageTokensResponse> CountMessageTokensAsync(
            string model,
            global::Gemini.MessagePrompt prompt,
            global::System.Threading.CancellationToken cancellationToken = default);
    }
}