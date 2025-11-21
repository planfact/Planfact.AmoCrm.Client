using Microsoft.Extensions.Options;

using Planfact.AmoCrm.Client.Common;

namespace Planfact.AmoCrm.Client;

/// <summary>
/// Класс-фабрика, предназначенный для создания экземпляров UriBuilder
/// с предзаполненными HostName и Path в зависимости от раздела amoCRM
/// </summary>
public sealed class AmoCrmUriBuilderFactory
{
    private readonly AmoCrmClientOptions _options;

    public AmoCrmUriBuilderFactory(IOptions<AmoCrmClientOptions> options)
    {
        _options = options.Value;

        AmoCrmClientOptionsValidator.Validate(_options);
    }

    /// <summary>
    /// Создает UriBuilder для запросов авторизации
    /// </summary>
    public UriBuilder CreateForAuthorization(string subdomain)
    {
        ValidateSubdomain(subdomain);

        return new UriBuilder(Uri.UriSchemeHttps, subdomain)
        {
            Path = _options.OAuthTokenPath,
        };
    }

    /// <summary>
    /// Создает UriBuilder для запросов компаний
    /// </summary>
    public UriBuilder CreateForCompanies(string subdomain)
    {
        ValidateSubdomain(subdomain);

        return new UriBuilder(Uri.UriSchemeHttps, subdomain)
        {
            Path = _options.CompaniesApiPath,
        };
    }

    /// <summary>
    /// Создает UriBuilder для запросов сделок
    /// </summary>
    public UriBuilder CreateForLeads(string subdomain)
    {
        ValidateSubdomain(subdomain);

        return new UriBuilder(Uri.UriSchemeHttps, subdomain)
        {
            Path = _options.LeadsApiPath,
        };
    }

    /// <summary>
    /// Создает UriBuilder для запросов задач
    /// </summary>
    public UriBuilder CreateForTasks(string subdomain)
    {
        ValidateSubdomain(subdomain);

        return new UriBuilder(Uri.UriSchemeHttps, subdomain)
        {
            Path = _options.TasksApiPath,
        };
    }

    /// <summary>
    /// Создает UriBuilder для запросов покупателей
    /// </summary>
    public UriBuilder CreateForCustomers(string subdomain)
    {
        ValidateSubdomain(subdomain);

        return new UriBuilder(Uri.UriSchemeHttps, subdomain)
        {
            Path = _options.CustomersApiPath,
        };
    }

    /// <summary>
    /// Создает UriBuilder для запросов данных аккаунта
    /// </summary>
    public UriBuilder CreateForAccount(string subdomain)
    {
        ValidateSubdomain(subdomain);

        return new UriBuilder(Uri.UriSchemeHttps, subdomain)
        {
            Path = _options.AccountsApiPath,
        };
    }

    /// <summary>
    /// Создает UriBuilder для запросов пользователей
    /// </summary>
    public UriBuilder CreateForUsers(string subdomain)
    {
        ValidateSubdomain(subdomain);

        return new UriBuilder(Uri.UriSchemeHttps, subdomain)
        {
            Path = _options.UsersApiPath,
        };
    }

    /// <summary>
    /// Создает UriBuilder для запросов виджета
    /// </summary>
    public UriBuilder CreateForWidget(string subdomain, string widgetCode)
    {
        ValidateSubdomain(subdomain);
        ValidateWidgetCode(widgetCode);

        return new UriBuilder(Uri.UriSchemeHttps, subdomain)
        {
            Path = $"{_options.WidgetsApiPath}/{widgetCode}",
        };
    }

    /// <summary>
    /// Создает UriBuilder для запросов контактов
    /// </summary>
    public UriBuilder CreateForContacts(string subdomain, int? contactId = null)
    {
        ValidateSubdomain(subdomain);
        ValidateEntityId(contactId);

        return new UriBuilder(Uri.UriSchemeHttps, subdomain)
        {
            Path = contactId is null
                ? _options.ContactsApiPath
                : $"{_options.ContactsApiPath}/{contactId}"
        };
    }

    /// <summary>
    /// Создает UriBuilder для запросов транзакций
    /// </summary>
    public UriBuilder CreateForTransactions(string subdomain, int? customerId = null)
    {
        ValidateSubdomain(subdomain);
        ValidateEntityId(customerId);

        return new UriBuilder(Uri.UriSchemeHttps, subdomain)
        {
            Path = customerId is null
                ? _options.TransactionsApiPath
                : $"{_options.CustomersApiPath}/{customerId}/{_options.TransactionsApiResourceName}"
        };
    }

