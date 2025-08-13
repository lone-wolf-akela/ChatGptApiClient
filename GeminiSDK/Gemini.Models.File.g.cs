
#nullable enable

namespace Gemini
{
    /// <summary>
    /// A file uploaded to the API.<br/>
    /// Next ID: 15
    /// </summary>
    public sealed partial class File
    {
        /// <summary>
        /// Output only. Metadata for a video.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("videoMetadata")]
        public global::Gemini.VideoFileMetadata? VideoMetadata { get; set; }

        /// <summary>
        /// Immutable. Identifier. The `File` resource name. The ID (name excluding the "files/" prefix) can<br/>
        /// contain up to 40 characters that are lowercase alphanumeric or dashes (-).<br/>
        /// The ID cannot start or end with a dash. If the name is empty on create, a<br/>
        /// unique name will be generated.<br/>
        /// Example: `files/123-456`
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("name")]
        public string? Name { get; set; }

        /// <summary>
        /// Optional. The human-readable display name for the `File`. The display name must be<br/>
        /// no more than 512 characters in length, including spaces.<br/>
        /// Example: "Welcome Image"
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("displayName")]
        public string? DisplayName { get; set; }

        /// <summary>
        /// Output only. MIME type of the file.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("mimeType")]
        public string? MimeType { get; set; }

        /// <summary>
        /// Output only. Size of the file in bytes.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("sizeBytes")]
        public string? SizeBytes { get; set; }

        /// <summary>
        /// Output only. The timestamp of when the `File` was created.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("createTime")]
        public global::System.DateTime? CreateTime { get; set; }

        /// <summary>
        /// Output only. The timestamp of when the `File` was last updated.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("updateTime")]
        public global::System.DateTime? UpdateTime { get; set; }

        /// <summary>
        /// Output only. The timestamp of when the `File` will be deleted. Only set if the `File` is<br/>
        /// scheduled to expire.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("expirationTime")]
        public global::System.DateTime? ExpirationTime { get; set; }

        /// <summary>
        /// Output only. SHA-256 hash of the uploaded bytes.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("sha256Hash")]
        public byte[]? Sha256Hash { get; set; }

        /// <summary>
        /// Output only. The uri of the `File`.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("uri")]
        public string? Uri { get; set; }

        /// <summary>
        /// Output only. The download uri of the `File`.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("downloadUri")]
        public string? DownloadUri { get; set; }

        /// <summary>
        /// Output only. Processing state of the File.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("state")]
        [global::System.Text.Json.Serialization.JsonConverter(typeof(global::Gemini.JsonConverters.FileStateJsonConverter))]
        public global::Gemini.FileState? State { get; set; }

        /// <summary>
        /// Source of the File.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("source")]
        [global::System.Text.Json.Serialization.JsonConverter(typeof(global::Gemini.JsonConverters.FileSourceJsonConverter))]
        public global::Gemini.FileSource? Source { get; set; }

        /// <summary>
        /// Output only. Error status if File processing failed.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("error")]
        public global::Gemini.Status? Error { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="File" /> class.
        /// </summary>
        /// <param name="videoMetadata">
        /// Output only. Metadata for a video.<br/>
        /// Included only in responses
        /// </param>
        /// <param name="name">
        /// Immutable. Identifier. The `File` resource name. The ID (name excluding the "files/" prefix) can<br/>
        /// contain up to 40 characters that are lowercase alphanumeric or dashes (-).<br/>
        /// The ID cannot start or end with a dash. If the name is empty on create, a<br/>
        /// unique name will be generated.<br/>
        /// Example: `files/123-456`
        /// </param>
        /// <param name="displayName">
        /// Optional. The human-readable display name for the `File`. The display name must be<br/>
        /// no more than 512 characters in length, including spaces.<br/>
        /// Example: "Welcome Image"
        /// </param>
        /// <param name="mimeType">
        /// Output only. MIME type of the file.<br/>
        /// Included only in responses
        /// </param>
        /// <param name="sizeBytes">
        /// Output only. Size of the file in bytes.<br/>
        /// Included only in responses
        /// </param>
        /// <param name="createTime">
        /// Output only. The timestamp of when the `File` was created.<br/>
        /// Included only in responses
        /// </param>
        /// <param name="updateTime">
        /// Output only. The timestamp of when the `File` was last updated.<br/>
        /// Included only in responses
        /// </param>
        /// <param name="expirationTime">
        /// Output only. The timestamp of when the `File` will be deleted. Only set if the `File` is<br/>
        /// scheduled to expire.<br/>
        /// Included only in responses
        /// </param>
        /// <param name="sha256Hash">
        /// Output only. SHA-256 hash of the uploaded bytes.<br/>
        /// Included only in responses
        /// </param>
        /// <param name="uri">
        /// Output only. The uri of the `File`.<br/>
        /// Included only in responses
        /// </param>
        /// <param name="downloadUri">
        /// Output only. The download uri of the `File`.<br/>
        /// Included only in responses
        /// </param>
        /// <param name="state">
        /// Output only. Processing state of the File.<br/>
        /// Included only in responses
        /// </param>
        /// <param name="source">
        /// Source of the File.
        /// </param>
        /// <param name="error">
        /// Output only. Error status if File processing failed.<br/>
        /// Included only in responses
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public File(
            global::Gemini.VideoFileMetadata? videoMetadata,
            string? name,
            string? displayName,
            string? mimeType,
            string? sizeBytes,
            global::System.DateTime? createTime,
            global::System.DateTime? updateTime,
            global::System.DateTime? expirationTime,
            byte[]? sha256Hash,
            string? uri,
            string? downloadUri,
            global::Gemini.FileState? state,
            global::Gemini.FileSource? source,
            global::Gemini.Status? error)
        {
            this.VideoMetadata = videoMetadata;
            this.Name = name;
            this.DisplayName = displayName;
            this.MimeType = mimeType;
            this.SizeBytes = sizeBytes;
            this.CreateTime = createTime;
            this.UpdateTime = updateTime;
            this.ExpirationTime = expirationTime;
            this.Sha256Hash = sha256Hash;
            this.Uri = uri;
            this.DownloadUri = downloadUri;
            this.State = state;
            this.Source = source;
            this.Error = error;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="File" /> class.
        /// </summary>
        public File()
        {
        }
    }
}