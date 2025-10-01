using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.CustomFields;

/// <summary>
/// Типы пользовательских полей в amoCRM с поддержкой JSON сериализации
/// </summary>
[JsonConverter(typeof(CustomFieldTypeConverter))]
public enum CustomFieldType
{
    /// <summary>
    /// Текст
    /// </summary>
    Text,

    /// <summary>
    /// Число
    /// </summary>
    Numeric,

    /// <summary>
    /// Флаг
    /// </summary>
    Checkbox,

    /// <summary>
    /// Список
    /// </summary>
    Select,

    /// <summary>
    /// Мультисписок
    /// </summary>
    Multiselect,

    /// <summary>
    /// Дата
    /// </summary>
    Date,

    /// <summary>
    /// Ссылка
    /// </summary>
    Url,

    /// <summary>
    /// Текстовая область
    /// </summary>
    Textarea,

    /// <summary>
    /// Переключатель
    /// </summary>
    Radiobutton,

    /// <summary>
    /// Короткий адрес
    /// </summary>
    Streetaddress,

    /// <summary>
    /// Адрес
    /// </summary>
    SmartAddress,

    /// <summary>
    /// День рождения
    /// </summary>
    Birthday,

    /// <summary>
    /// Юр.лицо
    /// </summary>
    LegalEntity,

    /// <summary>
    /// Дата и время
    /// </summary>
    DateTime,

    /// <summary>
    /// Цена
    /// </summary>
    Price,

    /// <summary>
    /// Категория
    /// </summary>
    Category,

    /// <summary>
    /// Предметы
    /// </summary>
    Items,

    /// <summary>
    /// Отслеживаемые данные
    /// </summary>
    TrackingData,

    /// <summary>
    /// Связь с другим элементом
    /// </summary>
    LinkedEntity,

    /// <summary>
    /// Каталоги и списки (платная опция Супер-поля)
    /// </summary>
    ChainedList,

    /// <summary>
    /// Денежное (платная опция Супер-поля)
    /// </summary>
    Monetary,

    /// <summary>
    /// Файл
    /// </summary>
    File,

    /// <summary>
    /// Плательщик (только в списке Счета-покупки)
    /// </summary>
    Payer,

    /// <summary>
    /// Поставщик (только в списке Счета-покупки)
    /// </summary>
    Supplier
}
