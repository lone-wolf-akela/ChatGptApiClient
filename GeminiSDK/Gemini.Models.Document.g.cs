
#nullable enable

namespace Gemini
{
    /// <summary>
    /// A `Document` is a collection of `Chunk`s.<br/>
    /// A `Corpus` can have a maximum of 10,000 `Document`s.
    /// </summary>
    public sealed partial class Document
    {
        /// <summary>
        /// Immutable. Identifier. The `Document` resource name. The ID (name excluding the<br/>
        /// "corpora/*/documents/" prefix) can contain up to 40 characters that are<br/>
        /// lowercase alphanumeric or dashes (-). The ID cannot start or end with a<br/>
        /// dash. If the name is empty on create, a unique name will be derived from<br/>
        /// `display_name` along with a 12 character random suffix.<br/>
        /// Example: `corpora/{corpus_id}/documents/my-awesome-doc-123a456b789c`
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("name")]
        public string? Name { get; set; }

        /// <summary>
        /// Optional. The human-readable display name for the `Document`. The display name must<br/>
        /// be no more than 512 characters in length, including spaces.<br/>
        /// Example: "Semantic Retriever Documentation"
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("displayName")]
        public string? DisplayName { get; set; }

        /// <summary>
        /// Optional. User provided custom metadata stored as key-value pairs used for querying.<br/>
        /// A `Document` can have a maximum of 20 `CustomMetadata`.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("customMetadata")]
        public global::System.Collections.Generic.IList<global::Gemini.CustomMetadata>? CustomMetadata { get; set; }

        /// <summary>
        /// Output only. The Timestamp of when the `Document` was last updated.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("updateTime")]
        public global::System.DateTime? UpdateTime { get; set; }

        /// <summary>
        /// Output only. The Timestamp of when the `Document` was created.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("createTime")]
        public global::System.DateTime? CreateTime { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Document" /> class.
        /// </summary>
        /// <param name="name">
        /// Immutable. Identifier. The `Document` resource name. The ID (name excluding the<br/>
        /// "corpora/*/documents/" prefix) can contain up to 40 characters that are<br/>
        /// lowercase alphanumeric or dashes (-). The ID cannot start or end with a<br/>
        /// dash. If the name is empty on create, a unique name will be derived from<br/>
        /// `display_name` along with a 12 character random suffix.<br/>
        /// Example: `corpora/{corpus_id}/documents/my-awesome-doc-123a456b789c`
        /// </param>
        /// <param name="displayName">
        /// Optional. The human-readable display name for the `Document`. The display name must<br/>
        /// be no more than 512 characters in length, including spaces.<br/>
        /// Example: "Semantic Retriever Documentation"
        /// </param>
        /// <param name="customMetadata">
        /// Optional. User provided custom metadata stored as key-value pairs used for querying.<br/>
        /// A `Document` can have a maximum of 20 `CustomMetadata`.
        /// </param>
        /// <param name="updateTime">
        /// Output only. The Timestamp of when the `Document` was last updated.<br/>
        /// Included only in responses
        /// </param>
        /// <param name="createTime">
        /// Output only. The Timestamp of when the `Document` was created.<br/>
        /// Included only in responses
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public Document(
            string? name,
            string? displayName,
            global::System.Collections.Generic.IList<global::Gemini.CustomMetadata>? customMetadata,
            global::System.DateTime? updateTime,
            global::System.DateTime? createTime)
        {
            this.Name = name;
            this.DisplayName = displayName;
            this.CustomMetadata = customMetadata;
            this.UpdateTime = updateTime;
            this.CreateTime = createTime;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Document" /> class.
        /// </summary>
        public Document()
        {
        }
    }
}