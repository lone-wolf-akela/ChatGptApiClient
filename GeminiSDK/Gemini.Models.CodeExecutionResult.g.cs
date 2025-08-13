
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Result of executing the `ExecutableCode`.<br/>
    /// Only generated when using the `CodeExecution`, and always follows a `part`<br/>
    /// containing the `ExecutableCode`.
    /// </summary>
    public sealed partial class CodeExecutionResult
    {
        /// <summary>
        /// Required. Outcome of the code execution.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("outcome")]
        [global::System.Text.Json.Serialization.JsonConverter(typeof(global::Gemini.JsonConverters.CodeExecutionResultOutcomeJsonConverter))]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required global::Gemini.CodeExecutionResultOutcome Outcome { get; set; }

        /// <summary>
        /// Optional. Contains stdout when code execution is successful, stderr or other<br/>
        /// description otherwise.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("output")]
        public string? Output { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeExecutionResult" /> class.
        /// </summary>
        /// <param name="outcome">
        /// Required. Outcome of the code execution.
        /// </param>
        /// <param name="output">
        /// Optional. Contains stdout when code execution is successful, stderr or other<br/>
        /// description otherwise.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public CodeExecutionResult(
            global::Gemini.CodeExecutionResultOutcome outcome,
            string? output)
        {
            this.Outcome = outcome;
            this.Output = output;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeExecutionResult" /> class.
        /// </summary>
        public CodeExecutionResult()
        {
        }
    }
}