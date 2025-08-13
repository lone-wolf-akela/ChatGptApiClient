
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Response for `ListGeneratedFiles`.
    /// </summary>
    public sealed partial class ListGeneratedFilesResponse
    {
        /// <summary>
        /// The list of `GeneratedFile`s.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("generatedFiles")]
        public global::System.Collections.Generic.IList<global::Gemini.GeneratedFile>? GeneratedFiles { get; set; }

        /// <summary>
        /// A token that can be sent as a `page_token` into a subsequent<br/>
        /// `ListGeneratedFiles` call.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("nextPageToken")]
        public string? NextPageToken { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ListGeneratedFilesResponse" /> class.
        /// </summary>
        /// <param name="generatedFiles">
        /// The list of `GeneratedFile`s.
        /// </param>
        /// <param name="nextPageToken">
        /// A token that can be sent as a `page_token` into a subsequent<br/>
        /// `ListGeneratedFiles` call.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public ListGeneratedFilesResponse(
            global::System.Collections.Generic.IList<global::Gemini.GeneratedFile>? generatedFiles,
            string? nextPageToken)
        {
            this.GeneratedFiles = generatedFiles;
            this.NextPageToken = nextPageToken;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListGeneratedFilesResponse" /> class.
        /// </summary>
        public ListGeneratedFilesResponse()
        {
        }
    }
}