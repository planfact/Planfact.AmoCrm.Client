using Microsoft.Extensions.Options;
using Planfact.AmoCrm.Client.Common;
using Reliable.HttpClient;

namespace Planfact.AmoCrm.Client.Authorization;

/// <summary>
/// Сервис авторизации в amoCRM
/// </summary>
public sealed class AmoCrmAuthorizationService : AmoCrmServiceBase, IAmoCrmAuthorizationService
{
    private readonly AmoCrmUriBuilderFactory _uriBuilderFactory;
    private readonly ILogger<AmoCrmAuthorizationService> _logger;
    private readonly AmoCrmClientOptions _options;

    public AmoCrmAuthorizationService(
        IHttpClientAdapter httpClient,
        AmoCrmUriBuilderFactory uriBuilderFactory,
        IOptions<AmoCrmClientOptions> options,
        ILogger<AmoCrmAuthorizationService> logger) : base(httpClient, logger)
    {
        _uriBuilderFactory = uriBuilderFactory;
        _logger = logger;
        _options = options.Value;

        AmoCrmClientOptionsValidator.Validate(_options);
    }

    /// <inheritdoc />
    public async Task<AuthorizationTokens> AuthorizeAsync(
        string subdomain,
        string authorizationCode,
        string redirectUri,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Авторизация в аккаунте {Subdomain}", subdomain);

        ValidateAuthorizationCredentials(subdomain, authorizationCode, redirectUri);

        UriBuilder uriBuilder = _uriBuilderFactory.CreateForAuthorization(subdomain);

        var request = new AuthorizationRequest()
        {
            ClientId = _options.ClientId,
            ClientSecret = _options.ClientSecret,
            Code = authorizationCode,
            RedirectUri = redirectUri
        };

        AuthorizationTokens response = await HttpClient.PostAsync<AuthorizationRequest, AuthorizationTokens>(
            uriBuilder.Uri.ToString(),
            request,
            cancellationToken
        ).ConfigureAwait(false);

        _logger.LogDebug("Авторизация в аккаунте {Subdomain} успешно завершена", subdomain);

        return response;
    }

    /// <inheritdoc />
    public async Task<AuthorizationTokens> RefreshTokenAsync(
        string subdomain,
        string refreshToken,
        string redirectUri,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Обновление токенов для доступа в аккаунт {Subdomain}", subdomain);

        ValidateRefreshTokenCredentials(subdomain, refreshToken, redirectUri);

        UriBuilder uriBuilder = _uriBuilderFactory.CreateForAuthorization(subdomain);

        var request = new RefreshTokenRequest()
        {
            ClientId = _options.ClientId,
            ClientSecret = _options.ClientSecret,
            RefreshToken = refreshToken,
            RedirectUri = redirectUri
        };

        AuthorizationTokens response = await HttpClient.PostAsync<RefreshTokenRequest, AuthorizationTokens>(
            uriBuilder.Uri.ToString(),
            request,
            cancellationToken
        ).ConfigureAwait(false);

        _logger.LogDebug("Обновление токенов для доступа в аккаунт {Subdomain} успешно завершено", subdomain);

        return response;
    }

    /// <summary>
    /// Валидирует параметры, необходимые для первичного получения доступа к amoCRM
    /// </summary>
    /// <param name="subdomain">Поддомен amoCRM</param>
    /// <param name="authorizationCode">Одноразовый код доступа</param>
    /// <param name="redirectUri">URI для перенаправления, заданный в настройках интеграции</param>
    private static void ValidateAuthorizationCredentials(string subdomain, string authorizationCode, string redirectUri)
    {
        if (string.IsNullOrWhiteSpace(subdomain))
        {
            throw new ArgumentException("Не задан поддомен amoCRM", nameof(subdomain));
        }

        if (string.IsNullOrWhiteSpace(authorizationCode))
        {
            throw new ArgumentException("Не задан код доступа", nameof(authorizationCode));
        }

        if (!IsValidAbsoluteUri(redirectUri))
        {
            throw new ArgumentException("Некорректный формат URI", nameof(redirectUri));
        }
    }

    /// <summary>
    /// Валидирует параметры, необходимые для обновления авторизационных данных
    /// </summary>
    /// <param name="subdomain">Поддомен amoCRM</param>
    /// <param name="refreshToken">Токен обновления авторизационных данных по протоколу OAuth 2.0</param>
    /// <param name="redirectUri">URI для перенаправления, заданный в настройках интеграции</param>
    private static void ValidateRefreshTokenCredentials(
        string subdomain,
        string refreshToken,
        string redirectUri)
    {
        if (string.IsNullOrWhiteSpace(subdomain))
        {
            throw new ArgumentException("Не задан поддомен amoCRM", nameof(subdomain));
        }

        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new ArgumentException("Не задан Refresh Token", nameof(refreshToken));
        }

        if (!IsValidAbsoluteUri(redirectUri))
        {
            throw new ArgumentException("Некорректный формат URI", nameof(redirectUri));
        }
    }

    private static bool IsValidAbsoluteUri(string uri) => Uri.TryCreate(uri, UriKind.Absolute, out Uri? uriResult);
}
