
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Request to create a `Chunk`.
    /// </summary>
    public sealed partial class CreateChunkRequest
    {
        /// <summary>
        /// Required. The name of the `Document` where this `Chunk` will be created.<br/>
        /// Example: `corpora/my-corpus-123/documents/the-doc-abc`
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("parent")]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required string Parent { get; set; }

        /// <summary>
        /// Required. The `Chunk` to create.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("chunk")]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required global::Gemini.Chunk Chunk { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateChunkRequest" /> class.
        /// </summary>
        /// <param name="parent">
        /// Required. The name of the `Document` where this `Chunk` will be created.<br/>
        /// Example: `corpora/my-corpus-123/documents/the-doc-abc`
        /// </param>
        /// <param name="chunk">
        /// Required. The `Chunk` to create.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public CreateChunkRequest(
            string parent,
            global::Gemini.Chunk chunk)
        {
            this.Parent = parent ?? throw new global::System.ArgumentNullException(nameof(parent));
            this.Chunk = chunk ?? throw new global::System.ArgumentNullException(nameof(chunk));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateChunkRequest" /> class.
        /// </summary>
        public CreateChunkRequest()
        {
        }
    }
}