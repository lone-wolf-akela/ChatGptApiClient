
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Output only. The state of the tuned model.<br/>
    /// Included only in responses
    /// </summary>
    public enum TunedModelState
    {
        /// <summary>
        /// 
        /// </summary>
        STATEUNSPECIFIED,
        /// <summary>
        /// 
        /// </summary>
        CREATING,
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
    public static class TunedModelStateExtensions
    {
        /// <summary>
        /// Converts an enum to a string.
        /// </summary>
        public static string ToValueString(this TunedModelState value)
        {
            return value switch
            {
                TunedModelState.STATEUNSPECIFIED => "STATE_UNSPECIFIED",
                TunedModelState.CREATING => "CREATING",
                TunedModelState.ACTIVE => "ACTIVE",
                TunedModelState.FAILED => "FAILED",
                _ => throw new global::System.ArgumentOutOfRangeException(nameof(value), value, null),
            };
        }
        /// <summary>
        /// Converts an string to a enum.
        /// </summary>
        public static TunedModelState? ToEnum(string value)
        {
            return value switch
            {
                "STATE_UNSPECIFIED" => TunedModelState.STATEUNSPECIFIED,
                "CREATING" => TunedModelState.CREATING,
                "ACTIVE" => TunedModelState.ACTIVE,
                "FAILED" => TunedModelState.FAILED,
                _ => null,
            };
        }
    }
}