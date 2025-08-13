#nullable enable

namespace Gemini
{
    public partial interface IGeminiApi
    {
        /// <summary>
        /// Runs a model's tokenizer on input `Content` and returns the token count.<br/>
        /// Refer to the [tokens guide](https://ai.google.dev/gemini-api/docs/tokens)<br/>
        /// to learn more about tokens.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::Gemini.ApiException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.CountTokensResponse> CountTokensAsync(
            string model,
            global::Gemini.CountTokensRequest request,
            global::System.Threading.CancellationToken cancellationToken = default);

        /// <summary>
        /// Runs a model's tokenizer on input `Content` and returns the token count.<br/>
        /// Refer to the [tokens guide](https://ai.google.dev/gemini-api/docs/tokens)<br/>
        /// to learn more about tokens.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="contents">
        /// Optional. The input given to the model as a prompt. This field is ignored when<br/>
        /// `generate_content_request` is set.
        /// </param>
        /// <param name="generateContentRequest">
        /// Optional. The overall input given to the `Model`. This includes the prompt as well as<br/>
        /// other model steering information like [system<br/>
        /// instructions](https://ai.google.dev/gemini-api/docs/system-instructions),<br/>
        /// and/or function declarations for [function<br/>
        /// calling](https://ai.google.dev/gemini-api/docs/function-calling).<br/>
        /// `Model`s/`Content`s and `generate_content_request`s are mutually<br/>
        /// exclusive. You can either send `Model` + `Content`s or a<br/>
        /// `generate_content_request`, but never both.
        /// </param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::System.InvalidOperationException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.CountTokensResponse> CountTokensAsync(
            string model,
            global::System.Collections.Generic.IList<global::Gemini.Content>? contents = default,
            global::Gemini.GenerateContentRequest? generateContentRequest = default,
            global::System.Threading.CancellationToken cancellationToken = default);
    }
}