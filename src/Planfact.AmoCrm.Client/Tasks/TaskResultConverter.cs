using System.Text.Json;
using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.Tasks;

/// <summary>
/// Конвертер для десериализации/сериализации результатов выполнения задач <see cref="TaskResult"/> из/в JSON-строки
/// </summary>
public class TaskResultConverter : JsonConverter<TaskResult?>
{
    public override TaskResult? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        JsonElement root = doc.RootElement;

        if (root.ValueKind == JsonValueKind.Object)
        {
            return root.Deserialize<TaskResult>(options);
        }

        return null;
    }

    public override void Write(Utf8JsonWriter writer, TaskResult? value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, options);
    }
}
