using Microsoft.Extensions.Options;
using Reliable.HttpClient;

using Planfact.AmoCrm.Client.Common;

namespace Planfact.AmoCrm.Client.Authorization;

/// <summary>
/// Сервис авторизации в amoCRM
/// </summary>
public sealed class AmoCrmAuthorizationService(
    IHttpClientAdapter httpClient,
    AmoCrmUriBuilderFactory uriBuilderFactory,
    IOptions<AmoCrmClientOptions> options,
    ILogger<AmoCrmAuthorizationService> logger) : AmoCrmServiceBase(httpClient, logger), IAmoCrmAuthorizationService
{
    private readonly AmoCrmUriBuilderFactory _uriBuilderFactory = uriBuilderFactory;
    private readonly AmoCrmClientOptions _options = options.Value;
    private readonly ILogger<AmoCrmAuthorizationService> _logger = logger;

    /// <inheritdoc />
    public async Task<AuthorizationTokens> AuthorizeAsync(
        string subdomain,
        string authorizationCode,
        string redirectUri,
        CancellationToken cancellationToken = default)
    {
        if (!IsValidAbsoluteUri(redirectUri))
        {
            throw new ArgumentException("Некорректный формат URI", nameof(redirectUri));
        }

        _logger.LogDebug("Авторизация в аккаунте {Subdomain}", subdomain);

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
        if (!IsValidAbsoluteUri(redirectUri))
        {
            throw new ArgumentException("Некорректный формат URI", nameof(redirectUri));
        }

        _logger.LogDebug("Обновление токенов для доступа в аккаунт {Subdomain}", subdomain);

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

    private static bool IsValidAbsoluteUri(string uri) => Uri.TryCreate(uri, UriKind.Absolute, out Uri? uriResult);
}
