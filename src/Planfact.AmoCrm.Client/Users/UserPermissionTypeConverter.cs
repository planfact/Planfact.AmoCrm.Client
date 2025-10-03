using System.Text.Json;
using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.Users;

/// <summary>
/// Конвертер для десериализации/сериализации <see cref="UserPermissionType"/> из/в JSON-строки
/// </summary>
public class UserPermissionTypeConverter : JsonConverter<UserPermissionType>
{
    private static readonly Dictionary<string, UserPermissionType> s_fromJson = new(StringComparer.OrdinalIgnoreCase)
    {
        { "A", UserPermissionType.All },
        { "G", UserPermissionType.Group },
        { "M", UserPermissionType.Mine },
        { "D", UserPermissionType.Denied }
    };

    private static readonly Dictionary<UserPermissionType, string> s_toJson = new()
    {
        { UserPermissionType.All, "A" },
        { UserPermissionType.Group, "G" },
        { UserPermissionType.Mine, "M" },
        { UserPermissionType.Denied, "D" }
    };

    /// <summary>
    /// Преобразует string в <see cref="UserPermissionType"/> при десериализации JSON
    /// </summary>
    public override UserPermissionType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var stringValue = reader.GetString();

        if (stringValue is null)
        {
            throw new JsonException($"Ожидалось строковое значение для типа {nameof(UserPermissionType)}. Получен null");
        }

        if (s_fromJson.TryGetValue(stringValue, out var result))
        {
            return result;
        }

        throw new JsonException($"Неизвестное значение типа {nameof(UserPermissionType)}: '{stringValue}'. Допустимые значения: A, G, M, D.");
    }

    /// <summary>
    /// Преобразует <see cref="UserPermissionType"/> в string при сериализации JSON
    /// </summary>
    public override void Write(Utf8JsonWriter writer, UserPermissionType value, JsonSerializerOptions options)
    {
        if (s_toJson.TryGetValue(value, out var stringValue))
        {
            writer.WriteStringValue(stringValue);
        }
        else
        {
            throw new JsonException($"Неподдерживаемое значение {nameof(UserPermissionType)}: {value}.");
        }
    }
}
