
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Output only. The state of the GeneratedFile.<br/>
    /// Included only in responses
    /// </summary>
    public enum GeneratedFileState
    {
        /// <summary>
        /// 
        /// </summary>
        STATEUNSPECIFIED,
        /// <summary>
        /// 
        /// </summary>
        GENERATING,
        /// <summary>
        /// 
        /// </summary>
        GENERATED,
        /// <summary>
        /// 
        /// </summary>
        FAILED,
    }

    /// <summary>
    /// Enum extensions to do fast conversions without the reflection.
    /// </summary>
    public static class GeneratedFileStateExtensions
    {
        /// <summary>
        /// Converts an enum to a string.
        /// </summary>
        public static string ToValueString(this GeneratedFileState value)
        {
            return value switch
            {
                GeneratedFileState.STATEUNSPECIFIED => "STATE_UNSPECIFIED",
                GeneratedFileState.GENERATING => "GENERATING",
                GeneratedFileState.GENERATED => "GENERATED",
                GeneratedFileState.FAILED => "FAILED",
                _ => throw new global::System.ArgumentOutOfRangeException(nameof(value), value, null),
            };
        }
        /// <summary>
        /// Converts an string to a enum.
        /// </summary>
        public static GeneratedFileState? ToEnum(string value)
        {
            return value switch
            {
                "STATE_UNSPECIFIED" => GeneratedFileState.STATEUNSPECIFIED,
                "GENERATING" => GeneratedFileState.GENERATING,
                "GENERATED" => GeneratedFileState.GENERATED,
                "FAILED" => GeneratedFileState.FAILED,
                _ => null,
            };
        }
    }
}