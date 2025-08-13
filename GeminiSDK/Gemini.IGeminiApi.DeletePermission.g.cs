#nullable enable

namespace Gemini
{
    public partial interface IGeminiApi
    {
        /// <summary>
        /// Deletes the permission.
        /// </summary>
        /// <param name="tunedModel"></param>
        /// <param name="permission"></param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::Gemini.ApiException"></exception>
        global::System.Threading.Tasks.Task<string> DeletePermissionAsync(
            string tunedModel,
            string permission,
            global::System.Threading.CancellationToken cancellationToken = default);
    }
}