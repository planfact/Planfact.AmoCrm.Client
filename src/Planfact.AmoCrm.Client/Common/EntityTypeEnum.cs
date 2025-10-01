
namespace Planfact.AmoCrm.Client.Common;

/// <summary>
/// Перечисление типов сущностей в amoCRM API.
/// Для преобразования в строковые представления используйте <see cref="EntityTypeConverter"/>.
/// </summary>
public enum EntityTypeEnum
{
    /// <summary>
    /// Сделки (leads в API)
    /// </summary>
    Leads = 1,

    /// <summary>
    /// Контакты (contacts в API)
    /// </summary>
    Contacts = 2,

    /// <summary>
    /// Компании (companies в API)
    /// </summary>
    Companies = 3,

    /// <summary>
    /// Задачи (tasks в API)
    /// </summary>
    Tasks = 4,

    /// <summary>
    /// Покупатели (customers в API)
    /// </summary>
    Customers = 5,
}
