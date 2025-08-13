
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Required. Style in which answers should be returned.
    /// </summary>
    public enum GenerateAnswerRequestAnswerStyle
    {
        /// <summary>
        /// 
        /// </summary>
        ANSWERSTYLEUNSPECIFIED,
        /// <summary>
        /// 
        /// </summary>
        ABSTRACTIVE,
        /// <summary>
        /// 
        /// </summary>
        EXTRACTIVE,
        /// <summary>
        /// 
        /// </summary>
        VERBOSE,
    }

    /// <summary>
    /// Enum extensions to do fast conversions without the reflection.
    /// </summary>
    public static class GenerateAnswerRequestAnswerStyleExtensions
    {
        /// <summary>
        /// Converts an enum to a string.
        /// </summary>
        public static string ToValueString(this GenerateAnswerRequestAnswerStyle value)
        {
            return value switch
            {
                GenerateAnswerRequestAnswerStyle.ANSWERSTYLEUNSPECIFIED => "ANSWER_STYLE_UNSPECIFIED",
                GenerateAnswerRequestAnswerStyle.ABSTRACTIVE => "ABSTRACTIVE",
                GenerateAnswerRequestAnswerStyle.EXTRACTIVE => "EXTRACTIVE",
                GenerateAnswerRequestAnswerStyle.VERBOSE => "VERBOSE",
                _ => throw new global::System.ArgumentOutOfRangeException(nameof(value), value, null),
            };
        }
        /// <summary>
        /// Converts an string to a enum.
        /// </summary>
        public static GenerateAnswerRequestAnswerStyle? ToEnum(string value)
        {
            return value switch
            {
                "ANSWER_STYLE_UNSPECIFIED" => GenerateAnswerRequestAnswerStyle.ANSWERSTYLEUNSPECIFIED,
                "ABSTRACTIVE" => GenerateAnswerRequestAnswerStyle.ABSTRACTIVE,
                "EXTRACTIVE" => GenerateAnswerRequestAnswerStyle.EXTRACTIVE,
                "VERBOSE" => GenerateAnswerRequestAnswerStyle.VERBOSE,
                _ => null,
            };
        }
    }
}