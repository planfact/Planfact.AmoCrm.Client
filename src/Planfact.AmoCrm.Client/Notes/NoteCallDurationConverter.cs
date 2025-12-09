using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.Notes;

/// <summary>
/// Конвертер для десериализации/сериализации длительности звонков в примечаниях <see cref="NoteDetails.Duration"/> из/в JSON-строки
/// </summary>
internal class NoteCallDurationConverter : JsonConverter<int?>
{
    public override int? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        JsonElement root = doc.RootElement;

        if (root.ValueKind == JsonValueKind.String)
        {
            var deserialized = root.Deserialize<string?>(options);

            if (string.IsNullOrWhiteSpace(deserialized))
            {
                return null;
            }

            return int.TryParse(deserialized, NumberStyles.Integer, CultureInfo.InvariantCulture, out var duration)
                ? duration
                : null;
        }

        return root.Deserialize<int?>(options);
    }

    public override void Write(Utf8JsonWriter writer, int? value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, options);
    }
}
