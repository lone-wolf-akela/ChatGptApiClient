#nullable enable

namespace Gemini
{
    public partial interface IGeminiApi
    {
        /// <summary>
        /// Creates a tuned model.<br/>
        /// Check intermediate tuning progress (if any) through the<br/>
        /// [google.longrunning.Operations] service.<br/>
        /// Access status and results through the Operations service.<br/>
        /// Example:<br/>
        ///   GET /v1/tunedModels/az2mb0bpw6i/operations/000-111-222
        /// </summary>
        /// <param name="tunedModelId"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::Gemini.ApiException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.CreateTunedModelOperation> CreateTunedModelAsync(
            global::Gemini.TunedModel request,
            string? tunedModelId = default,
            global::System.Threading.CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a tuned model.<br/>
        /// Check intermediate tuning progress (if any) through the<br/>
        /// [google.longrunning.Operations] service.<br/>
        /// Access status and results through the Operations service.<br/>
        /// Example:<br/>
        ///   GET /v1/tunedModels/az2mb0bpw6i/operations/000-111-222
        /// </summary>
        /// <param name="tunedModelId"></param>
        /// <param name="tunedModelSource">
        /// Optional. TunedModel to use as the starting point for training the new model.
        /// </param>
        /// <param name="baseModel">
        /// Immutable. The name of the `Model` to tune.<br/>
        /// Example: `models/gemini-1.5-flash-001`
        /// </param>
        /// <param name="displayName">
        /// Optional. The name to display for this model in user interfaces.<br/>
        /// The display name must be up to 40 characters including spaces.
        /// </param>
        /// <param name="description">
        /// Optional. A short description of this model.
        /// </param>
        /// <param name="temperature">
        /// Optional. Controls the randomness of the output.<br/>
        /// Values can range over `[0.0,1.0]`, inclusive. A value closer to `1.0` will<br/>
        /// produce responses that are more varied, while a value closer to `0.0` will<br/>
        /// typically result in less surprising responses from the model.<br/>
        /// This value specifies default to be the one used by the base model while<br/>
        /// creating the model.
        /// </param>
        /// <param name="topP">
        /// Optional. For Nucleus sampling.<br/>
        /// Nucleus sampling considers the smallest set of tokens whose probability<br/>
        /// sum is at least `top_p`.<br/>
        /// This value specifies default to be the one used by the base model while<br/>
        /// creating the model.
        /// </param>
        /// <param name="topK">
        /// Optional. For Top-k sampling.<br/>
        /// Top-k sampling considers the set of `top_k` most probable tokens.<br/>
        /// This value specifies default to be used by the backend while making the<br/>
        /// call to the model.<br/>
        /// This value specifies default to be the one used by the base model while<br/>
        /// creating the model.
        /// </param>
        /// <param name="tuningTask">
        /// Required. The tuning task that creates the tuned model.
        /// </param>
        /// <param name="readerProjectNumbers">
        /// Optional. List of project numbers that have read access to the tuned model.
        /// </param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::System.InvalidOperationException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.CreateTunedModelOperation> CreateTunedModelAsync(
            global::Gemini.TuningTask tuningTask,
            string? tunedModelId = default,
            global::Gemini.TunedModelSource? tunedModelSource = default,
            string? baseModel = default,
            string? displayName = default,
            string? description = default,
            float? temperature = default,
            float? topP = default,
            int? topK = default,
            global::System.Collections.Generic.IList<string>? readerProjectNumbers = default,
            global::System.Threading.CancellationToken cancellationToken = default);
    }
}