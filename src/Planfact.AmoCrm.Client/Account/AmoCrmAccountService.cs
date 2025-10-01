using Microsoft.Extensions.Logging;
using Reliable.HttpClient;

using Planfact.AmoCrm.Client.Common;

namespace Planfact.AmoCrm.Client.Account;

/// <summary>
/// Сервис для работы с аккаунтом и виджетами amoCRM
/// </summary>
public sealed class AmoCrmAccountService(
    IHttpClientAdapter httpClient,
    AmoCrmUriBuilderFactory uriBuilderFactory,
    ILogger<AmoCrmAccountService> logger) : AmoCrmServiceBase(httpClient, logger), IAmoCrmAccountService
{
    private readonly AmoCrmUriBuilderFactory _uriBuilderFactory = uriBuilderFactory;
    private readonly ILogger<AmoCrmAccountService> _logger = logger;

    /// <inheritdoc />
    public async Task<AccountResponse> GetAccountAsync(
        string accessToken,
        string subdomain,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Получение информации об аккаунте {Subdomain}", subdomain);

        UriBuilder uriBuilder = _uriBuilderFactory.CreateForAccount(subdomain);
        IDictionary<string, string> headers = GetDefaultHeaders(accessToken);

        AccountResponse response = await HttpClient.GetAsync<AccountResponse>(
            uriBuilder.Uri,
            headers,
            cancellationToken
        ).ConfigureAwait(false);

        _logger.LogDebug("Успешно получена информация об аккаунте {AccountName} (ID: {AccountId})",
            response.Name, response.Id);

        return response;
    }

    /// <inheritdoc />
    public async Task<WidgetResponse> GetWidgetAsync(
        string accessToken,
        string subdomain,
        string widgetCode,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Получение информации о виджете {WidgetCode} для аккаунта {Subdomain}", widgetCode, subdomain);

        UriBuilder uriBuilder = _uriBuilderFactory.CreateForWidget(subdomain, widgetCode);
        IDictionary<string, string> headers = GetDefaultHeaders(accessToken);

        WidgetResponse response = await HttpClient.GetAsync<WidgetResponse>(
            uriBuilder.Uri,
            headers,
            cancellationToken
        ).ConfigureAwait(false);

        _logger.LogDebug("Успешно получена информация о виджете {WidgetCode}", widgetCode);

        return response;
    }
}
