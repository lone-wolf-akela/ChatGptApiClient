
#nullable enable

namespace Gemini
{
    /// <summary>
    /// 
    /// </summary>
    public enum GenerationConfigResponseModalitie
    {
        /// <summary>
        /// 
        /// </summary>
        MODALITYUNSPECIFIED,
        /// <summary>
        /// 
        /// </summary>
        TEXT,
        /// <summary>
        /// 
        /// </summary>
        IMAGE,
        /// <summary>
        /// 
        /// </summary>
        AUDIO,
    }

    /// <summary>
    /// Enum extensions to do fast conversions without the reflection.
    /// </summary>
    public static class GenerationConfigResponseModalitieExtensions
    {
        /// <summary>
        /// Converts an enum to a string.
        /// </summary>
        public static string ToValueString(this GenerationConfigResponseModalitie value)
        {
            return value switch
            {
                GenerationConfigResponseModalitie.MODALITYUNSPECIFIED => "MODALITY_UNSPECIFIED",
                GenerationConfigResponseModalitie.TEXT => "TEXT",
                GenerationConfigResponseModalitie.IMAGE => "IMAGE",
                GenerationConfigResponseModalitie.AUDIO => "AUDIO",
                _ => throw new global::System.ArgumentOutOfRangeException(nameof(value), value, null),
            };
        }
        /// <summary>
        /// Converts an string to a enum.
        /// </summary>
        public static GenerationConfigResponseModalitie? ToEnum(string value)
        {
            return value switch
            {
                "MODALITY_UNSPECIFIED" => GenerationConfigResponseModalitie.MODALITYUNSPECIFIED,
                "TEXT" => GenerationConfigResponseModalitie.TEXT,
                "IMAGE" => GenerationConfigResponseModalitie.IMAGE,
                "AUDIO" => GenerationConfigResponseModalitie.AUDIO,
                _ => null,
            };
        }
    }
}