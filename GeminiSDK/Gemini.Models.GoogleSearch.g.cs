
#nullable enable

namespace Gemini
{
    /// <summary>
    /// GoogleSearch tool type.<br/>
    /// Tool to support Google Search in Model. Powered by Google.
    /// </summary>
    public sealed partial class GoogleSearch
    {
        /// <summary>
        /// Optional. Filter search results to a specific time range.<br/>
        /// If customers set a start time, they must set an end time (and vice<br/>
        /// versa).
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("timeRangeFilter")]
        public global::Gemini.Interval? TimeRangeFilter { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleSearch" /> class.
        /// </summary>
        /// <param name="timeRangeFilter">
        /// Optional. Filter search results to a specific time range.<br/>
        /// If customers set a start time, they must set an end time (and vice<br/>
        /// versa).
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public GoogleSearch(
            global::Gemini.Interval? timeRangeFilter)
        {
            this.TimeRangeFilter = timeRangeFilter;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleSearch" /> class.
        /// </summary>
        public GoogleSearch()
        {
        }
    }
}