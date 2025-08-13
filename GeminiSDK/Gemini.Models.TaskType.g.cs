
#nullable enable

namespace Gemini
{
    /// <summary>
    /// 
    /// </summary>
    public enum TaskType
    {
        /// <summary>
        /// 
        /// </summary>
        TASKTYPEUNSPECIFIED,
        /// <summary>
        /// 
        /// </summary>
        RETRIEVALQUERY,
        /// <summary>
        /// 
        /// </summary>
        RETRIEVALDOCUMENT,
        /// <summary>
        /// 
        /// </summary>
        SEMANTICSIMILARITY,
        /// <summary>
        /// 
        /// </summary>
        CLASSIFICATION,
        /// <summary>
        /// 
        /// </summary>
        CLUSTERING,
        /// <summary>
        /// 
        /// </summary>
        QUESTIONANSWERING,
        /// <summary>
        /// 
        /// </summary>
        FACTVERIFICATION,
        /// <summary>
        /// 
        /// </summary>
        CODERETRIEVALQUERY,
    }

    /// <summary>
    /// Enum extensions to do fast conversions without the reflection.
    /// </summary>
    public static class TaskTypeExtensions
    {
        /// <summary>
        /// Converts an enum to a string.
        /// </summary>
        public static string ToValueString(this TaskType value)
        {
            return value switch
            {
                TaskType.TASKTYPEUNSPECIFIED => "TASK_TYPE_UNSPECIFIED",
                TaskType.RETRIEVALQUERY => "RETRIEVAL_QUERY",
                TaskType.RETRIEVALDOCUMENT => "RETRIEVAL_DOCUMENT",
                TaskType.SEMANTICSIMILARITY => "SEMANTIC_SIMILARITY",
                TaskType.CLASSIFICATION => "CLASSIFICATION",
                TaskType.CLUSTERING => "CLUSTERING",
                TaskType.QUESTIONANSWERING => "QUESTION_ANSWERING",
                TaskType.FACTVERIFICATION => "FACT_VERIFICATION",
                TaskType.CODERETRIEVALQUERY => "CODE_RETRIEVAL_QUERY",
                _ => throw new global::System.ArgumentOutOfRangeException(nameof(value), value, null),
            };
        }
        /// <summary>
        /// Converts an string to a enum.
        /// </summary>
        public static TaskType? ToEnum(string value)
        {
            return value switch
            {
                "TASK_TYPE_UNSPECIFIED" => TaskType.TASKTYPEUNSPECIFIED,
                "RETRIEVAL_QUERY" => TaskType.RETRIEVALQUERY,
                "RETRIEVAL_DOCUMENT" => TaskType.RETRIEVALDOCUMENT,
                "SEMANTIC_SIMILARITY" => TaskType.SEMANTICSIMILARITY,
                "CLASSIFICATION" => TaskType.CLASSIFICATION,
                "CLUSTERING" => TaskType.CLUSTERING,
                "QUESTION_ANSWERING" => TaskType.QUESTIONANSWERING,
                "FACT_VERIFICATION" => TaskType.FACTVERIFICATION,
                "CODE_RETRIEVAL_QUERY" => TaskType.CODERETRIEVALQUERY,
                _ => null,
            };
        }
    }
}