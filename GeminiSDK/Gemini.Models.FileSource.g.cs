
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Source of the File.
    /// </summary>
    public enum FileSource
    {
        /// <summary>
        /// 
        /// </summary>
        SOURCEUNSPECIFIED,
        /// <summary>
        /// 
        /// </summary>
        UPLOADED,
        /// <summary>
        /// 
        /// </summary>
        GENERATED,
    }

    /// <summary>
    /// Enum extensions to do fast conversions without the reflection.
    /// </summary>
    public static class FileSourceExtensions
    {
        /// <summary>
        /// Converts an enum to a string.
        /// </summary>
        public static string ToValueString(this FileSource value)
        {
            return value switch
            {
                FileSource.SOURCEUNSPECIFIED => "SOURCE_UNSPECIFIED",
                FileSource.UPLOADED => "UPLOADED",
                FileSource.GENERATED => "GENERATED",
                _ => throw new global::System.ArgumentOutOfRangeException(nameof(value), value, null),
            };
        }
        /// <summary>
        /// Converts an string to a enum.
        /// </summary>
        public static FileSource? ToEnum(string value)
        {
            return value switch
            {
                "SOURCE_UNSPECIFIED" => FileSource.SOURCEUNSPECIFIED,
                "UPLOADED" => FileSource.UPLOADED,
                "GENERATED" => FileSource.GENERATED,
                _ => null,
            };
        }
    }
}