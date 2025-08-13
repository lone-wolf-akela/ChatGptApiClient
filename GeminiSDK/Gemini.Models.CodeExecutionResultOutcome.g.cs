
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Required. Outcome of the code execution.
    /// </summary>
    public enum CodeExecutionResultOutcome
    {
        /// <summary>
        /// 
        /// </summary>
        OUTCOMEUNSPECIFIED,
        /// <summary>
        /// 
        /// </summary>
        OUTCOMEOK,
        /// <summary>
        /// 
        /// </summary>
        OUTCOMEFAILED,
        /// <summary>
        /// 
        /// </summary>
        OUTCOMEDEADLINEEXCEEDED,
    }

    /// <summary>
    /// Enum extensions to do fast conversions without the reflection.
    /// </summary>
    public static class CodeExecutionResultOutcomeExtensions
    {
        /// <summary>
        /// Converts an enum to a string.
        /// </summary>
        public static string ToValueString(this CodeExecutionResultOutcome value)
        {
            return value switch
            {
                CodeExecutionResultOutcome.OUTCOMEUNSPECIFIED => "OUTCOME_UNSPECIFIED",
                CodeExecutionResultOutcome.OUTCOMEOK => "OUTCOME_OK",
                CodeExecutionResultOutcome.OUTCOMEFAILED => "OUTCOME_FAILED",
                CodeExecutionResultOutcome.OUTCOMEDEADLINEEXCEEDED => "OUTCOME_DEADLINE_EXCEEDED",
                _ => throw new global::System.ArgumentOutOfRangeException(nameof(value), value, null),
            };
        }
        /// <summary>
        /// Converts an string to a enum.
        /// </summary>
        public static CodeExecutionResultOutcome? ToEnum(string value)
        {
            return value switch
            {
                "OUTCOME_UNSPECIFIED" => CodeExecutionResultOutcome.OUTCOMEUNSPECIFIED,
                "OUTCOME_OK" => CodeExecutionResultOutcome.OUTCOMEOK,
                "OUTCOME_FAILED" => CodeExecutionResultOutcome.OUTCOMEFAILED,
                "OUTCOME_DEADLINE_EXCEEDED" => CodeExecutionResultOutcome.OUTCOMEDEADLINEEXCEEDED,
                _ => null,
            };
        }
    }
}