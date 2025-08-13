
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Veo response.
    /// </summary>
    public sealed partial class GenerateVideoResponse
    {
        /// <summary>
        /// The generated samples.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("generatedSamples")]
        public global::System.Collections.Generic.IList<global::Gemini.Media>? GeneratedSamples { get; set; }

        /// <summary>
        /// Returns if any videos were filtered due to RAI policies.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("raiMediaFilteredCount")]
        public int? RaiMediaFilteredCount { get; set; }

        /// <summary>
        /// Returns rai failure reasons if any.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("raiMediaFilteredReasons")]
        public global::System.Collections.Generic.IList<string>? RaiMediaFilteredReasons { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateVideoResponse" /> class.
        /// </summary>
        /// <param name="generatedSamples">
        /// The generated samples.
        /// </param>
        /// <param name="raiMediaFilteredCount">
        /// Returns if any videos were filtered due to RAI policies.
        /// </param>
        /// <param name="raiMediaFilteredReasons">
        /// Returns rai failure reasons if any.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public GenerateVideoResponse(
            global::System.Collections.Generic.IList<global::Gemini.Media>? generatedSamples,
            int? raiMediaFilteredCount,
            global::System.Collections.Generic.IList<string>? raiMediaFilteredReasons)
        {
            this.GeneratedSamples = generatedSamples;
            this.RaiMediaFilteredCount = raiMediaFilteredCount;
            this.RaiMediaFilteredReasons = raiMediaFilteredReasons;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateVideoResponse" /> class.
        /// </summary>
        public GenerateVideoResponse()
        {
        }
    }
}