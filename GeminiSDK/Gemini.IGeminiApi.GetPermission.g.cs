#nullable enable

namespace Gemini
{
    public partial interface IGeminiApi
    {
        /// <summary>
        /// Gets information about a specific Permission.
        /// </summary>
        /// <param name="tunedModel"></param>
        /// <param name="permission"></param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::Gemini.ApiException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.Permission> GetPermissionAsync(
            string tunedModel,
            string permission,
            global::System.Threading.CancellationToken cancellationToken = default);
    }
}