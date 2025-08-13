
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Required. Programming language of the `code`.
    /// </summary>
    public enum ExecutableCodeLanguage
    {
        /// <summary>
        /// 
        /// </summary>
        LANGUAGEUNSPECIFIED,
        /// <summary>
        /// 
        /// </summary>
        PYTHON,
    }

    /// <summary>
    /// Enum extensions to do fast conversions without the reflection.
    /// </summary>
    public static class ExecutableCodeLanguageExtensions
    {
        /// <summary>
        /// Converts an enum to a string.
        /// </summary>
        public static string ToValueString(this ExecutableCodeLanguage value)
        {
            return value switch
            {
                ExecutableCodeLanguage.LANGUAGEUNSPECIFIED => "LANGUAGE_UNSPECIFIED",
                ExecutableCodeLanguage.PYTHON => "PYTHON",
                _ => throw new global::System.ArgumentOutOfRangeException(nameof(value), value, null),
            };
        }
        /// <summary>
        /// Converts an string to a enum.
        /// </summary>
        public static ExecutableCodeLanguage? ToEnum(string value)
        {
            return value switch
            {
                "LANGUAGE_UNSPECIFIED" => ExecutableCodeLanguage.LANGUAGEUNSPECIFIED,
                "PYTHON" => ExecutableCodeLanguage.PYTHON,
                _ => null,
            };
        }
    }
}