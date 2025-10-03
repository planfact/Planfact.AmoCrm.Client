using Planfact.AmoCrm.Client.Exceptions;

namespace Planfact.AmoCrm.Client.Common;

/// <summary>
/// Статический вспомогательный класс для конвертации между перечислением <see cref="EntityType"/>
/// и строковыми представлениями, используемыми в API amoCRM.
/// </summary>
internal static class EntityTypeConverter
{
    /// <summary>
    /// Преобразует значение перечисления <see cref="EntityType"/> в соответствующую строку,
    /// используемую в API amoCRM (в формате snake_case).
    /// </summary>
    /// <param name="entityType">Тип сущности, который нужно преобразовать.</param>
    /// <returns>Строковое представление типа сущности, совместимое с API amoCRM.</returns>
    /// <exception cref="AmoCrmValidationException">
    /// Выбрасывается, если передано неподдерживаемое значение <see cref="EntityType"/>.
    /// </exception>
    public static string ToString(EntityType entityType)
    {
        return entityType switch
        {
            EntityType.Leads => "leads",
            EntityType.Contacts => "contacts",
            EntityType.Companies => "companies",
            EntityType.Tasks => "tasks",
            EntityType.Customers => "customers",
            _ => throw new AmoCrmValidationException($"Неподдерживаемый тип сущности: {entityType}"),
        };
    }

    /// <summary>
    /// Преобразует строковое представление типа сущности из API amoCRM обратно в значение перечисления <see cref="EntityType"/>.
    /// Ожидается строка в формате snake_case (например, "leads").
    /// </summary>
    /// <param name="entityTypeString">Строковое значение типа сущности из API.</param>
    /// <returns>Соответствующее значение перечисления <see cref="EntityType"/>.</returns>
    /// <exception cref="AmoCrmValidationException">
    /// Выбрасывается, если строка не соответствует ни одному известному типу сущности.
    /// </exception>
    public static EntityType FromString(string? entityTypeString)
    {
        return entityTypeString switch
        {
            "leads" => EntityType.Leads,
            "contacts" => EntityType.Contacts,
            "companies" => EntityType.Companies,
            "tasks" => EntityType.Tasks,
            "customers" => EntityType.Customers,
            _ => throw new AmoCrmValidationException($"Неподдерживаемая строка типа сущности: {entityTypeString}"),
        };
    }
}
