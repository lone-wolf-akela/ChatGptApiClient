#nullable enable

namespace Gemini
{
    public partial interface IGeminiApi
    {
        /// <summary>
        /// Lists the metadata for `File`s owned by the requesting project.
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageToken"></param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::Gemini.ApiException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.ListFilesResponse> ListFilesAsync(
            int? pageSize = default,
            string? pageToken = default,
            global::System.Threading.CancellationToken cancellationToken = default);
    }
}