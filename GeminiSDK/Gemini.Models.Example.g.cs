
#nullable enable

namespace Gemini
{
    /// <summary>
    /// An input/output example used to instruct the Model.<br/>
    /// It demonstrates how the model should respond or format its response.
    /// </summary>
    public sealed partial class Example
    {
        /// <summary>
        /// Required. An example of an input `Message` from the user.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("input")]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required global::Gemini.Message Input { get; set; }

        /// <summary>
        /// Required. An example of what the model should output given the input.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("output")]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required global::Gemini.Message Output { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Example" /> class.
        /// </summary>
        /// <param name="input">
        /// Required. An example of an input `Message` from the user.
        /// </param>
        /// <param name="output">
        /// Required. An example of what the model should output given the input.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public Example(
            global::Gemini.Message input,
            global::Gemini.Message output)
        {
            this.Input = input ?? throw new global::System.ArgumentNullException(nameof(input));
            this.Output = output ?? throw new global::System.ArgumentNullException(nameof(output));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Example" /> class.
        /// </summary>
        public Example()
        {
        }
    }
}