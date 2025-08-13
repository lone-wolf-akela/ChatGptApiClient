
#nullable enable

namespace Gemini
{
    /// <summary>
    /// A proto encapsulate various type of media.
    /// </summary>
    public sealed partial class Media
    {
        /// <summary>
        /// Video as the only one for now.  This is mimicking Vertex proto.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("video")]
        public global::Gemini.Video? Video { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Media" /> class.
        /// </summary>
        /// <param name="video">
        /// Video as the only one for now.  This is mimicking Vertex proto.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public Media(
            global::Gemini.Video? video)
        {
            this.Video = video;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Media" /> class.
        /// </summary>
        public Media()
        {
        }
    }
}