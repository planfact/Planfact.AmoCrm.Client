using Reliable.HttpClient.Caching;

using Planfact.AmoCrm.Client.Account;
using Planfact.AmoCrm.Client.Authorization;
using Planfact.AmoCrm.Client.Companies;
using Planfact.AmoCrm.Client.Contacts;
using Planfact.AmoCrm.Client.Customers;
using Planfact.AmoCrm.Client.CustomFields;
using Planfact.AmoCrm.Client.Leads;
using Planfact.AmoCrm.Client.Links;
using Planfact.AmoCrm.Client.Notes;
using Planfact.AmoCrm.Client.Pipelines;
using Planfact.AmoCrm.Client.Tasks;
using Planfact.AmoCrm.Client.Transactions;
using Planfact.AmoCrm.Client.Users;

namespace Planfact.AmoCrm.Client;

/// <summary>
/// Реализация фабрики сервисов AmoCRM с использованием отдельных фабрик
/// </summary>
public class AmoCrmServiceFactory(
    Func<HttpClientWithCache, IAmoCrmAccountService> accountServiceFactory,
    Func<HttpClientWithCache, IAmoCrmAuthorizationService> authorizationServiceFactory,
    Func<HttpClientWithCache, IAmoCrmLeadService> leadServiceFactory,
    Func<HttpClientWithCache, IAmoCrmCompanyService> companyServiceFactory,
    Func<HttpClientWithCache, IAmoCrmTaskService> taskServiceFactory,
    Func<HttpClientWithCache, IAmoCrmCustomerService> customerServiceFactory,
    Func<HttpClientWithCache, IAmoCrmUserService> userServiceFactory,
    Func<HttpClientWithCache, IAmoCrmContactService> contactServiceFactory,
    Func<HttpClientWithCache, IAmoCrmTransactionService> transactionServiceFactory,
    Func<HttpClientWithCache, IAmoCrmCustomFieldService> customFieldServiceFactory,
    Func<HttpClientWithCache, IAmoCrmPipelineService> pipelineServiceFactory,
    Func<HttpClientWithCache, IAmoCrmNoteService> noteServiceFactory,
    Func<HttpClientWithCache, IAmoCrmLinkService> linkServiceFactory) : IAmoCrmServiceFactory
{
    /// <inheritdoc />
    public IAmoCrmAccountService CreateAccountService(HttpClientWithCache httpClient) =>
accountServiceFactory(httpClient);

    /// <inheritdoc />
    public IAmoCrmAuthorizationService CreateAuthorizationService(HttpClientWithCache httpClient) =>
authorizationServiceFactory(httpClient);

    /// <inheritdoc />
    public IAmoCrmLeadService CreateLeadService(HttpClientWithCache httpClient) =>
leadServiceFactory(httpClient);

    /// <inheritdoc />
    public IAmoCrmCompanyService CreateCompanyService(HttpClientWithCache httpClient) =>
companyServiceFactory(httpClient);

    /// <inheritdoc />
    public IAmoCrmTaskService CreateTaskService(HttpClientWithCache httpClient) =>
taskServiceFactory(httpClient);

    /// <inheritdoc />
    public IAmoCrmCustomerService CreateCustomerService(HttpClientWithCache httpClient) =>
customerServiceFactory(httpClient);

    /// <inheritdoc />
    public IAmoCrmUserService CreateUserService(HttpClientWithCache httpClient) =>
userServiceFactory(httpClient);

    /// <inheritdoc />
    public IAmoCrmContactService CreateContactService(HttpClientWithCache httpClient) =>
contactServiceFactory(httpClient);

    /// <inheritdoc />
    public IAmoCrmTransactionService CreateTransactionService(HttpClientWithCache httpClient) =>
transactionServiceFactory(httpClient);

    /// <inheritdoc />
    public IAmoCrmCustomFieldService CreateCustomFieldService(HttpClientWithCache httpClient) =>
customFieldServiceFactory(httpClient);

    /// <inheritdoc />
    public IAmoCrmPipelineService CreatePipelineService(HttpClientWithCache httpClient) =>
pipelineServiceFactory(httpClient);

    /// <inheritdoc />
    public IAmoCrmNoteService CreateNoteService(HttpClientWithCache httpClient) =>
noteServiceFactory(httpClient);

    /// <inheritdoc />
    public IAmoCrmLinkService CreateLinkService(HttpClientWithCache httpClient) =>
linkServiceFactory(httpClient);
}
