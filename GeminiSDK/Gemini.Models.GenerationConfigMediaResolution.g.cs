
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Optional. If specified, the media resolution specified will be used.
    /// </summary>
    public enum GenerationConfigMediaResolution
    {
        /// <summary>
        /// 
        /// </summary>
        MEDIARESOLUTIONUNSPECIFIED,
        /// <summary>
        /// 
        /// </summary>
        MEDIARESOLUTIONLOW,
        /// <summary>
        /// 
        /// </summary>
        MEDIARESOLUTIONMEDIUM,
        /// <summary>
        /// 
        /// </summary>
        MEDIARESOLUTIONHIGH,
    }

    /// <summary>
    /// Enum extensions to do fast conversions without the reflection.
    /// </summary>
    public static class GenerationConfigMediaResolutionExtensions
    {
        /// <summary>
        /// Converts an enum to a string.
        /// </summary>
        public static string ToValueString(this GenerationConfigMediaResolution value)
        {
            return value switch
            {
                GenerationConfigMediaResolution.MEDIARESOLUTIONUNSPECIFIED => "MEDIA_RESOLUTION_UNSPECIFIED",
                GenerationConfigMediaResolution.MEDIARESOLUTIONLOW => "MEDIA_RESOLUTION_LOW",
                GenerationConfigMediaResolution.MEDIARESOLUTIONMEDIUM => "MEDIA_RESOLUTION_MEDIUM",
                GenerationConfigMediaResolution.MEDIARESOLUTIONHIGH => "MEDIA_RESOLUTION_HIGH",
                _ => throw new global::System.ArgumentOutOfRangeException(nameof(value), value, null),
            };
        }
        /// <summary>
        /// Converts an string to a enum.
        /// </summary>
        public static GenerationConfigMediaResolution? ToEnum(string value)
        {
            return value switch
            {
                "MEDIA_RESOLUTION_UNSPECIFIED" => GenerationConfigMediaResolution.MEDIARESOLUTIONUNSPECIFIED,
                "MEDIA_RESOLUTION_LOW" => GenerationConfigMediaResolution.MEDIARESOLUTIONLOW,
                "MEDIA_RESOLUTION_MEDIUM" => GenerationConfigMediaResolution.MEDIARESOLUTIONMEDIUM,
                "MEDIA_RESOLUTION_HIGH" => GenerationConfigMediaResolution.MEDIARESOLUTIONHIGH,
                _ => null,
            };
        }
    }
}