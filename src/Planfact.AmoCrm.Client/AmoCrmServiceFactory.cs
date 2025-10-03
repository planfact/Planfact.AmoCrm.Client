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
    Func<HttpClientWithCache, IAmoCrmNoteService> noteServiceFactory) : IAmoCrmServiceFactory
{
    public IAmoCrmAccountService CreateAccountService(HttpClientWithCache httpClient) =>
accountServiceFactory(httpClient);

    public IAmoCrmAuthorizationService CreateAuthorizationService(HttpClientWithCache httpClient) =>
authorizationServiceFactory(httpClient);

    public IAmoCrmLeadService CreateLeadService(HttpClientWithCache httpClient) =>
leadServiceFactory(httpClient);

    public IAmoCrmCompanyService CreateCompanyService(HttpClientWithCache httpClient) =>
companyServiceFactory(httpClient);

    public IAmoCrmTaskService CreateTaskService(HttpClientWithCache httpClient) =>
taskServiceFactory(httpClient);

    public IAmoCrmCustomerService CreateCustomerService(HttpClientWithCache httpClient) =>
customerServiceFactory(httpClient);

    public IAmoCrmUserService CreateUserService(HttpClientWithCache httpClient) =>
userServiceFactory(httpClient);

    public IAmoCrmContactService CreateContactService(HttpClientWithCache httpClient) =>
contactServiceFactory(httpClient);

    public IAmoCrmTransactionService CreateTransactionService(HttpClientWithCache httpClient) =>
transactionServiceFactory(httpClient);

    public IAmoCrmCustomFieldService CreateCustomFieldService(HttpClientWithCache httpClient) =>
customFieldServiceFactory(httpClient);

    public IAmoCrmPipelineService CreatePipelineService(HttpClientWithCache httpClient) =>
pipelineServiceFactory(httpClient);

    public IAmoCrmNoteService CreateNoteService(HttpClientWithCache httpClient) =>
noteServiceFactory(httpClient);
}
