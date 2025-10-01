
namespace Planfact.AmoCrm.Client.Notes;

/// <summary>
/// Типы примечаний в amoCRM. Для преобразования в строковое представление API используйте <see cref="Utils.NoteTypeConverter"/>.
/// </summary>
public enum AmoCrmNoteTypeEnum
{
    /// <summary>
    /// Текстовое примечание (common в API)
    /// </summary>
    Common = 1,

    /// <summary>
    /// Входящий звонок (call_in в API)
    /// </summary>
    CallIn = 2,

    /// <summary>
    /// Исходящий звонок (call_out в API)
    /// </summary>
    CallOut = 3,

    /// <summary>
    /// Системное сообщение (service_message в API)
    /// </summary>
    ServiceMessage = 4,

    /// <summary>
    /// Сообщение кассиру (message_cashier в API)
    /// </summary>
    MessageCashier = 5,

    /// <summary>
    /// Примечание с гео-координатами (geolocation в API)
    /// </summary>
    Geolocation = 6,

    /// <summary>
    /// Входящее SMS (sms_in в API)
    /// </summary>
    SmsIn = 7,

    /// <summary>
    /// Исходящее SMS (sms_out в API)
    /// </summary>
    SmsOut = 8,

    /// <summary>
    /// Расширенное системное сообщение (extended_service_message в API)
    /// </summary>
    ExtendedServiceMessage = 9,

    /// <summary>
    /// Примечание с файлом (attachment в API)
    /// </summary>
    Attachment = 10,
}
