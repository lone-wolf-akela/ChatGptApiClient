
#nullable enable

namespace Gemini
{
    /// <summary>
    /// User provided filter to limit retrieval based on `Chunk` or `Document` level<br/>
    /// metadata values.<br/>
    /// Example (genre = drama OR genre = action):<br/>
    ///   key = "document.custom_metadata.genre"<br/>
    ///   conditions = [{string_value = "drama", operation = EQUAL},<br/>
    ///                 {string_value = "action", operation = EQUAL}]
    /// </summary>
    public sealed partial class MetadataFilter
    {
        /// <summary>
        /// Required. The key of the metadata to filter on.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("key")]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required string Key { get; set; }

        /// <summary>
        /// Required. The `Condition`s for the given key that will trigger this filter. Multiple<br/>
        /// `Condition`s are joined by logical ORs.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("conditions")]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required global::System.Collections.Generic.IList<global::Gemini.Condition> Conditions { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataFilter" /> class.
        /// </summary>
        /// <param name="key">
        /// Required. The key of the metadata to filter on.
        /// </param>
        /// <param name="conditions">
        /// Required. The `Condition`s for the given key that will trigger this filter. Multiple<br/>
        /// `Condition`s are joined by logical ORs.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public MetadataFilter(
            string key,
            global::System.Collections.Generic.IList<global::Gemini.Condition> conditions)
        {
            this.Key = key ?? throw new global::System.ArgumentNullException(nameof(key));
            this.Conditions = conditions ?? throw new global::System.ArgumentNullException(nameof(conditions));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataFilter" /> class.
        /// </summary>
        public MetadataFilter()
        {
        }
    }
}