using Reliable.HttpClient;

using Planfact.AmoCrm.Client.Account;
using Planfact.AmoCrm.Client.Authorization;
using Planfact.AmoCrm.Client.Leads;
using Planfact.AmoCrm.Client.Companies;
using Planfact.AmoCrm.Client.Tasks;
using Planfact.AmoCrm.Client.Customers;
using Planfact.AmoCrm.Client.Users;
using Planfact.AmoCrm.Client.Contacts;
using Planfact.AmoCrm.Client.CustomFields;
using Planfact.AmoCrm.Client.Transactions;
using Planfact.AmoCrm.Client.Pipelines;
using Planfact.AmoCrm.Client.Notes;

namespace Planfact.AmoCrm.Client.Tests;

public class AmoCrmClientTests : AmoCrmClientTestsBase
{
    public AmoCrmClientTests()
    {
        ResponseHandlerMock = new Mock<IHttpResponseHandler>();

        var clientLoggerMock = new Mock<ILogger<AmoCrmClient>>();
        var options = new AmoCrmClientOptions
        {
            ServerIntegrationSubdomain = TestSubdomain,
            ServerIntegrationRedirectUri = TestRedirectUri
        };
        IOptions<AmoCrmClientOptions> optionsWrapper = Options.Create(options);
        var uriBuilderFactory = new AmoCrmUriBuilderFactory(optionsWrapper);

        var httpClient = new HttpClient();
        var httpClientAdapter = new HttpClientAdapter(httpClient, ResponseHandlerMock.Object);

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

        var accountService = new AmoCrmAccountService(httpClientAdapter, uriBuilderFactory, accountServiceLoggerMock.Object);
        var authorizationService = new AmoCrmAuthorizationService(httpClientAdapter, uriBuilderFactory, optionsWrapper, authorizationServiceLoggerMock.Object);
        var leadService = new AmoCrmLeadService(httpClientAdapter, uriBuilderFactory, leadServiceLoggerMock.Object);
        var companyService = new AmoCrmCompanyService(httpClientAdapter, uriBuilderFactory, companyServiceLoggerMock.Object);
        var taskService = new AmoCrmTaskService(httpClientAdapter, uriBuilderFactory, taskServiceLoggerMock.Object);
        var customerService = new AmoCrmCustomerService(httpClientAdapter, uriBuilderFactory, customerServiceLoggerMock.Object);
        var userService = new AmoCrmUserService(httpClientAdapter, uriBuilderFactory, userServiceLoggerMock.Object);
        var contactService = new AmoCrmContactService(httpClientAdapter, uriBuilderFactory, contactServiceLoggerMock.Object);
        var transactionService = new AmoCrmTransactionService(httpClientAdapter, uriBuilderFactory, transactionServiceLoggerMock.Object);
        var customFieldService = new AmoCrmCustomFieldService(httpClientAdapter, uriBuilderFactory, customFieldServiceLoggerMock.Object);
        var pipelineService = new AmoCrmPipelineService(httpClientAdapter, uriBuilderFactory, pipelineServiceLoggerMock.Object);
        var noteService = new AmoCrmNoteService(httpClientAdapter, uriBuilderFactory, noteServiceLoggerMock.Object);

        Client = new AmoCrmClient(
            accountService,
            authorizationService,
            leadService,
            companyService,
            taskService,
            customerService,
            userService,
            contactService,
            transactionService,
            customFieldService,
            pipelineService,
            noteService,
            optionsWrapper,
            clientLoggerMock.Object
        );
    }
}
