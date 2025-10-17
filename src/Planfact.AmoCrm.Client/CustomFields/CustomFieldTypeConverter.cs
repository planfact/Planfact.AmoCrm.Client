using System.Text.Json;
using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.CustomFields;

/// <summary>
/// Конвертер для десериализации/сериализации <see cref="CustomFieldType"/> из/в JSON-строки
/// </summary>
public class CustomFieldTypeConverter : JsonConverter<CustomFieldType>
{
    private static readonly Dictionary<string, CustomFieldType> s_fromJson = new(StringComparer.OrdinalIgnoreCase)
    {
        { "text", CustomFieldType.Text },
        { "numeric", CustomFieldType.Numeric },
        { "checkbox", CustomFieldType.Checkbox },
        { "select", CustomFieldType.Select },
        { "multiselect", CustomFieldType.Multiselect },
        { "date", CustomFieldType.Date },
        { "url", CustomFieldType.Url },
        { "textarea", CustomFieldType.Textarea },
        { "radiobutton", CustomFieldType.Radiobutton },
        { "streetaddress", CustomFieldType.Streetaddress },
        { "smart_address", CustomFieldType.SmartAddress },
        { "birthday", CustomFieldType.Birthday },
        { "legal_entity", CustomFieldType.LegalEntity },
        { "date_time", CustomFieldType.DateTime },
        { "price", CustomFieldType.Price },
        { "category", CustomFieldType.Category },
        { "items", CustomFieldType.Items },
        { "tracking_data", CustomFieldType.TrackingData },
        { "linked_entity", CustomFieldType.LinkedEntity },
        { "chained_list", CustomFieldType.ChainedList },
        { "monetary", CustomFieldType.Monetary },
        { "file", CustomFieldType.File },
        { "payer", CustomFieldType.Payer },
        { "supplier", CustomFieldType.Supplier },
        { "multitext", CustomFieldType.Multitext }
    };

    private static readonly Dictionary<CustomFieldType, string> s_toJson = new()
    {
        { CustomFieldType.Text, "text" },
        { CustomFieldType.Numeric, "numeric" },
        { CustomFieldType.Checkbox, "checkbox" },
        { CustomFieldType.Select, "select" },
        { CustomFieldType.Multiselect, "multiselect" },
        { CustomFieldType.Date, "date" },
        { CustomFieldType.Url, "url" },
        { CustomFieldType.Textarea, "textarea" },
        { CustomFieldType.Radiobutton, "radiobutton" },
        { CustomFieldType.Streetaddress, "streetaddress" },
        { CustomFieldType.SmartAddress, "smart_address" },
        { CustomFieldType.Birthday, "birthday" },
        { CustomFieldType.LegalEntity, "legal_entity" },
        { CustomFieldType.DateTime, "date_time" },
        { CustomFieldType.Price, "price" },
        { CustomFieldType.Category, "category" },
        { CustomFieldType.Items, "items" },
        { CustomFieldType.TrackingData, "tracking_data" },
        { CustomFieldType.LinkedEntity, "linked_entity" },
        { CustomFieldType.ChainedList, "chained_list" },
        { CustomFieldType.Monetary, "monetary" },
        { CustomFieldType.File, "file" },
        { CustomFieldType.Payer, "payer" },
        { CustomFieldType.Supplier, "supplier" },
        { CustomFieldType.Multitext, "multitext" }
    };

    /// <summary>
    /// Преобразует string в <see cref="CustomFieldType"/> при десериализации JSON
    /// </summary>
    public override CustomFieldType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var stringValue = reader.GetString();

        if (stringValue is null)
        {
            throw new JsonException($"Ожидалось строковое значение для типа {nameof(CustomFieldType)}. Получено null");
        }

        if (s_fromJson.TryGetValue(stringValue, out CustomFieldType result))
        {
            return result;
        }

        throw new JsonException($"Неизвестное значение типа {nameof(CustomFieldType)}: '{stringValue}'.");
    }

    /// <summary>
    /// Преобразует <see cref="CustomFieldType"/> в string при сериализации JSON
    /// </summary>
    public override void Write(Utf8JsonWriter writer, CustomFieldType value, JsonSerializerOptions options)
    {
        if (s_toJson.TryGetValue(value, out var stringValue))
        {
            writer.WriteStringValue(stringValue);
        }
        else
        {
            throw new JsonException($"Неподдерживаемое значение {nameof(CustomFieldType)}: {value}.");
        }
    }
}
