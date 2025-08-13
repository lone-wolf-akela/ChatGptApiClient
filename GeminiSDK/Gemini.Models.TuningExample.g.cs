
#nullable enable

namespace Gemini
{
    /// <summary>
    /// A single example for tuning.
    /// </summary>
    public sealed partial class TuningExample
    {
        /// <summary>
        /// Optional. Text model input.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("textInput")]
        public string? TextInput { get; set; }

        /// <summary>
        /// Required. The expected model output.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("output")]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required string Output { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TuningExample" /> class.
        /// </summary>
        /// <param name="textInput">
        /// Optional. Text model input.
        /// </param>
        /// <param name="output">
        /// Required. The expected model output.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public TuningExample(
            string output,
            string? textInput)
        {
            this.Output = output ?? throw new global::System.ArgumentNullException(nameof(output));
            this.TextInput = textInput;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TuningExample" /> class.
        /// </summary>
        public TuningExample()
        {
        }
    }
}