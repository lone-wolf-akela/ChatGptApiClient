
#nullable enable

namespace Gemini
{
    /// <summary>
    /// The Tool configuration containing parameters for specifying `Tool` use<br/>
    /// in the request.
    /// </summary>
    public sealed partial class ToolConfig
    {
        /// <summary>
        /// Optional. Function calling config.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("functionCallingConfig")]
        public global::Gemini.FunctionCallingConfig? FunctionCallingConfig { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolConfig" /> class.
        /// </summary>
        /// <param name="functionCallingConfig">
        /// Optional. Function calling config.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public ToolConfig(
            global::Gemini.FunctionCallingConfig? functionCallingConfig)
        {
            this.FunctionCallingConfig = functionCallingConfig;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolConfig" /> class.
        /// </summary>
        public ToolConfig()
        {
        }
    }
}