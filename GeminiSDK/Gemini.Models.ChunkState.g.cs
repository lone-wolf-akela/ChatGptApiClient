
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Output only. Current state of the `Chunk`.<br/>
    /// Included only in responses
    /// </summary>
    public enum ChunkState
    {
        /// <summary>
        /// 
        /// </summary>
        STATEUNSPECIFIED,
        /// <summary>
        /// 
        /// </summary>
        STATEPENDINGPROCESSING,
        /// <summary>
        /// 
        /// </summary>
        STATEACTIVE,
        /// <summary>
        /// 
        /// </summary>
        STATEFAILED,
    }

    /// <summary>
    /// Enum extensions to do fast conversions without the reflection.
    /// </summary>
    public static class ChunkStateExtensions
    {
        /// <summary>
        /// Converts an enum to a string.
        /// </summary>
        public static string ToValueString(this ChunkState value)
        {
            return value switch
            {
                ChunkState.STATEUNSPECIFIED => "STATE_UNSPECIFIED",
                ChunkState.STATEPENDINGPROCESSING => "STATE_PENDING_PROCESSING",
                ChunkState.STATEACTIVE => "STATE_ACTIVE",
                ChunkState.STATEFAILED => "STATE_FAILED",
                _ => throw new global::System.ArgumentOutOfRangeException(nameof(value), value, null),
            };
        }
        /// <summary>
        /// Converts an string to a enum.
        /// </summary>
        public static ChunkState? ToEnum(string value)
        {
            return value switch
            {
                "STATE_UNSPECIFIED" => ChunkState.STATEUNSPECIFIED,
                "STATE_PENDING_PROCESSING" => ChunkState.STATEPENDINGPROCESSING,
                "STATE_ACTIVE" => ChunkState.STATEACTIVE,
                "STATE_FAILED" => ChunkState.STATEFAILED,
                _ => null,
            };
        }
    }
}