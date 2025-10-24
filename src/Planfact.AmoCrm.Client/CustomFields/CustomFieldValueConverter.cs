using System.Text.Json;
using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.CustomFields;

/// <summary>
/// Конвертер для десериализации/сериализации значений дополнительных полей <see cref="CustomFieldValue"/> из/в JSON-строки
/// </summary>
public class CustomFieldValueConverter : JsonConverter<string>
{
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        JsonElement element = doc.RootElement;

        return element.ValueKind switch
        {
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number => element.GetRawText(),
            JsonValueKind.True => "true",
            JsonValueKind.False => "false",
            JsonValueKind.Null => null,
            _ => element.GetRawText()
        };
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        if (bool.TryParse(value, out var boolValue))
        {
            writer.WriteBooleanValue(boolValue);
        }
        else
        {
            writer.WriteStringValue(value);
        }
    }
}
