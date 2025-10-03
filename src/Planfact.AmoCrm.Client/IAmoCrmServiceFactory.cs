using Reliable.HttpClient.Caching;

using Planfact.AmoCrm.Client.Account;
using Planfact.AmoCrm.Client.Authorization;
using Planfact.AmoCrm.Client.Companies;
using Planfact.AmoCrm.Client.Contacts;
using Planfact.AmoCrm.Client.Customers;
using Planfact.AmoCrm.Client.CustomFields;
using Planfact.AmoCrm.Client.Leads;
using Planfact.AmoCrm.Client.Notes;
using Planfact.AmoCrm.Client.Pipelines;
using Planfact.AmoCrm.Client.Tasks;
using Planfact.AmoCrm.Client.Transactions;
using Planfact.AmoCrm.Client.Users;

namespace Planfact.AmoCrm.Client;

/// <summary>
/// Фабрика для создания сервисов AmoCRM с кэшированным HTTP-клиентом
/// </summary>
public interface IAmoCrmServiceFactory
{
    /// <summary>
    /// Создает сервис для работы с аккаунтом
    /// </summary>
    IAmoCrmAccountService CreateAccountService(HttpClientWithCache httpClient);

    /// <summary>
    /// Создает сервис авторизации
    /// </summary>
    IAmoCrmAuthorizationService CreateAuthorizationService(HttpClientWithCache httpClient);

    /// <summary>
    /// Создает сервис для работы со сделками
    /// </summary>
    IAmoCrmLeadService CreateLeadService(HttpClientWithCache httpClient);

    /// <summary>
    /// Создает сервис для работы с компаниями
    /// </summary>
    IAmoCrmCompanyService CreateCompanyService(HttpClientWithCache httpClient);

    /// <summary>
    /// Создает сервис для работы с задачами
    /// </summary>
    IAmoCrmTaskService CreateTaskService(HttpClientWithCache httpClient);

    /// <summary>
    /// Создает сервис для работы с покупателями
    /// </summary>
    IAmoCrmCustomerService CreateCustomerService(HttpClientWithCache httpClient);

    /// <summary>
    /// Создает сервис для работы с пользователями
    /// </summary>
    IAmoCrmUserService CreateUserService(HttpClientWithCache httpClient);

    /// <summary>
    /// Создает сервис для работы с контактами
    /// </summary>
    IAmoCrmContactService CreateContactService(HttpClientWithCache httpClient);

    /// <summary>
    /// Создает сервис для работы с транзакциями
    /// </summary>
    IAmoCrmTransactionService CreateTransactionService(HttpClientWithCache httpClient);

    /// <summary>
    /// Создает сервис для работы с пользовательскими полями
    /// </summary>
    IAmoCrmCustomFieldService CreateCustomFieldService(HttpClientWithCache httpClient);

    /// <summary>
    /// Создает сервис для работы с воронками
    /// </summary>
    IAmoCrmPipelineService CreatePipelineService(HttpClientWithCache httpClient);

    /// <summary>
    /// Создает сервис для работы с примечаниями
    /// </summary>
    IAmoCrmNoteService CreateNoteService(HttpClientWithCache httpClient);
}
