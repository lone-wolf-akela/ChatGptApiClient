#nullable enable

namespace Gemini
{
    public partial interface IGeminiApi
    {
        /// <summary>
        /// Deletes the `File`.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::Gemini.ApiException"></exception>
        global::System.Threading.Tasks.Task<string> DeleteFileAsync(
            string file,
            global::System.Threading.CancellationToken cancellationToken = default);
    }
}