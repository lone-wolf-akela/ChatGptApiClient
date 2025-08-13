
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Request to delete a `Chunk`.
    /// </summary>
    public sealed partial class DeleteChunkRequest
    {
        /// <summary>
        /// Required. The resource name of the `Chunk` to delete.<br/>
        /// Example: `corpora/my-corpus-123/documents/the-doc-abc/chunks/some-chunk`
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("name")]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required string Name { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteChunkRequest" /> class.
        /// </summary>
        /// <param name="name">
        /// Required. The resource name of the `Chunk` to delete.<br/>
        /// Example: `corpora/my-corpus-123/documents/the-doc-abc/chunks/some-chunk`
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public DeleteChunkRequest(
            string name)
        {
            this.Name = name ?? throw new global::System.ArgumentNullException(nameof(name));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteChunkRequest" /> class.
        /// </summary>
        public DeleteChunkRequest()
        {
        }
    }
}