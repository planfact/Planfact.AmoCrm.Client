using Moq;
using Reliable.HttpClient;

using Planfact.AmoCrm.Client.Authorization;
using Planfact.AmoCrm.Client.Common;
using Planfact.AmoCrm.Client.Companies;
using Planfact.AmoCrm.Client.Contacts;
using Planfact.AmoCrm.Client.Customers;
using Planfact.AmoCrm.Client.CustomFields;
using Planfact.AmoCrm.Client.Exceptions;
using Planfact.AmoCrm.Client.Leads;
using Planfact.AmoCrm.Client.Notes;
using Planfact.AmoCrm.Client.Pipelines;
using Planfact.AmoCrm.Client.Tasks;
using Planfact.AmoCrm.Client.Transactions;
using Planfact.AmoCrm.Client.Users;

using AccountModel = Planfact.AmoCrm.Client.Account.AccountResponse;
using AmoCrmTask = Planfact.AmoCrm.Client.Tasks.Task;
using Task = System.Threading.Tasks.Task;
using Widget = Planfact.AmoCrm.Client.Account.WidgetResponse;

namespace Planfact.AmoCrm.Client.Tests;

public abstract class AmoCrmClientTestsBase
{
    protected const string TestAccessToken = "access-token";
    protected const string TestSubdomain = "example.amocrm.ru";
    protected const string TestRedirectUri = "https://example.com";

    protected IAmoCrmClient Client { get; set; } = new Mock<IAmoCrmClient>().Object;
    protected Mock<IHttpResponseHandler> ResponseHandlerMock { get; set; } = new Mock<IHttpResponseHandler>();

    [SetUp]
    public abstract void SetUp();

    [Test]
    public async Task AuthorizeAsync_ValidParameters_ReturnsTokensAsync()
    {
        const string authCode = "auth-code";
        const string redirectDomain = "https://example.com";
        var expectedTokens = new AuthorizationTokens
        {
            AccessToken = "access-token",
            RefreshToken = "refresh-token",
            ExpiresIn = 3600
        };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTokens);

