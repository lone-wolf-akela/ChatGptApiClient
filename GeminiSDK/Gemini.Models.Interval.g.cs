
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Represents a time interval, encoded as a Timestamp start (inclusive) and a<br/>
    /// Timestamp end (exclusive).<br/>
    /// The start must be less than or equal to the end.<br/>
    /// When the start equals the end, the interval is empty (matches no time).<br/>
    /// When both start and end are unspecified, the interval matches any time.
    /// </summary>
    public sealed partial class Interval
    {
        /// <summary>
        /// Optional. Inclusive start of the interval.<br/>
        /// If specified, a Timestamp matching this interval will have to be the same<br/>
        /// or after the start.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("startTime")]
        public global::System.DateTime? StartTime { get; set; }

        /// <summary>
        /// Optional. Exclusive end of the interval.<br/>
        /// If specified, a Timestamp matching this interval will have to be before the<br/>
        /// end.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("endTime")]
        public global::System.DateTime? EndTime { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Interval" /> class.
        /// </summary>
        /// <param name="startTime">
        /// Optional. Inclusive start of the interval.<br/>
        /// If specified, a Timestamp matching this interval will have to be the same<br/>
        /// or after the start.
        /// </param>
        /// <param name="endTime">
        /// Optional. Exclusive end of the interval.<br/>
        /// If specified, a Timestamp matching this interval will have to be before the<br/>
        /// end.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public Interval(
            global::System.DateTime? startTime,
            global::System.DateTime? endTime)
        {
            this.StartTime = startTime;
            this.EndTime = endTime;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Interval" /> class.
        /// </summary>
        public Interval()
        {
        }
    }
}