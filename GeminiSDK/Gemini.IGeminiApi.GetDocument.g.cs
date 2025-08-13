#nullable enable

namespace Gemini
{
    public partial interface IGeminiApi
    {
        /// <summary>
        /// Gets information about a specific `Document`.
        /// </summary>
        /// <param name="corpus"></param>
        /// <param name="document"></param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::Gemini.ApiException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.Document> GetDocumentAsync(
            string corpus,
            string document,
            global::System.Threading.CancellationToken cancellationToken = default);
    }
}