#nullable enable

namespace Gemini
{
    public partial interface IGeminiApi
    {
        /// <summary>
        /// Gets the metadata for the given `File`.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::Gemini.ApiException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.File> GetFileAsync(
            string file,
            global::System.Threading.CancellationToken cancellationToken = default);
    }
}