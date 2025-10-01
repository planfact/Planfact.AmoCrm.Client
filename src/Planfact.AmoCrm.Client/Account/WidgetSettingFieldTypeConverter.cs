using System.Text.Json;
using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.Account;

/// <summary>
/// Конвертер для десериализации/сериализации <see cref="WidgetSettingFieldType"/> из/в JSON-строки
/// </summary>
public class WidgetSettingFieldTypeConverter : JsonConverter<WidgetSettingFieldType>
{
    private static readonly Dictionary<string, WidgetSettingFieldType> s_fromJson = new(StringComparer.OrdinalIgnoreCase)
    {
        { "text", WidgetSettingFieldType.Text },
        { "pass", WidgetSettingFieldType.Password },
        { "custom", WidgetSettingFieldType.Custom },
        { "users", WidgetSettingFieldType.Users },
        { "users_lp", WidgetSettingFieldType.UsersLandingPage }
    };

    private static readonly Dictionary<WidgetSettingFieldType, string> s_toJson = new()
    {
        { WidgetSettingFieldType.Text, "text" },
        { WidgetSettingFieldType.Password, "pass" },
        { WidgetSettingFieldType.Custom, "custom" },
        { WidgetSettingFieldType.Users, "users" },
        { WidgetSettingFieldType.UsersLandingPage, "users_lp" }
    };

    /// <summary>
    /// Преобразует string в <see cref="WidgetSettingFieldType"/> при десериализации JSON
    /// </summary>
    public override WidgetSettingFieldType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var stringValue = reader.GetString();

        if (stringValue is null)
        {
            throw new JsonException($"Ожидалось строковое значение для типа {nameof(WidgetSettingFieldType)}. Получено null");
        }

        if (s_fromJson.TryGetValue(stringValue, out WidgetSettingFieldType result))
        {
            return result;
        }

        throw new JsonException($"Неизвестное значение типа {nameof(WidgetSettingFieldType)}: '{stringValue}'.");
    }

    /// <summary>
    /// Преобразует <see cref="WidgetSettingFieldType"/> в string при сериализации JSON
    /// </summary>
    public override void Write(Utf8JsonWriter writer, WidgetSettingFieldType value, JsonSerializerOptions options)
    {
        if (s_toJson.TryGetValue(value, out var stringValue))
        {
            writer.WriteStringValue(stringValue);
        }
        else
        {
            throw new JsonException($"Неподдерживаемое значение {nameof(WidgetSettingFieldType)}: {value}.");
        }
    }
}
