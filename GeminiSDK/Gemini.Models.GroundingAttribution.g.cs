
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Attribution for a source that contributed to an answer.
    /// </summary>
    public sealed partial class GroundingAttribution
    {
        /// <summary>
        /// Output only. Identifier for the source contributing to this attribution.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("sourceId")]
        public global::Gemini.AttributionSourceId? SourceId { get; set; }

        /// <summary>
        /// Grounding source content that makes up this attribution.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("content")]
        public global::Gemini.Content? Content { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GroundingAttribution" /> class.
        /// </summary>
        /// <param name="sourceId">
        /// Output only. Identifier for the source contributing to this attribution.<br/>
        /// Included only in responses
        /// </param>
        /// <param name="content">
        /// Grounding source content that makes up this attribution.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public GroundingAttribution(
            global::Gemini.AttributionSourceId? sourceId,
            global::Gemini.Content? content)
        {
            this.SourceId = sourceId;
            this.Content = content;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GroundingAttribution" /> class.
        /// </summary>
        public GroundingAttribution()
        {
        }
    }
}