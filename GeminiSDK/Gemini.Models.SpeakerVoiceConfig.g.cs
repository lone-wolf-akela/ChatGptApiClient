
#nullable enable

namespace Gemini
{
    /// <summary>
    /// The configuration for a single speaker in a multi speaker setup.
    /// </summary>
    public sealed partial class SpeakerVoiceConfig
    {
        /// <summary>
        /// Required. The name of the speaker to use. Should be the same as in the prompt.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("speaker")]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required string Speaker { get; set; }

        /// <summary>
        /// Required. The configuration for the voice to use.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("voiceConfig")]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required global::Gemini.VoiceConfig VoiceConfig { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SpeakerVoiceConfig" /> class.
        /// </summary>
        /// <param name="speaker">
        /// Required. The name of the speaker to use. Should be the same as in the prompt.
        /// </param>
        /// <param name="voiceConfig">
        /// Required. The configuration for the voice to use.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public SpeakerVoiceConfig(
            string speaker,
            global::Gemini.VoiceConfig voiceConfig)
        {
            this.Speaker = speaker ?? throw new global::System.ArgumentNullException(nameof(speaker));
            this.VoiceConfig = voiceConfig ?? throw new global::System.ArgumentNullException(nameof(voiceConfig));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpeakerVoiceConfig" /> class.
        /// </summary>
        public SpeakerVoiceConfig()
        {
        }
    }
}