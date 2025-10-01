using Planfact.AmoCrm.Client.Exceptions;

namespace Planfact.AmoCrm.Client.Notes;

/// <summary>
/// Статический вспомогательный класс для конвертации между перечислением <see cref="AmoCrmNoteTypeEnum"/> 
/// и строковыми представлениями, используемыми в API amoCRM.
/// </summary>
internal static class NoteTypeConverter
{
    /// <summary>
    /// Преобразует значение перечисления <see cref="AmoCrmNoteTypeEnum"/> в соответствующую строку,
    /// используемую в API amoCRM (в формате snake_case).
    /// </summary>
    /// <param name="noteType">Тип примечания, который нужно преобразовать.</param>
    /// <returns>Строковое представление типа примечания, совместимое с API amoCRM.</returns>
    /// <exception cref="AmoCrmValidationException">
    /// Выбрасывается, если передано неподдерживаемое значение <see cref="AmoCrmNoteTypeEnum"/>.
    /// </exception>
    public static string ToString(AmoCrmNoteTypeEnum noteType)
    {
        return noteType switch
        {
            AmoCrmNoteTypeEnum.Common => "common",
            AmoCrmNoteTypeEnum.CallIn => "call_in",
            AmoCrmNoteTypeEnum.CallOut => "call_out",
            AmoCrmNoteTypeEnum.ServiceMessage => "service_message",
            AmoCrmNoteTypeEnum.MessageCashier => "message_cashier",
            AmoCrmNoteTypeEnum.Geolocation => "geolocation",
            AmoCrmNoteTypeEnum.SmsIn => "sms_in",
            AmoCrmNoteTypeEnum.SmsOut => "sms_out",
            AmoCrmNoteTypeEnum.ExtendedServiceMessage => "extended_service_message",
            AmoCrmNoteTypeEnum.Attachment => "attachment",
            _ => throw new AmoCrmValidationException($"Неподдерживаемый тип примечания: {noteType}")
        };
    }

    /// <summary>
    /// Преобразует строковое представление типа примечания из API amoCRM обратно в значение перечисления <see cref="AmoCrmNoteTypeEnum"/>.
    /// Ожидается строка в формате snake_case (например, "call_in").
    /// </summary>
    /// <param name="noteTypeString">Строковое значение типа примечания из API.</param>
    /// <returns>Соответствующее значение перечисления <see cref="AmoCrmNoteTypeEnum"/>.</returns>
    /// <exception cref="AmoCrmValidationException">
    /// Выбрасывается, если строка не соответствует ни одному известному типу примечания.
    /// </exception>
    public static AmoCrmNoteTypeEnum FromString(string noteTypeString)
    {
        return noteTypeString switch
        {
            "common" => AmoCrmNoteTypeEnum.Common,
            "call_in" => AmoCrmNoteTypeEnum.CallIn,
            "call_out" => AmoCrmNoteTypeEnum.CallOut,
            "service_message" => AmoCrmNoteTypeEnum.ServiceMessage,
            "message_cashier" => AmoCrmNoteTypeEnum.MessageCashier,
            "geolocation" => AmoCrmNoteTypeEnum.Geolocation,
            "sms_in" => AmoCrmNoteTypeEnum.SmsIn,
            "sms_out" => AmoCrmNoteTypeEnum.SmsOut,
            "extended_service_message" => AmoCrmNoteTypeEnum.ExtendedServiceMessage,
            "attachment" => AmoCrmNoteTypeEnum.Attachment,
            _ => throw new AmoCrmValidationException($"Неподдерживаемая строка типа примечания: {noteTypeString}")
        };
    }
}
