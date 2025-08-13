
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Request to generate a message response from the model.
    /// </summary>
    public sealed partial class GenerateMessageRequest
    {
        /// <summary>
        /// Required. The structured textual input given to the model as a prompt.<br/>
        /// Given a<br/>
        /// prompt, the model will return what it predicts is the next message in the<br/>
        /// discussion.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("prompt")]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required global::Gemini.MessagePrompt Prompt { get; set; }

        /// <summary>
        /// Optional. Controls the randomness of the output.<br/>
        /// Values can range over `[0.0,1.0]`,<br/>
        /// inclusive. A value closer to `1.0` will produce responses that are more<br/>
        /// varied, while a value closer to `0.0` will typically result in<br/>
        /// less surprising responses from the model.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("temperature")]
        public float? Temperature { get; set; }

        /// <summary>
        /// Optional. The number of generated response messages to return.<br/>
        /// This value must be between<br/>
        /// `[1, 8]`, inclusive. If unset, this will default to `1`.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("candidateCount")]
        public int? CandidateCount { get; set; }

        /// <summary>
        /// Optional. The maximum cumulative probability of tokens to consider when sampling.<br/>
        /// The model uses combined Top-k and nucleus sampling.<br/>
        /// Nucleus sampling considers the smallest set of tokens whose probability<br/>
        /// sum is at least `top_p`.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("topP")]
        public float? TopP { get; set; }

        /// <summary>
        /// Optional. The maximum number of tokens to consider when sampling.<br/>
        /// The model uses combined Top-k and nucleus sampling.<br/>
        /// Top-k sampling considers the set of `top_k` most probable tokens.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("topK")]
        public int? TopK { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateMessageRequest" /> class.
        /// </summary>
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
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public GenerateMessageRequest(
            global::Gemini.MessagePrompt prompt,
            float? temperature,
            int? candidateCount,
            float? topP,
            int? topK)
        {
            this.Prompt = prompt ?? throw new global::System.ArgumentNullException(nameof(prompt));
            this.Temperature = temperature;
            this.CandidateCount = candidateCount;
            this.TopP = topP;
            this.TopK = topK;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateMessageRequest" /> class.
        /// </summary>
        public GenerateMessageRequest()
        {
        }
    }
}