
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Request for `CreateFile`.
    /// </summary>
    public sealed partial class CreateFileRequest
    {
        /// <summary>
        /// Optional. Metadata for the file to create.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("file")]
        public global::Gemini.File? File { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateFileRequest" /> class.
        /// </summary>
        /// <param name="file">
        /// Optional. Metadata for the file to create.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public CreateFileRequest(
            global::Gemini.File? file)
        {
            this.File = file;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateFileRequest" /> class.
        /// </summary>
        public CreateFileRequest()
        {
        }
    }
}