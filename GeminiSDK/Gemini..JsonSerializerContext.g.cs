
#nullable enable

#pragma warning disable CS0618 // Type or member is obsolete
#pragma warning disable CS3016 // Arrays as attribute arguments is not CLS-compliant

namespace Gemini
{
    /// <summary>
    /// 
    /// </summary>
    [global::System.Text.Json.Serialization.JsonSourceGenerationOptions(
        DefaultIgnoreCondition = global::System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        Converters = new global::System.Type[] 
        { 
            typeof(global::Gemini.JsonConverters.FunctionResponseSchedulingJsonConverter),
            typeof(global::Gemini.JsonConverters.FunctionResponseSchedulingNullableJsonConverter),
            typeof(global::Gemini.JsonConverters.ExecutableCodeLanguageJsonConverter),
            typeof(global::Gemini.JsonConverters.ExecutableCodeLanguageNullableJsonConverter),
            typeof(global::Gemini.JsonConverters.CodeExecutionResultOutcomeJsonConverter),
            typeof(global::Gemini.JsonConverters.CodeExecutionResultOutcomeNullableJsonConverter),
            typeof(global::Gemini.JsonConverters.TypeJsonConverter),
            typeof(global::Gemini.JsonConverters.TypeNullableJsonConverter),
            typeof(global::Gemini.JsonConverters.FunctionDeclarationBehaviorJsonConverter),
            typeof(global::Gemini.JsonConverters.FunctionDeclarationBehaviorNullableJsonConverter),
            typeof(global::Gemini.JsonConverters.DynamicRetrievalConfigModeJsonConverter),
            typeof(global::Gemini.JsonConverters.DynamicRetrievalConfigModeNullableJsonConverter),
            typeof(global::Gemini.JsonConverters.FunctionCallingConfigModeJsonConverter),
            typeof(global::Gemini.JsonConverters.FunctionCallingConfigModeNullableJsonConverter),
            typeof(global::Gemini.JsonConverters.HarmCategoryJsonConverter),
            typeof(global::Gemini.JsonConverters.HarmCategoryNullableJsonConverter),
            typeof(global::Gemini.JsonConverters.SafetySettingThresholdJsonConverter),
            typeof(global::Gemini.JsonConverters.SafetySettingThresholdNullableJsonConverter),
            typeof(global::Gemini.JsonConverters.GenerationConfigResponseModalitieJsonConverter),
            typeof(global::Gemini.JsonConverters.GenerationConfigResponseModalitieNullableJsonConverter),
            typeof(global::Gemini.JsonConverters.GenerationConfigMediaResolutionJsonConverter),
            typeof(global::Gemini.JsonConverters.GenerationConfigMediaResolutionNullableJsonConverter),
            typeof(global::Gemini.JsonConverters.CandidateFinishReasonJsonConverter),
            typeof(global::Gemini.JsonConverters.CandidateFinishReasonNullableJsonConverter),
            typeof(global::Gemini.JsonConverters.SafetyRatingProbabilityJsonConverter),
            typeof(global::Gemini.JsonConverters.SafetyRatingProbabilityNullableJsonConverter),
            typeof(global::Gemini.JsonConverters.UrlMetadataUrlRetrievalStatusJsonConverter),
            typeof(global::Gemini.JsonConverters.UrlMetadataUrlRetrievalStatusNullableJsonConverter),
            typeof(global::Gemini.JsonConverters.PromptFeedbackBlockReasonJsonConverter),
            typeof(global::Gemini.JsonConverters.PromptFeedbackBlockReasonNullableJsonConverter),
            typeof(global::Gemini.JsonConverters.ModalityJsonConverter),
            typeof(global::Gemini.JsonConverters.ModalityNullableJsonConverter),
            typeof(global::Gemini.JsonConverters.ConditionOperationJsonConverter),
            typeof(global::Gemini.JsonConverters.ConditionOperationNullableJsonConverter),
            typeof(global::Gemini.JsonConverters.GenerateAnswerRequestAnswerStyleJsonConverter),
            typeof(global::Gemini.JsonConverters.GenerateAnswerRequestAnswerStyleNullableJsonConverter),
            typeof(global::Gemini.JsonConverters.InputFeedbackBlockReasonJsonConverter),
            typeof(global::Gemini.JsonConverters.InputFeedbackBlockReasonNullableJsonConverter),
            typeof(global::Gemini.JsonConverters.TaskTypeJsonConverter),
            typeof(global::Gemini.JsonConverters.TaskTypeNullableJsonConverter),
            typeof(global::Gemini.JsonConverters.BatchStateJsonConverter),
            typeof(global::Gemini.JsonConverters.BatchStateNullableJsonConverter),
            typeof(global::Gemini.JsonConverters.ContentFilterReasonJsonConverter),
            typeof(global::Gemini.JsonConverters.ContentFilterReasonNullableJsonConverter),
            typeof(global::Gemini.JsonConverters.FileStateJsonConverter),
            typeof(global::Gemini.JsonConverters.FileStateNullableJsonConverter),
            typeof(global::Gemini.JsonConverters.FileSourceJsonConverter),
            typeof(global::Gemini.JsonConverters.FileSourceNullableJsonConverter),
            typeof(global::Gemini.JsonConverters.GeneratedFileStateJsonConverter),
            typeof(global::Gemini.JsonConverters.GeneratedFileStateNullableJsonConverter),
            typeof(global::Gemini.JsonConverters.TunedModelStateJsonConverter),
            typeof(global::Gemini.JsonConverters.TunedModelStateNullableJsonConverter),
            typeof(global::Gemini.JsonConverters.PermissionGranteeTypeJsonConverter),
            typeof(global::Gemini.JsonConverters.PermissionGranteeTypeNullableJsonConverter),
            typeof(global::Gemini.JsonConverters.PermissionRoleJsonConverter),
            typeof(global::Gemini.JsonConverters.PermissionRoleNullableJsonConverter),
            typeof(global::Gemini.JsonConverters.ChunkStateJsonConverter),
            typeof(global::Gemini.JsonConverters.ChunkStateNullableJsonConverter),
            typeof(global::Gemini.JsonConverters.OperationJsonConverter),
            typeof(global::Gemini.JsonConverters.BatchGenerateContentOperationJsonConverter),
            typeof(global::Gemini.JsonConverters.AsyncBatchEmbedContentOperationJsonConverter),
            typeof(global::Gemini.JsonConverters.CreateTunedModelOperationJsonConverter),
            typeof(global::Gemini.JsonConverters.PredictLongRunningOperationJsonConverter),
            typeof(global::Gemini.JsonConverters.UnixTimestampJsonConverter),
        })]

    [global::System.Text.Json.Serialization.JsonSerializable(typeof(global::Gemini.JsonSerializerContextTypes))]
    public sealed partial class SourceGenerationContext : global::System.Text.Json.Serialization.JsonSerializerContext
    {
    }
}