
namespace Planfact.AmoCrm.Client.Exceptions;

/// <summary>
/// Исключение для ошибок HTTP-запросов к amoCRM API
/// </summary>
public sealed class AmoCrmHttpException : AmoCrmException
{
    /// <summary>
    /// HTTP-статус код ответа (если доступен)
    /// </summary>
    public int? StatusCode { get; }

    /// <summary>
    /// Создаёт исключение HTTP с сообщением и статус-кодом
    /// </summary>
    /// <param name="message">Сообщение об ошибке</param>
    /// <param name="statusCode">HTTP-статус код</param>
    public AmoCrmHttpException(string message, int? statusCode = null) : base(message)
    {
        StatusCode = statusCode;
    }

    /// <summary>
    /// Создаёт исключение HTTP с сообщением, внутренним исключением и статус-кодом
    /// </summary>
    /// <param name="message">Сообщение об ошибке</param>
    /// <param name="innerException">Внутреннее исключение</param>
    /// <param name="statusCode">HTTP-статус код</param>
    public AmoCrmHttpException(string message, Exception innerException, int? statusCode = null)
        : base(message, innerException)
    {
        StatusCode = statusCode;
    }
}
