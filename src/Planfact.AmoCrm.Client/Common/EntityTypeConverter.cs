using Planfact.AmoCrm.Client.Exceptions;

namespace Planfact.AmoCrm.Client.Common;

/// <summary>
/// Статический вспомогательный класс для конвертации между перечислением <see cref="EntityTypeEnum"/>
/// и строковыми представлениями, используемыми в API amoCRM.
/// </summary>
internal static class EntityTypeConverter
{
    /// <summary>
    /// Преобразует значение перечисления <see cref="EntityTypeEnum"/> в соответствующую строку,
    /// используемую в API amoCRM (в формате snake_case).
    /// </summary>
    /// <param name="entityType">Тип сущности, который нужно преобразовать.</param>
    /// <returns>Строковое представление типа сущности, совместимое с API amoCRM.</returns>
    /// <exception cref="AmoCrmValidationException">
    /// Выбрасывается, если передано неподдерживаемое значение <see cref="EntityTypeEnum"/>.
    /// </exception>
    public static string ToString(EntityTypeEnum entityType)
    {
        return entityType switch
        {
            EntityTypeEnum.Leads => "leads",
            EntityTypeEnum.Contacts => "contacts",
            EntityTypeEnum.Companies => "companies",
            EntityTypeEnum.Tasks => "tasks",
            EntityTypeEnum.Customers => "customers",
            _ => throw new AmoCrmValidationException($"Неподдерживаемый тип сущности: {entityType}"),
        };
    }

    /// <summary>
    /// Преобразует строковое представление типа сущности из API amoCRM обратно в значение перечисления <see cref="EntityTypeEnum"/>.
    /// Ожидается строка в формате snake_case (например, "leads").
    /// </summary>
    /// <param name="entityTypeString">Строковое значение типа сущности из API.</param>
    /// <returns>Соответствующее значение перечисления <see cref="EntityTypeEnum"/>.</returns>
    /// <exception cref="AmoCrmValidationException">
    /// Выбрасывается, если строка не соответствует ни одному известному типу сущности.
    /// </exception>
    public static EntityTypeEnum? FromString(string? entityTypeString)
    {
        return entityTypeString switch
        {
            "leads" => EntityTypeEnum.Leads,
            "contacts" => EntityTypeEnum.Contacts,
            "companies" => EntityTypeEnum.Companies,
            "tasks" => EntityTypeEnum.Tasks,
            "customers" => EntityTypeEnum.Customers,
            _ => throw new AmoCrmValidationException($"Неподдерживаемая строка типа сущности: {entityTypeString}"),
        };
    }
}
