
#nullable enable

namespace Gemini
{
    /// <summary>
    /// The result output from a `FunctionCall` that contains a string<br/>
    /// representing the `FunctionDeclaration.name` and a structured JSON<br/>
    /// object containing any output from the function is used as context to<br/>
    /// the model. This should contain the result of a`FunctionCall` made<br/>
    /// based on model prediction.
    /// </summary>
    public sealed partial class FunctionResponse
    {
        /// <summary>
        /// Optional. The id of the function call this response is for. Populated by the client<br/>
        /// to match the corresponding function call `id`.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("id")]
        public string? Id { get; set; }

        /// <summary>
        /// Required. The name of the function to call.<br/>
        /// Must be a-z, A-Z, 0-9, or contain underscores and dashes, with a maximum<br/>
        /// length of 63.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("name")]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required string Name { get; set; }

        /// <summary>
        /// Required. The function response in JSON object format.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("response")]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required object Response { get; set; }

        /// <summary>
        /// Optional. Signals that function call continues, and more responses will be<br/>
        /// returned, turning the function call into a generator.<br/>
        /// Is only applicable to NON_BLOCKING function calls, is ignored otherwise.<br/>
        /// If set to false, future responses will not be considered.<br/>
        /// It is allowed to return empty `response` with `will_continue=False` to<br/>
        /// signal that the function call is finished. This may still trigger the model<br/>
        /// generation. To avoid triggering the generation and finish the function<br/>
        /// call, additionally set `scheduling` to `SILENT`.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("willContinue")]
        public bool? WillContinue { get; set; }

        /// <summary>
        /// Optional. Specifies how the response should be scheduled in the conversation.<br/>
        /// Only applicable to NON_BLOCKING function calls, is ignored otherwise.<br/>
        /// Defaults to WHEN_IDLE.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("scheduling")]
        [global::System.Text.Json.Serialization.JsonConverter(typeof(global::Gemini.JsonConverters.FunctionResponseSchedulingJsonConverter))]
        public global::Gemini.FunctionResponseScheduling? Scheduling { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionResponse" /> class.
        /// </summary>
        /// <param name="id">
        /// Optional. The id of the function call this response is for. Populated by the client<br/>
        /// to match the corresponding function call `id`.
        /// </param>
        /// <param name="name">
        /// Required. The name of the function to call.<br/>
        /// Must be a-z, A-Z, 0-9, or contain underscores and dashes, with a maximum<br/>
        /// length of 63.
        /// </param>
        /// <param name="response">
        /// Required. The function response in JSON object format.
        /// </param>
        /// <param name="willContinue">
        /// Optional. Signals that function call continues, and more responses will be<br/>
        /// returned, turning the function call into a generator.<br/>
        /// Is only applicable to NON_BLOCKING function calls, is ignored otherwise.<br/>
        /// If set to false, future responses will not be considered.<br/>
        /// It is allowed to return empty `response` with `will_continue=False` to<br/>
        /// signal that the function call is finished. This may still trigger the model<br/>
        /// generation. To avoid triggering the generation and finish the function<br/>
        /// call, additionally set `scheduling` to `SILENT`.
        /// </param>
        /// <param name="scheduling">
        /// Optional. Specifies how the response should be scheduled in the conversation.<br/>
        /// Only applicable to NON_BLOCKING function calls, is ignored otherwise.<br/>
        /// Defaults to WHEN_IDLE.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public FunctionResponse(
            string name,
            object response,
            string? id,
            bool? willContinue,
            global::Gemini.FunctionResponseScheduling? scheduling)
        {
            this.Name = name ?? throw new global::System.ArgumentNullException(nameof(name));
            this.Response = response ?? throw new global::System.ArgumentNullException(nameof(response));
            this.Id = id;
            this.WillContinue = willContinue;
            this.Scheduling = scheduling;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionResponse" /> class.
        /// </summary>
        public FunctionResponse()
        {
        }
    }
}