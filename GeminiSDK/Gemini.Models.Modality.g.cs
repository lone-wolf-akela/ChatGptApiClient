
#nullable enable

namespace Gemini
{
    /// <summary>
    /// 
    /// </summary>
    public enum Modality
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
        VIDEO,
        /// <summary>
        /// 
        /// </summary>
        AUDIO,
        /// <summary>
        /// 
        /// </summary>
        DOCUMENT,
    }

    /// <summary>
    /// Enum extensions to do fast conversions without the reflection.
    /// </summary>
    public static class ModalityExtensions
    {
        /// <summary>
        /// Converts an enum to a string.
        /// </summary>
        public static string ToValueString(this Modality value)
        {
            return value switch
            {
                Modality.MODALITYUNSPECIFIED => "MODALITY_UNSPECIFIED",
                Modality.TEXT => "TEXT",
                Modality.IMAGE => "IMAGE",
                Modality.VIDEO => "VIDEO",
                Modality.AUDIO => "AUDIO",
                Modality.DOCUMENT => "DOCUMENT",
                _ => throw new global::System.ArgumentOutOfRangeException(nameof(value), value, null),
            };
        }
        /// <summary>
        /// Converts an string to a enum.
        /// </summary>
        public static Modality? ToEnum(string value)
        {
            return value switch
            {
                "MODALITY_UNSPECIFIED" => Modality.MODALITYUNSPECIFIED,
                "TEXT" => Modality.TEXT,
                "IMAGE" => Modality.IMAGE,
                "VIDEO" => Modality.VIDEO,
                "AUDIO" => Modality.AUDIO,
                "DOCUMENT" => Modality.DOCUMENT,
                _ => null,
            };
        }
    }
}