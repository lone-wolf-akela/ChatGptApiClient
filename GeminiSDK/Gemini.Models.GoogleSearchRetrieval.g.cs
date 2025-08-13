
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Tool to retrieve public web data for grounding, powered by Google.
    /// </summary>
    public sealed partial class GoogleSearchRetrieval
    {
        /// <summary>
        /// Specifies the dynamic retrieval configuration for the given source.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("dynamicRetrievalConfig")]
        public global::Gemini.DynamicRetrievalConfig? DynamicRetrievalConfig { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleSearchRetrieval" /> class.
        /// </summary>
        /// <param name="dynamicRetrievalConfig">
        /// Specifies the dynamic retrieval configuration for the given source.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public GoogleSearchRetrieval(
            global::Gemini.DynamicRetrievalConfig? dynamicRetrievalConfig)
        {
            this.DynamicRetrievalConfig = dynamicRetrievalConfig;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleSearchRetrieval" /> class.
        /// </summary>
        public GoogleSearchRetrieval()
        {
        }
    }
}