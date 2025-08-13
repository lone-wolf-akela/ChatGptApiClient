#nullable enable

namespace Gemini.JsonConverters
{
    /// <inheritdoc />
    public sealed class GenerationConfigMediaResolutionNullableJsonConverter : global::System.Text.Json.Serialization.JsonConverter<global::Gemini.GenerationConfigMediaResolution?>
    {
        /// <inheritdoc />
        public override global::Gemini.GenerationConfigMediaResolution? Read(
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
                        return global::Gemini.GenerationConfigMediaResolutionExtensions.ToEnum(stringValue);
                    }
                    
                    break;
                }
                case global::System.Text.Json.JsonTokenType.Number:
                {
                    var numValue = reader.GetInt32();
                    return (global::Gemini.GenerationConfigMediaResolution)numValue;
                }
                case global::System.Text.Json.JsonTokenType.Null:
                {
                    return default(global::Gemini.GenerationConfigMediaResolution?);
                }
                default:
                    throw new global::System.ArgumentOutOfRangeException(nameof(reader));
            }

            return default;
        }

        /// <inheritdoc />
        public override void Write(
            global::System.Text.Json.Utf8JsonWriter writer,
            global::Gemini.GenerationConfigMediaResolution? value,
            global::System.Text.Json.JsonSerializerOptions options)
        {
            writer = writer ?? throw new global::System.ArgumentNullException(nameof(writer));

            if (value == null)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteStringValue(global::Gemini.GenerationConfigMediaResolutionExtensions.ToValueString(value.Value));
            }
        }
    }
}
