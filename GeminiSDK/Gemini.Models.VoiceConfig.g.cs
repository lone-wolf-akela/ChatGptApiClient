
#nullable enable

namespace Gemini
{
    /// <summary>
    /// The configuration for the voice to use.
    /// </summary>
    public sealed partial class VoiceConfig
    {
        /// <summary>
        /// The configuration for the prebuilt voice to use.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("prebuiltVoiceConfig")]
        public global::Gemini.PrebuiltVoiceConfig? PrebuiltVoiceConfig { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="VoiceConfig" /> class.
        /// </summary>
        /// <param name="prebuiltVoiceConfig">
        /// The configuration for the prebuilt voice to use.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public VoiceConfig(
            global::Gemini.PrebuiltVoiceConfig? prebuiltVoiceConfig)
        {
            this.PrebuiltVoiceConfig = prebuiltVoiceConfig;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VoiceConfig" /> class.
        /// </summary>
        public VoiceConfig()
        {
        }
    }
}