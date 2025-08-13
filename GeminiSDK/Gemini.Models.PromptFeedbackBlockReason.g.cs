
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Optional. If set, the prompt was blocked and no candidates are returned.<br/>
    /// Rephrase the prompt.
    /// </summary>
    public enum PromptFeedbackBlockReason
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
        /// <summary>
        /// 
        /// </summary>
        BLOCKLIST,
        /// <summary>
        /// 
        /// </summary>
        PROHIBITEDCONTENT,
        /// <summary>
        /// 
        /// </summary>
        IMAGESAFETY,
    }

    /// <summary>
    /// Enum extensions to do fast conversions without the reflection.
    /// </summary>
    public static class PromptFeedbackBlockReasonExtensions
    {
        /// <summary>
        /// Converts an enum to a string.
        /// </summary>
        public static string ToValueString(this PromptFeedbackBlockReason value)
        {
            return value switch
            {
                PromptFeedbackBlockReason.BLOCKREASONUNSPECIFIED => "BLOCK_REASON_UNSPECIFIED",
                PromptFeedbackBlockReason.SAFETY => "SAFETY",
                PromptFeedbackBlockReason.OTHER => "OTHER",
                PromptFeedbackBlockReason.BLOCKLIST => "BLOCKLIST",
                PromptFeedbackBlockReason.PROHIBITEDCONTENT => "PROHIBITED_CONTENT",
                PromptFeedbackBlockReason.IMAGESAFETY => "IMAGE_SAFETY",
                _ => throw new global::System.ArgumentOutOfRangeException(nameof(value), value, null),
            };
        }
        /// <summary>
        /// Converts an string to a enum.
        /// </summary>
        public static PromptFeedbackBlockReason? ToEnum(string value)
        {
            return value switch
            {
                "BLOCK_REASON_UNSPECIFIED" => PromptFeedbackBlockReason.BLOCKREASONUNSPECIFIED,
                "SAFETY" => PromptFeedbackBlockReason.SAFETY,
                "OTHER" => PromptFeedbackBlockReason.OTHER,
                "BLOCKLIST" => PromptFeedbackBlockReason.BLOCKLIST,
                "PROHIBITED_CONTENT" => PromptFeedbackBlockReason.PROHIBITEDCONTENT,
                "IMAGE_SAFETY" => PromptFeedbackBlockReason.IMAGESAFETY,
                _ => null,
            };
        }
    }
}