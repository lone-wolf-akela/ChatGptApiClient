
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Grounding chunk.
    /// </summary>
    public sealed partial class GroundingChunk
    {
        /// <summary>
        /// Grounding chunk from the web.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("web")]
        public global::Gemini.Web? Web { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GroundingChunk" /> class.
        /// </summary>
        /// <param name="web">
        /// Grounding chunk from the web.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public GroundingChunk(
            global::Gemini.Web? web)
        {
            this.Web = web;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GroundingChunk" /> class.
        /// </summary>
        public GroundingChunk()
        {
        }
    }
}