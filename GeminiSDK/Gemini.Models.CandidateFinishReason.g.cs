
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Optional. Output only. The reason why the model stopped generating tokens.<br/>
    /// If empty, the model has not stopped generating tokens.<br/>
    /// Included only in responses
    /// </summary>
    public enum CandidateFinishReason
    {
        /// <summary>
        /// 
        /// </summary>
        FINISHREASONUNSPECIFIED,
        /// <summary>
        /// 
        /// </summary>
        STOP,
        /// <summary>
        /// 
        /// </summary>
        MAXTOKENS,
        /// <summary>
        /// 
        /// </summary>
        SAFETY,
        /// <summary>
        /// 
        /// </summary>
        RECITATION,
        /// <summary>
        /// 
        /// </summary>
        LANGUAGE,
        /// <summary>
        /// 
        /// </summary>
        OTHER,
        /// <summary>
        /// 
        /// </summary>
        BLOCKLIST,
        /// <summary>
        /// 
        /// </summary>
        PROHIBITEDCONTENT,
        /// <summary>
        /// 
        /// </summary>
        SPII,
        /// <summary>
        /// 
        /// </summary>
        MALFORMEDFUNCTIONCALL,
        /// <summary>
        /// 
        /// </summary>
        IMAGESAFETY,
        /// <summary>
        /// 
        /// </summary>
        UNEXPECTEDTOOLCALL,
        /// <summary>
        /// 
        /// </summary>
        TOOMANYTOOLCALLS,
    }

    /// <summary>
    /// Enum extensions to do fast conversions without the reflection.
    /// </summary>
    public static class CandidateFinishReasonExtensions
    {
        /// <summary>
        /// Converts an enum to a string.
        /// </summary>
        public static string ToValueString(this CandidateFinishReason value)
        {
            return value switch
            {
                CandidateFinishReason.FINISHREASONUNSPECIFIED => "FINISH_REASON_UNSPECIFIED",
                CandidateFinishReason.STOP => "STOP",
                CandidateFinishReason.MAXTOKENS => "MAX_TOKENS",
                CandidateFinishReason.SAFETY => "SAFETY",
                CandidateFinishReason.RECITATION => "RECITATION",
                CandidateFinishReason.LANGUAGE => "LANGUAGE",
                CandidateFinishReason.OTHER => "OTHER",
                CandidateFinishReason.BLOCKLIST => "BLOCKLIST",
                CandidateFinishReason.PROHIBITEDCONTENT => "PROHIBITED_CONTENT",
                CandidateFinishReason.SPII => "SPII",
                CandidateFinishReason.MALFORMEDFUNCTIONCALL => "MALFORMED_FUNCTION_CALL",
                CandidateFinishReason.IMAGESAFETY => "IMAGE_SAFETY",
                CandidateFinishReason.UNEXPECTEDTOOLCALL => "UNEXPECTED_TOOL_CALL",
                CandidateFinishReason.TOOMANYTOOLCALLS => "TOO_MANY_TOOL_CALLS",
                _ => throw new global::System.ArgumentOutOfRangeException(nameof(value), value, null),
            };
        }
        /// <summary>
        /// Converts an string to a enum.
        /// </summary>
        public static CandidateFinishReason? ToEnum(string value)
        {
            return value switch
            {
                "FINISH_REASON_UNSPECIFIED" => CandidateFinishReason.FINISHREASONUNSPECIFIED,
                "STOP" => CandidateFinishReason.STOP,
                "MAX_TOKENS" => CandidateFinishReason.MAXTOKENS,
                "SAFETY" => CandidateFinishReason.SAFETY,
                "RECITATION" => CandidateFinishReason.RECITATION,
                "LANGUAGE" => CandidateFinishReason.LANGUAGE,
                "OTHER" => CandidateFinishReason.OTHER,
                "BLOCKLIST" => CandidateFinishReason.BLOCKLIST,
                "PROHIBITED_CONTENT" => CandidateFinishReason.PROHIBITEDCONTENT,
                "SPII" => CandidateFinishReason.SPII,
                "MALFORMED_FUNCTION_CALL" => CandidateFinishReason.MALFORMEDFUNCTIONCALL,
                "IMAGE_SAFETY" => CandidateFinishReason.IMAGESAFETY,
                "UNEXPECTED_TOOL_CALL" => CandidateFinishReason.UNEXPECTEDTOOLCALL,
                "TOO_MANY_TOOL_CALLS" => CandidateFinishReason.TOOMANYTOOLCALLS,
                _ => null,
            };
        }
    }
}