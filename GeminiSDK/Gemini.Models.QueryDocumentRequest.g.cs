
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Request for querying a `Document`.
    /// </summary>
    public sealed partial class QueryDocumentRequest
    {
        /// <summary>
        /// Required. Query string to perform semantic search.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("query")]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required string Query { get; set; }

        /// <summary>
        /// Optional. The maximum number of `Chunk`s to return.<br/>
        /// The service may return fewer `Chunk`s.<br/>
        /// If unspecified, at most 10 `Chunk`s will be returned.<br/>
        /// The maximum specified result count is 100.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("resultsCount")]
        public int? ResultsCount { get; set; }

        /// <summary>
        /// Optional. Filter for `Chunk` metadata. Each `MetadataFilter` object should<br/>
        /// correspond to a unique key. Multiple `MetadataFilter` objects are joined by<br/>
        /// logical "AND"s.<br/>
        /// Note: `Document`-level filtering is not supported for this request because<br/>
        /// a `Document` name is already specified.<br/>
        /// Example query:<br/>
        /// (year &gt;= 2020 OR year &lt; 2010) AND (genre = drama OR genre = action)<br/>
        /// `MetadataFilter` object list:<br/>
        ///  metadata_filters = [<br/>
        ///  {key = "chunk.custom_metadata.year"<br/>
        ///   conditions = [{int_value = 2020, operation = GREATER_EQUAL},<br/>
        ///                 {int_value = 2010, operation = LESS}},<br/>
        ///  {key = "chunk.custom_metadata.genre"<br/>
        ///   conditions = [{string_value = "drama", operation = EQUAL},<br/>
        ///                 {string_value = "action", operation = EQUAL}}]<br/>
        /// Example query for a numeric range of values:<br/>
        /// (year &gt; 2015 AND year &lt;= 2020)<br/>
        /// `MetadataFilter` object list:<br/>
        ///  metadata_filters = [<br/>
        ///  {key = "chunk.custom_metadata.year"<br/>
        ///   conditions = [{int_value = 2015, operation = GREATER}]},<br/>
        ///  {key = "chunk.custom_metadata.year"<br/>
        ///   conditions = [{int_value = 2020, operation = LESS_EQUAL}]}]<br/>
        /// Note: "AND"s for the same key are only supported for numeric values. String<br/>
        /// values only support "OR"s for the same key.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("metadataFilters")]
        public global::System.Collections.Generic.IList<global::Gemini.MetadataFilter>? MetadataFilters { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryDocumentRequest" /> class.
        /// </summary>
        /// <param name="query">
        /// Required. Query string to perform semantic search.
        /// </param>
        /// <param name="resultsCount">
        /// Optional. The maximum number of `Chunk`s to return.<br/>
        /// The service may return fewer `Chunk`s.<br/>
        /// If unspecified, at most 10 `Chunk`s will be returned.<br/>
        /// The maximum specified result count is 100.
        /// </param>
        /// <param name="metadataFilters">
        /// Optional. Filter for `Chunk` metadata. Each `MetadataFilter` object should<br/>
        /// correspond to a unique key. Multiple `MetadataFilter` objects are joined by<br/>
        /// logical "AND"s.<br/>
        /// Note: `Document`-level filtering is not supported for this request because<br/>
        /// a `Document` name is already specified.<br/>
        /// Example query:<br/>
        /// (year &gt;= 2020 OR year &lt; 2010) AND (genre = drama OR genre = action)<br/>
        /// `MetadataFilter` object list:<br/>
        ///  metadata_filters = [<br/>
        ///  {key = "chunk.custom_metadata.year"<br/>
        ///   conditions = [{int_value = 2020, operation = GREATER_EQUAL},<br/>
        ///                 {int_value = 2010, operation = LESS}},<br/>
        ///  {key = "chunk.custom_metadata.genre"<br/>
        ///   conditions = [{string_value = "drama", operation = EQUAL},<br/>
        ///                 {string_value = "action", operation = EQUAL}}]<br/>
        /// Example query for a numeric range of values:<br/>
        /// (year &gt; 2015 AND year &lt;= 2020)<br/>
        /// `MetadataFilter` object list:<br/>
        ///  metadata_filters = [<br/>
        ///  {key = "chunk.custom_metadata.year"<br/>
        ///   conditions = [{int_value = 2015, operation = GREATER}]},<br/>
        ///  {key = "chunk.custom_metadata.year"<br/>
        ///   conditions = [{int_value = 2020, operation = LESS_EQUAL}]}]<br/>
        /// Note: "AND"s for the same key are only supported for numeric values. String<br/>
        /// values only support "OR"s for the same key.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public QueryDocumentRequest(
            string query,
            int? resultsCount,
            global::System.Collections.Generic.IList<global::Gemini.MetadataFilter>? metadataFilters)
        {
            this.Query = query ?? throw new global::System.ArgumentNullException(nameof(query));
            this.ResultsCount = resultsCount;
            this.MetadataFilters = metadataFilters;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryDocumentRequest" /> class.
        /// </summary>
        public QueryDocumentRequest()
        {
        }
    }
}