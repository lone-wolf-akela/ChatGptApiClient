
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Required. Operator applied to the given key-value pair to trigger the condition.
    /// </summary>
    public enum ConditionOperation
    {
        /// <summary>
        /// 
        /// </summary>
        OPERATORUNSPECIFIED,
        /// <summary>
        /// 
        /// </summary>
        LESS,
        /// <summary>
        /// 
        /// </summary>
        LESSEQUAL,
        /// <summary>
        /// 
        /// </summary>
        EQUAL,
        /// <summary>
        /// 
        /// </summary>
        GREATEREQUAL,
        /// <summary>
        /// 
        /// </summary>
        GREATER,
        /// <summary>
        /// 
        /// </summary>
        NOTEQUAL,
        /// <summary>
        /// 
        /// </summary>
        INCLUDES,
        /// <summary>
        /// 
        /// </summary>
        EXCLUDES,
    }

    /// <summary>
    /// Enum extensions to do fast conversions without the reflection.
    /// </summary>
    public static class ConditionOperationExtensions
    {
        /// <summary>
        /// Converts an enum to a string.
        /// </summary>
        public static string ToValueString(this ConditionOperation value)
        {
            return value switch
            {
                ConditionOperation.OPERATORUNSPECIFIED => "OPERATOR_UNSPECIFIED",
                ConditionOperation.LESS => "LESS",
                ConditionOperation.LESSEQUAL => "LESS_EQUAL",
                ConditionOperation.EQUAL => "EQUAL",
                ConditionOperation.GREATEREQUAL => "GREATER_EQUAL",
                ConditionOperation.GREATER => "GREATER",
                ConditionOperation.NOTEQUAL => "NOT_EQUAL",
                ConditionOperation.INCLUDES => "INCLUDES",
                ConditionOperation.EXCLUDES => "EXCLUDES",
                _ => throw new global::System.ArgumentOutOfRangeException(nameof(value), value, null),
            };
        }
        /// <summary>
        /// Converts an string to a enum.
        /// </summary>
        public static ConditionOperation? ToEnum(string value)
        {
            return value switch
            {
                "OPERATOR_UNSPECIFIED" => ConditionOperation.OPERATORUNSPECIFIED,
                "LESS" => ConditionOperation.LESS,
                "LESS_EQUAL" => ConditionOperation.LESSEQUAL,
                "EQUAL" => ConditionOperation.EQUAL,
                "GREATER_EQUAL" => ConditionOperation.GREATEREQUAL,
                "GREATER" => ConditionOperation.GREATER,
                "NOT_EQUAL" => ConditionOperation.NOTEQUAL,
                "INCLUDES" => ConditionOperation.INCLUDES,
                "EXCLUDES" => ConditionOperation.EXCLUDES,
                _ => null,
            };
        }
    }
}