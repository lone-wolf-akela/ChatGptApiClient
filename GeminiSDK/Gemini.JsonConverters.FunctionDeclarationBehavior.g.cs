#nullable enable

namespace Gemini.JsonConverters
{
    /// <inheritdoc />
    public sealed class FunctionDeclarationBehaviorJsonConverter : global::System.Text.Json.Serialization.JsonConverter<global::Gemini.FunctionDeclarationBehavior>
    {
        /// <inheritdoc />
        public override global::Gemini.FunctionDeclarationBehavior Read(
            ref global::System.Text.Json.Utf8JsonReader reader,
            global::System.Type typeToConvert,
            global::System.Text.Json.JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case global::System.Text.Json.JsonTokenType.String:
                {
                    var stringValue = reader.GetString();
                    if (stringValue != null)
                    {
                        return global::Gemini.FunctionDeclarationBehaviorExtensions.ToEnum(stringValue) ?? default;
                    }
                    
                    break;
                }
                case global::System.Text.Json.JsonTokenType.Number:
                {
                    var numValue = reader.GetInt32();
                    return (global::Gemini.FunctionDeclarationBehavior)numValue;
                }
                case global::System.Text.Json.JsonTokenType.Null:
                {
                    return default(global::Gemini.FunctionDeclarationBehavior);
                }
                default:
                    throw new global::System.ArgumentOutOfRangeException(nameof(reader));
            }

            return default;
        }

        /// <inheritdoc />
        public override void Write(
            global::System.Text.Json.Utf8JsonWriter writer,
            global::Gemini.FunctionDeclarationBehavior value,
            global::System.Text.Json.JsonSerializerOptions options)
        {
            writer = writer ?? throw new global::System.ArgumentNullException(nameof(writer));

            writer.WriteStringValue(global::Gemini.FunctionDeclarationBehaviorExtensions.ToValueString(value));
        }
    }
}
