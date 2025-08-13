
#nullable enable

namespace Gemini
{
    /// <summary>
    /// A file generated on behalf of a user.
    /// </summary>
    public sealed partial class GeneratedFile
    {
        /// <summary>
        /// Identifier. The name of the generated file.<br/>
        /// Example: `generatedFiles/abc-123`
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("name")]
        public string? Name { get; set; }

        /// <summary>
        /// MIME type of the generatedFile.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("mimeType")]
        public string? MimeType { get; set; }

        /// <summary>
        /// Output only. The state of the GeneratedFile.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("state")]
        [global::System.Text.Json.Serialization.JsonConverter(typeof(global::Gemini.JsonConverters.GeneratedFileStateJsonConverter))]
        public global::Gemini.GeneratedFileState? State { get; set; }

        /// <summary>
        /// Error details if the GeneratedFile ends up in the STATE_FAILED state.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("error")]
        public global::Gemini.Status? Error { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneratedFile" /> class.
        /// </summary>
        /// <param name="name">
        /// Identifier. The name of the generated file.<br/>
        /// Example: `generatedFiles/abc-123`
        /// </param>
        /// <param name="mimeType">
        /// MIME type of the generatedFile.
        /// </param>
        /// <param name="state">
        /// Output only. The state of the GeneratedFile.<br/>
        /// Included only in responses
        /// </param>
        /// <param name="error">
        /// Error details if the GeneratedFile ends up in the STATE_FAILED state.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public GeneratedFile(
            string? name,
            string? mimeType,
            global::Gemini.GeneratedFileState? state,
            global::Gemini.Status? error)
        {
            this.Name = name;
            this.MimeType = mimeType;
            this.State = state;
            this.Error = error;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneratedFile" /> class.
        /// </summary>
        public GeneratedFile()
        {
        }
    }
}