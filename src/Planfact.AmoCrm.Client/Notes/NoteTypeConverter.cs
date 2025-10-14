using System.Text.Json;
using System.Text.Json.Serialization;

using Planfact.AmoCrm.Client.Exceptions;

namespace Planfact.AmoCrm.Client.Notes;

/// <summary>
/// Конвертер для преобразования между перечислением <see cref="NoteType"/> и строковыми представлениями типов примечаний в amoCRM
/// </summary>
internal class NoteTypeConverter : JsonConverter<NoteType>
{
    private static readonly Dictionary<string, NoteType> s_fromString = new(StringComparer.OrdinalIgnoreCase)
    {
        { "common", NoteType.Common },
        { "call_in", NoteType.CallIn },
        { "call_out", NoteType.CallOut },
        { "service_message", NoteType.ServiceMessage },
        { "message_cashier", NoteType.MessageCashier },
        { "geolocation", NoteType.Geolocation },
        { "sms_in", NoteType.SmsIn },
        { "sms_out", NoteType.SmsOut },
        { "extended_service_message", NoteType.ExtendedServiceMessage },
        { "attachment", NoteType.Attachment }
    };

    private static readonly Dictionary<NoteType, string> s_toString = new()
    {
        { NoteType.Common, "common" },
        { NoteType.CallIn, "call_in" },
        { NoteType.CallOut, "call_out" },
        { NoteType.ServiceMessage, "service_message" },
        { NoteType.MessageCashier, "message_cashier" },
        { NoteType.Geolocation, "geolocation" },
        { NoteType.SmsIn, "sms_in" },
        { NoteType.SmsOut, "sms_out" },
        { NoteType.ExtendedServiceMessage, "extended_service_message" },
        { NoteType.Attachment, "attachment" },
    };

    /// <summary>
    /// Преобразует string в <see cref="NoteType"/> при десериализации JSON
    /// </summary>
    public override NoteType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var stringValue = reader.GetString();

        if (stringValue is null)
        {
            throw new JsonException($"Ожидалось строковое значение для типа {nameof(NoteType)}. Получено null");
        }

        if (s_fromString.TryGetValue(stringValue, out NoteType result))
        {
            return result;
        }

        throw new JsonException($"Неизвестное значение типа {nameof(NoteType)}: '{stringValue}'");
    }

    /// <summary>
    /// Преобразует <see cref="NoteType"/> в string при сериализации JSON
    /// </summary>
    public override void Write(Utf8JsonWriter writer, NoteType value, JsonSerializerOptions options)
    {
        if (s_toString.TryGetValue(value, out var stringValue))
        {
            writer.WriteStringValue(stringValue);
        }
        else
        {
            throw new JsonException($"Неподдерживаемое значение {nameof(NoteType)}: {value}.");
        }
    }

    /// <summary>
    /// Преобразует значение перечисления <see cref="NoteType"/> в соответствующую строку,
    /// используемую в API amoCRM (в формате snake_case).
    /// </summary>
    /// <param name="noteType">Тип примечания, который нужно преобразовать.</param>
    /// <returns>Строковое представление типа примечания, совместимое с API amoCRM.</returns>
    /// <exception cref="AmoCrmValidationException">
    /// Выбрасывается, если передано неподдерживаемое значение <see cref="NoteType"/>.
    /// </exception>
    public static string ToString(NoteType noteType)
    {
        if (s_toString.TryGetValue(noteType, out var stringValue))
        {
            return stringValue;
        }

        throw new AmoCrmValidationException($"Неподдерживаемое значение {nameof(NoteType)}: {noteType}");
    }

    /// <summary>
    /// Преобразует строковое представление типа примечания из API amoCRM обратно в значение перечисления <see cref="NoteType"/>.
    /// Ожидается строка в формате snake_case (например, "call_in").
    /// </summary>
    /// <param name="noteTypeString">Строковое значение типа примечания из API.</param>
    /// <returns>Соответствующее значение перечисления <see cref="NoteType"/>.</returns>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="AmoCrmValidationException">
    /// Выбрасывается, если строка не соответствует ни одному известному типу примечания.
    /// </exception>
    public static NoteType FromString(string noteTypeString)
    {
        ArgumentNullException.ThrowIfNull(noteTypeString);

        if (s_fromString.TryGetValue(noteTypeString, out NoteType result))
        {
            return result;
        }

        throw new AmoCrmValidationException($"Неизвестное значение типа {nameof(NoteType)}: '{noteTypeString}'");
    }
}
