
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Optional. Specifies how the response should be scheduled in the conversation.<br/>
    /// Only applicable to NON_BLOCKING function calls, is ignored otherwise.<br/>
    /// Defaults to WHEN_IDLE.
    /// </summary>
    public enum FunctionResponseScheduling
    {
        /// <summary>
        /// 
        /// </summary>
        SCHEDULINGUNSPECIFIED,
        /// <summary>
        /// 
        /// </summary>
        SILENT,
        /// <summary>
        /// 
        /// </summary>
        WHENIDLE,
        /// <summary>
        /// 
        /// </summary>
        INTERRUPT,
    }

    /// <summary>
    /// Enum extensions to do fast conversions without the reflection.
    /// </summary>
    public static class FunctionResponseSchedulingExtensions
    {
        /// <summary>
        /// Converts an enum to a string.
        /// </summary>
        public static string ToValueString(this FunctionResponseScheduling value)
        {
            return value switch
            {
                FunctionResponseScheduling.SCHEDULINGUNSPECIFIED => "SCHEDULING_UNSPECIFIED",
                FunctionResponseScheduling.SILENT => "SILENT",
                FunctionResponseScheduling.WHENIDLE => "WHEN_IDLE",
                FunctionResponseScheduling.INTERRUPT => "INTERRUPT",
                _ => throw new global::System.ArgumentOutOfRangeException(nameof(value), value, null),
            };
        }
        /// <summary>
        /// Converts an string to a enum.
        /// </summary>
        public static FunctionResponseScheduling? ToEnum(string value)
        {
            return value switch
            {
                "SCHEDULING_UNSPECIFIED" => FunctionResponseScheduling.SCHEDULINGUNSPECIFIED,
                "SILENT" => FunctionResponseScheduling.SILENT,
                "WHEN_IDLE" => FunctionResponseScheduling.WHENIDLE,
                "INTERRUPT" => FunctionResponseScheduling.INTERRUPT,
                _ => null,
            };
        }
    }
}