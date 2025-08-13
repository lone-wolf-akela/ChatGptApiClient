
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Representation of a video.
    /// </summary>
    public sealed partial class Video
    {
        /// <summary>
        /// Raw bytes.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("video")]
        public byte[]? Video1 { get; set; }

        /// <summary>
        /// Path to another storage.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("uri")]
        public string? Uri { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Video" /> class.
        /// </summary>
        /// <param name="video1">
        /// Raw bytes.
        /// </param>
        /// <param name="uri">
        /// Path to another storage.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public Video(
            byte[]? video1,
            string? uri)
        {
            this.Video1 = video1;
            this.Uri = uri;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Video" /> class.
        /// </summary>
        public Video()
        {
        }
    }
}