        AuthorizationTokens result = await Client.AuthorizeAsync(TestSubdomain, authCode, redirectDomain);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.AccessToken, Is.EqualTo("access-token"));
            Assert.That(result.RefreshToken, Is.EqualTo("refresh-token"));
            Assert.That(result.ExpiresIn, Is.EqualTo(3600));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public async Task AuthorizeInternalAsync_ValidParameters_ReturnsTokensAsync()
    {
        var expectedTokens = new AuthorizationTokens
        {
            AccessToken = "internal-access-token",
            RefreshToken = "internal-refresh-token",
            ExpiresIn = 3600
        };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTokens);

        AuthorizationTokens result = await Client.AuthorizeInternalAsync();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.AccessToken, Is.EqualTo("internal-access-token"));
            Assert.That(result.RefreshToken, Is.EqualTo("internal-refresh-token"));
            Assert.That(result.ExpiresIn, Is.EqualTo(3600));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public async Task RefreshTokenAsync_ValidParameters_ReturnsTokensAsync()
    {
        const string refreshToken = "refresh-token";
        const string redirectDomain = "https://example.com";
        var expectedTokens = new AuthorizationTokens
        {
            AccessToken = "new-access-token",
            RefreshToken = "new-refresh-token",
            ExpiresIn = 3600
        };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTokens);

        AuthorizationTokens result = await Client.RefreshTokenAsync(TestSubdomain, refreshToken, redirectDomain);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.AccessToken, Is.EqualTo("new-access-token"));
            Assert.That(result.RefreshToken, Is.EqualTo("new-refresh-token"));
            Assert.That(result.ExpiresIn, Is.EqualTo(3600));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public async Task RefreshTokenInternalAsync_ValidParameters_ReturnsTokensAsync()
    {
        const string refreshToken = "internal-refresh-token";
        var expectedTokens = new AuthorizationTokens
        {
            AccessToken = "new-internal-access-token",
            RefreshToken = "new-internal-refresh-token",
            ExpiresIn = 3600
        };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTokens);

        AuthorizationTokens result = await Client.RefreshTokenInternalAsync(refreshToken);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.AccessToken, Is.EqualTo("new-internal-access-token"));
            Assert.That(result.RefreshToken, Is.EqualTo("new-internal-refresh-token"));
            Assert.That(result.ExpiresIn, Is.EqualTo(3600));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public async Task GetLeadsAsync_ValidQuery_ReturnsLeadsAsync()
    {
        const string query = "test-query";
        List<Lead> leads1 = [new Lead { Id = 1, Name = "Lead 1", AccountId = 100 }];
        List<Lead> leads2 = [new Lead { Id = 2, Name = "Lead 2", AccountId = 101 }];
        EntitiesResponse response1 = new() { Embedded = new EmbeddedEntitiesResponse { Leads = leads1.ToArray() }, Links = new LinksResponse { Next = new Link { Uri = "https://example.amocrm.ru/api/v4/leads?page=2&limit=1" } } };
        EntitiesResponse response2 = new() { Embedded = new EmbeddedEntitiesResponse { Leads = leads2.ToArray() } };

        ResponseHandlerMock
            .SetupSequence(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response1)
            .ReturnsAsync(response2);

        IReadOnlyCollection<Lead> result = await Client.GetLeadsAsync(TestAccessToken, TestSubdomain, query);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result.First().Id, Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("Lead 1"));
            Assert.That(result.First().AccountId, Is.EqualTo(100));
            Assert.That(result.Last().Id, Is.EqualTo(2));
            Assert.That(result.Last().Name, Is.EqualTo("Lead 2"));
            Assert.That(result.Last().AccountId, Is.EqualTo(101));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Test]
    public async Task GetLeadsInternalAsync_ValidQuery_ReturnsLeadsAsync()
    {
        const string query = "test-query";
        List<Lead> leads1 = [new Lead { Id = 1, Name = "Lead 1", AccountId = 100 }];
        List<Lead> leads2 = [new Lead { Id = 2, Name = "Lead 2", AccountId = 101 }];
        EntitiesResponse response1 = new() { Embedded = new EmbeddedEntitiesResponse { Leads = leads1.ToArray() }, Links = new LinksResponse { Next = new Link { Uri = "https://example.amocrm.ru/api/v4/leads?page=2&limit=1" } } };
        EntitiesResponse response2 = new() { Embedded = new EmbeddedEntitiesResponse { Leads = leads2.ToArray() } };

        ResponseHandlerMock
            .SetupSequence(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response1)
            .ReturnsAsync(response2);

        IReadOnlyCollection<Lead> result = await Client.GetLeadsInternalAsync(TestAccessToken, query);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result.First().Id, Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("Lead 1"));
            Assert.That(result.First().AccountId, Is.EqualTo(100));
            Assert.That(result.Last().Id, Is.EqualTo(2));
            Assert.That(result.Last().Name, Is.EqualTo("Lead 2"));
            Assert.That(result.Last().AccountId, Is.EqualTo(101));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Test]
    public async Task GetLeadsAsync_ValidIds_ReturnsLeadsAsync()
    {
        int[] ids = [1, 2];
        List<Lead> leads = [new Lead { Id = 1, Name = "Lead 1", AccountId = 100 }, new Lead { Id = 2, Name = "Lead 2", AccountId = 101 }];
        EntitiesResponse response = new() { Embedded = new EmbeddedEntitiesResponse { Leads = leads.ToArray() } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Lead> result = await Client.GetLeadsAsync(TestAccessToken, TestSubdomain, ids);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result.First().Id, Is.EqualTo(1));
            Assert.That(result.Last().Id, Is.EqualTo(2));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public async Task GetLeadsInternalAsync_ValidIds_ReturnsLeadsAsync()
    {
        int[] ids = [1, 2];
        List<Lead> leads = [new Lead { Id = 1, Name = "Lead 1", AccountId = 100 }, new Lead { Id = 2, Name = "Lead 2", AccountId = 101 }];
        EntitiesResponse response = new() { Embedded = new EmbeddedEntitiesResponse { Leads = leads.ToArray() } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Lead> result = await Client.GetLeadsInternalAsync(TestAccessToken, ids);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result.First().Id, Is.EqualTo(1));
            Assert.That(result.Last().Id, Is.EqualTo(2));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public async Task AddLeadsAsync_ValidRequests_ReturnsCreatedLeadsAsync()
    {
        List<AddLeadRequest> requests = [new AddLeadRequest { Name = "Lead 1", Price = 1000 }];
        List<Lead> expectedLeads = [new Lead { Id = 1, Name = "Lead 1", Price = 1000 }];
        EntitiesResponse response = new() { Embedded = new EmbeddedEntitiesResponse { Leads = expectedLeads.ToArray() } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Lead> result = await Client.AddLeadsAsync(TestAccessToken, TestSubdomain, requests);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result.First().Id, Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("Lead 1"));
            Assert.That(result.First().Price, Is.EqualTo(1000));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public async Task AddLeadsInternalAsync_ValidRequests_ReturnsCreatedLeadsAsync()
    {
        List<AddLeadRequest> requests = [new AddLeadRequest { Name = "Lead 1", Price = 1000 }];
        List<Lead> expectedLeads = [new Lead { Id = 1, Name = "Lead 1", Price = 1000 }];
        EntitiesResponse response = new() { Embedded = new EmbeddedEntitiesResponse { Leads = expectedLeads.ToArray() } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Lead> result = await Client.AddLeadsInternalAsync(TestAccessToken, requests);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result.First().Id, Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("Lead 1"));
            Assert.That(result.First().Price, Is.EqualTo(1000));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public async Task UpdateLeadsAsync_ValidRequests_ReturnsUpdatedLeadsAsync()
    {
        List<UpdateLeadRequest> requests = [new UpdateLeadRequest(1) { Name = "Updated Lead", Price = 2000 }];
        List<Lead> expectedLeads = [new Lead { Id = 1, Name = "Updated Lead", Price = 2000 }];
        EntitiesResponse response = new() { Embedded = new EmbeddedEntitiesResponse { Leads = expectedLeads.ToArray() } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Lead> result = await Client.UpdateLeadsAsync(TestAccessToken, TestSubdomain, requests);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result.First().Id, Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("Updated Lead"));
            Assert.That(result.First().Price, Is.EqualTo(2000));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public async Task UpdateLeadsInternalAsync_ValidRequests_ReturnsUpdatedLeadsAsync()
    {
        List<UpdateLeadRequest> requests = [new UpdateLeadRequest(1) { Name = "Updated Lead", Price = 2000 }];
        List<Lead> expectedLeads = [new Lead { Id = 1, Name = "Updated Lead", Price = 2000 }];
        EntitiesResponse response = new() { Embedded = new EmbeddedEntitiesResponse { Leads = expectedLeads.ToArray() } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Lead> result = await Client.UpdateLeadsInternalAsync(TestAccessToken, requests);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result.First().Id, Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("Updated Lead"));
            Assert.That(result.First().Price, Is.EqualTo(2000));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public async Task GetCompaniesAsync_ValidQuery_ReturnsCompaniesAsync()
    {
        const string query = "test";
        List<Company> companies1 = [new Company { Id = 1, Name = "Company 1" }];
        List<Company> companies2 = [new Company { Id = 2, Name = "Company 2" }];
        EntitiesResponse response1 = new() { Embedded = new EmbeddedEntitiesResponse { Companies = companies1.ToArray() }, Links = new LinksResponse { Next = new Link { Uri = "https://example.amocrm.ru/api/v4/companies?page=2&limit=1" } } };
        EntitiesResponse response2 = new() { Embedded = new EmbeddedEntitiesResponse { Companies = companies2.ToArray() } };

        ResponseHandlerMock
            .SetupSequence(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response1)
            .ReturnsAsync(response2);

        IReadOnlyCollection<Company> result = await Client.GetCompaniesAsync(TestAccessToken, TestSubdomain, query);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result.First().Id, Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("Company 1"));
            Assert.That(result.Last().Id, Is.EqualTo(2));
            Assert.That(result.Last().Name, Is.EqualTo("Company 2"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Test]
    public async Task GetCompaniesInternalAsync_ValidQuery_ReturnsCompaniesAsync()
    {
        const string query = "test";
        List<Company> companies1 = [new Company { Id = 1, Name = "Company 1" }];
        List<Company> companies2 = [new Company { Id = 2, Name = "Company 2" }];
        EntitiesResponse response1 = new() { Embedded = new EmbeddedEntitiesResponse { Companies = companies1.ToArray() }, Links = new LinksResponse { Next = new Link { Uri = "https://example.amocrm.ru/api/v4/companies?page=2&limit=1" } } };
        EntitiesResponse response2 = new() { Embedded = new EmbeddedEntitiesResponse { Companies = companies2.ToArray() } };

        ResponseHandlerMock
            .SetupSequence(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response1)
            .ReturnsAsync(response2);

        IReadOnlyCollection<Company> result = await Client.GetCompaniesInternalAsync(TestAccessToken, query);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result.First().Id, Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("Company 1"));
            Assert.That(result.Last().Id, Is.EqualTo(2));
            Assert.That(result.Last().Name, Is.EqualTo("Company 2"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Test]
    public async Task AddCompaniesAsync_ValidRequests_ReturnsCreatedCompaniesAsync()
    {
        List<AddCompanyRequest> requests = [new AddCompanyRequest { Name = "Company 1" }];
        List<Company> expectedCompanies = [new Company { Id = 1, Name = "Company 1" }];
        EntitiesResponse response = new() { Embedded = new EmbeddedEntitiesResponse { Companies = expectedCompanies.ToArray() } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Company> result = await Client.AddCompaniesAsync(TestAccessToken, TestSubdomain, requests);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result.First().Id, Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("Company 1"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public async Task AddCompaniesInternalAsync_ValidRequests_ReturnsCreatedCompaniesAsync()
    {
        List<AddCompanyRequest> requests = [new AddCompanyRequest { Name = "Company 1" }];
        List<Company> expectedCompanies = [new Company { Id = 1, Name = "Company 1" }];
        EntitiesResponse response = new() { Embedded = new EmbeddedEntitiesResponse { Companies = expectedCompanies.ToArray() } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Company> result = await Client.AddCompaniesInternalAsync(TestAccessToken, requests);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result.First().Id, Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("Company 1"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public async Task UpdateCompaniesAsync_ValidRequests_ReturnsUpdatedCompaniesAsync()
    {
        List<UpdateCompanyRequest> requests = [new UpdateCompanyRequest(1) { Name = "Updated Company" }];
        List<Company> expectedCompanies = [new Company { Id = 1, Name = "Updated Company" }];
        EntitiesResponse response = new() { Embedded = new EmbeddedEntitiesResponse { Companies = expectedCompanies.ToArray() } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Company> result = await Client.UpdateCompaniesAsync(TestAccessToken, TestSubdomain, requests);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result.First().Id, Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("Updated Company"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public async Task UpdateCompaniesInternalAsync_ValidRequests_ReturnsUpdatedCompaniesAsync()
    {
        List<UpdateCompanyRequest> requests = [new UpdateCompanyRequest(1) { Name = "Updated Company" }];
        List<Company> expectedCompanies = [new Company { Id = 1, Name = "Updated Company" }];
        EntitiesResponse response = new() { Embedded = new EmbeddedEntitiesResponse { Companies = expectedCompanies.ToArray() } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Company> result = await Client.UpdateCompaniesInternalAsync(TestAccessToken, requests);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result.First().Id, Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("Updated Company"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public async Task GetTasksAsync_ValidParameters_ReturnsTasksAsync()
    {
        List<AmoCrmTask> tasks1 = [new AmoCrmTask { Id = 1, Description = "Task 1" }];
        List<AmoCrmTask> tasks2 = [new AmoCrmTask { Id = 2, Description = "Task 2" }];
        EntitiesResponse response1 = new() { Embedded = new EmbeddedEntitiesResponse { Tasks = tasks1.ToArray() }, Links = new LinksResponse { Next = new Link { Uri = "https://example.amocrm.ru/api/v4/tasks?page=2&limit=1" } } };
        EntitiesResponse response2 = new() { Embedded = new EmbeddedEntitiesResponse { Tasks = tasks2.ToArray() } };

        ResponseHandlerMock
            .SetupSequence(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response1)
            .ReturnsAsync(response2);

        IReadOnlyCollection<AmoCrmTask> result = await Client.GetTasksAsync(TestAccessToken, TestSubdomain);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result.First().Id, Is.EqualTo(1));
            Assert.That(result.First().Description, Is.EqualTo("Task 1"));
            Assert.That(result.Last().Id, Is.EqualTo(2));
            Assert.That(result.Last().Description, Is.EqualTo("Task 2"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Test]
    public async Task GetTasksInternalAsync_ValidParameters_ReturnsTasksAsync()
    {
        List<AmoCrmTask> tasks1 = [new AmoCrmTask { Id = 1, Description = "Task 1" }];
        List<AmoCrmTask> tasks2 = [new AmoCrmTask { Id = 2, Description = "Task 2" }];
        EntitiesResponse response1 = new() { Embedded = new EmbeddedEntitiesResponse { Tasks = tasks1.ToArray() }, Links = new LinksResponse { Next = new Link { Uri = "https://example.amocrm.ru/api/v4/tasks?page=2&limit=1" } } };
        EntitiesResponse response2 = new() { Embedded = new EmbeddedEntitiesResponse { Tasks = tasks2.ToArray() } };

        ResponseHandlerMock
            .SetupSequence(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response1)
            .ReturnsAsync(response2);

        IReadOnlyCollection<AmoCrmTask> result = await Client.GetTasksInternalAsync(TestAccessToken);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result.First().Id, Is.EqualTo(1));
            Assert.That(result.First().Description, Is.EqualTo("Task 1"));
            Assert.That(result.Last().Id, Is.EqualTo(2));
            Assert.That(result.Last().Description, Is.EqualTo("Task 2"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Test]
    public async Task AddTasksAsync_ValidRequests_ReturnsCreatedTasksAsync()
    {
        List<AddTaskRequest> requests = [new AddTaskRequest("Task 1", 1234567890, EntityTypeEnum.Leads) { EntityId = 1 }];
        List<AmoCrmTask> expectedTasks = [new AmoCrmTask { Id = 1, Description = "Task 1", EntityId = 1, EntityTypeName = "leads" }];
        EntitiesResponse response = new() { Embedded = new EmbeddedEntitiesResponse { Tasks = expectedTasks.ToArray() } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<AmoCrmTask> result = await Client.AddTasksAsync(TestAccessToken, TestSubdomain, requests);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result.First().Id, Is.EqualTo(1));
            Assert.That(result.First().Description, Is.EqualTo("Task 1"));
            Assert.That(result.First().EntityId, Is.EqualTo(1));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public async Task AddTasksInternalAsync_ValidRequests_ReturnsCreatedTasksAsync()
    {
        List<AddTaskRequest> requests = [new AddTaskRequest("Task 1", 1234567890, EntityTypeEnum.Leads) { EntityId = 1 }];
        List<AmoCrmTask> expectedTasks = [new AmoCrmTask { Id = 1, Description = "Task 1", EntityId = 1, EntityTypeName = "leads" }];
        EntitiesResponse response = new() { Embedded = new EmbeddedEntitiesResponse { Tasks = expectedTasks.ToArray() } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<AmoCrmTask> result = await Client.AddTasksInternalAsync(TestAccessToken, requests);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result.First().Id, Is.EqualTo(1));
            Assert.That(result.First().Description, Is.EqualTo("Task 1"));
            Assert.That(result.First().EntityId, Is.EqualTo(1));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public async Task UpdateTasksAsync_ValidRequests_ReturnsUpdatedTasksAsync()
    {
        List<UpdateTaskRequest> requests = [new UpdateTaskRequest(1, "Updated Task", 123123123123)];
        List<AmoCrmTask> expectedTasks = [new AmoCrmTask { Id = 1, Description = "Updated Task", EntityId = 1, CompleteTill = 123123123123 }];
        EntitiesResponse response = new() { Embedded = new EmbeddedEntitiesResponse { Tasks = expectedTasks.ToArray() } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<AmoCrmTask> result = await Client.UpdateTasksAsync(TestAccessToken, TestSubdomain, requests);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result.First().Id, Is.EqualTo(1));
            Assert.That(result.First().Description, Is.EqualTo("Updated Task"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public async Task UpdateTasksInternalAsync_ValidRequests_ReturnsUpdatedTasksAsync()
    {
        List<UpdateTaskRequest> requests = [new UpdateTaskRequest(1, "Updated Task", 123123123123)];
        List<AmoCrmTask> expectedTasks = [new AmoCrmTask { Id = 1, Description = "Updated Task", EntityId = 1, CompleteTill = 123123123123 }];
        EntitiesResponse response = new() { Embedded = new EmbeddedEntitiesResponse { Tasks = expectedTasks.ToArray() } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<AmoCrmTask> result = await Client.UpdateTasksInternalAsync(TestAccessToken, requests);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result.First().Id, Is.EqualTo(1));
            Assert.That(result.First().Description, Is.EqualTo("Updated Task"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public async Task GetCustomersAsync_ValidQuery_ReturnsCustomersAsync()
    {
        const string query = "test";
        List<Customer> customers1 = [new Customer { Id = 1, Name = "Customer 1" }];
        List<Customer> customers2 = [new Customer { Id = 2, Name = "Customer 2" }];
        EntitiesResponse response1 = new() { Embedded = new EmbeddedEntitiesResponse { Customers = customers1.ToArray() }, Links = new LinksResponse { Next = new Link { Uri = "https://example.amocrm.ru/api/v4/customers?page=2&limit=1" } } };
        EntitiesResponse response2 = new() { Embedded = new EmbeddedEntitiesResponse { Customers = customers2.ToArray() } };

        ResponseHandlerMock
            .SetupSequence(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response1)
            .ReturnsAsync(response2);

        IReadOnlyCollection<Customer> result = await Client.GetCustomersAsync(TestAccessToken, TestSubdomain, query);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result.First().Id, Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("Customer 1"));
            Assert.That(result.Last().Id, Is.EqualTo(2));
            Assert.That(result.Last().Name, Is.EqualTo("Customer 2"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Test]
    public async Task GetCustomersInternalAsync_ValidQuery_ReturnsCustomersAsync()
    {
        const string query = "test";
        List<Customer> customers1 = [new Customer { Id = 1, Name = "Customer 1" }];
        List<Customer> customers2 = [new Customer { Id = 2, Name = "Customer 2" }];
        EntitiesResponse response1 = new() { Embedded = new EmbeddedEntitiesResponse { Customers = customers1.ToArray() }, Links = new LinksResponse { Next = new Link { Uri = "https://example.amocrm.ru/api/v4/customers?page=2&limit=1" } } };
        EntitiesResponse response2 = new() { Embedded = new EmbeddedEntitiesResponse { Customers = customers2.ToArray() } };

        ResponseHandlerMock
            .SetupSequence(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response1)
            .ReturnsAsync(response2);

        IReadOnlyCollection<Customer> result = await Client.GetCustomersInternalAsync(TestAccessToken, query);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result.First().Id, Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("Customer 1"));
            Assert.That(result.Last().Id, Is.EqualTo(2));
            Assert.That(result.Last().Name, Is.EqualTo("Customer 2"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Test]
    public async Task AddCustomersAsync_ValidRequests_ReturnsCreatedCustomersAsync()
    {
        List<AddCustomerRequest> requests = [new AddCustomerRequest("Customer 1") { NextPrice = 500 }];
        List<Customer> expectedCustomers = [new Customer { Id = 1, Name = "Customer 1", NextPrice = 500 }];
        EntitiesResponse response = new() { Embedded = new EmbeddedEntitiesResponse { Customers = expectedCustomers.ToArray() } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Customer> result = await Client.AddCustomersAsync(TestAccessToken, TestSubdomain, requests);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result.First().Id, Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("Customer 1"));
            Assert.That(result.First().NextPrice, Is.EqualTo(500));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public async Task AddCustomersInternalAsync_ValidRequests_ReturnsCreatedCustomersAsync()
    {
        List<AddCustomerRequest> requests = [new AddCustomerRequest("Customer 1") { NextPrice = 500 }];
        List<Customer> expectedCustomers = [new Customer { Id = 1, Name = "Customer 1", NextPrice = 500 }];
        EntitiesResponse response = new() { Embedded = new EmbeddedEntitiesResponse { Customers = expectedCustomers.ToArray() } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Customer> result = await Client.AddCustomersInternalAsync(TestAccessToken, requests);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result.First().Id, Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("Customer 1"));
            Assert.That(result.First().NextPrice, Is.EqualTo(500));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public async Task UpdateCustomersAsync_ValidRequests_ReturnsUpdatedCustomersAsync()
    {
        List<UpdateCustomerRequest> requests = [new UpdateCustomerRequest(1, "Updated Customer") { NextPrice = 600 }];
        List<Customer> expectedCustomers = [new Customer() { Id = 1, Name = "Updated Customer", NextPrice = 600 }];
        EntitiesResponse response = new() { Embedded = new EmbeddedEntitiesResponse { Customers = expectedCustomers.ToArray() } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Customer> result = await Client.UpdateCustomersAsync(TestAccessToken, TestSubdomain, requests);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result.First().Id, Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("Updated Customer"));
            Assert.That(result.First().NextPrice, Is.EqualTo(600));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public async Task UpdateCustomersInternalAsync_ValidRequests_ReturnsUpdatedCustomersAsync()
    {
        List<UpdateCustomerRequest> requests = [new UpdateCustomerRequest(1, "Updated Customer") { NextPrice = 600 }];
        List<Customer> expectedCustomers = [new Customer() { Id = 1, Name = "Updated Customer", NextPrice = 600 }];
        EntitiesResponse response = new() { Embedded = new EmbeddedEntitiesResponse { Customers = expectedCustomers.ToArray() } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Customer> result = await Client.UpdateCustomersInternalAsync(TestAccessToken, requests);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result.First().Id, Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("Updated Customer"));
            Assert.That(result.First().NextPrice, Is.EqualTo(600));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public async Task GetAccountAsync_ValidParameters_ReturnsAccountAsync()
    {
        AccountModel expectedAccount = new AccountModel { Id = 1, Name = "Test Account" };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<AccountModel>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedAccount);

        AccountModel result = await Client.GetAccountAsync(TestAccessToken, TestSubdomain);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.Name, Is.EqualTo("Test Account"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<AccountModel>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public async Task GetAccountInternalAsync_ValidParameters_ReturnsAccountAsync()
    {
        AccountModel expectedAccount = new AccountModel { Id = 1, Name = "Test Account" };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<AccountModel>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedAccount);

        AccountModel result = await Client.GetAccountInternalAsync(TestAccessToken);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.Name, Is.EqualTo("Test Account"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<AccountModel>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public async Task GetUsersAsync_ValidParameters_ReturnsUsersAsync()
    {
        List<User> users1 = [new User { Id = 1, FullName = "User 1" }];
        List<User> users2 = [new User { Id = 2, FullName = "User 2" }];
        EntitiesResponse response1 = new() { Embedded = new EmbeddedEntitiesResponse { Users = users1.ToArray() }, Links = new LinksResponse { Next = new Link { Uri = "https://example.amocrm.ru/api/v4/users?page=2&limit=1" } } };
        EntitiesResponse response2 = new() { Embedded = new EmbeddedEntitiesResponse { Users = users2.ToArray() } };

        ResponseHandlerMock
            .SetupSequence(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response1)
            .ReturnsAsync(response2);

        IReadOnlyCollection<User> result = await Client.GetUsersAsync(TestAccessToken, TestSubdomain);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result.First().Id, Is.EqualTo(1));
            Assert.That(result.First().FullName, Is.EqualTo("User 1"));
            Assert.That(result.Last().Id, Is.EqualTo(2));
            Assert.That(result.Last().FullName, Is.EqualTo("User 2"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Test]
    public async Task GetWidgetAsync_ValidParameters_ReturnsWidgetAsync()
    {
        const string widgetCode = "test-widget";
        Widget expectedWidget = new Widget { Id = 1, Code = "test-widget" };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<Widget>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedWidget);

        Widget result = await Client.GetWidgetAsync(TestAccessToken, TestSubdomain, widgetCode);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.Code, Is.EqualTo("test-widget"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<Widget>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public async Task GetContactsAsync_ValidQuery_ReturnsContactsAsync()
    {
        const string query = "test";
        List<Contact> contacts1 = [new Contact { Id = 1, Name = "Contact 1" }];
        List<Contact> contacts2 = [new Contact { Id = 2, Name = "Contact 2" }];
        EntitiesResponse response1 = new() { Embedded = new EmbeddedEntitiesResponse { Contacts = contacts1.ToArray() }, Links = new LinksResponse { Next = new Link { Uri = "https://example.amocrm.ru/api/v4/contacts?page=2&limit=1" } } };
        EntitiesResponse response2 = new() { Embedded = new EmbeddedEntitiesResponse { Contacts = contacts2.ToArray() } };

        ResponseHandlerMock
            .SetupSequence(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response1)
            .ReturnsAsync(response2);

        IReadOnlyCollection<Contact> result = await Client.GetContactsAsync(TestAccessToken, TestSubdomain, query);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result.First().Id, Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("Contact 1"));
            Assert.That(result.Last().Id, Is.EqualTo(2));
            Assert.That(result.Last().Name, Is.EqualTo("Contact 2"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Test]
    public async Task GetContactsInternalAsync_ValidQuery_ReturnsContactsAsync()
    {
        const string query = "test";
        List<Contact> contacts1 = [new Contact { Id = 1, Name = "Contact 1" }];
        List<Contact> contacts2 = [new Contact { Id = 2, Name = "Contact 2" }];
        EntitiesResponse response1 = new() { Embedded = new EmbeddedEntitiesResponse { Contacts = contacts1.ToArray() }, Links = new LinksResponse { Next = new Link { Uri = "https://example.amocrm.ru/api/v4/contacts?page=2&limit=1" } } };
        EntitiesResponse response2 = new() { Embedded = new EmbeddedEntitiesResponse { Contacts = contacts2.ToArray() } };

        ResponseHandlerMock
            .SetupSequence(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response1)
            .ReturnsAsync(response2);

        IReadOnlyCollection<Contact> result = await Client.GetContactsInternalAsync(TestAccessToken, query);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result.First().Id, Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("Contact 1"));
            Assert.That(result.Last().Id, Is.EqualTo(2));
            Assert.That(result.Last().Name, Is.EqualTo("Contact 2"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Test]
    public async Task GetContactByIdAsync_ValidId_ReturnsContactAsync()
    {
        const int contactId = 1;
        Contact expectedContact = new() { Id = 1, Name = "Contact 1", FirstName = "John" };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<Contact>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedContact);

        Contact result = await Client.GetContactByIdAsync(TestAccessToken, TestSubdomain, contactId);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.Name, Is.EqualTo("Contact 1"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<Contact>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public async Task GetContactByIdInternalAsync_ValidId_ReturnsContactAsync()
    {
        const int contactId = 1;
        Contact expectedContact = new() { Id = 1, Name = "Contact 1", FirstName = "John" };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<Contact>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedContact);

        Contact result = await Client.GetContactByIdInternalAsync(TestAccessToken, contactId);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.Name, Is.EqualTo("Contact 1"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<Contact>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public async Task AddContactsAsync_ValidRequests_ReturnsCreatedContactsAsync()
    {
        List<AddContactRequest> requests = [new AddContactRequest { Name = "Contact 1", FirstName = "John" }];
        List<Contact> expectedContacts = [new Contact { Id = 1, Name = "Contact 1", FirstName = "John" }];
        EntitiesResponse response = new() { Embedded = new EmbeddedEntitiesResponse { Contacts = expectedContacts.ToArray() } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Contact> result = await Client.AddContactsAsync(TestAccessToken, TestSubdomain, requests);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result.First().Id, Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("Contact 1"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public async Task AddContactsInternalAsync_ValidRequests_ReturnsCreatedContactsAsync()
    {
        List<AddContactRequest> requests = [new AddContactRequest { Name = "Contact 1", FirstName = "John" }];
        List<Contact> expectedContacts = [new Contact { Id = 1, Name = "Contact 1", FirstName = "John" }];
        EntitiesResponse response = new() { Embedded = new EmbeddedEntitiesResponse { Contacts = expectedContacts.ToArray() } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Contact> result = await Client.AddContactsInternalAsync(TestAccessToken, requests);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result.First().Id, Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("Contact 1"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public async Task UpdateContactsAsync_ValidRequests_ReturnsUpdatedContactsAsync()
    {
        List<UpdateContactRequest> requests = [new UpdateContactRequest(1) { Name = "Updated Contact" }];
        List<Contact> expectedContacts = [new Contact { Id = 1, Name = "Updated Contact" }];
        EntitiesResponse response = new() { Embedded = new EmbeddedEntitiesResponse { Contacts = expectedContacts.ToArray() } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Contact> result = await Client.UpdateContactsAsync(TestAccessToken, TestSubdomain, requests);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result.First().Id, Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("Updated Contact"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public async Task UpdateContactsInternalAsync_ValidRequests_ReturnsUpdatedContactsAsync()
    {
        List<UpdateContactRequest> requests = [new UpdateContactRequest(1) { Name = "Updated Contact" }];
        List<Contact> expectedContacts = [new Contact { Id = 1, Name = "Updated Contact" }];
        EntitiesResponse response = new() { Embedded = new EmbeddedEntitiesResponse { Contacts = expectedContacts.ToArray() } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Contact> result = await Client.UpdateContactsInternalAsync(TestAccessToken, requests);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result.First().Id, Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("Updated Contact"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public async Task GetTransactionsAsync_ValidParameters_ReturnsTransactionsAsync()
    {
        const int customerId = 1;
        List<Transaction> transactions1 = [new Transaction { Id = 1, Price = 100 }];
        List<Transaction> transactions2 = [new Transaction { Id = 2, Price = 200 }];
        EntitiesResponse response1 = new() { Embedded = new EmbeddedEntitiesResponse { Transactions = transactions1.ToArray() }, Links = new LinksResponse { Next = new Link { Uri = "https://example.amocrm.ru/api/v4/customers/transactions?page=2&limit=1" } } };
        EntitiesResponse response2 = new() { Embedded = new EmbeddedEntitiesResponse { Transactions = transactions2.ToArray() } };

        ResponseHandlerMock
            .SetupSequence(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response1)
            .ReturnsAsync(response2);

        IReadOnlyCollection<Transaction> result = await Client.GetTransactionsAsync(TestAccessToken, TestSubdomain, customerId);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result.First().Id, Is.EqualTo(1));
            Assert.That(result.First().Price, Is.EqualTo(100));
            Assert.That(result.Last().Id, Is.EqualTo(2));
            Assert.That(result.Last().Price, Is.EqualTo(200));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Test]
    public async Task GetTransactionsInternalAsync_ValidParameters_ReturnsTransactionsAsync()
    {
        const int customerId = 1;
        List<Transaction> transactions1 = [new Transaction { Id = 1, Price = 100 }];
        List<Transaction> transactions2 = [new Transaction { Id = 2, Price = 200 }];
        EntitiesResponse response1 = new() { Embedded = new EmbeddedEntitiesResponse { Transactions = transactions1.ToArray() }, Links = new LinksResponse { Next = new Link { Uri = "https://example.amocrm.ru/api/v4/customers/transactions?page=2&limit=1" } } };
        EntitiesResponse response2 = new() { Embedded = new EmbeddedEntitiesResponse { Transactions = transactions2.ToArray() } };

        ResponseHandlerMock
            .SetupSequence(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response1)
            .ReturnsAsync(response2);

        IReadOnlyCollection<Transaction> result = await Client.GetTransactionsInternalAsync(TestAccessToken, customerId);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result.First().Id, Is.EqualTo(1));
            Assert.That(result.First().Price, Is.EqualTo(100));
            Assert.That(result.Last().Id, Is.EqualTo(2));
            Assert.That(result.Last().Price, Is.EqualTo(200));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Test]
    public async Task AddTransactionsAsync_ValidRequests_ReturnsCreatedTransactionsAsync()
    {
        const int customerId = 1;
        List<AddTransactionRequest> requests = [new AddTransactionRequest(1000) { CustomerId = customerId, Comment = "Transaction 1" }];
        List<Transaction> expectedTransactions = [new Transaction { Id = 1, CustomerId = customerId, Price = 1000, Comment = "Transaction 1" }];
        EntitiesResponse response = new() { Embedded = new EmbeddedEntitiesResponse { Transactions = expectedTransactions.ToArray() } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Transaction> result = await Client.AddTransactionsAsync(TestAccessToken, TestSubdomain, customerId, requests);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result.First().Id, Is.EqualTo(1));
            Assert.That(result.First().CustomerId, Is.EqualTo(customerId));
            Assert.That(result.First().Price, Is.EqualTo(1000));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public async Task AddTransactionsInternalAsync_ValidRequests_ReturnsCreatedTransactionsAsync()
    {
        const int customerId = 1;
        List<AddTransactionRequest> requests = [new AddTransactionRequest(1000) { CustomerId = customerId, Comment = "Transaction 1" }];
        List<Transaction> expectedTransactions = [new Transaction { Id = 1, CustomerId = customerId, Price = 1000, Comment = "Transaction 1" }];
        EntitiesResponse response = new() { Embedded = new EmbeddedEntitiesResponse { Transactions = expectedTransactions.ToArray() } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Transaction> result = await Client.AddTransactionsInternalAsync(TestAccessToken, customerId, requests);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result.First().Id, Is.EqualTo(1));
            Assert.That(result.First().CustomerId, Is.EqualTo(customerId));
            Assert.That(result.First().Price, Is.EqualTo(1000));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public async Task GetCustomFieldsAsync_ValidParameters_ReturnsCustomFieldsAsync()
    {
        EntityTypeEnum entityType = EntityTypeEnum.Leads;
        List<CustomField> customFields1 = [new CustomField { Id = 1, Name = "Field 1" }];
        List<CustomField> customFields2 = [new CustomField { Id = 2, Name = "Field 2" }];
        EntitiesResponse response1 = new() { Embedded = new EmbeddedEntitiesResponse { CustomFields = customFields1.ToArray() }, Links = new LinksResponse { Next = new Link { Uri = "https://example.amocrm.ru/api/v4/leads/custom_fields?page=2&limit=1" } } };
        EntitiesResponse response2 = new() { Embedded = new EmbeddedEntitiesResponse { CustomFields = customFields2.ToArray() } };

        ResponseHandlerMock
            .SetupSequence(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response1)
            .ReturnsAsync(response2);

        IReadOnlyCollection<CustomField> result = await Client.GetCustomFieldsAsync(TestAccessToken, TestSubdomain, entityType);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result.First().Id, Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("Field 1"));
            Assert.That(result.Last().Id, Is.EqualTo(2));
            Assert.That(result.Last().Name, Is.EqualTo("Field 2"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Test]
    public async Task GetCustomFieldsInternalAsync_ValidParameters_ReturnsCustomFieldsAsync()
    {
        EntityTypeEnum entityType = EntityTypeEnum.Leads;
        List<CustomField> customFields1 = [new CustomField { Id = 1, Name = "Field 1" }];
        List<CustomField> customFields2 = [new CustomField { Id = 2, Name = "Field 2" }];
        EntitiesResponse response1 = new() { Embedded = new EmbeddedEntitiesResponse { CustomFields = customFields1.ToArray() }, Links = new LinksResponse { Next = new Link { Uri = "https://example.amocrm.ru/api/v4/leads/custom_fields?page=2&limit=1" } } };
        EntitiesResponse response2 = new() { Embedded = new EmbeddedEntitiesResponse { CustomFields = customFields2.ToArray() } };

        ResponseHandlerMock
            .SetupSequence(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response1)
            .ReturnsAsync(response2);

        IReadOnlyCollection<CustomField> result = await Client.GetCustomFieldsInternalAsync(TestAccessToken, entityType);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result.First().Id, Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("Field 1"));
            Assert.That(result.Last().Id, Is.EqualTo(2));
            Assert.That(result.Last().Name, Is.EqualTo("Field 2"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Test]
    public void AuthorizeAsync_AuthenticationError_ThrowsAmoCrmAuthenticationException()
    {
        const string authCode = "auth-code";
        const string redirectDomain = "https://example.com";

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthenticationException("Authentication failed"));

        AmoCrmAuthenticationException exception = Assert.ThrowsAsync<AmoCrmAuthenticationException>(async () =>
            await Client.AuthorizeAsync(TestSubdomain, authCode, redirectDomain));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Authentication failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void AuthorizeInternalAsync_AuthenticationError_ThrowsAmoCrmAuthenticationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthenticationException("Authentication failed"));

        AmoCrmAuthenticationException exception = Assert.ThrowsAsync<AmoCrmAuthenticationException>(async () =>
            await Client.AuthorizeInternalAsync());

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Authentication failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void RefreshTokenAsync_AuthenticationError_ThrowsAmoCrmAuthenticationException()
    {
        const string refreshToken = "refresh-token";
        const string redirectDomain = "https://example.com";

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthenticationException("Authentication failed"));

        AmoCrmAuthenticationException exception = Assert.ThrowsAsync<AmoCrmAuthenticationException>(async () =>
            await Client.RefreshTokenAsync(TestSubdomain, refreshToken, redirectDomain));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Authentication failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void RefreshTokenInternalAsync_AuthenticationError_ThrowsAmoCrmAuthenticationException()
    {
        const string refreshToken = "internal-refresh-token";

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthenticationException("Authentication failed"));

        AmoCrmAuthenticationException exception = Assert.ThrowsAsync<AmoCrmAuthenticationException>(async () =>
            await Client.RefreshTokenInternalAsync(refreshToken));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Authentication failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetLeadsAsync_AuthenticationError_ThrowsAmoCrmAuthenticationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthenticationException("Authentication failed"));

        AmoCrmAuthenticationException exception = Assert.ThrowsAsync<AmoCrmAuthenticationException>(async () =>
            await Client.GetLeadsAsync(TestAccessToken, TestSubdomain));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Authentication failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetLeadsInternalAsync_AuthenticationError_ThrowsAmoCrmAuthenticationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthenticationException("Authentication failed"));

        AmoCrmAuthenticationException exception = Assert.ThrowsAsync<AmoCrmAuthenticationException>(async () =>
            await Client.GetLeadsInternalAsync(TestAccessToken));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Authentication failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void AddLeadsAsync_AuthenticationError_ThrowsAmoCrmAuthenticationException()
    {
        List<AddLeadRequest> requests = [new AddLeadRequest { Name = "Lead 1" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthenticationException("Authentication failed"));

        AmoCrmAuthenticationException exception = Assert.ThrowsAsync<AmoCrmAuthenticationException>(async () =>
            await Client.AddLeadsAsync(TestAccessToken, TestSubdomain, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Authentication failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void AddLeadsInternalAsync_AuthenticationError_ThrowsAmoCrmAuthenticationException()
    {
        List<AddLeadRequest> requests = [new AddLeadRequest { Name = "Lead 1" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthenticationException("Authentication failed"));

        AmoCrmAuthenticationException exception = Assert.ThrowsAsync<AmoCrmAuthenticationException>(async () =>
            await Client.AddLeadsInternalAsync(TestAccessToken, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Authentication failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void UpdateLeadsAsync_AuthenticationError_ThrowsAmoCrmAuthenticationException()
    {
        List<UpdateLeadRequest> requests = [new UpdateLeadRequest(1) { Name = "Updated Lead" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthenticationException("Authentication failed"));

        AmoCrmAuthenticationException exception = Assert.ThrowsAsync<AmoCrmAuthenticationException>(async () =>
            await Client.UpdateLeadsAsync(TestAccessToken, TestSubdomain, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Authentication failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void UpdateLeadsInternalAsync_AuthenticationError_ThrowsAmoCrmAuthenticationException()
    {
        List<UpdateLeadRequest> requests = [new UpdateLeadRequest(1) { Name = "Updated Lead" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthenticationException("Authentication failed"));

        AmoCrmAuthenticationException exception = Assert.ThrowsAsync<AmoCrmAuthenticationException>(async () =>
            await Client.UpdateLeadsInternalAsync(TestAccessToken, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Authentication failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetCompaniesAsync_AuthenticationError_ThrowsAmoCrmAuthenticationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthenticationException("Authentication failed"));

        AmoCrmAuthenticationException exception = Assert.ThrowsAsync<AmoCrmAuthenticationException>(async () =>
            await Client.GetCompaniesAsync(TestAccessToken, TestSubdomain));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Authentication failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetCompaniesInternalAsync_AuthenticationError_ThrowsAmoCrmAuthenticationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthenticationException("Authentication failed"));

        AmoCrmAuthenticationException exception = Assert.ThrowsAsync<AmoCrmAuthenticationException>(async () =>
            await Client.GetCompaniesInternalAsync(TestAccessToken));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Authentication failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void AddCompaniesAsync_AuthenticationError_ThrowsAmoCrmAuthenticationException()
    {
        List<AddCompanyRequest> requests = [new AddCompanyRequest { Name = "Company 1" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthenticationException("Authentication failed"));

        AmoCrmAuthenticationException exception = Assert.ThrowsAsync<AmoCrmAuthenticationException>(async () =>
            await Client.AddCompaniesAsync(TestAccessToken, TestSubdomain, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Authentication failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void AddCompaniesInternalAsync_AuthenticationError_ThrowsAmoCrmAuthenticationException()
    {
        List<AddCompanyRequest> requests = [new AddCompanyRequest { Name = "Company 1" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthenticationException("Authentication failed"));

        AmoCrmAuthenticationException exception = Assert.ThrowsAsync<AmoCrmAuthenticationException>(async () =>
            await Client.AddCompaniesInternalAsync(TestAccessToken, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Authentication failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void UpdateCompaniesAsync_AuthenticationError_ThrowsAmoCrmAuthenticationException()
    {
        List<UpdateCompanyRequest> requests = [new UpdateCompanyRequest(1) { Name = "Updated Company" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthenticationException("Authentication failed"));

        AmoCrmAuthenticationException exception = Assert.ThrowsAsync<AmoCrmAuthenticationException>(async () =>
            await Client.UpdateCompaniesAsync(TestAccessToken, TestSubdomain, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Authentication failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void UpdateCompaniesInternalAsync_AuthenticationError_ThrowsAmoCrmAuthenticationException()
    {
        List<UpdateCompanyRequest> requests = [new UpdateCompanyRequest(1) { Name = "Updated Company" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthenticationException("Authentication failed"));

        AmoCrmAuthenticationException exception = Assert.ThrowsAsync<AmoCrmAuthenticationException>(async () =>
            await Client.UpdateCompaniesInternalAsync(TestAccessToken, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Authentication failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetTasksAsync_AuthenticationError_ThrowsAmoCrmAuthenticationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthenticationException("Authentication failed"));

        AmoCrmAuthenticationException exception = Assert.ThrowsAsync<AmoCrmAuthenticationException>(async () =>
            await Client.GetTasksAsync(TestAccessToken, TestSubdomain));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Authentication failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetTasksInternalAsync_AuthenticationError_ThrowsAmoCrmAuthenticationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthenticationException("Authentication failed"));

        AmoCrmAuthenticationException exception = Assert.ThrowsAsync<AmoCrmAuthenticationException>(async () =>
            await Client.GetTasksInternalAsync(TestAccessToken));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Authentication failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void AddTasksAsync_AuthenticationError_ThrowsAmoCrmAuthenticationException()
    {
        List<AddTaskRequest> requests = [new AddTaskRequest("Task 1", 1234567890, EntityTypeEnum.Leads) { EntityId = 1 }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthenticationException("Authentication failed"));

        AmoCrmAuthenticationException exception = Assert.ThrowsAsync<AmoCrmAuthenticationException>(async () =>
            await Client.AddTasksAsync(TestAccessToken, TestSubdomain, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Authentication failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void AddTasksInternalAsync_AuthenticationError_ThrowsAmoCrmAuthenticationException()
    {
        List<AddTaskRequest> requests = [new AddTaskRequest("Task 1", 1234567890, EntityTypeEnum.Leads) { EntityId = 1 }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthenticationException("Authentication failed"));

        AmoCrmAuthenticationException exception = Assert.ThrowsAsync<AmoCrmAuthenticationException>(async () =>
            await Client.AddTasksInternalAsync(TestAccessToken, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Authentication failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void UpdateTasksAsync_AuthenticationError_ThrowsAmoCrmAuthenticationException()
    {
        List<UpdateTaskRequest> requests = [new UpdateTaskRequest(1, "Updated Task", 123123123123)];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthenticationException("Authentication failed"));

        AmoCrmAuthenticationException exception = Assert.ThrowsAsync<AmoCrmAuthenticationException>(async () =>
            await Client.UpdateTasksAsync(TestAccessToken, TestSubdomain, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Authentication failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void UpdateTasksInternalAsync_AuthenticationError_ThrowsAmoCrmAuthenticationException()
    {
        List<UpdateTaskRequest> requests = [new UpdateTaskRequest(1, "Updated Task", 123123123123)];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthenticationException("Authentication failed"));

        AmoCrmAuthenticationException exception = Assert.ThrowsAsync<AmoCrmAuthenticationException>(async () =>
            await Client.UpdateTasksInternalAsync(TestAccessToken, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Authentication failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetCustomersAsync_AuthenticationError_ThrowsAmoCrmAuthenticationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthenticationException("Authentication failed"));

        AmoCrmAuthenticationException exception = Assert.ThrowsAsync<AmoCrmAuthenticationException>(async () =>
            await Client.GetCustomersAsync(TestAccessToken, TestSubdomain));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Authentication failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetCustomersInternalAsync_AuthenticationError_ThrowsAmoCrmAuthenticationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthenticationException("Authentication failed"));

        AmoCrmAuthenticationException exception = Assert.ThrowsAsync<AmoCrmAuthenticationException>(async () =>
            await Client.GetCustomersInternalAsync(TestAccessToken));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Authentication failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void AddCustomersAsync_AuthenticationError_ThrowsAmoCrmAuthenticationException()
    {
        List<AddCustomerRequest> requests = [new AddCustomerRequest("Customer 1") { NextPrice = 500 }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthenticationException("Authentication failed"));

        AmoCrmAuthenticationException exception = Assert.ThrowsAsync<AmoCrmAuthenticationException>(async () =>
            await Client.AddCustomersAsync(TestAccessToken, TestSubdomain, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Authentication failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void AddCustomersInternalAsync_AuthenticationError_ThrowsAmoCrmAuthenticationException()
    {
        List<AddCustomerRequest> requests = [new AddCustomerRequest("Customer 1") { NextPrice = 500 }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthenticationException("Authentication failed"));

        AmoCrmAuthenticationException exception = Assert.ThrowsAsync<AmoCrmAuthenticationException>(async () =>
            await Client.AddCustomersInternalAsync(TestAccessToken, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Authentication failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void UpdateCustomersAsync_AuthenticationError_ThrowsAmoCrmAuthenticationException()
    {
        List<UpdateCustomerRequest> requests = [new UpdateCustomerRequest(1, "Updated Customer")];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthenticationException("Authentication failed"));

        AmoCrmAuthenticationException exception = Assert.ThrowsAsync<AmoCrmAuthenticationException>(async () =>
            await Client.UpdateCustomersAsync(TestAccessToken, TestSubdomain, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Authentication failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void UpdateCustomersInternalAsync_AuthenticationError_ThrowsAmoCrmAuthenticationException()
    {
        List<UpdateCustomerRequest> requests = [new UpdateCustomerRequest(1, "Updated Customer")];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthenticationException("Authentication failed"));

        AmoCrmAuthenticationException exception = Assert.ThrowsAsync<AmoCrmAuthenticationException>(async () =>
            await Client.UpdateCustomersInternalAsync(TestAccessToken, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Authentication failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetAccountAsync_AuthenticationError_ThrowsAmoCrmAuthenticationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<AccountModel>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthenticationException("Authentication failed"));

        AmoCrmAuthenticationException exception = Assert.ThrowsAsync<AmoCrmAuthenticationException>(async () =>
            await Client.GetAccountAsync(TestAccessToken, TestSubdomain));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Authentication failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<AccountModel>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetAccountInternalAsync_AuthenticationError_ThrowsAmoCrmAuthenticationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<AccountModel>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthenticationException("Authentication failed"));

        AmoCrmAuthenticationException exception = Assert.ThrowsAsync<AmoCrmAuthenticationException>(async () =>
            await Client.GetAccountInternalAsync(TestAccessToken));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Authentication failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<AccountModel>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetUsersAsync_AuthenticationError_ThrowsAmoCrmAuthenticationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthenticationException("Authentication failed"));

        AmoCrmAuthenticationException exception = Assert.ThrowsAsync<AmoCrmAuthenticationException>(async () =>
            await Client.GetUsersAsync(TestAccessToken, TestSubdomain));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Authentication failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetWidgetAsync_AuthenticationError_ThrowsAmoCrmAuthenticationException()
    {
        const string widgetCode = "test-widget";

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<Widget>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthenticationException("Authentication failed"));

        AmoCrmAuthenticationException exception = Assert.ThrowsAsync<AmoCrmAuthenticationException>(async () =>
            await Client.GetWidgetAsync(TestAccessToken, TestSubdomain, widgetCode));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Authentication failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<Widget>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetContactsAsync_AuthenticationError_ThrowsAmoCrmAuthenticationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthenticationException("Authentication failed"));

        AmoCrmAuthenticationException exception = Assert.ThrowsAsync<AmoCrmAuthenticationException>(async () =>
            await Client.GetContactsAsync(TestAccessToken, TestSubdomain));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Authentication failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetContactsInternalAsync_AuthenticationError_ThrowsAmoCrmAuthenticationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthenticationException("Authentication failed"));

        AmoCrmAuthenticationException exception = Assert.ThrowsAsync<AmoCrmAuthenticationException>(async () =>
            await Client.GetContactsInternalAsync(TestAccessToken));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Authentication failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetContactByIdAsync_AuthenticationError_ThrowsAmoCrmAuthenticationException()
    {
        const int contactId = 1;

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<Contact>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthenticationException("Authentication failed"));

        AmoCrmAuthenticationException exception = Assert.ThrowsAsync<AmoCrmAuthenticationException>(async () =>
            await Client.GetContactByIdAsync(TestAccessToken, TestSubdomain, contactId));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Authentication failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<Contact>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetContactByIdInternalAsync_AuthenticationError_ThrowsAmoCrmAuthenticationException()
    {
        const int contactId = 1;

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<Contact>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthenticationException("Authentication failed"));

        AmoCrmAuthenticationException exception = Assert.ThrowsAsync<AmoCrmAuthenticationException>(async () =>
            await Client.GetContactByIdInternalAsync(TestAccessToken, contactId));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Authentication failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<Contact>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void AddContactsAsync_AuthenticationError_ThrowsAmoCrmAuthenticationException()
    {
        List<AddContactRequest> requests = [new AddContactRequest { Name = "Contact 1", FirstName = "John" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthenticationException("Authentication failed"));

        AmoCrmAuthenticationException exception = Assert.ThrowsAsync<AmoCrmAuthenticationException>(async () =>
            await Client.AddContactsAsync(TestAccessToken, TestSubdomain, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Authentication failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void AddContactsInternalAsync_AuthenticationError_ThrowsAmoCrmAuthenticationException()
    {
        List<AddContactRequest> requests = [new AddContactRequest { Name = "Contact 1", FirstName = "John" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthenticationException("Authentication failed"));

        AmoCrmAuthenticationException exception = Assert.ThrowsAsync<AmoCrmAuthenticationException>(async () =>
            await Client.AddContactsInternalAsync(TestAccessToken, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Authentication failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void UpdateContactsAsync_AuthenticationError_ThrowsAmoCrmAuthenticationException()
    {
        List<UpdateContactRequest> requests = [new UpdateContactRequest(1) { Name = "Updated Contact" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthenticationException("Authentication failed"));

        AmoCrmAuthenticationException exception = Assert.ThrowsAsync<AmoCrmAuthenticationException>(async () =>
            await Client.UpdateContactsAsync(TestAccessToken, TestSubdomain, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Authentication failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void UpdateContactsInternalAsync_AuthenticationError_ThrowsAmoCrmAuthenticationException()
    {
        List<UpdateContactRequest> requests = [new UpdateContactRequest(1) { Name = "Updated Contact" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthenticationException("Authentication failed"));

        AmoCrmAuthenticationException exception = Assert.ThrowsAsync<AmoCrmAuthenticationException>(async () =>
            await Client.UpdateContactsInternalAsync(TestAccessToken, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Authentication failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetTransactionsAsync_AuthenticationError_ThrowsAmoCrmAuthenticationException()
    {
        const int customerId = 1;

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthenticationException("Authentication failed"));

        AmoCrmAuthenticationException exception = Assert.ThrowsAsync<AmoCrmAuthenticationException>(async () =>
            await Client.GetTransactionsAsync(TestAccessToken, TestSubdomain, customerId));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Authentication failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetTransactionsInternalAsync_AuthenticationError_ThrowsAmoCrmAuthenticationException()
    {
        const int customerId = 1;

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthenticationException("Authentication failed"));

        AmoCrmAuthenticationException exception = Assert.ThrowsAsync<AmoCrmAuthenticationException>(async () =>
            await Client.GetTransactionsInternalAsync(TestAccessToken, customerId));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Authentication failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void AddTransactionsAsync_AuthenticationError_ThrowsAmoCrmAuthenticationException()
    {
        const int customerId = 1;
        List<AddTransactionRequest> requests = [new AddTransactionRequest(1000) { CustomerId = customerId, Comment = "Transaction 1" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthenticationException("Authentication failed"));

        AmoCrmAuthenticationException exception = Assert.ThrowsAsync<AmoCrmAuthenticationException>(async () =>
            await Client.AddTransactionsAsync(TestAccessToken, TestSubdomain, customerId, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Authentication failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void AddTransactionsInternalAsync_AuthenticationError_ThrowsAmoCrmAuthenticationException()
    {
        const int customerId = 1;
        List<AddTransactionRequest> requests = [new AddTransactionRequest(1000) { CustomerId = customerId, Comment = "Transaction 1" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthenticationException("Authentication failed"));

        AmoCrmAuthenticationException exception = Assert.ThrowsAsync<AmoCrmAuthenticationException>(async () =>
            await Client.AddTransactionsInternalAsync(TestAccessToken, customerId, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Authentication failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetCustomFieldsAsync_AuthenticationError_ThrowsAmoCrmAuthenticationException()
    {
        EntityTypeEnum entityType = EntityTypeEnum.Leads;

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthenticationException("Authentication failed"));

        AmoCrmAuthenticationException exception = Assert.ThrowsAsync<AmoCrmAuthenticationException>(async () =>
            await Client.GetCustomFieldsAsync(TestAccessToken, TestSubdomain, entityType));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Authentication failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetCustomFieldsInternalAsync_AuthenticationError_ThrowsAmoCrmAuthenticationException()
    {
        EntityTypeEnum entityType = EntityTypeEnum.Leads;

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthenticationException("Authentication failed"));

        AmoCrmAuthenticationException exception = Assert.ThrowsAsync<AmoCrmAuthenticationException>(async () =>
            await Client.GetCustomFieldsInternalAsync(TestAccessToken, entityType));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Authentication failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetLeadStatusesAsync_AuthenticationError_ThrowsAmoCrmAuthenticationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthenticationException("Authentication failed"));

        AmoCrmAuthenticationException exception = Assert.ThrowsAsync<AmoCrmAuthenticationException>(async () =>
            await Client.GetLeadStatusesAsync(TestAccessToken, TestSubdomain));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Authentication failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetLeadStatusesInternalAsync_AuthenticationError_ThrowsAmoCrmAuthenticationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthenticationException("Authentication failed"));

        AmoCrmAuthenticationException exception = Assert.ThrowsAsync<AmoCrmAuthenticationException>(async () =>
            await Client.GetLeadStatusesInternalAsync(TestAccessToken));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Authentication failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetNotesAsync_AuthenticationError_ThrowsAmoCrmAuthenticationException()
    {
        EntityTypeEnum entityType = EntityTypeEnum.Leads;
        AmoCrmNoteTypeEnum noteType = AmoCrmNoteTypeEnum.Common;

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthenticationException("Authentication failed"));

        AmoCrmAuthenticationException exception = Assert.ThrowsAsync<AmoCrmAuthenticationException>(async () =>
            await Client.GetNotesAsync(TestAccessToken, TestSubdomain, entityType, noteType));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Authentication failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetNotesInternalAsync_AuthenticationError_ThrowsAmoCrmAuthenticationException()
    {
        EntityTypeEnum entityType = EntityTypeEnum.Leads;
        AmoCrmNoteTypeEnum noteType = AmoCrmNoteTypeEnum.Common;

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthenticationException("Authentication failed"));

        AmoCrmAuthenticationException exception = Assert.ThrowsAsync<AmoCrmAuthenticationException>(async () =>
            await Client.GetNotesInternalAsync(TestAccessToken, entityType, noteType));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Authentication failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void AddNotesAsync_AuthenticationError_ThrowsAmoCrmAuthenticationException()
    {
        EntityTypeEnum entityType = EntityTypeEnum.Leads;
        List<AddNoteRequest> requests = [new AddNoteRequest(1, AmoCrmNoteTypeEnum.Common) { Parameters = new NoteDetails { Text = "Note 1" } }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthenticationException("Authentication failed"));

        AmoCrmAuthenticationException exception = Assert.ThrowsAsync<AmoCrmAuthenticationException>(async () =>
            await Client.AddNotesAsync(TestAccessToken, TestSubdomain, entityType, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Authentication failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void AddNotesInternalAsync_AuthenticationError_ThrowsAmoCrmAuthenticationException()
    {
        EntityTypeEnum entityType = EntityTypeEnum.Leads;
        List<AddNoteRequest> requests = [new AddNoteRequest(1, AmoCrmNoteTypeEnum.Common) { Parameters = new NoteDetails { Text = "Note 1" } }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthenticationException("Authentication failed"));

        AmoCrmAuthenticationException exception = Assert.ThrowsAsync<AmoCrmAuthenticationException>(async () =>
            await Client.AddNotesInternalAsync(TestAccessToken, entityType, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Authentication failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public async Task GetLeadStatusesAsync_ValidParameters_ReturnsLeadStatusesAsync()
    {
        List<LeadStatus> statuses1 = [new LeadStatus { Id = 1, Name = "Status 1" }];
        List<LeadStatus> statuses2 = [new LeadStatus { Id = 2, Name = "Status 2" }];
        EntitiesResponse response1 = new()
        {
            Embedded = new EmbeddedEntitiesResponse
            {
                Pipelines = [new Pipeline() { AvailableStatuses = new PipelineStatusesContainer { Statuses = statuses1.ToArray() } }]
            },
            Links = new LinksResponse { Next = new Link { Uri = "https://example.amocrm.ru/api/v4/pipelines?page=2&limit=1" } }
        };
        EntitiesResponse response2 = new()
        {
            Embedded = new EmbeddedEntitiesResponse
            {
                Pipelines = [new Pipeline() { AvailableStatuses = new PipelineStatusesContainer { Statuses = statuses2.ToArray() } }]
            }
        };

        ResponseHandlerMock
            .SetupSequence(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response1)
            .ReturnsAsync(response2);

        IReadOnlyCollection<LeadStatus> result = await Client.GetLeadStatusesAsync(TestAccessToken, TestSubdomain);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result.First().Id, Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("Status 1"));
            Assert.That(result.Last().Id, Is.EqualTo(2));
            Assert.That(result.Last().Name, Is.EqualTo("Status 2"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Test]
    public async Task GetLeadStatusesInternalAsync_ValidParameters_ReturnsLeadStatusesAsync()
    {
        List<LeadStatus> statuses1 = [new LeadStatus { Id = 1, Name = "Status 1" }];
        List<LeadStatus> statuses2 = [new LeadStatus { Id = 2, Name = "Status 2" }];
        EntitiesResponse response1 = new()
        {
            Embedded = new EmbeddedEntitiesResponse
            {
                Pipelines = [new Pipeline() { AvailableStatuses = new PipelineStatusesContainer { Statuses = statuses1.ToArray() } }]
            },
            Links = new LinksResponse { Next = new Link { Uri = "https://example.amocrm.ru/api/v4/pipelines?page=2&limit=1" } }
        };
        EntitiesResponse response2 = new()
        {
            Embedded = new EmbeddedEntitiesResponse
            {
                Pipelines = [new Pipeline() { AvailableStatuses = new PipelineStatusesContainer { Statuses = statuses2.ToArray() } }]
            }
        };

        ResponseHandlerMock
            .SetupSequence(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response1)
            .ReturnsAsync(response2);

        IReadOnlyCollection<LeadStatus> result = await Client.GetLeadStatusesInternalAsync(TestAccessToken);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result.First().Id, Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("Status 1"));
            Assert.That(result.Last().Id, Is.EqualTo(2));
            Assert.That(result.Last().Name, Is.EqualTo("Status 2"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Test]
    public async Task GetNotesAsync_ValidParameters_ReturnsNotesAsync()
    {
        EntityTypeEnum entityType = EntityTypeEnum.Leads;
        AmoCrmNoteTypeEnum noteType = AmoCrmNoteTypeEnum.Common;
        List<Note> notes1 = [new Note { Id = 1, EntityId = 1, NoteTypeName = "common" }];
        List<Note> notes2 = [new Note { Id = 2, EntityId = 2, NoteTypeName = "common" }];
        EntitiesResponse response1 = new() { Embedded = new EmbeddedEntitiesResponse { Notes = notes1.ToArray() }, Links = new LinksResponse { Next = new Link { Uri = "https://example.amocrm.ru/api/v4/notes?page=2&limit=1" } } };
        EntitiesResponse response2 = new() { Embedded = new EmbeddedEntitiesResponse { Notes = notes2.ToArray() } };

        ResponseHandlerMock
            .SetupSequence(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response1)
            .ReturnsAsync(response2);

        IReadOnlyCollection<Note> result = await Client.GetNotesAsync(TestAccessToken, TestSubdomain, entityType, noteType);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result.First().Id, Is.EqualTo(1));
            Assert.That(result.First().EntityId, Is.EqualTo(1));
            Assert.That(result.Last().Id, Is.EqualTo(2));
            Assert.That(result.Last().EntityId, Is.EqualTo(2));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Test]
    public async Task GetNotesInternalAsync_ValidParameters_ReturnsNotesAsync()
    {
        EntityTypeEnum entityType = EntityTypeEnum.Leads;
        AmoCrmNoteTypeEnum noteType = AmoCrmNoteTypeEnum.Common;
        List<Note> notes1 = [new Note { Id = 1, EntityId = 1, NoteTypeName = "common" }];
        List<Note> notes2 = [new Note { Id = 2, EntityId = 2, NoteTypeName = "common" }];
        EntitiesResponse response1 = new() { Embedded = new EmbeddedEntitiesResponse { Notes = notes1.ToArray() }, Links = new LinksResponse { Next = new Link { Uri = "https://example.amocrm.ru/api/v4/notes?page=2&limit=1" } } };
        EntitiesResponse response2 = new() { Embedded = new EmbeddedEntitiesResponse { Notes = notes2.ToArray() } };

        ResponseHandlerMock
            .SetupSequence(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response1)
            .ReturnsAsync(response2);

        IReadOnlyCollection<Note> result = await Client.GetNotesInternalAsync(TestAccessToken, entityType, noteType);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result.First().Id, Is.EqualTo(1));
            Assert.That(result.First().EntityId, Is.EqualTo(1));
            Assert.That(result.Last().Id, Is.EqualTo(2));
            Assert.That(result.Last().EntityId, Is.EqualTo(2));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Test]
    public async Task AddNotesAsync_ValidRequests_ReturnsCreatedNotesAsync()
    {
        EntityTypeEnum entityType = EntityTypeEnum.Leads;
        List<AddNoteRequest> requests = [new AddNoteRequest(1, AmoCrmNoteTypeEnum.Common) { Parameters = new NoteDetails { Text = "Note 1" } }];
        List<Note> expectedNotes = [new Note { Id = 1, EntityId = 1, NoteTypeName = "common", Parameters = new NoteDetails { Text = "Note 1" } }];
        EntitiesResponse response = new() { Embedded = new EmbeddedEntitiesResponse { Notes = expectedNotes.ToArray() } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Note> result = await Client.AddNotesAsync(TestAccessToken, TestSubdomain, entityType, requests);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result.First().Id, Is.EqualTo(1));
            Assert.That(result.First().EntityId, Is.EqualTo(1));
            Assert.That(result.First().Parameters!.Text, Is.EqualTo("Note 1"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public async Task AddNotesInternalAsync_ValidRequests_ReturnsCreatedNotesAsync()
    {
        EntityTypeEnum entityType = EntityTypeEnum.Leads;
        List<AddNoteRequest> requests = [new AddNoteRequest(1, AmoCrmNoteTypeEnum.Common) { Parameters = new NoteDetails { Text = "Note 1" } }];
        List<Note> expectedNotes = [new Note { Id = 1, EntityId = 1, NoteTypeName = "common", Parameters = new NoteDetails { Text = "Note 1" } }];
        EntitiesResponse response = new() { Embedded = new EmbeddedEntitiesResponse { Notes = expectedNotes.ToArray() } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Note> result = await Client.AddNotesInternalAsync(TestAccessToken, entityType, requests);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result.First().Id, Is.EqualTo(1));
            Assert.That(result.First().EntityId, Is.EqualTo(1));
            Assert.That(result.First().Parameters!.Text, Is.EqualTo("Note 1"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void AuthorizeAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        const string authCode = "auth-code";
        const string redirectDomain = "https://example.com";

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        AmoCrmHttpException exception = Assert.ThrowsAsync<AmoCrmHttpException>(async () =>
            await Client.AuthorizeAsync(TestSubdomain, authCode, redirectDomain));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("HTTP request failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void AuthorizeInternalAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        AmoCrmHttpException exception = Assert.ThrowsAsync<AmoCrmHttpException>(async () =>
            await Client.AuthorizeInternalAsync());

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("HTTP request failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void RefreshTokenAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        const string refreshToken = "refresh-token";
        const string redirectDomain = "https://example.com";

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        AmoCrmHttpException exception = Assert.ThrowsAsync<AmoCrmHttpException>(async () =>
            await Client.RefreshTokenAsync(TestSubdomain, refreshToken, redirectDomain));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("HTTP request failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void RefreshTokenInternalAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        const string refreshToken = "internal-refresh-token";

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        AmoCrmHttpException exception = Assert.ThrowsAsync<AmoCrmHttpException>(async () =>
            await Client.RefreshTokenInternalAsync(refreshToken));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("HTTP request failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetLeadsAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        const string accessToken = "access-token";

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        AmoCrmHttpException exception = Assert.ThrowsAsync<AmoCrmHttpException>(async () =>
            await Client.GetLeadsAsync(accessToken, TestSubdomain));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("HTTP request failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetLeadsInternalAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        const string accessToken = "access-token";

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        AmoCrmHttpException exception = Assert.ThrowsAsync<AmoCrmHttpException>(async () =>
            await Client.GetLeadsInternalAsync(accessToken));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("HTTP request failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void AddLeadsAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        List<AddLeadRequest> requests = [new AddLeadRequest { Name = "Lead 1" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        AmoCrmHttpException exception = Assert.ThrowsAsync<AmoCrmHttpException>(async () =>
            await Client.AddLeadsAsync(TestAccessToken, TestSubdomain, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("HTTP request failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void AddLeadsInternalAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        List<AddLeadRequest> requests = [new AddLeadRequest { Name = "Lead 1" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        AmoCrmHttpException exception = Assert.ThrowsAsync<AmoCrmHttpException>(async () =>
            await Client.AddLeadsInternalAsync(TestAccessToken, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("HTTP request failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void UpdateLeadsAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        List<UpdateLeadRequest> requests = [new UpdateLeadRequest(1) { Name = "Updated Lead" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        AmoCrmHttpException exception = Assert.ThrowsAsync<AmoCrmHttpException>(async () =>
            await Client.UpdateLeadsAsync(TestAccessToken, TestSubdomain, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("HTTP request failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void UpdateLeadsInternalAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        List<UpdateLeadRequest> requests = [new UpdateLeadRequest(1) { Name = "Updated Lead" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        AmoCrmHttpException exception = Assert.ThrowsAsync<AmoCrmHttpException>(async () =>
            await Client.UpdateLeadsInternalAsync(TestAccessToken, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("HTTP request failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetCompaniesAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        AmoCrmHttpException exception = Assert.ThrowsAsync<AmoCrmHttpException>(async () =>
            await Client.GetCompaniesAsync(TestAccessToken, TestSubdomain));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("HTTP request failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetCompaniesInternalAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        AmoCrmHttpException exception = Assert.ThrowsAsync<AmoCrmHttpException>(async () =>
            await Client.GetCompaniesInternalAsync(TestAccessToken));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("HTTP request failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void AddCompaniesAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        List<AddCompanyRequest> requests = [new AddCompanyRequest { Name = "Company 1" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        AmoCrmHttpException exception = Assert.ThrowsAsync<AmoCrmHttpException>(async () =>
            await Client.AddCompaniesAsync(TestAccessToken, TestSubdomain, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("HTTP request failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void AddCompaniesInternalAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        List<AddCompanyRequest> requests = [new AddCompanyRequest { Name = "Company 1" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        AmoCrmHttpException exception = Assert.ThrowsAsync<AmoCrmHttpException>(async () =>
            await Client.AddCompaniesInternalAsync(TestAccessToken, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("HTTP request failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void UpdateCompaniesAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        List<UpdateCompanyRequest> requests = [new UpdateCompanyRequest(1) { Name = "Updated Company" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        AmoCrmHttpException exception = Assert.ThrowsAsync<AmoCrmHttpException>(async () =>
            await Client.UpdateCompaniesAsync(TestAccessToken, TestSubdomain, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("HTTP request failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void UpdateCompaniesInternalAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        List<UpdateCompanyRequest> requests = [new UpdateCompanyRequest(1) { Name = "Updated Company" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        AmoCrmHttpException exception = Assert.ThrowsAsync<AmoCrmHttpException>(async () =>
            await Client.UpdateCompaniesInternalAsync(TestAccessToken, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("HTTP request failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetTasksAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        AmoCrmHttpException exception = Assert.ThrowsAsync<AmoCrmHttpException>(async () =>
            await Client.GetTasksAsync(TestAccessToken, TestSubdomain));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("HTTP request failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetTasksInternalAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        AmoCrmHttpException exception = Assert.ThrowsAsync<AmoCrmHttpException>(async () =>
            await Client.GetTasksInternalAsync(TestAccessToken));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("HTTP request failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void AddTasksAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        List<AddTaskRequest> requests = [new AddTaskRequest("Task 1", 1234567890, EntityTypeEnum.Leads) { EntityId = 1 }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        AmoCrmHttpException exception = Assert.ThrowsAsync<AmoCrmHttpException>(async () =>
            await Client.AddTasksAsync(TestAccessToken, TestSubdomain, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("HTTP request failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void AddTasksInternalAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        List<AddTaskRequest> requests = [new AddTaskRequest("Task 1", 1234567890, EntityTypeEnum.Leads) { EntityId = 1 }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        AmoCrmHttpException exception = Assert.ThrowsAsync<AmoCrmHttpException>(async () =>
            await Client.AddTasksInternalAsync(TestAccessToken, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("HTTP request failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void UpdateTasksAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        List<UpdateTaskRequest> requests = [new UpdateTaskRequest(1, "Updated Task", 123123123123)];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        AmoCrmHttpException exception = Assert.ThrowsAsync<AmoCrmHttpException>(async () =>
            await Client.UpdateTasksAsync(TestAccessToken, TestSubdomain, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("HTTP request failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void UpdateTasksInternalAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        List<UpdateTaskRequest> requests = [new UpdateTaskRequest(1, "Updated Task", 123123123123)];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        AmoCrmHttpException exception = Assert.ThrowsAsync<AmoCrmHttpException>(async () =>
            await Client.UpdateTasksInternalAsync(TestAccessToken, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("HTTP request failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetCustomersAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        AmoCrmHttpException exception = Assert.ThrowsAsync<AmoCrmHttpException>(async () =>
            await Client.GetCustomersAsync(TestAccessToken, TestSubdomain));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("HTTP request failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetCustomersInternalAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        AmoCrmHttpException exception = Assert.ThrowsAsync<AmoCrmHttpException>(async () =>
            await Client.GetCustomersInternalAsync(TestAccessToken));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("HTTP request failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void AddCustomersAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        List<AddCustomerRequest> requests = [new AddCustomerRequest("Customer 1") { NextPrice = 500 }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        AmoCrmHttpException exception = Assert.ThrowsAsync<AmoCrmHttpException>(async () =>
            await Client.AddCustomersAsync(TestAccessToken, TestSubdomain, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("HTTP request failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void AddCustomersInternalAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        List<AddCustomerRequest> requests = [new AddCustomerRequest("Customer 1") { NextPrice = 500 }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        AmoCrmHttpException exception = Assert.ThrowsAsync<AmoCrmHttpException>(async () =>
            await Client.AddCustomersInternalAsync(TestAccessToken, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("HTTP request failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void UpdateCustomersAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        List<UpdateCustomerRequest> requests = [new UpdateCustomerRequest(1, "Updated Customer")];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        AmoCrmHttpException exception = Assert.ThrowsAsync<AmoCrmHttpException>(async () =>
            await Client.UpdateCustomersAsync(TestAccessToken, TestSubdomain, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("HTTP request failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void UpdateCustomersInternalAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        List<UpdateCustomerRequest> requests = [new UpdateCustomerRequest(1, "Updated Customer")];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        AmoCrmHttpException exception = Assert.ThrowsAsync<AmoCrmHttpException>(async () =>
            await Client.UpdateCustomersInternalAsync(TestAccessToken, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("HTTP request failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetAccountAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<AccountModel>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        AmoCrmHttpException exception = Assert.ThrowsAsync<AmoCrmHttpException>(async () =>
            await Client.GetAccountAsync(TestAccessToken, TestSubdomain));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("HTTP request failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<AccountModel>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetAccountInternalAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<AccountModel>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        AmoCrmHttpException exception = Assert.ThrowsAsync<AmoCrmHttpException>(async () =>
            await Client.GetAccountInternalAsync(TestAccessToken));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("HTTP request failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<AccountModel>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetUsersAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        AmoCrmHttpException exception = Assert.ThrowsAsync<AmoCrmHttpException>(async () =>
            await Client.GetUsersAsync(TestAccessToken, TestSubdomain));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("HTTP request failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetWidgetAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        const string widgetCode = "test-widget";

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<Widget>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        AmoCrmHttpException exception = Assert.ThrowsAsync<AmoCrmHttpException>(async () =>
            await Client.GetWidgetAsync(TestAccessToken, TestSubdomain, widgetCode));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("HTTP request failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<Widget>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetContactsAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        AmoCrmHttpException exception = Assert.ThrowsAsync<AmoCrmHttpException>(async () =>
            await Client.GetContactsAsync(TestAccessToken, TestSubdomain));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("HTTP request failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetContactsInternalAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        AmoCrmHttpException exception = Assert.ThrowsAsync<AmoCrmHttpException>(async () =>
            await Client.GetContactsInternalAsync(TestAccessToken));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("HTTP request failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetContactByIdAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        const int contactId = 1;

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<Contact>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        AmoCrmHttpException exception = Assert.ThrowsAsync<AmoCrmHttpException>(async () =>
            await Client.GetContactByIdAsync(TestAccessToken, TestSubdomain, contactId));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("HTTP request failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<Contact>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetContactByIdInternalAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        const int contactId = 1;

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<Contact>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        AmoCrmHttpException exception = Assert.ThrowsAsync<AmoCrmHttpException>(async () =>
            await Client.GetContactByIdInternalAsync(TestAccessToken, contactId));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("HTTP request failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<Contact>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void AddContactsAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        List<AddContactRequest> requests = [new AddContactRequest { Name = "Contact 1", FirstName = "John" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        AmoCrmHttpException exception = Assert.ThrowsAsync<AmoCrmHttpException>(async () =>
            await Client.AddContactsAsync(TestAccessToken, TestSubdomain, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("HTTP request failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void AddContactsInternalAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        List<AddContactRequest> requests = [new AddContactRequest { Name = "Contact 1", FirstName = "John" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        AmoCrmHttpException exception = Assert.ThrowsAsync<AmoCrmHttpException>(async () =>
            await Client.AddContactsInternalAsync(TestAccessToken, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("HTTP request failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void UpdateContactsAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        List<UpdateContactRequest> requests = [new UpdateContactRequest(1) { Name = "Updated Contact" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        AmoCrmHttpException exception = Assert.ThrowsAsync<AmoCrmHttpException>(async () =>
            await Client.UpdateContactsAsync(TestAccessToken, TestSubdomain, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("HTTP request failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void UpdateContactsInternalAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        List<UpdateContactRequest> requests = [new UpdateContactRequest(1) { Name = "Updated Contact" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        AmoCrmHttpException exception = Assert.ThrowsAsync<AmoCrmHttpException>(async () =>
            await Client.UpdateContactsInternalAsync(TestAccessToken, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("HTTP request failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetTransactionsAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        const int customerId = 1;

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        AmoCrmHttpException exception = Assert.ThrowsAsync<AmoCrmHttpException>(async () =>
            await Client.GetTransactionsAsync(TestAccessToken, TestSubdomain, customerId));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("HTTP request failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetTransactionsInternalAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        const int customerId = 1;

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        AmoCrmHttpException exception = Assert.ThrowsAsync<AmoCrmHttpException>(async () =>
            await Client.GetTransactionsInternalAsync(TestAccessToken, customerId));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("HTTP request failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void AddTransactionsAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        const int customerId = 1;
        List<AddTransactionRequest> requests = [new AddTransactionRequest(1000) { CustomerId = customerId, Comment = "Transaction 1" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        AmoCrmHttpException exception = Assert.ThrowsAsync<AmoCrmHttpException>(async () =>
            await Client.AddTransactionsAsync(TestAccessToken, TestSubdomain, customerId, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("HTTP request failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void AddTransactionsInternalAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        const int customerId = 1;
        List<AddTransactionRequest> requests = [new AddTransactionRequest(1000) { CustomerId = customerId, Comment = "Transaction 1" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        AmoCrmHttpException exception = Assert.ThrowsAsync<AmoCrmHttpException>(async () =>
            await Client.AddTransactionsInternalAsync(TestAccessToken, customerId, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("HTTP request failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetCustomFieldsAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        EntityTypeEnum entityType = EntityTypeEnum.Leads;

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        AmoCrmHttpException exception = Assert.ThrowsAsync<AmoCrmHttpException>(async () =>
            await Client.GetCustomFieldsAsync(TestAccessToken, TestSubdomain, entityType));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("HTTP request failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetCustomFieldsInternalAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        EntityTypeEnum entityType = EntityTypeEnum.Leads;

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        AmoCrmHttpException exception = Assert.ThrowsAsync<AmoCrmHttpException>(async () =>
            await Client.GetCustomFieldsInternalAsync(TestAccessToken, entityType));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("HTTP request failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetLeadStatusesAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        AmoCrmHttpException exception = Assert.ThrowsAsync<AmoCrmHttpException>(async () =>
            await Client.GetLeadStatusesAsync(TestAccessToken, TestSubdomain));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("HTTP request failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetLeadStatusesInternalAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        AmoCrmHttpException exception = Assert.ThrowsAsync<AmoCrmHttpException>(async () =>
            await Client.GetLeadStatusesInternalAsync(TestAccessToken));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("HTTP request failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetNotesAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        EntityTypeEnum entityType = EntityTypeEnum.Leads;
        AmoCrmNoteTypeEnum noteType = AmoCrmNoteTypeEnum.Common;

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        AmoCrmHttpException exception = Assert.ThrowsAsync<AmoCrmHttpException>(async () =>
            await Client.GetNotesAsync(TestAccessToken, TestSubdomain, entityType, noteType));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("HTTP request failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetNotesInternalAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        EntityTypeEnum entityType = EntityTypeEnum.Leads;
        AmoCrmNoteTypeEnum noteType = AmoCrmNoteTypeEnum.Common;

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        AmoCrmHttpException exception = Assert.ThrowsAsync<AmoCrmHttpException>(async () =>
            await Client.GetNotesInternalAsync(TestAccessToken, entityType, noteType));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("HTTP request failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void AddNotesAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        EntityTypeEnum entityType = EntityTypeEnum.Leads;
        List<AddNoteRequest> requests = [new AddNoteRequest(1, AmoCrmNoteTypeEnum.Common) { Parameters = new NoteDetails { Text = "Note 1" } }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        AmoCrmHttpException exception = Assert.ThrowsAsync<AmoCrmHttpException>(async () =>
            await Client.AddNotesAsync(TestAccessToken, TestSubdomain, entityType, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("HTTP request failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void AddNotesInternalAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        EntityTypeEnum entityType = EntityTypeEnum.Leads;
        List<AddNoteRequest> requests = [new AddNoteRequest(1, AmoCrmNoteTypeEnum.Common) { Parameters = new NoteDetails { Text = "Note 1" } }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        AmoCrmHttpException exception = Assert.ThrowsAsync<AmoCrmHttpException>(async () =>
            await Client.AddNotesInternalAsync(TestAccessToken, entityType, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("HTTP request failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void AuthorizeAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        const string authCode = "auth-code";
        const string redirectDomain = "https://example.com";

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        AmoCrmValidationException exception = Assert.ThrowsAsync<AmoCrmValidationException>(async () =>
            await Client.AuthorizeAsync(TestSubdomain, authCode, redirectDomain));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Validation failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void AuthorizeInternalAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        AmoCrmValidationException exception = Assert.ThrowsAsync<AmoCrmValidationException>(async () =>
            await Client.AuthorizeInternalAsync());

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Validation failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void RefreshTokenAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        const string refreshToken = "refresh-token";
        const string redirectDomain = "https://example.com";

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        AmoCrmValidationException exception = Assert.ThrowsAsync<AmoCrmValidationException>(async () =>
            await Client.RefreshTokenAsync(TestSubdomain, refreshToken, redirectDomain));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Validation failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void RefreshTokenInternalAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        const string refreshToken = "internal-refresh-token";

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        AmoCrmValidationException exception = Assert.ThrowsAsync<AmoCrmValidationException>(async () =>
            await Client.RefreshTokenInternalAsync(refreshToken));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Validation failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetLeadsAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        AmoCrmValidationException exception = Assert.ThrowsAsync<AmoCrmValidationException>(async () =>
            await Client.GetLeadsAsync(TestAccessToken, TestSubdomain));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Validation failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetLeadsInternalAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        AmoCrmValidationException exception = Assert.ThrowsAsync<AmoCrmValidationException>(async () =>
            await Client.GetLeadsInternalAsync(TestAccessToken));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Validation failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void AddLeadsAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        List<AddLeadRequest> requests = [new AddLeadRequest { Name = "Lead 1" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        AmoCrmValidationException exception = Assert.ThrowsAsync<AmoCrmValidationException>(async () =>
            await Client.AddLeadsAsync(TestAccessToken, TestSubdomain, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Validation failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void AddLeadsInternalAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        List<AddLeadRequest> requests = [new AddLeadRequest { Name = "Lead 1" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        AmoCrmValidationException exception = Assert.ThrowsAsync<AmoCrmValidationException>(async () =>
            await Client.AddLeadsInternalAsync(TestAccessToken, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Validation failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void UpdateLeadsAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        List<UpdateLeadRequest> requests = [new UpdateLeadRequest(1) { Name = "Updated Lead" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        AmoCrmValidationException exception = Assert.ThrowsAsync<AmoCrmValidationException>(async () =>
            await Client.UpdateLeadsAsync(TestAccessToken, TestSubdomain, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Validation failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void UpdateLeadsInternalAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        List<UpdateLeadRequest> requests = [new UpdateLeadRequest(1) { Name = "Updated Lead" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        AmoCrmValidationException exception = Assert.ThrowsAsync<AmoCrmValidationException>(async () =>
            await Client.UpdateLeadsInternalAsync(TestAccessToken, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Validation failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetCompaniesAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        AmoCrmValidationException exception = Assert.ThrowsAsync<AmoCrmValidationException>(async () =>
            await Client.GetCompaniesAsync(TestAccessToken, TestSubdomain));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Validation failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetCompaniesInternalAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        AmoCrmValidationException exception = Assert.ThrowsAsync<AmoCrmValidationException>(async () =>
            await Client.GetCompaniesInternalAsync(TestAccessToken));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Validation failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void AddCompaniesAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        List<AddCompanyRequest> requests = [new AddCompanyRequest { Name = "Company 1" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        AmoCrmValidationException exception = Assert.ThrowsAsync<AmoCrmValidationException>(async () =>
            await Client.AddCompaniesAsync(TestAccessToken, TestSubdomain, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Validation failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void AddCompaniesInternalAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        List<AddCompanyRequest> requests = [new AddCompanyRequest { Name = "Company 1" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        AmoCrmValidationException exception = Assert.ThrowsAsync<AmoCrmValidationException>(async () =>
            await Client.AddCompaniesInternalAsync(TestAccessToken, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Validation failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void UpdateCompaniesAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        List<UpdateCompanyRequest> requests = [new UpdateCompanyRequest(1) { Name = "Updated Company" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        AmoCrmValidationException exception = Assert.ThrowsAsync<AmoCrmValidationException>(async () =>
            await Client.UpdateCompaniesAsync(TestAccessToken, TestSubdomain, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Validation failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void UpdateCompaniesInternalAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        List<UpdateCompanyRequest> requests = [new UpdateCompanyRequest(1) { Name = "Updated Company" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        AmoCrmValidationException exception = Assert.ThrowsAsync<AmoCrmValidationException>(async () =>
            await Client.UpdateCompaniesInternalAsync(TestAccessToken, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Validation failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetTasksAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        AmoCrmValidationException exception = Assert.ThrowsAsync<AmoCrmValidationException>(async () =>
            await Client.GetTasksAsync(TestAccessToken, TestSubdomain));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Validation failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetTasksInternalAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        AmoCrmValidationException exception = Assert.ThrowsAsync<AmoCrmValidationException>(async () =>
            await Client.GetTasksInternalAsync(TestAccessToken));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Validation failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void AddTasksAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        List<AddTaskRequest> requests = [new AddTaskRequest("Task 1", 1234567890, EntityTypeEnum.Leads) { EntityId = 1 }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        AmoCrmValidationException exception = Assert.ThrowsAsync<AmoCrmValidationException>(async () =>
            await Client.AddTasksAsync(TestAccessToken, TestSubdomain, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Validation failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void AddTasksInternalAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        List<AddTaskRequest> requests = [new AddTaskRequest("Task 1", 1234567890, EntityTypeEnum.Leads) { EntityId = 1 }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        AmoCrmValidationException exception = Assert.ThrowsAsync<AmoCrmValidationException>(async () =>
            await Client.AddTasksInternalAsync(TestAccessToken, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Validation failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void UpdateTasksAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        List<UpdateTaskRequest> requests = [new UpdateTaskRequest(1, "Updated Task", 123123123123)];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        AmoCrmValidationException exception = Assert.ThrowsAsync<AmoCrmValidationException>(async () =>
            await Client.UpdateTasksAsync(TestAccessToken, TestSubdomain, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Validation failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void UpdateTasksInternalAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        List<UpdateTaskRequest> requests = [new UpdateTaskRequest(1, "Updated Task", 123123123123)];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        AmoCrmValidationException exception = Assert.ThrowsAsync<AmoCrmValidationException>(async () =>
            await Client.UpdateTasksInternalAsync(TestAccessToken, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Validation failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetCustomersAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        AmoCrmValidationException exception = Assert.ThrowsAsync<AmoCrmValidationException>(async () =>
            await Client.GetCustomersAsync(TestAccessToken, TestSubdomain));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Validation failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetCustomersInternalAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        AmoCrmValidationException exception = Assert.ThrowsAsync<AmoCrmValidationException>(async () =>
            await Client.GetCustomersInternalAsync(TestAccessToken));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Validation failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void AddCustomersAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        List<AddCustomerRequest> requests = [new AddCustomerRequest("Customer 1") { NextPrice = 500 }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        AmoCrmValidationException exception = Assert.ThrowsAsync<AmoCrmValidationException>(async () =>
            await Client.AddCustomersAsync(TestAccessToken, TestSubdomain, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Validation failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void AddCustomersInternalAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        List<AddCustomerRequest> requests = [new AddCustomerRequest("Customer 1") { NextPrice = 500 }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        AmoCrmValidationException exception = Assert.ThrowsAsync<AmoCrmValidationException>(async () =>
            await Client.AddCustomersInternalAsync(TestAccessToken, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Validation failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void UpdateCustomersAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        List<UpdateCustomerRequest> requests = [new UpdateCustomerRequest(1, "Updated Customer")];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        AmoCrmValidationException exception = Assert.ThrowsAsync<AmoCrmValidationException>(async () =>
            await Client.UpdateCustomersAsync(TestAccessToken, TestSubdomain, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Validation failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void UpdateCustomersInternalAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        List<UpdateCustomerRequest> requests = [new UpdateCustomerRequest(1, "Updated Customer")];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        AmoCrmValidationException exception = Assert.ThrowsAsync<AmoCrmValidationException>(async () =>
            await Client.UpdateCustomersInternalAsync(TestAccessToken, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Validation failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetAccountAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<AccountModel>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        AmoCrmValidationException exception = Assert.ThrowsAsync<AmoCrmValidationException>(async () =>
            await Client.GetAccountAsync(TestAccessToken, TestSubdomain));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Validation failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<AccountModel>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetAccountInternalAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<AccountModel>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        AmoCrmValidationException exception = Assert.ThrowsAsync<AmoCrmValidationException>(async () =>
            await Client.GetAccountInternalAsync(TestAccessToken));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Validation failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<AccountModel>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetUsersAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        AmoCrmValidationException exception = Assert.ThrowsAsync<AmoCrmValidationException>(async () =>
            await Client.GetUsersAsync(TestAccessToken, TestSubdomain));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Validation failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetWidgetAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        const string widgetCode = "test-widget";

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<Widget>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        AmoCrmValidationException exception = Assert.ThrowsAsync<AmoCrmValidationException>(async () =>
            await Client.GetWidgetAsync(TestAccessToken, TestSubdomain, widgetCode));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Validation failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<Widget>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetContactsAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        AmoCrmValidationException exception = Assert.ThrowsAsync<AmoCrmValidationException>(async () =>
            await Client.GetContactsAsync(TestAccessToken, TestSubdomain));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Validation failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetContactsInternalAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        AmoCrmValidationException exception = Assert.ThrowsAsync<AmoCrmValidationException>(async () =>
            await Client.GetContactsInternalAsync(TestAccessToken));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Validation failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetContactByIdAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        const int contactId = 1;

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<Contact>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        AmoCrmValidationException exception = Assert.ThrowsAsync<AmoCrmValidationException>(async () =>
            await Client.GetContactByIdAsync(TestAccessToken, TestSubdomain, contactId));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Validation failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<Contact>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetContactByIdInternalAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        const int contactId = 1;

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<Contact>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        AmoCrmValidationException exception = Assert.ThrowsAsync<AmoCrmValidationException>(async () =>
            await Client.GetContactByIdInternalAsync(TestAccessToken, contactId));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Validation failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<Contact>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void AddContactsAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        List<AddContactRequest> requests = [new AddContactRequest { Name = "Contact 1", FirstName = "John" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        AmoCrmValidationException exception = Assert.ThrowsAsync<AmoCrmValidationException>(async () =>
            await Client.AddContactsAsync(TestAccessToken, TestSubdomain, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Validation failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void AddContactsInternalAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        List<AddContactRequest> requests = [new AddContactRequest { Name = "Contact 1", FirstName = "John" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        AmoCrmValidationException exception = Assert.ThrowsAsync<AmoCrmValidationException>(async () =>
            await Client.AddContactsInternalAsync(TestAccessToken, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Validation failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void UpdateContactsAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        List<UpdateContactRequest> requests = [new UpdateContactRequest(1) { Name = "Updated Contact" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        AmoCrmValidationException exception = Assert.ThrowsAsync<AmoCrmValidationException>(async () =>
            await Client.UpdateContactsAsync(TestAccessToken, TestSubdomain, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Validation failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void UpdateContactsInternalAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        List<UpdateContactRequest> requests = [new UpdateContactRequest(1) { Name = "Updated Contact" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        AmoCrmValidationException exception = Assert.ThrowsAsync<AmoCrmValidationException>(async () =>
            await Client.UpdateContactsInternalAsync(TestAccessToken, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Validation failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetTransactionsAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        const int customerId = 1;

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        AmoCrmValidationException exception = Assert.ThrowsAsync<AmoCrmValidationException>(async () =>
            await Client.GetTransactionsAsync(TestAccessToken, TestSubdomain, customerId));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Validation failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetTransactionsInternalAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        const int customerId = 1;

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        AmoCrmValidationException exception = Assert.ThrowsAsync<AmoCrmValidationException>(async () =>
            await Client.GetTransactionsInternalAsync(TestAccessToken, customerId));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Validation failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void AddTransactionsAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        const int customerId = 1;
        List<AddTransactionRequest> requests = [new AddTransactionRequest(1000) { CustomerId = customerId, Comment = "Transaction 1" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        AmoCrmValidationException exception = Assert.ThrowsAsync<AmoCrmValidationException>(async () =>
            await Client.AddTransactionsAsync(TestAccessToken, TestSubdomain, customerId, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Validation failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void AddTransactionsInternalAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        const int customerId = 1;
        List<AddTransactionRequest> requests = [new AddTransactionRequest(1000) { CustomerId = customerId, Comment = "Transaction 1" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        AmoCrmValidationException exception = Assert.ThrowsAsync<AmoCrmValidationException>(async () =>
            await Client.AddTransactionsInternalAsync(TestAccessToken, customerId, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Validation failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetCustomFieldsAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        EntityTypeEnum entityType = EntityTypeEnum.Leads;

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        AmoCrmValidationException exception = Assert.ThrowsAsync<AmoCrmValidationException>(async () =>
            await Client.GetCustomFieldsAsync(TestAccessToken, TestSubdomain, entityType));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Validation failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetCustomFieldsInternalAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        EntityTypeEnum entityType = EntityTypeEnum.Leads;

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        AmoCrmValidationException exception = Assert.ThrowsAsync<AmoCrmValidationException>(async () =>
            await Client.GetCustomFieldsInternalAsync(TestAccessToken, entityType));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Validation failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetLeadStatusesAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        AmoCrmValidationException exception = Assert.ThrowsAsync<AmoCrmValidationException>(async () =>
            await Client.GetLeadStatusesAsync(TestAccessToken, TestSubdomain));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Validation failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetLeadStatusesInternalAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        AmoCrmValidationException exception = Assert.ThrowsAsync<AmoCrmValidationException>(async () =>
            await Client.GetLeadStatusesInternalAsync(TestAccessToken));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Validation failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetNotesAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        EntityTypeEnum entityType = EntityTypeEnum.Leads;
        AmoCrmNoteTypeEnum noteType = AmoCrmNoteTypeEnum.Common;

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        AmoCrmValidationException exception = Assert.ThrowsAsync<AmoCrmValidationException>(async () =>
            await Client.GetNotesAsync(TestAccessToken, TestSubdomain, entityType, noteType));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Validation failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GetNotesInternalAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        EntityTypeEnum entityType = EntityTypeEnum.Leads;
        AmoCrmNoteTypeEnum noteType = AmoCrmNoteTypeEnum.Common;

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        AmoCrmValidationException exception = Assert.ThrowsAsync<AmoCrmValidationException>(async () =>
            await Client.GetNotesInternalAsync(TestAccessToken, entityType, noteType));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Validation failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void AddNotesAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        EntityTypeEnum entityType = EntityTypeEnum.Leads;
        List<AddNoteRequest> requests = [new AddNoteRequest(1, AmoCrmNoteTypeEnum.Common) { Parameters = new NoteDetails { Text = "Note 1" } }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        AmoCrmValidationException exception = Assert.ThrowsAsync<AmoCrmValidationException>(async () =>
            await Client.AddNotesAsync(TestAccessToken, TestSubdomain, entityType, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Validation failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void AddNotesInternalAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        EntityTypeEnum entityType = EntityTypeEnum.Leads;
        List<AddNoteRequest> requests = [new AddNoteRequest(1, AmoCrmNoteTypeEnum.Common) { Parameters = new NoteDetails { Text = "Note 1" } }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        AmoCrmValidationException exception = Assert.ThrowsAsync<AmoCrmValidationException>(async () =>
            await Client.AddNotesInternalAsync(TestAccessToken, entityType, requests));

        Assert.Multiple(() =>
        {
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Does.Contain("Validation failed"));
        });

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public async Task AddLeadsAsync_EmptyRequests_ReturnsEmptyListAsync()
    {
        List<AddLeadRequest> requests = [];

        EntitiesResponse response = new EntitiesResponse { Embedded = new EmbeddedEntitiesResponse { Leads = [] } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Lead> result = await Client.AddLeadsAsync(TestAccessToken, TestSubdomain, requests);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task AddLeadsInternalAsync_EmptyRequests_ReturnsEmptyListAsync()
    {
        List<AddLeadRequest> requests = [];

        EntitiesResponse response = new EntitiesResponse { Embedded = new EmbeddedEntitiesResponse { Leads = [] } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Lead> result = await Client.AddLeadsInternalAsync(TestAccessToken, requests);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void AddLeadsAsync_NullRequests_ThrowsArgumentNullException()
    {
        IReadOnlyCollection<AddLeadRequest> requests = null!;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await Client.AddLeadsAsync(TestAccessToken, TestSubdomain, requests));
    }

    [Test]
    public void AddLeadsInternalAsync_NullRequests_ThrowsArgumentNullException()
    {
        IReadOnlyCollection<AddLeadRequest> requests = null!;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await Client.AddLeadsInternalAsync(TestAccessToken, requests));
    }

    [Test]
    public async Task UpdateLeadsAsync_EmptyRequests_ReturnsEmptyListAsync()
    {
        List<UpdateLeadRequest> requests = [];

        EntitiesResponse response = new EntitiesResponse { Embedded = new EmbeddedEntitiesResponse { Leads = [] } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Lead> result = await Client.UpdateLeadsAsync(TestAccessToken, TestSubdomain, requests);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task UpdateLeadsInternalAsync_EmptyRequests_ReturnsEmptyListAsync()
    {
        List<UpdateLeadRequest> requests = [];

        EntitiesResponse response = new EntitiesResponse { Embedded = new EmbeddedEntitiesResponse { Leads = [] } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Lead> result = await Client.UpdateLeadsInternalAsync(TestAccessToken, requests);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void UpdateLeadsAsync_NullRequests_ThrowsArgumentNullException()
    {
        IReadOnlyCollection<UpdateLeadRequest> requests = null!;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await Client.UpdateLeadsAsync(TestAccessToken, TestSubdomain, requests));
    }

    [Test]
    public void UpdateLeadsInternalAsync_NullRequests_ThrowsArgumentNullException()
    {
        IReadOnlyCollection<UpdateLeadRequest> requests = null!;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await Client.UpdateLeadsInternalAsync(TestAccessToken, requests));
    }

    [Test]
    public async Task AddCompaniesAsync_EmptyRequests_ReturnsEmptyListAsync()
    {
        List<AddCompanyRequest> requests = [];

        EntitiesResponse response = new EntitiesResponse { Embedded = new EmbeddedEntitiesResponse { Companies = [] } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Company> result = await Client.AddCompaniesAsync(TestAccessToken, TestSubdomain, requests);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task AddCompaniesInternalAsync_EmptyRequests_ReturnsEmptyListAsync()
    {
        List<AddCompanyRequest> requests = [];

        EntitiesResponse response = new EntitiesResponse { Embedded = new EmbeddedEntitiesResponse { Companies = [] } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Company> result = await Client.AddCompaniesInternalAsync(TestAccessToken, requests);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void AddCompaniesAsync_NullRequests_ThrowsArgumentNullException()
    {
        IReadOnlyCollection<AddCompanyRequest> requests = null!;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await Client.AddCompaniesAsync(TestAccessToken, TestSubdomain, requests));
    }

    [Test]
    public void AddCompaniesInternalAsync_NullRequests_ThrowsArgumentNullException()
    {
        IReadOnlyCollection<AddCompanyRequest> requests = null!;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await Client.AddCompaniesInternalAsync(TestAccessToken, requests));
    }

    [Test]
    public async Task UpdateCompaniesAsync_EmptyRequests_ReturnsEmptyListAsync()
    {
        List<UpdateCompanyRequest> requests = [];

        EntitiesResponse response = new EntitiesResponse { Embedded = new EmbeddedEntitiesResponse { Companies = [] } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Company> result = await Client.UpdateCompaniesAsync(TestAccessToken, TestSubdomain, requests);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task UpdateCompaniesInternalAsync_EmptyRequests_ReturnsEmptyListAsync()
    {
        List<UpdateCompanyRequest> requests = [];

        EntitiesResponse response = new EntitiesResponse { Embedded = new EmbeddedEntitiesResponse { Companies = [] } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Company> result = await Client.UpdateCompaniesInternalAsync(TestAccessToken, requests);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void UpdateCompaniesAsync_NullRequests_ThrowsArgumentNullException()
    {
        IReadOnlyCollection<UpdateCompanyRequest> requests = null!;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await Client.UpdateCompaniesAsync(TestAccessToken, TestSubdomain, requests));
    }

    [Test]
    public void UpdateCompaniesInternalAsync_NullRequests_ThrowsArgumentNullException()
    {
        IReadOnlyCollection<UpdateCompanyRequest> requests = null!;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await Client.UpdateCompaniesInternalAsync(TestAccessToken, requests));
    }

    [Test]
    public async Task AddTasksAsync_EmptyRequests_ReturnsEmptyListAsync()
    {
        List<AddTaskRequest> requests = [];

        EntitiesResponse response = new EntitiesResponse { Embedded = new EmbeddedEntitiesResponse { Tasks = [] } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<AmoCrmTask> result = await Client.AddTasksAsync(TestAccessToken, TestSubdomain, requests);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task AddTasksInternalAsync_EmptyRequests_ReturnsEmptyListAsync()
    {
        List<AddTaskRequest> requests = [];

        EntitiesResponse response = new EntitiesResponse { Embedded = new EmbeddedEntitiesResponse { Tasks = [] } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<AmoCrmTask> result = await Client.AddTasksInternalAsync(TestAccessToken, requests);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void AddTasksAsync_NullRequests_ThrowsArgumentNullException()
    {
        IReadOnlyCollection<AddTaskRequest> requests = null!;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await Client.AddTasksAsync(TestAccessToken, TestSubdomain, requests));
    }

    [Test]
    public void AddTasksInternalAsync_NullRequests_ThrowsArgumentNullException()
    {
        IReadOnlyCollection<AddTaskRequest> requests = null!;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await Client.AddTasksInternalAsync(TestAccessToken, requests));
    }

    [Test]
    public async Task UpdateTasksAsync_EmptyRequests_ReturnsEmptyListAsync()
    {
        List<UpdateTaskRequest> requests = [];

        EntitiesResponse response = new EntitiesResponse { Embedded = new EmbeddedEntitiesResponse { Tasks = [] } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<AmoCrmTask> result = await Client.UpdateTasksAsync(TestAccessToken, TestSubdomain, requests);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task UpdateTasksInternalAsync_EmptyRequests_ReturnsEmptyListAsync()
    {
        List<UpdateTaskRequest> requests = [];

        EntitiesResponse response = new EntitiesResponse { Embedded = new EmbeddedEntitiesResponse { Tasks = [] } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<AmoCrmTask> result = await Client.UpdateTasksInternalAsync(TestAccessToken, requests);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void UpdateTasksAsync_NullRequests_ThrowsArgumentNullException()
    {
        IReadOnlyCollection<UpdateTaskRequest> requests = null!;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await Client.UpdateTasksAsync(TestAccessToken, TestSubdomain, requests));
    }

    [Test]
    public void UpdateTasksInternalAsync_NullRequests_ThrowsArgumentNullException()
    {
        IReadOnlyCollection<UpdateTaskRequest> requests = null!;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await Client.UpdateTasksInternalAsync(TestAccessToken, requests));
    }

    [Test]
    public async Task AddCustomersAsync_EmptyRequests_ReturnsEmptyListAsync()
    {
        List<AddCustomerRequest> requests = [];

        EntitiesResponse response = new EntitiesResponse { Embedded = new EmbeddedEntitiesResponse { Customers = [] } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Customer> result = await Client.AddCustomersAsync(TestAccessToken, TestSubdomain, requests);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task AddCustomersInternalAsync_EmptyRequests_ReturnsEmptyListAsync()
    {
        List<AddCustomerRequest> requests = [];

        EntitiesResponse response = new EntitiesResponse { Embedded = new EmbeddedEntitiesResponse { Customers = [] } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Customer> result = await Client.AddCustomersInternalAsync(TestAccessToken, requests);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void AddCustomersAsync_NullRequests_ThrowsArgumentNullException()
    {
        IReadOnlyCollection<AddCustomerRequest> requests = null!;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await Client.AddCustomersAsync(TestAccessToken, TestSubdomain, requests));
    }

    [Test]
    public void AddCustomersInternalAsync_NullRequests_ThrowsArgumentNullException()
    {
        IReadOnlyCollection<AddCustomerRequest> requests = null!;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await Client.AddCustomersInternalAsync(TestAccessToken, requests));
    }

    [Test]
    public async Task UpdateCustomersAsync_EmptyRequests_ReturnsEmptyListAsync()
    {
        List<UpdateCustomerRequest> requests = [];

        EntitiesResponse response = new EntitiesResponse { Embedded = new EmbeddedEntitiesResponse { Customers = [] } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Customer> result = await Client.UpdateCustomersAsync(TestAccessToken, TestSubdomain, requests);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task UpdateCustomersInternalAsync_EmptyRequests_ReturnsEmptyListAsync()
    {
        List<UpdateCustomerRequest> requests = [];

        EntitiesResponse response = new EntitiesResponse { Embedded = new EmbeddedEntitiesResponse { Customers = [] } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Customer> result = await Client.UpdateCustomersInternalAsync(TestAccessToken, requests);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void UpdateCustomersAsync_NullRequests_ThrowsArgumentNullException()
    {
        IReadOnlyCollection<UpdateCustomerRequest> requests = null!;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await Client.UpdateCustomersAsync(TestAccessToken, TestSubdomain, requests));
    }

    [Test]
    public void UpdateCustomersInternalAsync_NullRequests_ThrowsArgumentNullException()
    {
        IReadOnlyCollection<UpdateCustomerRequest> requests = null!;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await Client.UpdateCustomersInternalAsync(TestAccessToken, requests));
    }

    [Test]
    public async Task AddContactsAsync_EmptyRequests_ReturnsEmptyListAsync()
    {
        List<AddContactRequest> requests = [];

        EntitiesResponse response = new EntitiesResponse { Embedded = new EmbeddedEntitiesResponse { Contacts = [] } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Contact> result = await Client.AddContactsAsync(TestAccessToken, TestSubdomain, requests);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task AddContactsInternalAsync_EmptyRequests_ReturnsEmptyListAsync()
    {
        List<AddContactRequest> requests = [];

        EntitiesResponse response = new EntitiesResponse { Embedded = new EmbeddedEntitiesResponse { Contacts = [] } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Contact> result = await Client.AddContactsInternalAsync(TestAccessToken, requests);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void AddContactsAsync_NullRequests_ThrowsArgumentNullException()
    {
        IReadOnlyCollection<AddContactRequest> requests = null!;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await Client.AddContactsAsync(TestAccessToken, TestSubdomain, requests));
    }

    [Test]
    public void AddContactsInternalAsync_NullRequests_ThrowsArgumentNullException()
    {
        IReadOnlyCollection<AddContactRequest> requests = null!;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await Client.AddContactsInternalAsync(TestAccessToken, requests));
    }

    [Test]
    public async Task UpdateContactsAsync_EmptyRequests_ReturnsEmptyListAsync()
    {
        List<UpdateContactRequest> requests = [];

        EntitiesResponse response = new EntitiesResponse { Embedded = new EmbeddedEntitiesResponse { Contacts = [] } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Contact> result = await Client.UpdateContactsAsync(TestAccessToken, TestSubdomain, requests);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task UpdateContactsInternalAsync_EmptyRequests_ReturnsEmptyListAsync()
    {
        List<UpdateContactRequest> requests = [];

        EntitiesResponse response = new EntitiesResponse { Embedded = new EmbeddedEntitiesResponse { Contacts = [] } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Contact> result = await Client.UpdateContactsInternalAsync(TestAccessToken, requests);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void UpdateContactsAsync_NullRequests_ThrowsArgumentNullException()
    {
        IReadOnlyCollection<UpdateContactRequest> requests = null!;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await Client.UpdateContactsAsync(TestAccessToken, TestSubdomain, requests));
    }

    [Test]
    public void UpdateContactsInternalAsync_NullRequests_ThrowsArgumentNullException()
    {
        IReadOnlyCollection<UpdateContactRequest> requests = null!;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await Client.UpdateContactsInternalAsync(TestAccessToken, requests));
    }

    [Test]
    public async Task AddTransactionsAsync_EmptyRequests_ReturnsEmptyListAsync()
    {
        const int customerId = 1;
        List<AddTransactionRequest> requests = [];

        EntitiesResponse response = new EntitiesResponse { Embedded = new EmbeddedEntitiesResponse { Transactions = [] } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Transaction> result = await Client.AddTransactionsAsync(TestAccessToken, TestSubdomain, customerId, requests);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task AddTransactionsInternalAsync_EmptyRequests_ReturnsEmptyListAsync()
    {
        const int customerId = 1;
        List<AddTransactionRequest> requests = [];

        EntitiesResponse response = new EntitiesResponse { Embedded = new EmbeddedEntitiesResponse { Transactions = [] } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Transaction> result = await Client.AddTransactionsInternalAsync(TestAccessToken, customerId, requests);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void AddTransactionsAsync_NullRequests_ThrowsArgumentNullException()
    {
        const int customerId = 1;
        IReadOnlyCollection<AddTransactionRequest> requests = null!;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await Client.AddTransactionsAsync(TestAccessToken, TestSubdomain, customerId, requests));
    }

    [Test]
    public void AddTransactionsInternalAsync_NullRequests_ThrowsArgumentNullException()
    {
        const int customerId = 1;
        IReadOnlyCollection<AddTransactionRequest> requests = null!;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await Client.AddTransactionsInternalAsync(TestAccessToken, customerId, requests));
    }

    [Test]
    public async Task AddNotesAsync_EmptyRequests_ReturnsEmptyListAsync()
    {
        EntityTypeEnum entityType = EntityTypeEnum.Leads;
        List<AddNoteRequest> requests = [];

        EntitiesResponse response = new EntitiesResponse { Embedded = new EmbeddedEntitiesResponse { Notes = [] } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Note> result = await Client.AddNotesAsync(TestAccessToken, TestSubdomain, entityType, requests);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task AddNotesInternalAsync_EmptyRequests_ReturnsEmptyListAsync()
    {
        EntityTypeEnum entityType = EntityTypeEnum.Leads;
        List<AddNoteRequest> requests = [];

        EntitiesResponse response = new EntitiesResponse { Embedded = new EmbeddedEntitiesResponse { Notes = [] } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Note> result = await Client.AddNotesInternalAsync(TestAccessToken, entityType, requests);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void AddNotesAsync_NullRequests_ThrowsArgumentNullException()
    {
        EntityTypeEnum entityType = EntityTypeEnum.Leads;
        IReadOnlyCollection<AddNoteRequest> requests = null!;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await Client.AddNotesAsync(TestAccessToken, TestSubdomain, entityType, requests));
    }

    [Test]
    public void AddNotesInternalAsync_NullRequests_ThrowsArgumentNullException()
    {
        EntityTypeEnum entityType = EntityTypeEnum.Leads;
        IReadOnlyCollection<AddNoteRequest> requests = null!;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await Client.AddNotesInternalAsync(TestAccessToken, entityType, requests));
    }
}
