using System.Net;
using System.Text.Json;

using Reliable.HttpClient;

using Planfact.AmoCrm.Client.Exceptions;

namespace Planfact.AmoCrm.Client;

/// <summary>
/// Обработчик HTTP-ответов от amoCRM API
/// Выполняет десериализацию, проверку ошибок и выбрасывает исключения при необходимости
/// </summary>
/// <param name="logger">Логгер для внутреннего логирования</param>
public sealed class AmoCrmHttpResponseHandler(ILogger<AmoCrmHttpResponseHandler> logger) : IHttpResponseHandler
{
    private readonly ILogger<AmoCrmHttpResponseHandler> _logger = logger;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    /// <summary>
    /// Обрабатывает HTTP-ответ, выполняет десериализацию и выбрасывает исключения при ошибках
    /// </summary>
    /// <param name="response">HTTP-ответ от сервера</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Десериализованный объект ответа</returns>
    public async Task<TResponse> HandleAsync<TResponse>(
        HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        var responseContent = await ReadResponseContentAsync(response, cancellationToken).ConfigureAwait(false);

        // Если HTTP статус не успешный, выбрасываем исключение
        if (!response.IsSuccessStatusCode)
        {
            HandleHttpError(response, responseContent);
        }

        if (response.StatusCode is HttpStatusCode.NoContent)
        {
            return default!;
        }

        // Если содержимое пустое
        if (string.IsNullOrWhiteSpace(responseContent))
        {
            throw new AmoCrmHttpException("Получен пустой ответ от API", (int)response.StatusCode);
        }

        // Десериализуем ответ
        TResponse responseDeserialized;
        try
        {
            responseDeserialized = JsonSerializer.Deserialize<TResponse>(responseContent, _jsonOptions) ??
                           throw new AmoCrmHttpException("Получен null ответ от API", (int)response.StatusCode);
        }
        catch (JsonException ex)
        {
            throw new AmoCrmHttpException("Ошибка при обработке ответа от API", ex, (int)response.StatusCode);
        }

        return responseDeserialized;
    }

    /// <summary>
    /// Обрабатывает HTTP ошибки (статус коды)
    /// </summary>
    private static void HandleHttpError(HttpResponseMessage response, string responseContent)
    {
        var statusCode = (int)response.StatusCode;
        var errorMessage = GetStatusCodeDescription(response.StatusCode);

        if (!string.IsNullOrWhiteSpace(responseContent))
        {
            errorMessage += $" Содержимое ответа: {responseContent}";
        }

        throw response.StatusCode switch
        {
            HttpStatusCode.BadRequest => new AmoCrmValidationException(errorMessage),
            HttpStatusCode.Unauthorized => new AmoCrmAuthenticationException(errorMessage),
            HttpStatusCode.PaymentRequired => new AmoCrmHttpException(errorMessage),
            HttpStatusCode.Forbidden => new AmoCrmAuthenticationException(errorMessage),
            _ => new AmoCrmHttpException(errorMessage, statusCode)
        };
    }

    /// <summary>
    /// Считывает содержимое HTTP ответа от API amoCRM в строку
    /// </summary>
    /// <param name="response">HTTP ответ</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Содержимое ответа в виде строки. В случае ошибки считывания возвращается пустая строка</returns>
    private async Task<string> ReadResponseContentAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Не удалось прочитать содержимое HTTP ответа");
            return string.Empty;
        }
    }

    /// <summary>
    /// Gets HTTP status code description
    /// </summary>
    /// <param name="statusCode">Status code</param>
    /// <returns>Status code description</returns>
    private static string GetStatusCodeDescription(HttpStatusCode statusCode)
    {
        return statusCode switch
        {
            HttpStatusCode.BadRequest =>
                "Ошибка валидации запроса. Неверная структура массива передаваемых данных, либо неверные идентификаторы кастомных полей. Проверьте корректность заполненных значений.",
            HttpStatusCode.Unauthorized =>
                "Ошибка аутентификации. Проверьте корректность заполнения и срок действия Access Token.",
            HttpStatusCode.PaymentRequired => "Требуется оплата. Срок действия подписки закончился.",
            HttpStatusCode.Forbidden =>
                "Доступ запрещен. Возможна блокировка из-за нарушения условий использования API. Обратитесь в поддержку.",
            HttpStatusCode.TooManyRequests => "Превышено допустимое количество запросов в секунду",
            _ => $"HTTP {(int)statusCode}: {statusCode}",
        };
    }
}
