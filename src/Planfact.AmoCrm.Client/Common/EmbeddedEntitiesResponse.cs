using System.Text.Json.Serialization;

using Planfact.AmoCrm.Client.Leads;
using Planfact.AmoCrm.Client.Contacts;
using Planfact.AmoCrm.Client.Companies;
using Planfact.AmoCrm.Client.Customers;
using Planfact.AmoCrm.Client.Notes;
using Planfact.AmoCrm.Client.Transactions;
using Planfact.AmoCrm.Client.Users;
using Planfact.AmoCrm.Client.CustomFields;
using Planfact.AmoCrm.Client.Pipelines;
using AmoCrmTask = Planfact.AmoCrm.Client.Tasks.Task;
using AmoCrmTaskType = Planfact.AmoCrm.Client.Tasks.TaskType;

namespace Planfact.AmoCrm.Client.Common;

/// <summary>
/// Вложенные сущности из поля _embedded в ответах API amoCRM
/// </summary>
public sealed record EmbeddedEntitiesResponse
{
    /// <summary>
    /// Сделки
    /// </summary>
    [JsonPropertyName("leads")]
    public Lead[]? Leads { get; init; }

    /// <summary>
    /// Теги
    /// </summary>
    [JsonPropertyName("tags")]
    public Tag[]? Tags { get; init; }

    /// <summary>
    /// Компании
    /// </summary>
    [JsonPropertyName("companies")]
    public Company[]? Companies { get; init; }

    /// <summary>
    /// Задачи
    /// </summary>
    [JsonPropertyName("tasks")]
    public AmoCrmTask[]? Tasks { get; init; }

    /// <summary>
    /// Покупатели
    /// </summary>
    [JsonPropertyName("customers")]
    public Customer[]? Customers { get; init; }

    /// <summary>
    /// Типы задач
    /// </summary>
    [JsonPropertyName("task_types")]
    public AmoCrmTaskType[]? TaskTypes { get; init; }

    /// <summary>
    /// Пользователи
    /// </summary>
    [JsonPropertyName("users")]
    public User[]? Users { get; init; }

    /// <summary>
    /// Контакты
    /// </summary>
    [JsonPropertyName("contacts")]
    public Contact[]? Contacts { get; init; }

    /// <summary>
    /// Транзакции
    /// </summary>
    [JsonPropertyName("transactions")]
    public Transaction[]? Transactions { get; init; }

    /// <summary>
    /// Дополнительные поля
    /// </summary>
    [JsonPropertyName("custom_fields")]
    public CustomField[]? CustomFields { get; init; }

    /// <summary>
    /// Воронки сделок
    /// </summary>
    [JsonPropertyName("pipelines")]
    public Pipeline[]? Pipelines { get; init; }

    /// <summary>
    /// Примечания
    /// </summary>
    [JsonPropertyName("notes")]
    public Note[]? Notes { get; init; }
}
