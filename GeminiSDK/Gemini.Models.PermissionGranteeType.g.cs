
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Optional. Immutable. The type of the grantee.
    /// </summary>
    public enum PermissionGranteeType
    {
        /// <summary>
        /// 
        /// </summary>
        GRANTEETYPEUNSPECIFIED,
        /// <summary>
        /// 
        /// </summary>
        USER,
        /// <summary>
        /// 
        /// </summary>
        GROUP,
        /// <summary>
        /// 
        /// </summary>
        EVERYONE,
    }

    /// <summary>
    /// Enum extensions to do fast conversions without the reflection.
    /// </summary>
    public static class PermissionGranteeTypeExtensions
    {
        /// <summary>
        /// Converts an enum to a string.
        /// </summary>
        public static string ToValueString(this PermissionGranteeType value)
        {
            return value switch
            {
                PermissionGranteeType.GRANTEETYPEUNSPECIFIED => "GRANTEE_TYPE_UNSPECIFIED",
                PermissionGranteeType.USER => "USER",
                PermissionGranteeType.GROUP => "GROUP",
                PermissionGranteeType.EVERYONE => "EVERYONE",
                _ => throw new global::System.ArgumentOutOfRangeException(nameof(value), value, null),
            };
        }
        /// <summary>
        /// Converts an string to a enum.
        /// </summary>
        public static PermissionGranteeType? ToEnum(string value)
        {
            return value switch
            {
                "GRANTEE_TYPE_UNSPECIFIED" => PermissionGranteeType.GRANTEETYPEUNSPECIFIED,
                "USER" => PermissionGranteeType.USER,
                "GROUP" => PermissionGranteeType.GROUP,
                "EVERYONE" => PermissionGranteeType.EVERYONE,
                _ => null,
            };
        }
    }
}