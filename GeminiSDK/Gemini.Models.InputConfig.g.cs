
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Configures the input to the batch request.
    /// </summary>
    public sealed partial class InputConfig
    {
        /// <summary>
        /// The name of the `File` containing the input requests.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("fileName")]
        public string? FileName { get; set; }

        /// <summary>
        /// The requests to be processed in the batch.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("requests")]
        public global::Gemini.InlinedRequests? Requests { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="InputConfig" /> class.
        /// </summary>
        /// <param name="fileName">
        /// The name of the `File` containing the input requests.
        /// </param>
        /// <param name="requests">
        /// The requests to be processed in the batch.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public InputConfig(
            string? fileName,
            global::Gemini.InlinedRequests? requests)
        {
            this.FileName = fileName;
            this.Requests = requests;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InputConfig" /> class.
        /// </summary>
        public InputConfig()
        {
        }
    }
}