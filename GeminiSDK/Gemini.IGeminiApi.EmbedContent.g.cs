#nullable enable

namespace Gemini
{
    public partial interface IGeminiApi
    {
        /// <summary>
        /// Generates a text embedding vector from the input `Content` using the<br/>
        /// specified [Gemini Embedding<br/>
        /// model](https://ai.google.dev/gemini-api/docs/models/gemini#text-embedding).
        /// </summary>
        /// <param name="model"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::Gemini.ApiException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.EmbedContentResponse> EmbedContentAsync(
            string model,
            global::Gemini.EmbedContentRequest request,
            global::System.Threading.CancellationToken cancellationToken = default);

        /// <summary>
        /// Generates a text embedding vector from the input `Content` using the<br/>
        /// specified [Gemini Embedding<br/>
        /// model](https://ai.google.dev/gemini-api/docs/models/gemini#text-embedding).
        /// </summary>
        /// <param name="model"></param>
        /// <param name="requestModel">
        /// Required. The model's resource name. This serves as an ID for the Model to use.<br/>
        /// This name should match a model name returned by the `ListModels` method.<br/>
        /// Format: `models/{model}`
        /// </param>
        /// <param name="content">
        /// Required. The content to embed. Only the `parts.text` fields will be counted.
        /// </param>
        /// <param name="taskType">
        /// Optional. Optional task type for which the embeddings will be used. Not supported on<br/>
        /// earlier models (`models/embedding-001`).
        /// </param>
        /// <param name="title">
        /// Optional. An optional title for the text. Only applicable when TaskType is<br/>
        /// `RETRIEVAL_DOCUMENT`.<br/>
        /// Note: Specifying a `title` for `RETRIEVAL_DOCUMENT` provides better quality<br/>
        /// embeddings for retrieval.
        /// </param>
        /// <param name="outputDimensionality">
        /// Optional. Optional reduced dimension for the output embedding. If set, excessive<br/>
        /// values in the output embedding are truncated from the end. Supported by<br/>
        /// newer models since 2024 only. You cannot set this value if using the<br/>
        /// earlier model (`models/embedding-001`).
        /// </param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::System.InvalidOperationException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.EmbedContentResponse> EmbedContentAsync(
            string model,
            string requestModel,
            global::Gemini.Content content,
            global::Gemini.TaskType? taskType = default,
            string? title = default,
            int? outputDimensionality = default,
            global::System.Threading.CancellationToken cancellationToken = default);
    }
}