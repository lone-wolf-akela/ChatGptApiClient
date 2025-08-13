
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Raw media bytes.<br/>
    /// Text should not be sent as raw bytes, use the 'text' field.
    /// </summary>
    public sealed partial class Blob
    {
        /// <summary>
        /// The IANA standard MIME type of the source data.<br/>
        /// Examples:<br/>
        ///   - image/png<br/>
        ///   - image/jpeg<br/>
        /// If an unsupported MIME type is provided, an error will be returned. For a<br/>
        /// complete list of supported types, see [Supported file<br/>
        /// formats](https://ai.google.dev/gemini-api/docs/prompting_with_media#supported_file_formats).
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("mimeType")]
        public string? MimeType { get; set; }

        /// <summary>
        /// Raw bytes for media formats.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("data")]
        public byte[]? Data { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Blob" /> class.
        /// </summary>
        /// <param name="mimeType">
        /// The IANA standard MIME type of the source data.<br/>
        /// Examples:<br/>
        ///   - image/png<br/>
        ///   - image/jpeg<br/>
        /// If an unsupported MIME type is provided, an error will be returned. For a<br/>
        /// complete list of supported types, see [Supported file<br/>
        /// formats](https://ai.google.dev/gemini-api/docs/prompting_with_media#supported_file_formats).
        /// </param>
        /// <param name="data">
        /// Raw bytes for media formats.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public Blob(
            string? mimeType,
            byte[]? data)
        {
            this.MimeType = mimeType;
            this.Data = data;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blob" /> class.
        /// </summary>
        public Blob()
        {
        }
    }
}