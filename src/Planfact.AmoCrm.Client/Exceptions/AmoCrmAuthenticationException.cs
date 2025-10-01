
namespace Planfact.AmoCrm.Client.Exceptions;

/// <summary>
/// Исключение для ошибок аутентификации в amoCRM API
/// </summary>
public sealed class AmoCrmAuthenticationException : AmoCrmException
{
    /// <summary>
    /// Создаёт исключение аутентификации с сообщением
    /// </summary>
    /// <param name="message">Сообщение об ошибке</param>
    public AmoCrmAuthenticationException(string message) : base(message) { }

    /// <summary>
    /// Создаёт исключение аутентификации с сообщением и внутренним исключением
    /// </summary>
    /// <param name="message">Сообщение об ошибке</param>
    /// <param name="innerException">Внутреннее исключение</param>
    public AmoCrmAuthenticationException(string message, Exception innerException)
        : base(message, innerException) { }
}
