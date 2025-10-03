
namespace Planfact.AmoCrm.Client;

public static class AmoCrmClientOptionsValidator
{
    /// <summary>
    /// Валидирует настройки клиента amoCRM.
    /// </summary>
    /// <param name="options">Настройки клиента</param>
    /// <exception cref="ArgumentNullException">Выбрасывается, если options равен null</exception>
    /// <exception cref="ArgumentException">Выбрасывается, если конфигурация некорректна</exception>
    public static void Validate(AmoCrmClientOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var errors = new List<string>();

        ValidateRequiredString(options.ClientId, nameof(options.ClientId), errors);
        ValidateRequiredString(options.ClientSecret, nameof(options.ClientSecret), errors);

        if (!string.IsNullOrWhiteSpace(options.WidgetCode))
        {
            ValidateRequiredString(options.WidgetApiKeyFieldName, nameof(options.WidgetApiKeyFieldName), errors);
            ValidateRequiredString(options.WidgetCallbackPath, nameof(options.WidgetCallbackPath), errors);
        }

        if (!string.IsNullOrWhiteSpace(options.ServerIntegrationSubdomain))
        {
            ValidateRequiredString(options.ServerIntegrationRedirectUri, nameof(options.ServerIntegrationRedirectUri), errors);
        }

        ValidateRequiredString(options.BaseApiPath, nameof(options.BaseApiPath), errors);

        ValidateApiPaths(
        [
            (options.LeadsApiPath, nameof(options.LeadsApiPath)),
            (options.CustomersApiPath, nameof(options.CustomersApiPath)),
            (options.AccountsApiPath, nameof(options.AccountsApiPath)),
            (options.TasksApiPath, nameof(options.TasksApiPath)),
            (options.CompaniesApiPath, nameof(options.CompaniesApiPath)),
            (options.WidgetsApiPath, nameof(options.WidgetsApiPath)),
            (options.UsersApiPath, nameof(options.UsersApiPath)),
            (options.ContactsApiPath, nameof(options.ContactsApiPath)),
            (options.TransactionsApiPath, nameof(options.TransactionsApiPath)),
            (options.PipelinesApiPath, nameof(options.PipelinesApiPath))
        ], errors);

        ValidateNonNegative(options.CacheExpiryMinutes, nameof(options.CacheExpiryMinutes), errors);
        ValidateNonNegative(options.MaxCacheSize, nameof(options.MaxCacheSize), errors);

        ValidateRequiredString(options.UserAgent, nameof(options.UserAgent), errors);
        ValidateNonNegative(options.TimeoutSeconds, nameof(options.TimeoutSeconds), errors);

        if (errors.Any())
        {
            throw new ArgumentException(
                $"Некорректная конфигурация {nameof(AmoCrmClientOptions)}:\n{string.Join('\n', errors)}",
                nameof(options));
        }
    }

    private static void ValidateRequiredString(string value, string paramName, List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            errors.Add($"{paramName} не может быть пустым.");
        }
    }

    private static void ValidateApiPaths((string Path, string Name)[] paths, ICollection<string> errors)
    {
        foreach (var (path, name) in paths)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                continue;
            }

            if (!Uri.IsWellFormedUriString(path, UriKind.Relative))
            {
                errors.Add($"Путь '{name}' имеет некорректный формат.");
            }
        }
    }

    private static void ValidateNonNegative(int value, string paramName, List<string> errors)
    {
        if (value < 0)
        {
            errors.Add($"{paramName} не может быть отрицательным.");
        }
    }
}
