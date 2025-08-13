
#nullable enable

namespace Gemini
{
    /// <summary>
    /// 
    /// </summary>
    public sealed partial class CreateTunedModelOperationVariant2
    {
        /// <summary>
        /// Metadata about the state and progress of creating a tuned model returned from<br/>
        /// the long-running operation
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("metadata")]
        public global::Gemini.CreateTunedModelMetadata? Metadata { get; set; }

        /// <summary>
        /// A fine-tuned model created using ModelService.CreateTunedModel.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("response")]
        public global::Gemini.TunedModel? Response { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateTunedModelOperationVariant2" /> class.
        /// </summary>
        /// <param name="metadata">
        /// Metadata about the state and progress of creating a tuned model returned from<br/>
        /// the long-running operation
        /// </param>
        /// <param name="response">
        /// A fine-tuned model created using ModelService.CreateTunedModel.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public CreateTunedModelOperationVariant2(
            global::Gemini.CreateTunedModelMetadata? metadata,
            global::Gemini.TunedModel? response)
        {
            this.Metadata = metadata;
            this.Response = response;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateTunedModelOperationVariant2" /> class.
        /// </summary>
        public CreateTunedModelOperationVariant2()
        {
        }
    }
}