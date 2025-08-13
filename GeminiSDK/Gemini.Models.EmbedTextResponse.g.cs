
#nullable enable

namespace Gemini
{
    /// <summary>
    /// The response to a EmbedTextRequest.
    /// </summary>
    public sealed partial class EmbedTextResponse
    {
        /// <summary>
        /// Output only. The embedding generated from the input text.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("embedding")]
        public global::Gemini.Embedding? Embedding { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbedTextResponse" /> class.
        /// </summary>
        /// <param name="embedding">
        /// Output only. The embedding generated from the input text.<br/>
        /// Included only in responses
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public EmbedTextResponse(
            global::Gemini.Embedding? embedding)
        {
            this.Embedding = embedding;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbedTextResponse" /> class.
        /// </summary>
        public EmbedTextResponse()
        {
        }
    }
}