
#nullable enable

namespace Gemini
{
    /// <summary>
    /// The speech generation config.
    /// </summary>
    public sealed partial class SpeechConfig
    {
        /// <summary>
        /// The configuration in case of single-voice output.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("voiceConfig")]
        public global::Gemini.VoiceConfig? VoiceConfig { get; set; }

        /// <summary>
        /// Optional. The configuration for the multi-speaker setup.<br/>
        /// It is mutually exclusive with the voice_config field.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("multiSpeakerVoiceConfig")]
        public global::Gemini.MultiSpeakerVoiceConfig? MultiSpeakerVoiceConfig { get; set; }

        /// <summary>
        /// Optional. Language code (in BCP 47 format, e.g. "en-US") for speech synthesis.<br/>
        /// Valid values are: de-DE, en-AU, en-GB, en-IN, en-US, es-US, fr-FR, hi-IN,<br/>
        /// pt-BR, ar-XA, es-ES, fr-CA, id-ID, it-IT, ja-JP, tr-TR, vi-VN, bn-IN,<br/>
        /// gu-IN, kn-IN, ml-IN, mr-IN, ta-IN, te-IN, nl-NL, ko-KR, cmn-CN, pl-PL,<br/>
        /// ru-RU, and th-TH.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("languageCode")]
        public string? LanguageCode { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SpeechConfig" /> class.
        /// </summary>
        /// <param name="voiceConfig">
        /// The configuration in case of single-voice output.
        /// </param>
        /// <param name="multiSpeakerVoiceConfig">
        /// Optional. The configuration for the multi-speaker setup.<br/>
        /// It is mutually exclusive with the voice_config field.
        /// </param>
        /// <param name="languageCode">
        /// Optional. Language code (in BCP 47 format, e.g. "en-US") for speech synthesis.<br/>
        /// Valid values are: de-DE, en-AU, en-GB, en-IN, en-US, es-US, fr-FR, hi-IN,<br/>
        /// pt-BR, ar-XA, es-ES, fr-CA, id-ID, it-IT, ja-JP, tr-TR, vi-VN, bn-IN,<br/>
        /// gu-IN, kn-IN, ml-IN, mr-IN, ta-IN, te-IN, nl-NL, ko-KR, cmn-CN, pl-PL,<br/>
        /// ru-RU, and th-TH.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public SpeechConfig(
            global::Gemini.VoiceConfig? voiceConfig,
            global::Gemini.MultiSpeakerVoiceConfig? multiSpeakerVoiceConfig,
            string? languageCode)
        {
            this.VoiceConfig = voiceConfig;
            this.MultiSpeakerVoiceConfig = multiSpeakerVoiceConfig;
            this.LanguageCode = languageCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpeechConfig" /> class.
        /// </summary>
        public SpeechConfig()
        {
        }
    }
}