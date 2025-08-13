
#nullable enable

namespace Gemini
{
    /// <summary>
    /// The responses to the requests in the batch.
    /// </summary>
    public sealed partial class InlinedResponses
    {
        /// <summary>
        /// Output only. The responses to the requests in the batch.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("inlinedResponses")]
        public global::System.Collections.Generic.IList<global::Gemini.InlinedResponse>? InlinedResponses1 { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="InlinedResponses" /> class.
        /// </summary>
        /// <param name="inlinedResponses1">
        /// Output only. The responses to the requests in the batch.<br/>
        /// Included only in responses
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public InlinedResponses(
            global::System.Collections.Generic.IList<global::Gemini.InlinedResponse>? inlinedResponses1)
        {
            this.InlinedResponses1 = inlinedResponses1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InlinedResponses" /> class.
        /// </summary>
        public InlinedResponses()
        {
        }
    }
}