
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Tool details that the model may use to generate response.<br/>
    /// A `Tool` is a piece of code that enables the system to interact with<br/>
    /// external systems to perform an action, or set of actions, outside of<br/>
    /// knowledge and scope of the model.
    /// </summary>
    public sealed partial class Tool
    {
        /// <summary>
        /// Optional. A list of `FunctionDeclarations` available to the model that can be used<br/>
        /// for function calling.<br/>
        /// The model or system does not execute the function. Instead the defined<br/>
        /// function may be returned as a FunctionCall<br/>
        /// with arguments to the client side for execution. The model may decide to<br/>
        /// call a subset of these functions by populating<br/>
        /// FunctionCall in the response. The next<br/>
        /// conversation turn may contain a<br/>
        /// FunctionResponse<br/>
        /// with the Content.role "function" generation context for the next model<br/>
        /// turn.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("functionDeclarations")]
        public global::System.Collections.Generic.IList<global::Gemini.FunctionDeclaration>? FunctionDeclarations { get; set; }

        /// <summary>
        /// Optional. Retrieval tool that is powered by Google search.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("googleSearchRetrieval")]
        public global::Gemini.GoogleSearchRetrieval? GoogleSearchRetrieval { get; set; }

        /// <summary>
        /// Optional. Enables the model to execute code as part of generation.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("codeExecution")]
        public object? CodeExecution { get; set; }

        /// <summary>
        /// Optional. GoogleSearch tool type.<br/>
        /// Tool to support Google Search in Model. Powered by Google.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("googleSearch")]
        public global::Gemini.GoogleSearch? GoogleSearch { get; set; }

        /// <summary>
        /// Optional. Tool to support URL context retrieval.
        /// </summary>
        [global::System.Text.Json.Serialization.JsonPropertyName("urlContext")]
        public object? UrlContext { get; set; }

        /// <summary>
        /// Additional properties that are not explicitly defined in the schema
        /// </summary>
        [global::System.Text.Json.Serialization.JsonExtensionData]
        public global::System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; } = new global::System.Collections.Generic.Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Tool" /> class.
        /// </summary>
        /// <param name="functionDeclarations">
        /// Optional. A list of `FunctionDeclarations` available to the model that can be used<br/>
        /// for function calling.<br/>
        /// The model or system does not execute the function. Instead the defined<br/>
        /// function may be returned as a FunctionCall<br/>
        /// with arguments to the client side for execution. The model may decide to<br/>
        /// call a subset of these functions by populating<br/>
        /// FunctionCall in the response. The next<br/>
        /// conversation turn may contain a<br/>
        /// FunctionResponse<br/>
        /// with the Content.role "function" generation context for the next model<br/>
        /// turn.
        /// </param>
        /// <param name="googleSearchRetrieval">
        /// Optional. Retrieval tool that is powered by Google search.
        /// </param>
        /// <param name="codeExecution">
        /// Optional. Enables the model to execute code as part of generation.
        /// </param>
        /// <param name="googleSearch">
        /// Optional. GoogleSearch tool type.<br/>
        /// Tool to support Google Search in Model. Powered by Google.
        /// </param>
        /// <param name="urlContext">
        /// Optional. Tool to support URL context retrieval.
        /// </param>
#if NET7_0_OR_GREATER
        [global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
#endif
        public Tool(
            global::System.Collections.Generic.IList<global::Gemini.FunctionDeclaration>? functionDeclarations,
            global::Gemini.GoogleSearchRetrieval? googleSearchRetrieval,
            object? codeExecution,
            global::Gemini.GoogleSearch? googleSearch,
            object? urlContext)
        {
            this.FunctionDeclarations = functionDeclarations;
            this.GoogleSearchRetrieval = googleSearchRetrieval;
            this.CodeExecution = codeExecution;
            this.GoogleSearch = googleSearch;
            this.UrlContext = urlContext;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tool" /> class.
        /// </summary>
        public Tool()
        {
        }
    }
}