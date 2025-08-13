
#nullable enable

namespace Gemini
{
    /// <summary>
    /// The normal, successful response of the operation.  If the original<br/>
    /// method returns no data on success, such as `Delete`, the response is<br/>
    /// `google.protobuf.Empty`.  If the original method is standard<br/>
    /// `Get`/`Create`/`Update`, the response should be the resource.  For other<br/>
    /// methods, the response should have the type `XxxResponse`, where `Xxx`<br/>
    /// is the original method name.  For example, if the original method name<br/>
    /// is `TakeSnapshot()`, the inferred response type is<br/>
    /// `TakeSnapshotResponse`.
    /// </summary>
    public sealed partial class OperationVariant2Response
    {

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();
    }
}