
namespace Planfact.AmoCrm.Client.Exceptions;

/// <summary>
/// Исключение для ошибок валидации запроса к amoCRM API
/// </summary>
public sealed class AmoCrmValidationException(string message) : AmoCrmException(message) { }
