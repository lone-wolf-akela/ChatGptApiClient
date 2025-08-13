
#nullable enable

namespace Gemini
{
    /// <summary>
    /// The mode of the predictor to be used in dynamic retrieval.
    /// </summary>
    public enum DynamicRetrievalConfigMode
    {
        /// <summary>
        /// 
        /// </summary>
        MODEUNSPECIFIED,
        /// <summary>
        /// 
        /// </summary>
        MODEDYNAMIC,
    }

    /// <summary>
    /// Enum extensions to do fast conversions without the reflection.
    /// </summary>
    public static class DynamicRetrievalConfigModeExtensions
    {
        /// <summary>
        /// Converts an enum to a string.
        /// </summary>
        public static string ToValueString(this DynamicRetrievalConfigMode value)
        {
            return value switch
            {
                DynamicRetrievalConfigMode.MODEUNSPECIFIED => "MODE_UNSPECIFIED",
                DynamicRetrievalConfigMode.MODEDYNAMIC => "MODE_DYNAMIC",
                _ => throw new global::System.ArgumentOutOfRangeException(nameof(value), value, null),
            };
        }
        /// <summary>
        /// Converts an string to a enum.
        /// </summary>
        public static DynamicRetrievalConfigMode? ToEnum(string value)
        {
            return value switch
            {
                "MODE_UNSPECIFIED" => DynamicRetrievalConfigMode.MODEUNSPECIFIED,
                "MODE_DYNAMIC" => DynamicRetrievalConfigMode.MODEDYNAMIC,
                _ => null,
            };
        }
    }
}