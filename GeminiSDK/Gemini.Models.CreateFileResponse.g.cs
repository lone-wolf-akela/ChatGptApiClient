
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Response for `CreateFile`.
    /// </summary>
    public sealed partial class CreateFileResponse
    {
        /// <summary>
        /// Metadata for the created file.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("file")]
        public global::Gemini.File? File { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateFileResponse" /> class.
        /// </summary>
        /// <param name="file">
        /// Metadata for the created file.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public CreateFileResponse(
            global::Gemini.File? file)
        {
            this.File = file;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateFileResponse" /> class.
        /// </summary>
        public CreateFileResponse()
        {
        }
    }
}