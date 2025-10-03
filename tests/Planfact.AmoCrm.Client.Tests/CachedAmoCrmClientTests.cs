using Microsoft.Extensions.Caching.Memory;
using Reliable.HttpClient;
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

namespace Planfact.AmoCrm.Client.Tests;

public class CachedAmoCrmClientTests : AmoCrmClientTestsBase
{
    public CachedAmoCrmClientTests()
    {
        ResponseHandlerMock = new Mock<IHttpResponseHandler>();
        var httpClient = new HttpClient();
        var cachedHttpClient = new HttpClientWithCache(
            httpClient,
            new MemoryCache(new MemoryCacheOptions()),
            ResponseHandlerMock.Object
        );

        var options = new AmoCrmClientOptions
        {
            ClientId = TestClientId,
            ClientSecret = TestClientSecret,
            ServerIntegrationAuthCode = TestAuthCode,
            ServerIntegrationSubdomain = TestSubdomain,
            ServerIntegrationRedirectUri = TestRedirectUri
        };
        IOptions<AmoCrmClientOptions> optionsWrapper = Options.Create(options);

        Client = BuildCachedClient(cachedHttpClient, optionsWrapper);
    }

    private static CachedAmoCrmClient BuildCachedClient(
        HttpClientWithCache cachedHttpClient,
        IOptions<AmoCrmClientOptions> optionsWrapper)
    {
        var uriBuilderFactory = new AmoCrmUriBuilderFactory(optionsWrapper);

        var accountServiceLoggerMock = new Mock<ILogger<AmoCrmAccountService>>();
        var authorizationServiceLoggerMock = new Mock<ILogger<AmoCrmAuthorizationService>>();
        var leadServiceLoggerMock = new Mock<ILogger<AmoCrmLeadService>>();
        var companyServiceLoggerMock = new Mock<ILogger<AmoCrmCompanyService>>();
        var taskServiceLoggerMock = new Mock<ILogger<AmoCrmTaskService>>();
        var customerServiceLoggerMock = new Mock<ILogger<AmoCrmCustomerService>>();
        var userServiceLoggerMock = new Mock<ILogger<AmoCrmUserService>>();
        var contactServiceLoggerMock = new Mock<ILogger<AmoCrmContactService>>();
        var transactionServiceLoggerMock = new Mock<ILogger<AmoCrmTransactionService>>();
        var customFieldServiceLoggerMock = new Mock<ILogger<AmoCrmCustomFieldService>>();
        var pipelineServiceLoggerMock = new Mock<ILogger<AmoCrmPipelineService>>();
        var noteServiceLoggerMock = new Mock<ILogger<AmoCrmNoteService>>();

        IAmoCrmAccountService GetAccountService(HttpClientWithCache httpClient) =>
            new AmoCrmAccountService(cachedHttpClient, uriBuilderFactory, accountServiceLoggerMock.Object);
        IAmoCrmAuthorizationService GetAuthorizationService(HttpClientWithCache httpClient) =>
            new AmoCrmAuthorizationService(cachedHttpClient, uriBuilderFactory, optionsWrapper, authorizationServiceLoggerMock.Object);
        IAmoCrmLeadService GetLeadService(HttpClientWithCache httpClient) =>
            new AmoCrmLeadService(cachedHttpClient, uriBuilderFactory, leadServiceLoggerMock.Object);
        IAmoCrmCompanyService GetCompanyService(HttpClientWithCache httpClient) =>
            new AmoCrmCompanyService(cachedHttpClient, uriBuilderFactory, companyServiceLoggerMock.Object);
        IAmoCrmTaskService GetTaskService(HttpClientWithCache httpClient) =>
            new AmoCrmTaskService(cachedHttpClient, uriBuilderFactory, taskServiceLoggerMock.Object);
        IAmoCrmCustomerService GetCustomerservice(HttpClientWithCache httpClient) =>
            new AmoCrmCustomerService(cachedHttpClient, uriBuilderFactory, customerServiceLoggerMock.Object);
        IAmoCrmUserService GetUserservice(HttpClientWithCache httpClient) =>
            new AmoCrmUserService(cachedHttpClient, uriBuilderFactory, userServiceLoggerMock.Object);
        IAmoCrmContactService GetContactService(HttpClientWithCache httpClient) =>
            new AmoCrmContactService(cachedHttpClient, uriBuilderFactory, contactServiceLoggerMock.Object);
        IAmoCrmTransactionService GetTransactionService(HttpClientWithCache httpClient) =>
            new AmoCrmTransactionService(cachedHttpClient, uriBuilderFactory, transactionServiceLoggerMock.Object);
        IAmoCrmCustomFieldService GetCustomFieldService(HttpClientWithCache httpClient) =>
            new AmoCrmCustomFieldService(cachedHttpClient, uriBuilderFactory, customFieldServiceLoggerMock.Object);
        IAmoCrmPipelineService GetPipelineService(HttpClientWithCache httpClient) =>
            new AmoCrmPipelineService(cachedHttpClient, uriBuilderFactory, pipelineServiceLoggerMock.Object);
        IAmoCrmNoteService GetNoteService(HttpClientWithCache httpClient) =>
            new AmoCrmNoteService(cachedHttpClient, uriBuilderFactory, noteServiceLoggerMock.Object);

        IAmoCrmServiceFactory serviceFactory = CreateServiceFactory(
            GetAccountService, GetAuthorizationService, GetLeadService,
            GetCompanyService, GetTaskService, GetCustomerservice,
            GetUserservice, GetContactService, GetTransactionService,
            GetCustomFieldService, GetPipelineService, GetNoteService);

        return new CachedAmoCrmClient(
            cachedHttpClient,
            serviceFactory,
            optionsWrapper
        );
    }

    private static IAmoCrmServiceFactory CreateServiceFactory(
        Func<HttpClientWithCache, IAmoCrmAccountService> accountFactory,
        Func<HttpClientWithCache, IAmoCrmAuthorizationService> authFactory,
        Func<HttpClientWithCache, IAmoCrmLeadService> leadFactory,
        Func<HttpClientWithCache, IAmoCrmCompanyService> companyFactory,
        Func<HttpClientWithCache, IAmoCrmTaskService> taskFactory,
        Func<HttpClientWithCache, IAmoCrmCustomerService> customerFactory,
        Func<HttpClientWithCache, IAmoCrmUserService> userFactory,
        Func<HttpClientWithCache, IAmoCrmContactService> contactFactory,
        Func<HttpClientWithCache, IAmoCrmTransactionService> transactionFactory,
        Func<HttpClientWithCache, IAmoCrmCustomFieldService> customFieldFactory,
        Func<HttpClientWithCache, IAmoCrmPipelineService> pipelineFactory,
        Func<HttpClientWithCache, IAmoCrmNoteService> noteFactory) =>
        new AmoCrmServiceFactory(accountFactory, authFactory, leadFactory,
            companyFactory, taskFactory, customerFactory, userFactory,
            contactFactory, transactionFactory, customFieldFactory,
            pipelineFactory, noteFactory);
}
