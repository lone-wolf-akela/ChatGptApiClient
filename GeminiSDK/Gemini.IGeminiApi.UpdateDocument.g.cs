#nullable enable

namespace Gemini
{
    public partial interface IGeminiApi
    {
        /// <summary>
        /// Updates a `Document`.
        /// </summary>
        /// <param name="corpus"></param>
        /// <param name="document"></param>
        /// <param name="updateMask"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::Gemini.ApiException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.Document> UpdateDocumentAsync(
            string corpus,
            string document,
            string updateMask,
            global::Gemini.Document request,
            global::System.Threading.CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates a `Document`.
        /// </summary>
        /// <param name="corpus"></param>
        /// <param name="document"></param>
        /// <param name="updateMask"></param>
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
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::System.InvalidOperationException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.Document> UpdateDocumentAsync(
            string corpus,
            string document,
            string updateMask,
            string? name = default,
            string? displayName = default,
            global::System.Collections.Generic.IList<global::Gemini.CustomMetadata>? customMetadata = default,
            global::System.Threading.CancellationToken cancellationToken = default);
    }
}