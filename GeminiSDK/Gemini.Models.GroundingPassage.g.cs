
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Passage included inline with a grounding configuration.
    /// </summary>
    public sealed partial class GroundingPassage
    {
        /// <summary>
        /// Identifier for the passage for attributing this passage in grounded<br/>
        /// answers.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("id")]
        public string? Id { get; set; }

        /// <summary>
        /// Content of the passage.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("content")]
        public global::Gemini.Content? Content { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GroundingPassage" /> class.
        /// </summary>
        /// <param name="id">
        /// Identifier for the passage for attributing this passage in grounded<br/>
        /// answers.
        /// </param>
        /// <param name="content">
        /// Content of the passage.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public GroundingPassage(
            string? id,
            global::Gemini.Content? content)
        {
            this.Id = id;
            this.Content = content;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GroundingPassage" /> class.
        /// </summary>
        public GroundingPassage()
        {
        }
    }
}