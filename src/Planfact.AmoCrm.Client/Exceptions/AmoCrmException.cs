
namespace Planfact.AmoCrm.Client.Exceptions;

/// <summary>
/// Базовый тип исключения для ошибок, связанных с amoCRM API
/// </summary>
public abstract class AmoCrmException : Exception
{
    /// <summary>
    /// Создаёт исключение с сообщением об ошибке
    /// </summary>
    /// <param name="message">Сообщение об ошибке</param>
    protected AmoCrmException(string message) : base(message) { }

    /// <summary>
    /// Создаёт исключение с сообщением и внутренним исключением
    /// </summary>
    /// <param name="message">Сообщение об ошибке</param>
    /// <param name="innerException">Внутреннее исключение</param>
    protected AmoCrmException(string message, Exception innerException) : base(message, innerException) { }
}
