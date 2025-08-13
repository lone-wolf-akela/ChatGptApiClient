
#nullable enable

namespace Gemini
{
    /// <summary>
    /// Status of the url retrieval.
    /// </summary>
    public enum UrlMetadataUrlRetrievalStatus
    {
        /// <summary>
        /// 
        /// </summary>
        URLRETRIEVALSTATUSUNSPECIFIED,
        /// <summary>
        /// 
        /// </summary>
        URLRETRIEVALSTATUSSUCCESS,
        /// <summary>
        /// 
        /// </summary>
        URLRETRIEVALSTATUSERROR,
    }

    /// <summary>
    /// Enum extensions to do fast conversions without the reflection.
    /// </summary>
    public static class UrlMetadataUrlRetrievalStatusExtensions
    {
        /// <summary>
        /// Converts an enum to a string.
        /// </summary>
        public static string ToValueString(this UrlMetadataUrlRetrievalStatus value)
        {
            return value switch
            {
                UrlMetadataUrlRetrievalStatus.URLRETRIEVALSTATUSUNSPECIFIED => "URL_RETRIEVAL_STATUS_UNSPECIFIED",
                UrlMetadataUrlRetrievalStatus.URLRETRIEVALSTATUSSUCCESS => "URL_RETRIEVAL_STATUS_SUCCESS",
                UrlMetadataUrlRetrievalStatus.URLRETRIEVALSTATUSERROR => "URL_RETRIEVAL_STATUS_ERROR",
                _ => throw new global::System.ArgumentOutOfRangeException(nameof(value), value, null),
            };
        }
        /// <summary>
        /// Converts an string to a enum.
        /// </summary>
        public static UrlMetadataUrlRetrievalStatus? ToEnum(string value)
        {
            return value switch
            {
                "URL_RETRIEVAL_STATUS_UNSPECIFIED" => UrlMetadataUrlRetrievalStatus.URLRETRIEVALSTATUSUNSPECIFIED,
                "URL_RETRIEVAL_STATUS_SUCCESS" => UrlMetadataUrlRetrievalStatus.URLRETRIEVALSTATUSSUCCESS,
                "URL_RETRIEVAL_STATUS_ERROR" => UrlMetadataUrlRetrievalStatus.URLRETRIEVALSTATUSERROR,
                _ => null,
            };
        }
    }
}