
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Response from `ListChunks` containing a paginated list of `Chunk`s.<br/>
    /// The `Chunk`s are sorted by ascending `chunk.create_time`.
    /// </summary>
    public sealed partial class ListChunksResponse
    {
        /// <summary>
        /// The returned `Chunk`s.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("chunks")]
        public global::System.Collections.Generic.IList<global::Gemini.Chunk>? Chunks { get; set; }

        /// <summary>
        /// A token, which can be sent as `page_token` to retrieve the next page.<br/>
        /// If this field is omitted, there are no more pages.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("nextPageToken")]
        public string? NextPageToken { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ListChunksResponse" /> class.
        /// </summary>
        /// <param name="chunks">
        /// The returned `Chunk`s.
        /// </param>
        /// <param name="nextPageToken">
        /// A token, which can be sent as `page_token` to retrieve the next page.<br/>
        /// If this field is omitted, there are no more pages.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public ListChunksResponse(
            global::System.Collections.Generic.IList<global::Gemini.Chunk>? chunks,
            string? nextPageToken)
        {
            this.Chunks = chunks;
            this.NextPageToken = nextPageToken;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListChunksResponse" /> class.
        /// </summary>
        public ListChunksResponse()
        {
        }
    }
}