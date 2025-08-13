#nullable enable

namespace Gemini
{
    public partial interface IGeminiApi
    {
        /// <summary>
        /// Performs semantic search over a `Document`.
        /// </summary>
        /// <param name="corpus"></param>
        /// <param name="document"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::Gemini.ApiException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.QueryDocumentResponse> QueryDocumentAsync(
            string corpus,
            string document,
            global::Gemini.QueryDocumentRequest request,
            global::System.Threading.CancellationToken cancellationToken = default);

        /// <summary>
        /// Performs semantic search over a `Document`.
        /// </summary>
        /// <param name="corpus"></param>
        /// <param name="document"></param>
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
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::System.InvalidOperationException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.QueryDocumentResponse> QueryDocumentAsync(
            string corpus,
            string document,
            string query,
            int? resultsCount = default,
            global::System.Collections.Generic.IList<global::Gemini.MetadataFilter>? metadataFilters = default,
            global::System.Threading.CancellationToken cancellationToken = default);
    }
}