
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Request to transfer the ownership of the tuned model.
    /// </summary>
    public sealed partial class TransferOwnershipRequest
    {
        /// <summary>
        /// Required. The email address of the user to whom the tuned model is being transferred<br/>
        /// to.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("emailAddress")]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required string EmailAddress { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TransferOwnershipRequest" /> class.
        /// </summary>
        /// <param name="emailAddress">
        /// Required. The email address of the user to whom the tuned model is being transferred<br/>
        /// to.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public TransferOwnershipRequest(
            string emailAddress)
        {
            this.EmailAddress = emailAddress ?? throw new global::System.ArgumentNullException(nameof(emailAddress));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransferOwnershipRequest" /> class.
        /// </summary>
        public TransferOwnershipRequest()
        {
        }
    }
}