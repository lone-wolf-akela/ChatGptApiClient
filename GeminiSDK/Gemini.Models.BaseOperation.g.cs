
#nullable enable

namespace Gemini
{
    /// <summary>
    /// This resource represents a long-running operation that is the result of a<br/>
    /// network API call.
    /// </summary>
    public sealed partial class BaseOperation
    {
        /// <summary>
        /// The server-assigned name, which is only unique within the same service that<br/>
        /// originally returns it. If you use the default HTTP mapping, the<br/>
        /// `name` should be a resource name ending with `operations/{unique_id}`.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("name")]
        public string? Name { get; set; }

        /// <summary>
        /// If the value is `false`, it means the operation is still in progress.<br/>
        /// If `true`, the operation is completed, and either `error` or `response` is<br/>
        /// available.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("done")]
        public bool? Done { get; set; }

        /// <summary>
        /// The error result of the operation in case of failure or cancellation.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("error")]
        public global::Gemini.Status? Error { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseOperation" /> class.
        /// </summary>
        /// <param name="name">
        /// The server-assigned name, which is only unique within the same service that<br/>
        /// originally returns it. If you use the default HTTP mapping, the<br/>
        /// `name` should be a resource name ending with `operations/{unique_id}`.
        /// </param>
        /// <param name="done">
        /// If the value is `false`, it means the operation is still in progress.<br/>
        /// If `true`, the operation is completed, and either `error` or `response` is<br/>
        /// available.
        /// </param>
        /// <param name="error">
        /// The error result of the operation in case of failure or cancellation.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public BaseOperation(
            string? name,
            bool? done,
            global::Gemini.Status? error)
        {
            this.Name = name;
            this.Done = done;
            this.Error = error;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseOperation" /> class.
        /// </summary>
        public BaseOperation()
        {
        }
    }
}