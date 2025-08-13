#nullable enable

namespace Gemini
{
    public partial interface IGeminiApi
    {
        /// <summary>
        /// Updates a `Chunk`.
        /// </summary>
        /// <param name="corpus"></param>
        /// <param name="document"></param>
        /// <param name="chunk"></param>
        /// <param name="updateMask"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::Gemini.ApiException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.Chunk> UpdateChunkAsync(
            string corpus,
            string document,
            string chunk,
            string updateMask,
            global::Gemini.Chunk request,
            global::System.Threading.CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates a `Chunk`.
        /// </summary>
        /// <param name="corpus"></param>
        /// <param name="document"></param>
        /// <param name="chunk"></param>
        /// <param name="updateMask"></param>
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
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::System.InvalidOperationException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.Chunk> UpdateChunkAsync(
            string corpus,
            string document,
            string chunk,
            string updateMask,
            global::Gemini.ChunkData data,
            string? name = default,
            global::System.Collections.Generic.IList<global::Gemini.CustomMetadata>? customMetadata = default,
            global::System.Threading.CancellationToken cancellationToken = default);
    }
}