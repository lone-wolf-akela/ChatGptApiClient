#nullable enable

namespace Gemini
{
    public partial interface IGeminiApi
    {
        /// <summary>
        /// Creates a `File`.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::Gemini.ApiException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.CreateFileResponse> CreateFileAsync(
            global::Gemini.CreateFileRequest request,
            global::System.Threading.CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a `File`.
        /// </summary>
        /// <param name="file">
        /// Optional. Metadata for the file to create.
        /// </param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::System.InvalidOperationException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.CreateFileResponse> CreateFileAsync(
            global::Gemini.File? file = default,
            global::System.Threading.CancellationToken cancellationToken = default);
    }
}