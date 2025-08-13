
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Permission resource grants user, group or the rest of the world access to the<br/>
    /// PaLM API resource (e.g. a tuned model, corpus).<br/>
    /// A role is a collection of permitted operations that allows users to perform<br/>
    /// specific actions on PaLM API resources. To make them available to users,<br/>
    /// groups, or service accounts, you assign roles. When you assign a role, you<br/>
    /// grant permissions that the role contains.<br/>
    /// There are three concentric roles. Each role is a superset of the previous<br/>
    /// role's permitted operations:<br/>
    /// - reader can use the resource (e.g. tuned model, corpus) for inference<br/>
    /// - writer has reader's permissions and additionally can edit and share<br/>
    /// - owner has writer's permissions and additionally can delete
    /// </summary>
    public sealed partial class Permission
    {
        /// <summary>
        /// Output only. Identifier. The permission name. A unique name will be generated on create.<br/>
        /// Examples:<br/>
        ///     tunedModels/{tuned_model}/permissions/{permission}<br/>
        ///     corpora/{corpus}/permissions/{permission}<br/>
        /// Output only.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("name")]
        public string? Name { get; set; }

        /// <summary>
        /// Optional. Immutable. The type of the grantee.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("granteeType")]
        [global::System.Text.Json.Serialization.JsonConverter(typeof(global::Gemini.JsonConverters.PermissionGranteeTypeJsonConverter))]
        public global::Gemini.PermissionGranteeType? GranteeType { get; set; }

        /// <summary>
        /// Optional. Immutable. The email address of the user of group which this permission refers.<br/>
        /// Field is not set when permission's grantee type is EVERYONE.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("emailAddress")]
        public string? EmailAddress { get; set; }

        /// <summary>
        /// Required. The role granted by this permission.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("role")]
        [global::System.Text.Json.Serialization.JsonConverter(typeof(global::Gemini.JsonConverters.PermissionRoleJsonConverter))]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required global::Gemini.PermissionRole Role { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Permission" /> class.
        /// </summary>
        /// <param name="name">
        /// Output only. Identifier. The permission name. A unique name will be generated on create.<br/>
        /// Examples:<br/>
        ///     tunedModels/{tuned_model}/permissions/{permission}<br/>
        ///     corpora/{corpus}/permissions/{permission}<br/>
        /// Output only.<br/>
        /// Included only in responses
        /// </param>
        /// <param name="granteeType">
        /// Optional. Immutable. The type of the grantee.
        /// </param>
        /// <param name="emailAddress">
        /// Optional. Immutable. The email address of the user of group which this permission refers.<br/>
        /// Field is not set when permission's grantee type is EVERYONE.
        /// </param>
        /// <param name="role">
        /// Required. The role granted by this permission.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public Permission(
            global::Gemini.PermissionRole role,
            string? name,
            global::Gemini.PermissionGranteeType? granteeType,
            string? emailAddress)
        {
            this.Role = role;
            this.Name = name;
            this.GranteeType = granteeType;
            this.EmailAddress = emailAddress;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Permission" /> class.
        /// </summary>
        public Permission()
        {
        }
    }
}