using Reliable.HttpClient;

using Planfact.AmoCrm.Client.Common;

namespace Planfact.AmoCrm.Client.Users;

/// <summary>
/// Сервис пользователей в amoCRM
/// </summary>
public sealed class AmoCrmUserService(
    IHttpClientAdapter httpClient,
    AmoCrmUriBuilderFactory uriBuilderFactory,
    ILogger<AmoCrmUserService> logger) : AmoCrmServiceBase(httpClient, logger), IAmoCrmUserService
{
    private readonly AmoCrmUriBuilderFactory _uriBuilderFactory = uriBuilderFactory;
    private readonly ILogger<AmoCrmUserService> _logger = logger;

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<User>> GetUsersAsync(
        string accessToken,
        string subdomain,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Загрузка пользователей из аккаунта {Subdomain}", subdomain);

        ValidateCredentials(accessToken, subdomain);

        UriBuilder uriBuilder = _uriBuilderFactory.CreateForUsers(subdomain);

        IAsyncEnumerable<EntitiesResponse> paginationTask = GetPaginatedAsync<EntitiesResponse>(
            uriBuilder.Uri,
            accessToken,
            cancellationToken
        );

        IReadOnlyCollection<User> response = await CollectPaginatedEntitiesAsync(
            paginationTask,
            r => r.Embedded?.Users ?? [],
            subdomain,
            OperationDescriptions.GetUsers,
            cancellationToken
        ).ConfigureAwait(false);

        _logger.LogDebug("Ползователи из аккаунта {Subdomain} успешно загружены. Получено {UsersCount} пользователей", subdomain, response.Count);

        return response;
    }
}
