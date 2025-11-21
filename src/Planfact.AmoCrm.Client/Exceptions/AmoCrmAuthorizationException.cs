
namespace Planfact.AmoCrm.Client.Exceptions;

/// <summary>
/// Исключение для ошибок авторизации в amoCRM API
/// </summary>
public sealed class AmoCrmAuthorizationException : AmoCrmException
{
    /// <summary>
    /// Создаёт исключение авторизации с сообщением
    /// </summary>
    /// <param name="message">Сообщение об ошибке</param>
    public AmoCrmAuthorizationException(string message) : base(message) { }

    /// <summary>
    /// Создаёт исключение авторизации с сообщением и внутренним исключением
    /// </summary>
    /// <param name="message">Сообщение об ошибке</param>
    /// <param name="innerException">Внутреннее исключение</param>
    public AmoCrmAuthorizationException(string message, Exception innerException)
        : base(message, innerException) { }
}
