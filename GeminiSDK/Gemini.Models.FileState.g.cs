
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Output only. Processing state of the File.<br/>
    /// Included only in responses
    /// </summary>
    public enum FileState
    {
        /// <summary>
        /// 
        /// </summary>
        STATEUNSPECIFIED,
        /// <summary>
        /// 
        /// </summary>
        PROCESSING,
        /// <summary>
        /// 
        /// </summary>
        ACTIVE,
        /// <summary>
        /// 
        /// </summary>
        FAILED,
    }

    /// <summary>
    /// Enum extensions to do fast conversions without the reflection.
    /// </summary>
    public static class FileStateExtensions
    {
        /// <summary>
        /// Converts an enum to a string.
        /// </summary>
        public static string ToValueString(this FileState value)
        {
            return value switch
            {
                FileState.STATEUNSPECIFIED => "STATE_UNSPECIFIED",
                FileState.PROCESSING => "PROCESSING",
                FileState.ACTIVE => "ACTIVE",
                FileState.FAILED => "FAILED",
                _ => throw new global::System.ArgumentOutOfRangeException(nameof(value), value, null),
            };
        }
        /// <summary>
        /// Converts an string to a enum.
        /// </summary>
        public static FileState? ToEnum(string value)
        {
            return value switch
            {
                "STATE_UNSPECIFIED" => FileState.STATEUNSPECIFIED,
                "PROCESSING" => FileState.PROCESSING,
                "ACTIVE" => FileState.ACTIVE,
                "FAILED" => FileState.FAILED,
                _ => null,
            };
        }
    }
}