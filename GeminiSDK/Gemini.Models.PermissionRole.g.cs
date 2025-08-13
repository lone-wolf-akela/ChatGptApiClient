
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Required. The role granted by this permission.
    /// </summary>
    public enum PermissionRole
    {
        /// <summary>
        /// 
        /// </summary>
        ROLEUNSPECIFIED,
        /// <summary>
        /// 
        /// </summary>
        OWNER,
        /// <summary>
        /// 
        /// </summary>
        WRITER,
        /// <summary>
        /// 
        /// </summary>
        READER,
    }

    /// <summary>
    /// Enum extensions to do fast conversions without the reflection.
    /// </summary>
    public static class PermissionRoleExtensions
    {
        /// <summary>
        /// Converts an enum to a string.
        /// </summary>
        public static string ToValueString(this PermissionRole value)
        {
            return value switch
            {
                PermissionRole.ROLEUNSPECIFIED => "ROLE_UNSPECIFIED",
                PermissionRole.OWNER => "OWNER",
                PermissionRole.WRITER => "WRITER",
                PermissionRole.READER => "READER",
                _ => throw new global::System.ArgumentOutOfRangeException(nameof(value), value, null),
            };
        }
        /// <summary>
        /// Converts an string to a enum.
        /// </summary>
        public static PermissionRole? ToEnum(string value)
        {
            return value switch
            {
                "ROLE_UNSPECIFIED" => PermissionRole.ROLEUNSPECIFIED,
                "OWNER" => PermissionRole.OWNER,
                "WRITER" => PermissionRole.WRITER,
                "READER" => PermissionRole.READER,
                _ => null,
            };
        }
    }
}