using System.Text.Json;
using System.Text.Json.Serialization;

using Planfact.AmoCrm.Client.Exceptions;

namespace Planfact.AmoCrm.Client.Common;

/// <summary>
/// Конвертер для преобразования между перечислением <see cref="EntityType"/> и строковыми представлениями типов сущностей в amoCRM
/// </summary>
public class EntityTypeConverter : JsonConverter<EntityType>
{
    private static readonly Dictionary<string, EntityType> s_fromString = new(StringComparer.OrdinalIgnoreCase)
    {
        { "leads", EntityType.Leads },
        { "contacts", EntityType.Contacts },
        { "companies", EntityType.Companies },
        { "tasks", EntityType.Tasks },
        { "customers", EntityType.Customers }
    };

    private static readonly Dictionary<EntityType, string> s_toString = new()
    {
        { EntityType.Leads, "leads" },
        { EntityType.Contacts, "contacts" },
        { EntityType.Companies, "companies" },
        { EntityType.Tasks, "tasks" },
        { EntityType.Customers, "customers" }
    };

    /// <summary>
    /// Преобразует string в <see cref="EntityType"/> при десериализации JSON
    /// </summary>
    public override EntityType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var stringValue = reader.GetString();

        if (stringValue is null)
        {
            throw new JsonException($"Ожидалось строковое значение для типа {nameof(EntityType)}. Получено null");
        }

        if (s_fromString.TryGetValue(stringValue, out EntityType result))
        {
            return result;
        }

        throw new JsonException($"Неизвестное значение типа {nameof(EntityType)}: '{stringValue}'");
    }

    /// <summary>
    /// Преобразует <see cref="EntityType"/> в string при сериализации JSON
    /// </summary>
    public override void Write(Utf8JsonWriter writer, EntityType value, JsonSerializerOptions options)
    {
        if (s_toString.TryGetValue(value, out var stringValue))
        {
            writer.WriteStringValue(stringValue);
        }
        else
        {
            throw new JsonException($"Неподдерживаемое значение {nameof(EntityType)}: {value}.");
        }
    }

    /// <summary>
    /// Преобразует значение перечисления <see cref="EntityType"/> в строковое представление amoCRM
    /// </summary>
    /// <param name="entityType">Тип сущности, который нужно преобразовать.</param>
    /// <returns>Строковое представление типа сущности, совместимое с API amoCRM.</returns>
    /// <exception cref="AmoCrmValidationException">
    /// Выбрасывается, если передано неподдерживаемое значение <see cref="EntityType"/>.
    /// </exception>
    public static string ToString(EntityType entityType)
    {
        if (s_toString.TryGetValue(entityType, out var stringValue))
        {
            return stringValue;
        }

        throw new AmoCrmValidationException($"Неподдерживаемое значение {nameof(EntityType)}: {entityType}");
    }

    /// <summary>
    /// Преобразует строковое представление типа сущности amoCRM в значение перечисления <see cref="EntityType"/>.
    /// </summary>
    /// <param name="entityTypeString">Строковое представление типа сущности</param>
    /// <returns>Соответствующее значение перечисления <see cref="EntityType"/>.</returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="AmoCrmValidationException">
    /// Выбрасывается, если переданная строка не соответствует ни одному известному типу сущности.
    /// </exception>
    public static EntityType FromString(string entityTypeString)
    {
        ArgumentNullException.ThrowIfNull(entityTypeString);

        if (s_fromString.TryGetValue(entityTypeString, out EntityType result))
        {
            return result;
        }

        throw new AmoCrmValidationException($"Неизвестное значение типа {nameof(EntityType)}: '{entityTypeString}'");
    }
}
