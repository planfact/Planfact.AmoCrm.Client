using System.Text.Json.Serialization;

using Planfact.AmoCrm.Client.Common;

namespace Planfact.AmoCrm.Client.CustomFields;

/// <summary>
/// Ответ API amoCRM, содержащий информацию о дополнительном поле
/// </summary>
public sealed record CustomField
{
    /// <summary>
    /// Идентификатор поля
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; init; }

    /// <summary>
    /// Название поля
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Символьный код поля, по которому можно обновлять значение в сущности, без передачи Id поля
    /// </summary>
    [JsonPropertyName("code")]
    public string? Code { get; init; }

    /// <summary>
    /// Сортировка поля
    /// </summary>
    [JsonPropertyName("sort")]
    public int? Sort { get; init; }

    /// <summary>
    /// Тип поля
    /// </summary>
    [JsonPropertyName("type")]
    public CustomFieldType Type { get; init; }

    /// <summary>
    /// Тип сущности <see cref="EntityTypeEnum"/>
    /// </summary>
    [JsonPropertyName("entity_type")]
    public string EntityTypeName { get; init; } = string.Empty;

    /// <summary>
    /// Является ли поле вычисляемым.
    /// Заполняется только при получении списка полей сделки
    /// </summary>
    [JsonPropertyName("is_computed")]
    public bool? IsComputed { get; init; }

    /// <summary>
    /// Является ли поле предустановленным.
    /// Заполняется только при получении списка полей контактов и компаний
    /// </summary>
    [JsonPropertyName("is_predefined")]
    public bool? IsPredefined { get; init; }

    /// <summary>
    /// Доступно ли поле для удаления.
    /// Заполняется только при получении списка полей контактов и компаний
    /// </summary>
    [JsonPropertyName("is_deletable")]
    public bool? IsDeletable { get; init; }

    /// <summary>
    /// Отображается ли поле в интерфейсе amoCRM.
    /// Заполняется только при получении списка полей каталогов
    /// </summary>
    [JsonPropertyName("is_visible")]
    public bool? IsVisible { get; init; }

    /// <summary>
    /// Обязательно ли поле для заполнения при создании элемента списка.
    /// Заполняется только при получении списка полей каталогов
    /// </summary>
    [JsonPropertyName("is_required")]
    public bool? IsRequired { get; init; }

    /// <summary>
    /// Тип напоминания о дне рождения (never – никогда, day – за день, week – за неделю, month – за месяц).
    /// Применимо только для типа поля <see cref="CustomFieldType.Birthday"/>.
    /// Заполняется только при получении списка полей контактов, сделок и компаний
    /// </summary>
    [JsonPropertyName("remind")]
    public string? BirthdayReminderType { get; init; }

    /// <summary>
    /// Код валюты поля. Применимо только для типа поля <see cref="CustomFieldType.Monetary"/>. Для других типов полей – null.
    /// </summary>
    [JsonPropertyName("currency")]
    public string? CurrencyCode { get; init; }

    /// <summary>
    /// Доступно ли поле для редактирования только через API.
    /// Заполняется только при получении списка полей контактов, сделок и компаний
    /// </summary>
    [JsonPropertyName("is_api_only")]
    public bool? IsApiOnly { get; init; }

    /// <summary>
    /// Идентификатор группы полей, в которой состоит данное поле.
    /// Заполняется только при получении списка полей контактов, сделок, покупателей и компаний
    /// </summary>
    [JsonPropertyName("group_id")]
    public string? GroupId { get; init; }

    /// <summary>
    /// Доступные значения для поля.
    /// Заполняется только для полей с поддержкой перечислений
    /// <see cref="CustomFieldType.Multiselect"/>
    /// <see cref="CustomFieldType.Radiobutton"/>
    /// </summary>
    [JsonPropertyName("enums")]
    public CustomFieldEnumValue[]? EnumValues { get; init; }

    /// <summary>
    /// Вложенные значения поля.
    /// Заполняется только при получении поля, содержащего список каталогов <see cref="CustomFieldType.ChainedList"/>
    /// </summary>
    [JsonPropertyName("nested")]
    public CustomFieldNestedValue[]? NestedValues { get; init; }

    /// <summary>
    /// Статусы сделок, в которых данное поле является обязательным.
    /// Заполняется только при получении списка полей контактов, сделок и компаний
    /// </summary>
    [JsonPropertyName("required_statuses")]
    public CustomFieldLeadStatus[]? RequiredInStatuses { get; init; }

    /// <summary>
    /// Статусы сделок, в которых данное поле скрыто из пользовательского интерфейса.
    /// Заполняется только при получении списка полей сделок.
    /// </summary>
    [JsonPropertyName("hidden_statuses")]
    public CustomFieldLeadStatus[]? HiddenInStatuses { get; init; }

    /// <summary>
    /// Структура элементов поля типа <see cref="CustomFieldType.ChainedList"/>.
    /// Заполняется только при получении списка полей сделок и покупателей.
    /// </summary>
    [JsonPropertyName("chained_lists")]
    public CustomFieldChainedListNode[]? ChainedListNodes { get; init; }

    /// <summary>
    /// Получить тип связанной сущности в формате перечисления
    /// </summary>
    public EntityTypeEnum? GetEntityType() => EntityTypeConverter.FromString(EntityTypeName);
}
