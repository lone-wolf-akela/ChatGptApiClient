#nullable enable

namespace Gemini
{
    public partial interface IGeminiApi
    {
        /// <summary>
        /// Gets a generated file. When calling this method via REST, only the metadata<br/>
        /// of the generated file is returned. To retrieve the file content via REST,<br/>
        /// add alt=media as a query parameter.
        /// </summary>
        /// <param name="generatedFile"></param>
        /// <param name="cancellationToken">The token to cancel the operation with</param>
        /// <exception cref="global::Gemini.ApiException"></exception>
        global::System.Threading.Tasks.Task<global::Gemini.GeneratedFile> GetGeneratedFileAsync(
            string generatedFile,
            global::System.Threading.CancellationToken cancellationToken = default);
    }
}