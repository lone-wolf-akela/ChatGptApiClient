#nullable enable

namespace Gemini
{
    public partial interface IGeminiApi
    {
        /// <summary>
        /// Updates a `Corpus`.
        /// </summary>
        /// <param name="corpus"></param>
        /// <param name="updateMask"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::Gemini.ApiException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.Corpus> UpdateCorpusAsync(
            string corpus,
            string updateMask,
            global::Gemini.Corpus request,
            global::System.Threading.CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates a `Corpus`.
        /// </summary>
        /// <param name="corpus"></param>
        /// <param name="updateMask"></param>
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
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::System.InvalidOperationException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.Corpus> UpdateCorpusAsync(
            string corpus,
            string updateMask,
            string? name = default,
            string? displayName = default,
            global::System.Threading.CancellationToken cancellationToken = default);
    }
}