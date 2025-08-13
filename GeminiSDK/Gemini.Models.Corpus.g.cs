
#nullable enable

namespace Gemini
{
    /// <summary>
    /// A `Corpus` is a collection of `Document`s.<br/>
    /// A project can create up to 5 corpora.
    /// </summary>
    public sealed partial class Corpus
    {
        /// <summary>
        /// Immutable. Identifier. The `Corpus` resource name. The ID (name excluding the "corpora/" prefix)<br/>
        /// can contain up to 40 characters that are lowercase alphanumeric or dashes<br/>
        /// (-). The ID cannot start or end with a dash. If the name is empty on<br/>
        /// create, a unique name will be derived from `display_name` along with a 12<br/>
        /// character random suffix.<br/>
        /// Example: `corpora/my-awesome-corpora-123a456b789c`
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("name")]
        public string? Name { get; set; }

        /// <summary>
        /// Optional. The human-readable display name for the `Corpus`. The display name must be<br/>
        /// no more than 512 characters in length, including spaces.<br/>
        /// Example: "Docs on Semantic Retriever"
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("displayName")]
        public string? DisplayName { get; set; }

        /// <summary>
        /// Output only. The Timestamp of when the `Corpus` was created.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("createTime")]
        public global::System.DateTime? CreateTime { get; set; }

        /// <summary>
        /// Output only. The Timestamp of when the `Corpus` was last updated.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("updateTime")]
        public global::System.DateTime? UpdateTime { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Corpus" /> class.
        /// </summary>
        /// <param name="name">
        /// Immutable. Identifier. The `Corpus` resource name. The ID (name excluding the "corpora/" prefix)<br/>
        /// can contain up to 40 characters that are lowercase alphanumeric or dashes<br/>
        /// (-). The ID cannot start or end with a dash. If the name is empty on<br/>
        /// create, a unique name will be derived from `display_name` along with a 12<br/>
        /// character random suffix.<br/>
        /// Example: `corpora/my-awesome-corpora-123a456b789c`
        /// </param>
        /// <param name="displayName">
        /// Optional. The human-readable display name for the `Corpus`. The display name must be<br/>
        /// no more than 512 characters in length, including spaces.<br/>
        /// Example: "Docs on Semantic Retriever"
        /// </param>
        /// <param name="createTime">
        /// Output only. The Timestamp of when the `Corpus` was created.<br/>
        /// Included only in responses
        /// </param>
        /// <param name="updateTime">
        /// Output only. The Timestamp of when the `Corpus` was last updated.<br/>
        /// Included only in responses
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public Corpus(
            string? name,
            string? displayName,
            global::System.DateTime? createTime,
            global::System.DateTime? updateTime)
        {
            this.Name = name;
            this.DisplayName = displayName;
            this.CreateTime = createTime;
            this.UpdateTime = updateTime;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Corpus" /> class.
        /// </summary>
        public Corpus()
        {
        }
    }
}