    /// <summary>
    /// Создает UriBuilder для запросов транзакций
    /// </summary>
    public UriBuilder CreateForDeleteTransaction(string subdomain, int transactionId)
    {
        ValidateSubdomain(subdomain);
        ValidateEntityId(transactionId);

        return new UriBuilder(Uri.UriSchemeHttps, subdomain)
        {
            Path = $"{_options.TransactionsApiPath}/{transactionId}"
        };
    }

    /// <summary>
    /// Создает UriBuilder для запросов дополнительных полей
    /// </summary>
    public UriBuilder CreateForCustomFields(string subdomain, string entityType)
    {
        ValidateSubdomain(subdomain);
        ValidateEntityType(entityType);

        return new UriBuilder(Uri.UriSchemeHttps, subdomain)
        {
            Path = $"{_options.BaseApiPath}/{entityType}/{_options.CustomFieldsApiResourceName}"
        };
    }

    /// <summary>
    /// Создает UriBuilder для запросов воронок
    /// </summary>
    public UriBuilder CreateForPipelines(string subdomain)
    {
        ValidateSubdomain(subdomain);

        return new UriBuilder(Uri.UriSchemeHttps, subdomain)
        {
            Path = _options.PipelinesApiPath
        };
    }

    /// <summary>
    /// Создает UriBuilder для запросов примечаний
    /// </summary>
    public UriBuilder CreateForNotes(string subdomain, string entityType)
    {
        ValidateSubdomain(subdomain);
        ValidateEntityType(entityType);

        return new UriBuilder(Uri.UriSchemeHttps, subdomain)
        {
            Path = $"{_options.BaseApiPath}/{entityType}/{_options.NotesApiResourceName}"
        };
    }

    /// <summary>
    /// Создает UriBuilder для запросов создания связей сущностей
    /// </summary>
    public UriBuilder CreateForAddLinks(string subdomain, string entityType)
    {
        ValidateSubdomain(subdomain);
        ValidateEntityType(entityType);

        return new UriBuilder(Uri.UriSchemeHttps, subdomain)
        {
            Path = $"{_options.BaseApiPath}/{entityType}/{_options.CreateLinksActionName}"
        };
    }

    /// <summary>
    /// Создает UriBuilder для запросов удаления связей сущностей
    /// </summary>
    public UriBuilder CreateForDeleteLinks(string subdomain, string entityType)
    {
        ValidateSubdomain(subdomain);
        ValidateEntityType(entityType);

        return new UriBuilder(Uri.UriSchemeHttps, subdomain)
        {
            Path = $"{_options.BaseApiPath}/{entityType}/{_options.DeleteLinksActionName}"
        };
    }

    /// <summary>
    /// Создает UriBuilder для запросов связей сущностей
    /// </summary>
    public UriBuilder CreateForLinks(string subdomain, string entityType)
    {
        ValidateSubdomain(subdomain);
        ValidateEntityType(entityType);

        return new UriBuilder(Uri.UriSchemeHttps, subdomain)
        {
            Path = $"{_options.BaseApiPath}/{entityType}/{_options.LinksApiResourceName}"
        };
    }

    private static void ValidateSubdomain(string subdomain)
    {
        if (string.IsNullOrWhiteSpace(subdomain))
        {
            throw new ArgumentException($"Поддомен не может быть пустым или содержать только пробелы. Значение: '{subdomain}'", nameof(subdomain));
        }

        if (subdomain.Length > 253)
        {
            throw new ArgumentException($"Поддомен не может быть длиннее 253 символов. Длина переданного значения: {subdomain.Length}.", nameof(subdomain));
        }
    }

    private static void ValidateWidgetCode(string widgetCode)
    {
        if (string.IsNullOrWhiteSpace(widgetCode))
        {
            throw new ArgumentException($"Код виджета не может быть пустым или содержать только пробелы. Значение: '{widgetCode}'", nameof(widgetCode));
        }
    }

    private static void ValidateEntityType(string entityType)
    {
        if (Enum.TryParse<EntityType>(entityType, ignoreCase: true, out _))
        {
            return;
        }

        var availableTypes = string.Join(", ", Enum.GetNames<EntityType>());

        throw new ArgumentException(
            $"Неподдерживаемый тип сущности: '{entityType}'. Допустимые значения: {availableTypes}.",
            nameof(entityType)
        );
    }

    private static void ValidateEntityId(int? entityId)
    {
        if (entityId.HasValue)
        {
            ValidateEntityId(entityId.Value);
        }
    }

    private static void ValidateEntityId(int entityId)
    {
        if (entityId < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(entityId),
                $"Идентификатор сущности не может быть меньше 0. Переданное значение: {entityId}."
            );
        }
    }
}
