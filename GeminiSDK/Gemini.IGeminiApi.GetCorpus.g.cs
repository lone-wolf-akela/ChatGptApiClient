#nullable enable

namespace Gemini
{
    public partial interface IGeminiApi
    {
        /// <summary>
        /// Gets information about a specific `Corpus`.
        /// </summary>
        /// <param name="corpus"></param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::Gemini.ApiException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.Corpus> GetCorpusAsync(
            string corpus,
            global::System.Threading.CancellationToken cancellationToken = default);
    }
}