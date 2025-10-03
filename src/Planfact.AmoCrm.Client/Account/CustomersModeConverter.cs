using System.Text.Json;
using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.Account;

/// <summary>
/// Конвертер для десериализации/сериализации <see cref="CustomersMode"/> из/в JSON-строки
/// </summary>
public class CustomersModeConverter : JsonConverter<CustomersMode>
{
    private static readonly Dictionary<string, CustomersMode> s_fromJson = new(StringComparer.OrdinalIgnoreCase)
    {
        { "unavailable", CustomersMode.Unavailable },
        { "disabled", CustomersMode.Disabled },
        { "segments", CustomersMode.Segments },
        { "dynamic", CustomersMode.Dynamic },
        { "periodicity", CustomersMode.Periodicity }
    };

    private static readonly Dictionary<CustomersMode, string> s_toJson = new()
    {
        { CustomersMode.Unavailable, "unavailable" },
        { CustomersMode.Disabled, "disabled" },
        { CustomersMode.Segments, "segments" },
        { CustomersMode.Dynamic, "dynamic" },
        { CustomersMode.Periodicity, "periodicity" }
    };

    /// <summary>
    /// Преобразует string в <see cref="CustomersMode"/> при десериализации JSON
    /// </summary>
    public override CustomersMode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var stringValue = reader.GetString();

        if (stringValue is null)
        {
            throw new JsonException($"Ожидалось строковое значение для типа {nameof(CustomersMode)}. Получено null");
        }

        if (s_fromJson.TryGetValue(stringValue, out CustomersMode result))
        {
            return result;
        }

        throw new JsonException($"Неизвестное значение типа {nameof(CustomersMode)}: '{stringValue}'.");
    }

    /// <summary>
    /// Преобразует <see cref="CustomersMode"/> в string при сериализации JSON
    /// </summary>
    public override void Write(Utf8JsonWriter writer, CustomersMode value, JsonSerializerOptions options)
    {
        if (s_toJson.TryGetValue(value, out var stringValue))
        {
            writer.WriteStringValue(stringValue);
        }
        else
        {
            throw new JsonException($"Неподдерживаемое значение {nameof(CustomersMode)}: {value}.");
        }
    }
}
