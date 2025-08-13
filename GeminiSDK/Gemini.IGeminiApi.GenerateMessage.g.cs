#nullable enable

namespace Gemini
{
    public partial interface IGeminiApi
    {
        /// <summary>
        /// Generates a response from the model given an input `MessagePrompt`.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::Gemini.ApiException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.GenerateMessageResponse> GenerateMessageAsync(
            string model,
            global::Gemini.GenerateMessageRequest request,
            global::System.Threading.CancellationToken cancellationToken = default);

        /// <summary>
        /// Generates a response from the model given an input `MessagePrompt`.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="prompt">
        /// Required. The structured textual input given to the model as a prompt.<br/>
        /// Given a<br/>
        /// prompt, the model will return what it predicts is the next message in the<br/>
        /// discussion.
        /// </param>
        /// <param name="temperature">
        /// Optional. Controls the randomness of the output.<br/>
        /// Values can range over `[0.0,1.0]`,<br/>
        /// inclusive. A value closer to `1.0` will produce responses that are more<br/>
        /// varied, while a value closer to `0.0` will typically result in<br/>
        /// less surprising responses from the model.
        /// </param>
        /// <param name="candidateCount">
        /// Optional. The number of generated response messages to return.<br/>
        /// This value must be between<br/>
        /// `[1, 8]`, inclusive. If unset, this will default to `1`.
        /// </param>
        /// <param name="topP">
        /// Optional. The maximum cumulative probability of tokens to consider when sampling.<br/>
        /// The model uses combined Top-k and nucleus sampling.<br/>
        /// Nucleus sampling considers the smallest set of tokens whose probability<br/>
        /// sum is at least `top_p`.
        /// </param>
        /// <param name="topK">
        /// Optional. The maximum number of tokens to consider when sampling.<br/>
        /// The model uses combined Top-k and nucleus sampling.<br/>
        /// Top-k sampling considers the set of `top_k` most probable tokens.
        /// </param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::System.InvalidOperationException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.GenerateMessageResponse> GenerateMessageAsync(
            string model,
            global::Gemini.MessagePrompt prompt,
            float? temperature = default,
            int? candidateCount = default,
            float? topP = default,
            int? topK = default,
            global::System.Threading.CancellationToken cancellationToken = default);
    }
}