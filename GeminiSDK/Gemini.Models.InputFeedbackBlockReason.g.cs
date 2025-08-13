
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Optional. If set, the input was blocked and no candidates are returned.<br/>
    /// Rephrase the input.
    /// </summary>
    public enum InputFeedbackBlockReason
    {
        /// <summary>
        /// 
        /// </summary>
        BLOCKREASONUNSPECIFIED,
        /// <summary>
        /// 
        /// </summary>
        SAFETY,
        /// <summary>
        /// 
        /// </summary>
        OTHER,
    }

    /// <summary>
    /// Enum extensions to do fast conversions without the reflection.
    /// </summary>
    public static class InputFeedbackBlockReasonExtensions
    {
        /// <summary>
        /// Converts an enum to a string.
        /// </summary>
        public static string ToValueString(this InputFeedbackBlockReason value)
        {
            return value switch
            {
                InputFeedbackBlockReason.BLOCKREASONUNSPECIFIED => "BLOCK_REASON_UNSPECIFIED",
                InputFeedbackBlockReason.SAFETY => "SAFETY",
                InputFeedbackBlockReason.OTHER => "OTHER",
                _ => throw new global::System.ArgumentOutOfRangeException(nameof(value), value, null),
            };
        }
        /// <summary>
        /// Converts an string to a enum.
        /// </summary>
        public static InputFeedbackBlockReason? ToEnum(string value)
        {
            return value switch
            {
                "BLOCK_REASON_UNSPECIFIED" => InputFeedbackBlockReason.BLOCKREASONUNSPECIFIED,
                "SAFETY" => InputFeedbackBlockReason.SAFETY,
                "OTHER" => InputFeedbackBlockReason.OTHER,
                _ => null,
            };
        }
    }
}