using System.Text.Json.Serialization;

using Planfact.AmoCrm.Client.Common;

namespace Planfact.AmoCrm.Client.Account;

/// <summary>
/// Ответ с информацией об аккаунте
/// </summary>
public sealed record AccountResponse : BaseResponse
{
    /// <summary>
    /// Идентификатор аккаунта
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; init; }

    /// <summary>
    /// Название аккаунта
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Поддомен аккаунта
    /// </summary>
    [JsonPropertyName("subdomain")]
    public string Subdomain { get; init; } = string.Empty;

    /// <summary>
    /// Идентификатор текущего пользователя
    /// </summary>
    [JsonPropertyName("current_user_id")]
    public int CurrentUserId { get; init; }

    /// <summary>
    /// Страна, указанная в настройках аккаунта
    /// </summary>
    [JsonPropertyName("country")]
    public string Country { get; init; } = string.Empty;

    /// <summary>
    /// Режим покупателей в amoCRM
    /// </summary>
    [JsonPropertyName("customers_mode")]
    public CustomersMode CustomersMode { get; init; }

    /// <summary>
    /// Включена ли функциональность отслеживания причин отказа
    /// </summary>
    [JsonPropertyName("is_loss_reason_enabled")]
    public bool IsLossReasonTrackingEnabled { get; init; }

    /// <summary>
    /// Включена ли функциональность "Неразобранного" в аккаунте
    /// </summary>
    [JsonPropertyName("is_unsorted_on")]
    public bool IsUnsortedEnabled { get; init; }

    /// <summary>
    /// Включена ли функциональность бота-помощника (доступна только на профессиональном тарифе)
    /// </summary>
    [JsonPropertyName("is_helpbot_enabled")]
    public bool IsHelperBotEnabled { get; init; }

    /// <summary>
    /// Является ли данный аккаунт техническим
    /// </summary>
    [JsonPropertyName("is_technical_account")]
    public bool IsTechnicalAccount { get; init; }
}
