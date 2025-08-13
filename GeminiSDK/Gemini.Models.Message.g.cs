
#nullable enable

namespace Gemini
{
    /// <summary>
    /// The base unit of structured text.<br/>
    /// A `Message` includes an `author` and the `content` of<br/>
    /// the `Message`.<br/>
    /// The `author` is used to tag messages when they are fed to the<br/>
    /// model as text.
    /// </summary>
    public sealed partial class Message
    {
        /// <summary>
        /// Optional. The author of this Message.<br/>
        /// This serves as a key for tagging<br/>
        /// the content of this Message when it is fed to the model as text.<br/>
        /// The author can be any alphanumeric string.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("author")]
        public string? Author { get; set; }

        /// <summary>
        /// Required. The text content of the structured `Message`.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("content")]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required string Content { get; set; }

        /// <summary>
        /// Output only. Citation information for model-generated `content` in this `Message`.<br/>
        /// If this `Message` was generated as output from the model, this field may be<br/>
        /// populated with attribution information for any text included in the<br/>
        /// `content`. This field is used only on output.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("citationMetadata")]
        public global::Gemini.CitationMetadata? CitationMetadata { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Message" /> class.
        /// </summary>
        /// <param name="author">
        /// Optional. The author of this Message.<br/>
        /// This serves as a key for tagging<br/>
        /// the content of this Message when it is fed to the model as text.<br/>
        /// The author can be any alphanumeric string.
        /// </param>
        /// <param name="content">
        /// Required. The text content of the structured `Message`.
        /// </param>
        /// <param name="citationMetadata">
        /// Output only. Citation information for model-generated `content` in this `Message`.<br/>
        /// If this `Message` was generated as output from the model, this field may be<br/>
        /// populated with attribution information for any text included in the<br/>
        /// `content`. This field is used only on output.<br/>
        /// Included only in responses
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public Message(
            string content,
            string? author,
            global::Gemini.CitationMetadata? citationMetadata)
        {
            this.Content = content ?? throw new global::System.ArgumentNullException(nameof(content));
            this.Author = author;
            this.CitationMetadata = citationMetadata;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Message" /> class.
        /// </summary>
        public Message()
        {
        }
    }
}