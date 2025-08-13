
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Represents token counting info for a single modality.
    /// </summary>
    public sealed partial class ModalityTokenCount
    {
        /// <summary>
        /// The modality associated with this token count.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("modality")]
        [global::System.Text.Json.Serialization.JsonConverter(typeof(global::Gemini.JsonConverters.ModalityJsonConverter))]
        public global::Gemini.Modality? Modality { get; set; }

        /// <summary>
        /// Number of tokens.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("tokenCount")]
        public int? TokenCount { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ModalityTokenCount" /> class.
        /// </summary>
        /// <param name="modality">
        /// The modality associated with this token count.
        /// </param>
        /// <param name="tokenCount">
        /// Number of tokens.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public ModalityTokenCount(
            global::Gemini.Modality? modality,
            int? tokenCount)
        {
            this.Modality = modality;
            this.TokenCount = tokenCount;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModalityTokenCount" /> class.
        /// </summary>
        public ModalityTokenCount()
        {
        }
    }
}