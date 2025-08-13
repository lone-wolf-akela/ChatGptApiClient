
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Optional. Specifies the mode in which function calling should execute. If<br/>
    /// unspecified, the default value will be set to AUTO.
    /// </summary>
    public enum FunctionCallingConfigMode
    {
        /// <summary>
        /// 
        /// </summary>
        MODEUNSPECIFIED,
        /// <summary>
        /// 
        /// </summary>
        AUTO,
        /// <summary>
        /// 
        /// </summary>
        ANY,
        /// <summary>
        /// 
        /// </summary>
        NONE,
        /// <summary>
        /// 
        /// </summary>
        VALIDATED,
    }

    /// <summary>
    /// Enum extensions to do fast conversions without the reflection.
    /// </summary>
    public static class FunctionCallingConfigModeExtensions
    {
        /// <summary>
        /// Converts an enum to a string.
        /// </summary>
        public static string ToValueString(this FunctionCallingConfigMode value)
        {
            return value switch
            {
                FunctionCallingConfigMode.MODEUNSPECIFIED => "MODE_UNSPECIFIED",
                FunctionCallingConfigMode.AUTO => "AUTO",
                FunctionCallingConfigMode.ANY => "ANY",
                FunctionCallingConfigMode.NONE => "NONE",
                FunctionCallingConfigMode.VALIDATED => "VALIDATED",
                _ => throw new global::System.ArgumentOutOfRangeException(nameof(value), value, null),
            };
        }
        /// <summary>
        /// Converts an string to a enum.
        /// </summary>
        public static FunctionCallingConfigMode? ToEnum(string value)
        {
            return value switch
            {
                "MODE_UNSPECIFIED" => FunctionCallingConfigMode.MODEUNSPECIFIED,
                "AUTO" => FunctionCallingConfigMode.AUTO,
                "ANY" => FunctionCallingConfigMode.ANY,
                "NONE" => FunctionCallingConfigMode.NONE,
                "VALIDATED" => FunctionCallingConfigMode.VALIDATED,
                _ => null,
            };
        }
    }
}