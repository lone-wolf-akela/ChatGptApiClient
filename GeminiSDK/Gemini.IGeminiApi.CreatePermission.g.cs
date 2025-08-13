#nullable enable

namespace Gemini
{
    public partial interface IGeminiApi
    {
        /// <summary>
        /// Create a permission to a specific resource.
        /// </summary>
        /// <param name="tunedModel"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::Gemini.ApiException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.Permission> CreatePermissionAsync(
            string tunedModel,
            global::Gemini.Permission request,
            global::System.Threading.CancellationToken cancellationToken = default);

        /// <summary>
        /// Create a permission to a specific resource.
        /// </summary>
        /// <param name="tunedModel"></param>
        /// <param name="granteeType">
        /// Optional. Immutable. The type of the grantee.
        /// </param>
        /// <param name="emailAddress">
        /// Optional. Immutable. The email address of the user of group which this permission refers.<br/>
        /// Field is not set when permission's grantee type is EVERYONE.
        /// </param>
        /// <param name="role">
        /// Required. The role granted by this permission.
        /// </param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::System.InvalidOperationException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.Permission> CreatePermissionAsync(
            string tunedModel,
            global::Gemini.PermissionRole role,
            global::Gemini.PermissionGranteeType? granteeType = default,
            string? emailAddress = default,
            global::System.Threading.CancellationToken cancellationToken = default);
    }
}