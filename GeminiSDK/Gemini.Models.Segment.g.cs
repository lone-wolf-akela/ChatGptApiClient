
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Segment of the content.
    /// </summary>
    public sealed partial class Segment
    {
        /// <summary>
        /// Output only. The index of a Part object within its parent Content object.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("partIndex")]
        public int? PartIndex { get; set; }

        /// <summary>
        /// Output only. Start index in the given Part, measured in bytes. Offset from the start of<br/>
        /// the Part, inclusive, starting at zero.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("startIndex")]
        public int? StartIndex { get; set; }

        /// <summary>
        /// Output only. End index in the given Part, measured in bytes. Offset from the start of<br/>
        /// the Part, exclusive, starting at zero.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("endIndex")]
        public int? EndIndex { get; set; }

        /// <summary>
        /// Output only. The text corresponding to the segment from the response.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("text")]
        public string? Text { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Segment" /> class.
        /// </summary>
        /// <param name="partIndex">
        /// Output only. The index of a Part object within its parent Content object.<br/>
        /// Included only in responses
        /// </param>
        /// <param name="startIndex">
        /// Output only. Start index in the given Part, measured in bytes. Offset from the start of<br/>
        /// the Part, inclusive, starting at zero.<br/>
        /// Included only in responses
        /// </param>
        /// <param name="endIndex">
        /// Output only. End index in the given Part, measured in bytes. Offset from the start of<br/>
        /// the Part, exclusive, starting at zero.<br/>
        /// Included only in responses
        /// </param>
        /// <param name="text">
        /// Output only. The text corresponding to the segment from the response.<br/>
        /// Included only in responses
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public Segment(
            int? partIndex,
            int? startIndex,
            int? endIndex,
            string? text)
        {
            this.PartIndex = partIndex;
            this.StartIndex = startIndex;
            this.EndIndex = endIndex;
            this.Text = text;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Segment" /> class.
        /// </summary>
        public Segment()
        {
        }
    }
}