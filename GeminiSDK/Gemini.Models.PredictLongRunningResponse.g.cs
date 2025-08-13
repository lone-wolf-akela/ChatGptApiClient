
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Response message for [PredictionService.PredictLongRunning]
    /// </summary>
    public sealed partial class PredictLongRunningResponse
    {
        /// <summary>
        /// The response of the video generation prediction.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("generateVideoResponse")]
        public global::Gemini.GenerateVideoResponse? GenerateVideoResponse { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PredictLongRunningResponse" /> class.
        /// </summary>
        /// <param name="generateVideoResponse">
        /// The response of the video generation prediction.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public PredictLongRunningResponse(
            global::Gemini.GenerateVideoResponse? generateVideoResponse)
        {
            this.GenerateVideoResponse = generateVideoResponse;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PredictLongRunningResponse" /> class.
        /// </summary>
        public PredictLongRunningResponse()
        {
        }
    }
}