
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Optional. Specifies the function Behavior.<br/>
    /// Currently only supported by the BidiGenerateContent method.
    /// </summary>
    public enum FunctionDeclarationBehavior
    {
        /// <summary>
        /// 
        /// </summary>
        UNSPECIFIED,
        /// <summary>
        /// 
        /// </summary>
        BLOCKING,
        /// <summary>
        /// 
        /// </summary>
        NONBLOCKING,
    }

    /// <summary>
    /// Enum extensions to do fast conversions without the reflection.
    /// </summary>
    public static class FunctionDeclarationBehaviorExtensions
    {
        /// <summary>
        /// Converts an enum to a string.
        /// </summary>
        public static string ToValueString(this FunctionDeclarationBehavior value)
        {
            return value switch
            {
                FunctionDeclarationBehavior.UNSPECIFIED => "UNSPECIFIED",
                FunctionDeclarationBehavior.BLOCKING => "BLOCKING",
                FunctionDeclarationBehavior.NONBLOCKING => "NON_BLOCKING",
                _ => throw new global::System.ArgumentOutOfRangeException(nameof(value), value, null),
            };
        }
        /// <summary>
        /// Converts an string to a enum.
        /// </summary>
        public static FunctionDeclarationBehavior? ToEnum(string value)
        {
            return value switch
            {
                "UNSPECIFIED" => FunctionDeclarationBehavior.UNSPECIFIED,
                "BLOCKING" => FunctionDeclarationBehavior.BLOCKING,
                "NON_BLOCKING" => FunctionDeclarationBehavior.NONBLOCKING,
                _ => null,
            };
        }
    }
}