
#nullable enable

namespace Gemini
{
    /// <summary>
    /// The reason content was blocked during request processing.
    /// </summary>
    public enum ContentFilterReason
    {
        /// <summary>
        /// 
        /// </summary>
        BLOCKEDREASONUNSPECIFIED,
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
    public static class ContentFilterReasonExtensions
    {
        /// <summary>
        /// Converts an enum to a string.
        /// </summary>
        public static string ToValueString(this ContentFilterReason value)
        {
            return value switch
            {
                ContentFilterReason.BLOCKEDREASONUNSPECIFIED => "BLOCKED_REASON_UNSPECIFIED",
                ContentFilterReason.SAFETY => "SAFETY",
                ContentFilterReason.OTHER => "OTHER",
                _ => throw new global::System.ArgumentOutOfRangeException(nameof(value), value, null),
            };
        }
        /// <summary>
        /// Converts an string to a enum.
        /// </summary>
        public static ContentFilterReason? ToEnum(string value)
        {
            return value switch
            {
                "BLOCKED_REASON_UNSPECIFIED" => ContentFilterReason.BLOCKEDREASONUNSPECIFIED,
                "SAFETY" => ContentFilterReason.SAFETY,
                "OTHER" => ContentFilterReason.OTHER,
                _ => null,
            };
        }
    }
}