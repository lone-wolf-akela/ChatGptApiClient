
#nullable enable

namespace Gemini
{
    /// <summary>
    /// All of the structured input text passed to the model as a prompt.<br/>
    /// A `MessagePrompt` contains a structured set of fields that provide context<br/>
    /// for the conversation, examples of user input/model output message pairs that<br/>
    /// prime the model to respond in different ways, and the conversation history<br/>
    /// or list of messages representing the alternating turns of the conversation<br/>
    /// between the user and the model.
    /// </summary>
    public sealed partial class MessagePrompt
    {
        /// <summary>
        /// Optional. Text that should be provided to the model first to ground the response.<br/>
        /// If not empty, this `context` will be given to the model first before the<br/>
        /// `examples` and `messages`. When using a `context` be sure to provide it<br/>
        /// with every request to maintain continuity.<br/>
        /// This field can be a description of your prompt to the model to help provide<br/>
        /// context and guide the responses. Examples: "Translate the phrase from<br/>
        /// English to French." or "Given a statement, classify the sentiment as happy,<br/>
        /// sad or neutral."<br/>
        /// Anything included in this field will take precedence over message history<br/>
        /// if the total input size exceeds the model's `input_token_limit` and the<br/>
        /// input request is truncated.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("context")]
        public string? Context { get; set; }

        /// <summary>
        /// Optional. Examples of what the model should generate.<br/>
        /// This includes both user input and the response that the model should<br/>
        /// emulate.<br/>
        /// These `examples` are treated identically to conversation messages except<br/>
        /// that they take precedence over the history in `messages`:<br/>
        /// If the total input size exceeds the model's `input_token_limit` the input<br/>
        /// will be truncated. Items will be dropped from `messages` before `examples`.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("examples")]
        public global::System.Collections.Generic.IList<global::Gemini.Example>? Examples { get; set; }

        /// <summary>
        /// Required. A snapshot of the recent conversation history sorted chronologically.<br/>
        /// Turns alternate between two authors.<br/>
        /// If the total input size exceeds the model's `input_token_limit` the input<br/>
        /// will be truncated: The oldest items will be dropped from `messages`.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("messages")]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required global::System.Collections.Generic.IList<global::Gemini.Message> Messages { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagePrompt" /> class.
        /// </summary>
        /// <param name="context">
        /// Optional. Text that should be provided to the model first to ground the response.<br/>
        /// If not empty, this `context` will be given to the model first before the<br/>
        /// `examples` and `messages`. When using a `context` be sure to provide it<br/>
        /// with every request to maintain continuity.<br/>
        /// This field can be a description of your prompt to the model to help provide<br/>
        /// context and guide the responses. Examples: "Translate the phrase from<br/>
        /// English to French." or "Given a statement, classify the sentiment as happy,<br/>
        /// sad or neutral."<br/>
        /// Anything included in this field will take precedence over message history<br/>
        /// if the total input size exceeds the model's `input_token_limit` and the<br/>
        /// input request is truncated.
        /// </param>
        /// <param name="examples">
        /// Optional. Examples of what the model should generate.<br/>
        /// This includes both user input and the response that the model should<br/>
        /// emulate.<br/>
        /// These `examples` are treated identically to conversation messages except<br/>
        /// that they take precedence over the history in `messages`:<br/>
        /// If the total input size exceeds the model's `input_token_limit` the input<br/>
        /// will be truncated. Items will be dropped from `messages` before `examples`.
        /// </param>
        /// <param name="messages">
        /// Required. A snapshot of the recent conversation history sorted chronologically.<br/>
        /// Turns alternate between two authors.<br/>
        /// If the total input size exceeds the model's `input_token_limit` the input<br/>
        /// will be truncated: The oldest items will be dropped from `messages`.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public MessagePrompt(
            global::System.Collections.Generic.IList<global::Gemini.Message> messages,
            string? context,
            global::System.Collections.Generic.IList<global::Gemini.Example>? examples)
        {
            this.Messages = messages ?? throw new global::System.ArgumentNullException(nameof(messages));
            this.Context = context;
            this.Examples = examples;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagePrompt" /> class.
        /// </summary>
        public MessagePrompt()
        {
        }
    }
}