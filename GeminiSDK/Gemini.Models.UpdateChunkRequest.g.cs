
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Request to update a `Chunk`.
    /// </summary>
    public sealed partial class UpdateChunkRequest
    {
        /// <summary>
        /// Required. The `Chunk` to update.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("chunk")]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required global::Gemini.Chunk Chunk { get; set; }

        /// <summary>
        /// Required. The list of fields to update.<br/>
        /// Currently, this only supports updating `custom_metadata` and `data`.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("updateMask")]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required string UpdateMask { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateChunkRequest" /> class.
        /// </summary>
        /// <param name="chunk">
        /// Required. The `Chunk` to update.
        /// </param>
        /// <param name="updateMask">
        /// Required. The list of fields to update.<br/>
        /// Currently, this only supports updating `custom_metadata` and `data`.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public UpdateChunkRequest(
            global::Gemini.Chunk chunk,
            string updateMask)
        {
            this.Chunk = chunk ?? throw new global::System.ArgumentNullException(nameof(chunk));
            this.UpdateMask = updateMask ?? throw new global::System.ArgumentNullException(nameof(updateMask));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateChunkRequest" /> class.
        /// </summary>
        public UpdateChunkRequest()
        {
        }
    }
}