
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Content that has been preprocessed and can be used in subsequent request<br/>
    /// to GenerativeService.<br/>
    /// Cached content can be only used with model it was created for.
    /// </summary>
    public sealed partial class CachedContent
    {
        /// <summary>
        /// Timestamp in UTC of when this resource is considered expired.<br/>
        /// This is *always* provided on output, regardless of what was sent<br/>
        /// on input.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("expireTime")]
        public global::System.DateTime? ExpireTime { get; set; }

        /// <summary>
        /// Input only. New TTL for this resource, input only.<br/>
        /// Included only in requests
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("ttl")]
        public string? Ttl { get; set; }

        /// <summary>
        /// Output only. Identifier. The resource name referring to the cached content.<br/>
        /// Format: `cachedContents/{id}`<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("name")]
        public string? Name { get; set; }

        /// <summary>
        /// Optional. Immutable. The user-generated meaningful display name of the cached content. Maximum<br/>
        /// 128 Unicode characters.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("displayName")]
        public string? DisplayName { get; set; }

        /// <summary>
        /// Required. Immutable. The name of the `Model` to use for cached content<br/>
        /// Format: `models/{model}`
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("model")]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required string Model { get; set; }

        /// <summary>
        /// Optional. Input only. Immutable. Developer set system instruction. Currently text only.<br/>
        /// Included only in requests
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("systemInstruction")]
        public global::Gemini.Content? SystemInstruction { get; set; }

        /// <summary>
        /// Optional. Input only. Immutable. The content to cache.<br/>
        /// Included only in requests
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("contents")]
        public global::System.Collections.Generic.IList<global::Gemini.Content>? Contents { get; set; }

        /// <summary>
        /// Optional. Input only. Immutable. A list of `Tools` the model may use to generate the next response<br/>
        /// Included only in requests
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("tools")]
        public global::System.Collections.Generic.IList<global::Gemini.Tool>? Tools { get; set; }

        /// <summary>
        /// Optional. Input only. Immutable. Tool config. This config is shared for all tools.<br/>
        /// Included only in requests
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("toolConfig")]
        public global::Gemini.ToolConfig? ToolConfig { get; set; }

        /// <summary>
        /// Output only. Creation time of the cache entry.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("createTime")]
        public global::System.DateTime? CreateTime { get; set; }

        /// <summary>
        /// Output only. When the cache entry was last updated in UTC time.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("updateTime")]
        public global::System.DateTime? UpdateTime { get; set; }

        /// <summary>
        /// Output only. Metadata on the usage of the cached content.<br/>
        /// Included only in responses
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("usageMetadata")]
        public global::Gemini.CachedContentUsageMetadata? UsageMetadata { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedContent" /> class.
        /// </summary>
        /// <param name="expireTime">
        /// Timestamp in UTC of when this resource is considered expired.<br/>
        /// This is *always* provided on output, regardless of what was sent<br/>
        /// on input.
        /// </param>
        /// <param name="ttl">
        /// Input only. New TTL for this resource, input only.<br/>
        /// Included only in requests
        /// </param>
        /// <param name="name">
        /// Output only. Identifier. The resource name referring to the cached content.<br/>
        /// Format: `cachedContents/{id}`<br/>
        /// Included only in responses
        /// </param>
        /// <param name="displayName">
        /// Optional. Immutable. The user-generated meaningful display name of the cached content. Maximum<br/>
        /// 128 Unicode characters.
        /// </param>
        /// <param name="model">
        /// Required. Immutable. The name of the `Model` to use for cached content<br/>
        /// Format: `models/{model}`
        /// </param>
        /// <param name="systemInstruction">
        /// Optional. Input only. Immutable. Developer set system instruction. Currently text only.<br/>
        /// Included only in requests
        /// </param>
        /// <param name="contents">
        /// Optional. Input only. Immutable. The content to cache.<br/>
        /// Included only in requests
        /// </param>
        /// <param name="tools">
        /// Optional. Input only. Immutable. A list of `Tools` the model may use to generate the next response<br/>
        /// Included only in requests
        /// </param>
        /// <param name="toolConfig">
        /// Optional. Input only. Immutable. Tool config. This config is shared for all tools.<br/>
        /// Included only in requests
        /// </param>
        /// <param name="createTime">
        /// Output only. Creation time of the cache entry.<br/>
        /// Included only in responses
        /// </param>
        /// <param name="updateTime">
        /// Output only. When the cache entry was last updated in UTC time.<br/>
        /// Included only in responses
        /// </param>
        /// <param name="usageMetadata">
        /// Output only. Metadata on the usage of the cached content.<br/>
        /// Included only in responses
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public CachedContent(
            string model,
            global::System.DateTime? expireTime,
            string? ttl,
            string? name,
            string? displayName,
            global::Gemini.Content? systemInstruction,
            global::System.Collections.Generic.IList<global::Gemini.Content>? contents,
            global::System.Collections.Generic.IList<global::Gemini.Tool>? tools,
            global::Gemini.ToolConfig? toolConfig,
            global::System.DateTime? createTime,
            global::System.DateTime? updateTime,
            global::Gemini.CachedContentUsageMetadata? usageMetadata)
        {
            this.Model = model ?? throw new global::System.ArgumentNullException(nameof(model));
            this.ExpireTime = expireTime;
            this.Ttl = ttl;
            this.Name = name;
            this.DisplayName = displayName;
            this.SystemInstruction = systemInstruction;
            this.Contents = contents;
            this.Tools = tools;
            this.ToolConfig = toolConfig;
            this.CreateTime = createTime;
            this.UpdateTime = updateTime;
            this.UsageMetadata = usageMetadata;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedContent" /> class.
        /// </summary>
        public CachedContent()
        {
        }
    }
}