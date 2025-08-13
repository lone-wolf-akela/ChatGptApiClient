
#nullable enable

namespace Gemini
{
    /// <summary>
    /// 
    /// </summary>
    public enum Type
    {
        /// <summary>
        /// 
        /// </summary>
        TYPEUNSPECIFIED,
        /// <summary>
        /// 
        /// </summary>
        STRING,
        /// <summary>
        /// 
        /// </summary>
        NUMBER,
        /// <summary>
        /// 
        /// </summary>
        INTEGER,
        /// <summary>
        /// 
        /// </summary>
        BOOLEAN,
        /// <summary>
        /// 
        /// </summary>
        ARRAY,
        /// <summary>
        /// 
        /// </summary>
        OBJECT,
        /// <summary>
        /// 
        /// </summary>
        NULL,
    }

    /// <summary>
    /// Enum extensions to do fast conversions without the reflection.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Converts an enum to a string.
        /// </summary>
        public static string ToValueString(this Type value)
        {
            return value switch
            {
                Type.TYPEUNSPECIFIED => "TYPE_UNSPECIFIED",
                Type.STRING => "STRING",
                Type.NUMBER => "NUMBER",
                Type.INTEGER => "INTEGER",
                Type.BOOLEAN => "BOOLEAN",
                Type.ARRAY => "ARRAY",
                Type.OBJECT => "OBJECT",
                Type.NULL => "NULL",
                _ => throw new global::System.ArgumentOutOfRangeException(nameof(value), value, null),
            };
        }
        /// <summary>
        /// Converts an string to a enum.
        /// </summary>
        public static Type? ToEnum(string value)
        {
            return value switch
            {
                "TYPE_UNSPECIFIED" => Type.TYPEUNSPECIFIED,
                "STRING" => Type.STRING,
                "NUMBER" => Type.NUMBER,
                "INTEGER" => Type.INTEGER,
                "BOOLEAN" => Type.BOOLEAN,
                "ARRAY" => Type.ARRAY,
                "OBJECT" => Type.OBJECT,
                "NULL" => Type.NULL,
                _ => null,
            };
        }
    }
}