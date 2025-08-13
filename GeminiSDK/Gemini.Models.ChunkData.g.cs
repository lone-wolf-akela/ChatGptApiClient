
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Extracted data that represents the `Chunk` content.
    /// </summary>
    public sealed partial class ChunkData
    {
        /// <summary>
        /// The `Chunk` content as a string.<br/>
        /// The maximum number of tokens per chunk is 2043.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("stringValue")]
        public string? StringValue { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ChunkData" /> class.
        /// </summary>
        /// <param name="stringValue">
        /// The `Chunk` content as a string.<br/>
        /// The maximum number of tokens per chunk is 2043.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public ChunkData(
            string? stringValue)
        {
            this.StringValue = stringValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChunkData" /> class.
        /// </summary>
        public ChunkData()
        {
        }
    }
}