
#nullable enable

namespace Gemini
{
    /// <summary>
    /// A `Chunk` is a subpart of a `Document` that is treated as an independent unit<br/>
    /// for the purposes of vector representation and storage.<br/>
    /// A `Corpus` can have a maximum of 1 million `Chunk`s.
    /// </summary>
    public sealed partial class Chunk
    {
        /// <summary>
        /// Immutable. Identifier. The `Chunk` resource name. The ID (name excluding the<br/>
        /// "corpora/*/documents/*/chunks/" prefix) can contain up to 40 characters<br/>
        /// that are lowercase alphanumeric or dashes (-). The ID cannot start or end<br/>
        /// with a dash. If the name is empty on create, a random 12-character unique<br/>
        /// ID will be generated.<br/>
        /// Example: `corpora/{corpus_id}/documents/{document_id}/chunks/123a456b789c`
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("name")]
        public string? Name { get; set; }

        /// <summary>
        /// Required. The content for the `Chunk`, such as the text string.<br/>
        /// The maximum number of tokens per chunk is 2043.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("data")]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required global::Gemini.ChunkData Data { get; set; }

        /// <summary>
        /// Optional. User provided custom metadata stored as key-value pairs.<br/>
        /// The maximum number of `CustomMetadata` per chunk is 20.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("customMetadata")]
        public global::System.Collections.Generic.IList<global::Gemini.CustomMetadata>? CustomMetadata { get; set; }

        /// <summary>
        /// Output only. The Timestamp of when the `Chunk` was created.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("createTime")]
        public global::System.DateTime? CreateTime { get; set; }

        /// <summary>
        /// Output only. The Timestamp of when the `Chunk` was last updated.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("updateTime")]
        public global::System.DateTime? UpdateTime { get; set; }

        /// <summary>
        /// Output only. Current state of the `Chunk`.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("state")]
        [global::System.Text.Json.Serialization.JsonConverter(typeof(global::Gemini.JsonConverters.ChunkStateJsonConverter))]
        public global::Gemini.ChunkState? State { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Chunk" /> class.
        /// </summary>
        /// <param name="name">
        /// Immutable. Identifier. The `Chunk` resource name. The ID (name excluding the<br/>
        /// "corpora/*/documents/*/chunks/" prefix) can contain up to 40 characters<br/>
        /// that are lowercase alphanumeric or dashes (-). The ID cannot start or end<br/>
        /// with a dash. If the name is empty on create, a random 12-character unique<br/>
        /// ID will be generated.<br/>
        /// Example: `corpora/{corpus_id}/documents/{document_id}/chunks/123a456b789c`
        /// </param>
        /// <param name="data">
        /// Required. The content for the `Chunk`, such as the text string.<br/>
        /// The maximum number of tokens per chunk is 2043.
        /// </param>
        /// <param name="customMetadata">
        /// Optional. User provided custom metadata stored as key-value pairs.<br/>
        /// The maximum number of `CustomMetadata` per chunk is 20.
        /// </param>
        /// <param name="createTime">
        /// Output only. The Timestamp of when the `Chunk` was created.<br/>
        /// Included only in responses
        /// </param>
        /// <param name="updateTime">
        /// Output only. The Timestamp of when the `Chunk` was last updated.<br/>
        /// Included only in responses
        /// </param>
        /// <param name="state">
        /// Output only. Current state of the `Chunk`.<br/>
        /// Included only in responses
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public Chunk(
            global::Gemini.ChunkData data,
            string? name,
            global::System.Collections.Generic.IList<global::Gemini.CustomMetadata>? customMetadata,
            global::System.DateTime? createTime,
            global::System.DateTime? updateTime,
            global::Gemini.ChunkState? state)
        {
            this.Data = data ?? throw new global::System.ArgumentNullException(nameof(data));
            this.Name = name;
            this.CustomMetadata = customMetadata;
            this.CreateTime = createTime;
            this.UpdateTime = updateTime;
            this.State = state;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Chunk" /> class.
        /// </summary>
        public Chunk()
        {
        }
    }
}