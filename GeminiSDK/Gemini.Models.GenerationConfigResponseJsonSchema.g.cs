
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Optional. Output schema of the generated response. This is an alternative to<br/>
    /// `response_schema` that accepts [JSON Schema](https://json-schema.org/).<br/>
    /// If set, `response_schema` must be omitted, but `response_mime_type` is<br/>
    /// required.<br/>
    /// While the full JSON Schema may be sent, not all features are supported.<br/>
    /// Specifically, only the following properties are supported:<br/>
    /// - `$id`<br/>
    /// - `$defs`<br/>
    /// - `$ref`<br/>
    /// - `$anchor`<br/>
    /// - `type`<br/>
    /// - `format`<br/>
    /// - `title`<br/>
    /// - `description`<br/>
    /// - `enum` (for strings and numbers)<br/>
    /// - `items`<br/>
    /// - `prefixItems`<br/>
    /// - `minItems`<br/>
    /// - `maxItems`<br/>
    /// - `minimum`<br/>
    /// - `maximum`<br/>
    /// - `anyOf`<br/>
    /// - `oneOf` (interpreted the same as `anyOf`)<br/>
    /// - `properties`<br/>
    /// - `additionalProperties`<br/>
    /// - `required`<br/>
    /// The non-standard `propertyOrdering` property may also be set.<br/>
    /// Cyclic references are unrolled to a limited degree and, as such, may only<br/>
    /// be used within non-required properties. (Nullable properties are not<br/>
    /// sufficient.) If `$ref` is set on a sub-schema, no other properties, except<br/>
    /// for than those starting as a `$`, may be set.
    /// </summary>
    public sealed partial class GenerationConfigResponseJsonSchema
    {

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();
    }
}