
#nullable enable

namespace Gemini
{
    /// <summary>
    /// 
    /// </summary>
    public enum HarmCategory
    {
        /// <summary>
        /// 
        /// </summary>
        UNSPECIFIED,
        /// <summary>
        /// 
        /// </summary>
        DEROGATORY,
        /// <summary>
        /// 
        /// </summary>
        TOXICITY,
        /// <summary>
        /// 
        /// </summary>
        VIOLENCE,
        /// <summary>
        /// 
        /// </summary>
        SEXUAL,
        /// <summary>
        /// 
        /// </summary>
        MEDICAL,
        /// <summary>
        /// 
        /// </summary>
        DANGEROUS,
        /// <summary>
        /// 
        /// </summary>
        HARASSMENT,
        /// <summary>
        /// 
        /// </summary>
        HATESPEECH,
        /// <summary>
        /// 
        /// </summary>
        SEXUALLYEXPLICIT,
        /// <summary>
        /// 
        /// </summary>
        DANGEROUSCONTENT,
        /// <summary>
        /// 
        /// </summary>
        CIVICINTEGRITY,
    }

    /// <summary>
    /// Enum extensions to do fast conversions without the reflection.
    /// </summary>
    public static class HarmCategoryExtensions
    {
        /// <summary>
        /// Converts an enum to a string.
        /// </summary>
        public static string ToValueString(this HarmCategory value)
        {
            return value switch
            {
                HarmCategory.UNSPECIFIED => "HARM_CATEGORY_UNSPECIFIED",
                HarmCategory.DEROGATORY => "HARM_CATEGORY_DEROGATORY",
                HarmCategory.TOXICITY => "HARM_CATEGORY_TOXICITY",
                HarmCategory.VIOLENCE => "HARM_CATEGORY_VIOLENCE",
                HarmCategory.SEXUAL => "HARM_CATEGORY_SEXUAL",
                HarmCategory.MEDICAL => "HARM_CATEGORY_MEDICAL",
                HarmCategory.DANGEROUS => "HARM_CATEGORY_DANGEROUS",
                HarmCategory.HARASSMENT => "HARM_CATEGORY_HARASSMENT",
                HarmCategory.HATESPEECH => "HARM_CATEGORY_HATE_SPEECH",
                HarmCategory.SEXUALLYEXPLICIT => "HARM_CATEGORY_SEXUALLY_EXPLICIT",
                HarmCategory.DANGEROUSCONTENT => "HARM_CATEGORY_DANGEROUS_CONTENT",
                HarmCategory.CIVICINTEGRITY => "HARM_CATEGORY_CIVIC_INTEGRITY",
                _ => throw new global::System.ArgumentOutOfRangeException(nameof(value), value, null),
            };
        }
        /// <summary>
        /// Converts an string to a enum.
        /// </summary>
        public static HarmCategory? ToEnum(string value)
        {
            return value switch
            {
                "HARM_CATEGORY_UNSPECIFIED" => HarmCategory.UNSPECIFIED,
                "HARM_CATEGORY_DEROGATORY" => HarmCategory.DEROGATORY,
                "HARM_CATEGORY_TOXICITY" => HarmCategory.TOXICITY,
                "HARM_CATEGORY_VIOLENCE" => HarmCategory.VIOLENCE,
                "HARM_CATEGORY_SEXUAL" => HarmCategory.SEXUAL,
                "HARM_CATEGORY_MEDICAL" => HarmCategory.MEDICAL,
                "HARM_CATEGORY_DANGEROUS" => HarmCategory.DANGEROUS,
                "HARM_CATEGORY_HARASSMENT" => HarmCategory.HARASSMENT,
                "HARM_CATEGORY_HATE_SPEECH" => HarmCategory.HATESPEECH,
                "HARM_CATEGORY_SEXUALLY_EXPLICIT" => HarmCategory.SEXUALLYEXPLICIT,
                "HARM_CATEGORY_DANGEROUS_CONTENT" => HarmCategory.DANGEROUSCONTENT,
                "HARM_CATEGORY_CIVIC_INTEGRITY" => HarmCategory.CIVICINTEGRITY,
                _ => null,
            };
        }
    }
}