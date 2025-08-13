
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Response from the model for a grounded answer.
    /// </summary>
    public sealed partial class GenerateAnswerResponse
    {
        /// <summary>
        /// Candidate answer from the model.<br/>
        /// Note: The model *always* attempts to provide a grounded answer, even when<br/>
        /// the answer is unlikely to be answerable from the given passages.<br/>
        /// In that case, a low-quality or ungrounded answer may be provided, along<br/>
        /// with a low `answerable_probability`.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("answer")]
        public global::Gemini.Candidate? Answer { get; set; }

        /// <summary>
        /// Output only. The model's estimate of the probability that its answer is correct and<br/>
        /// grounded in the input passages.<br/>
        /// A low `answerable_probability` indicates that the answer might not be<br/>
        /// grounded in the sources.<br/>
        /// When `answerable_probability` is low, you may want to:<br/>
        /// * Display a message to the effect of "We couldn’t answer that question" to<br/>
        /// the user.<br/>
        /// * Fall back to a general-purpose LLM that answers the question from world<br/>
        /// knowledge. The threshold and nature of such fallbacks will depend on<br/>
        /// individual use cases. `0.5` is a good starting threshold.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("answerableProbability")]
        public float? AnswerableProbability { get; set; }

        /// <summary>
        /// Output only. Feedback related to the input data used to answer the question, as opposed<br/>
        /// to the model-generated response to the question.<br/>
        /// The input data can be one or more of the following:<br/>
        /// - Question specified by the last entry in `GenerateAnswerRequest.content`<br/>
        /// - Conversation history specified by the other entries in<br/>
        /// `GenerateAnswerRequest.content`<br/>
        /// - Grounding sources (`GenerateAnswerRequest.semantic_retriever` or<br/>
        /// `GenerateAnswerRequest.inline_passages`)<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("inputFeedback")]
        public global::Gemini.InputFeedback? InputFeedback { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateAnswerResponse" /> class.
        /// </summary>
        /// <param name="answer">
        /// Candidate answer from the model.<br/>
        /// Note: The model *always* attempts to provide a grounded answer, even when<br/>
        /// the answer is unlikely to be answerable from the given passages.<br/>
        /// In that case, a low-quality or ungrounded answer may be provided, along<br/>
        /// with a low `answerable_probability`.
        /// </param>
        /// <param name="answerableProbability">
        /// Output only. The model's estimate of the probability that its answer is correct and<br/>
        /// grounded in the input passages.<br/>
        /// A low `answerable_probability` indicates that the answer might not be<br/>
        /// grounded in the sources.<br/>
        /// When `answerable_probability` is low, you may want to:<br/>
        /// * Display a message to the effect of "We couldn’t answer that question" to<br/>
        /// the user.<br/>
        /// * Fall back to a general-purpose LLM that answers the question from world<br/>
        /// knowledge. The threshold and nature of such fallbacks will depend on<br/>
        /// individual use cases. `0.5` is a good starting threshold.<br/>
        /// Included only in responses
        /// </param>
        /// <param name="inputFeedback">
        /// Output only. Feedback related to the input data used to answer the question, as opposed<br/>
        /// to the model-generated response to the question.<br/>
        /// The input data can be one or more of the following:<br/>
        /// - Question specified by the last entry in `GenerateAnswerRequest.content`<br/>
        /// - Conversation history specified by the other entries in<br/>
        /// `GenerateAnswerRequest.content`<br/>
        /// - Grounding sources (`GenerateAnswerRequest.semantic_retriever` or<br/>
        /// `GenerateAnswerRequest.inline_passages`)<br/>
        /// Included only in responses
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public GenerateAnswerResponse(
            global::Gemini.Candidate? answer,
            float? answerableProbability,
            global::Gemini.InputFeedback? inputFeedback)
        {
            this.Answer = answer;
            this.AnswerableProbability = answerableProbability;
            this.InputFeedback = inputFeedback;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateAnswerResponse" /> class.
        /// </summary>
        public GenerateAnswerResponse()
        {
        }
    }
}