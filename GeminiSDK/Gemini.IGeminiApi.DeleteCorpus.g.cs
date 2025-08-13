#nullable enable

namespace Gemini
{
    public partial interface IGeminiApi
    {
        /// <summary>
        /// Deletes a `Corpus`.
        /// </summary>
        /// <param name="corpus"></param>
        /// <param name="force"></param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::Gemini.ApiException"></exception>
        global::System.Threading.Tasks.Task<string> DeleteCorpusAsync(
            string corpus,
            bool? force = default,
            global::System.Threading.CancellationToken cancellationToken = default);
    }
}