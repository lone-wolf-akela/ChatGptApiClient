
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Response for `ListFiles`.
    /// </summary>
    public sealed partial class ListFilesResponse
    {
        /// <summary>
        /// The list of `File`s.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("files")]
        public global::System.Collections.Generic.IList<global::Gemini.File>? Files { get; set; }

        /// <summary>
        /// A token that can be sent as a `page_token` into a subsequent `ListFiles`<br/>
        /// call.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("nextPageToken")]
        public string? NextPageToken { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ListFilesResponse" /> class.
        /// </summary>
        /// <param name="files">
        /// The list of `File`s.
        /// </param>
        /// <param name="nextPageToken">
        /// A token that can be sent as a `page_token` into a subsequent `ListFiles`<br/>
        /// call.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public ListFilesResponse(
            global::System.Collections.Generic.IList<global::Gemini.File>? files,
            string? nextPageToken)
        {
            this.Files = files;
            this.NextPageToken = nextPageToken;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListFilesResponse" /> class.
        /// </summary>
        public ListFilesResponse()
        {
        }
    }
}