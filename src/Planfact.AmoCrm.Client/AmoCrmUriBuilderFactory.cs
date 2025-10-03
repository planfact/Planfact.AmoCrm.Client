using Microsoft.Extensions.Options;

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
        return new UriBuilder(Uri.UriSchemeHttps, subdomain)
        {
            Path = customerId is null
                ? _options.TransactionsApiPath
                : $"{_options.CustomersApiPath}/{customerId}/{_options.TransactionsApiResourceName}"
        };
    }

    public UriBuilder CreateForCustomFields(string subdomain, string entityType)
    {
        return new UriBuilder(Uri.UriSchemeHttps, subdomain)
        {
            Path = $"{_options.BaseApiPath}/{entityType}/{_options.CustomFieldsApiResourceName}"
        };
    }

    public UriBuilder CreateForPipelines(string subdomain)
    {
        return new UriBuilder(Uri.UriSchemeHttps, subdomain)
        {
            Path = _options.PipelinesApiPath
        };
    }

    public UriBuilder CreateForNotes(string subdomain, string entityType)
    {
        return new UriBuilder(Uri.UriSchemeHttps, subdomain)
        {
            Path = $"{_options.BaseApiPath}/{entityType}/{_options.NotesApiResourceName}"
        };
    }
}
