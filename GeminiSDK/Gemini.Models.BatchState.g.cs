
#nullable enable

namespace Gemini
{
    /// <summary>
    /// 
    /// </summary>
    public enum BatchState
    {
        /// <summary>
        /// 
        /// </summary>
        UNSPECIFIED,
        /// <summary>
        /// 
        /// </summary>
        PENDING,
        /// <summary>
        /// 
        /// </summary>
        RUNNING,
        /// <summary>
        /// 
        /// </summary>
        SUCCEEDED,
        /// <summary>
        /// 
        /// </summary>
        FAILED,
        /// <summary>
        /// 
        /// </summary>
        CANCELLED,
        /// <summary>
        /// 
        /// </summary>
        EXPIRED,
    }

    /// <summary>
    /// Enum extensions to do fast conversions without the reflection.
    /// </summary>
    public static class BatchStateExtensions
    {
        /// <summary>
        /// Converts an enum to a string.
        /// </summary>
        public static string ToValueString(this BatchState value)
        {
            return value switch
            {
                BatchState.UNSPECIFIED => "BATCH_STATE_UNSPECIFIED",
                BatchState.PENDING => "BATCH_STATE_PENDING",
                BatchState.RUNNING => "BATCH_STATE_RUNNING",
                BatchState.SUCCEEDED => "BATCH_STATE_SUCCEEDED",
                BatchState.FAILED => "BATCH_STATE_FAILED",
                BatchState.CANCELLED => "BATCH_STATE_CANCELLED",
                BatchState.EXPIRED => "BATCH_STATE_EXPIRED",
                _ => throw new global::System.ArgumentOutOfRangeException(nameof(value), value, null),
            };
        }
        /// <summary>
        /// Converts an string to a enum.
        /// </summary>
        public static BatchState? ToEnum(string value)
        {
            return value switch
            {
                "BATCH_STATE_UNSPECIFIED" => BatchState.UNSPECIFIED,
                "BATCH_STATE_PENDING" => BatchState.PENDING,
                "BATCH_STATE_RUNNING" => BatchState.RUNNING,
                "BATCH_STATE_SUCCEEDED" => BatchState.SUCCEEDED,
                "BATCH_STATE_FAILED" => BatchState.FAILED,
                "BATCH_STATE_CANCELLED" => BatchState.CANCELLED,
                "BATCH_STATE_EXPIRED" => BatchState.EXPIRED,
                _ => null,
            };
        }
    }
}