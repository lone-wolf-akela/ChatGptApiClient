
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Request for querying a `Corpus`.
    /// </summary>
    public sealed partial class QueryCorpusRequest
    {
        /// <summary>
        /// Required. Query string to perform semantic search.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("query")]
        [global::System.Text.Json.Serialization.JsonRequired]
        public required string Query { get; set; }

        /// <summary>
        /// Optional. Filter for `Chunk` and `Document` metadata. Each `MetadataFilter` object<br/>
        /// should correspond to a unique key. Multiple `MetadataFilter` objects are<br/>
        /// joined by logical "AND"s.<br/>
        /// Example query at document level:<br/>
        /// (year &gt;= 2020 OR year &lt; 2010) AND (genre = drama OR genre = action)<br/>
        /// `MetadataFilter` object list:<br/>
        ///  metadata_filters = [<br/>
        ///  {key = "document.custom_metadata.year"<br/>
        ///   conditions = [{int_value = 2020, operation = GREATER_EQUAL},<br/>
        ///                 {int_value = 2010, operation = LESS}]},<br/>
        ///  {key = "document.custom_metadata.year"<br/>
        ///   conditions = [{int_value = 2020, operation = GREATER_EQUAL},<br/>
        ///                 {int_value = 2010, operation = LESS}]},<br/>
        ///  {key = "document.custom_metadata.genre"<br/>
        ///   conditions = [{string_value = "drama", operation = EQUAL},<br/>
        ///                 {string_value = "action", operation = EQUAL}]}]<br/>
        /// Example query at chunk level for a numeric range of values:<br/>
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
        /// Optional. The maximum number of `Chunk`s to return.<br/>
        /// The service may return fewer `Chunk`s.<br/>
        /// If unspecified, at most 10 `Chunk`s will be returned.<br/>
        /// The maximum specified result count is 100.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("resultsCount")]
        public int? ResultsCount { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryCorpusRequest" /> class.
        /// </summary>
        /// <param name="query">
        /// Required. Query string to perform semantic search.
        /// </param>
        /// <param name="metadataFilters">
        /// Optional. Filter for `Chunk` and `Document` metadata. Each `MetadataFilter` object<br/>
        /// should correspond to a unique key. Multiple `MetadataFilter` objects are<br/>
        /// joined by logical "AND"s.<br/>
        /// Example query at document level:<br/>
        /// (year &gt;= 2020 OR year &lt; 2010) AND (genre = drama OR genre = action)<br/>
        /// `MetadataFilter` object list:<br/>
        ///  metadata_filters = [<br/>
        ///  {key = "document.custom_metadata.year"<br/>
        ///   conditions = [{int_value = 2020, operation = GREATER_EQUAL},<br/>
        ///                 {int_value = 2010, operation = LESS}]},<br/>
        ///  {key = "document.custom_metadata.year"<br/>
        ///   conditions = [{int_value = 2020, operation = GREATER_EQUAL},<br/>
        ///                 {int_value = 2010, operation = LESS}]},<br/>
        ///  {key = "document.custom_metadata.genre"<br/>
        ///   conditions = [{string_value = "drama", operation = EQUAL},<br/>
        ///                 {string_value = "action", operation = EQUAL}]}]<br/>
        /// Example query at chunk level for a numeric range of values:<br/>
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
        /// <param name="resultsCount">
        /// Optional. The maximum number of `Chunk`s to return.<br/>
        /// The service may return fewer `Chunk`s.<br/>
        /// If unspecified, at most 10 `Chunk`s will be returned.<br/>
        /// The maximum specified result count is 100.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public QueryCorpusRequest(
            string query,
            global::System.Collections.Generic.IList<global::Gemini.MetadataFilter>? metadataFilters,
            int? resultsCount)
        {
            this.Query = query ?? throw new global::System.ArgumentNullException(nameof(query));
            this.MetadataFilters = metadataFilters;
            this.ResultsCount = resultsCount;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryCorpusRequest" /> class.
        /// </summary>
        public QueryCorpusRequest()
        {
        }
    }
}