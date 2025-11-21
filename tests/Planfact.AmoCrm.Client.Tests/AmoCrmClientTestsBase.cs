using Reliable.HttpClient;

using Planfact.AmoCrm.Client.Authorization;
using Planfact.AmoCrm.Client.Common;
using Planfact.AmoCrm.Client.Companies;
using Planfact.AmoCrm.Client.Contacts;
using Planfact.AmoCrm.Client.Customers;
using Planfact.AmoCrm.Client.CustomFields;
using Planfact.AmoCrm.Client.Exceptions;
using Planfact.AmoCrm.Client.Leads;
using Planfact.AmoCrm.Client.Links;
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
    protected const string TestClientId = "test-client-id";
    protected const string TestClientSecret = "test-client-secret";
    protected const string TestAuthCode = "test-auth-code";
    protected const string TestAccessToken = "access-token";
    protected const string TestSubdomain = "example.amocrm.ru";
    protected const string TestRedirectUri = "https://example.com";

    protected IAmoCrmClient Client { get; set; } = new Mock<IAmoCrmClient>().Object;
    protected Mock<IHttpResponseHandler> ResponseHandlerMock { get; set; } = new Mock<IHttpResponseHandler>();

    [Fact]
    public async Task AuthorizeAsync_ValidParameters_ReturnsTokens()
    {
        const string authCode = "auth-code";
        const string redirectDomain = "https://example.com  ";
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

        result.Should().NotBeNull();
        result.AccessToken.Should().Be("access-token");
        result.RefreshToken.Should().Be("refresh-token");
        result.ExpiresIn.Should().Be(3600);

        ResponseHandlerMock.Verify(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AuthorizeInternalAsync_ValidParameters_ReturnsTokens()
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

        result.Should().NotBeNull();
        result.AccessToken.Should().Be("internal-access-token");
        result.RefreshToken.Should().Be("internal-refresh-token");
        result.ExpiresIn.Should().Be(3600);

        ResponseHandlerMock.Verify(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task RefreshTokenAsync_ValidParameters_ReturnsTokens()
    {
        const string refreshToken = "refresh-token";
        const string redirectDomain = "https://example.com  ";
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

        result.Should().NotBeNull();
        result.AccessToken.Should().Be("new-access-token");
        result.RefreshToken.Should().Be("new-refresh-token");
        result.ExpiresIn.Should().Be(3600);

        ResponseHandlerMock.Verify(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task RefreshTokenInternalAsync_ValidParameters_ReturnsTokens()
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

        result.Should().NotBeNull();
        result.AccessToken.Should().Be("new-internal-access-token");
        result.RefreshToken.Should().Be("new-internal-refresh-token");
        result.ExpiresIn.Should().Be(3600);

        ResponseHandlerMock.Verify(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetLeadsAsync_ValidQuery_ReturnsLeads()
    {
        const string query = "test-query";
        Lead[] leads1 = [new Lead { Id = 1, Name = "Lead 1", AccountId = 100 }];
        Lead[] leads2 = [new Lead { Id = 2, Name = "Lead 2", AccountId = 101 }];
        var response1 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Leads = leads1 },
            PaginationLinks = new PaginationLinksResponse { Next = new NavigationLink { Uri = "https://example.amocrm.ru/api/v4/leads?page=2&limit=1" } }
        };
        var response2 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Leads = leads2 }
        };

        ResponseHandlerMock
            .SetupSequence(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response1)
            .ReturnsAsync(response2);

        IReadOnlyCollection<Lead> result = await Client.GetLeadsAsync(TestAccessToken, TestSubdomain, query);

        result.Should().NotBeNull().And.HaveCount(2);
        result.First().Id.Should().Be(1);
        result.First().Name.Should().Be("Lead 1");
        result.First().AccountId.Should().Be(100);
        result.Last().Id.Should().Be(2);
        result.Last().Name.Should().Be("Lead 2");
        result.Last().AccountId.Should().Be(101);

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task GetLeadsAsync_ValidQueryAndLinkedEntityTypes_ReturnsLeads()
    {
        const string query = "test-query";
        EntityType[] linkedEntityTypes = new[] { EntityType.Contacts, EntityType.Companies };
        Lead[] leads1 = [new Lead { Id = 1, Name = "Lead 1", AccountId = 100 }];
        Lead[] leads2 = [new Lead { Id = 2, Name = "Lead 2", AccountId = 101 }];
        var response1 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Leads = leads1 },
            PaginationLinks = new PaginationLinksResponse { Next = new NavigationLink { Uri = "https://example.amocrm.ru/api/v4/leads?page=2&limit=1" } }
        };
        var response2 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Leads = leads2 }
        };

        ResponseHandlerMock
            .SetupSequence(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response1)
            .ReturnsAsync(response2);

        IReadOnlyCollection<Lead> result = await Client.GetLeadsAsync(TestAccessToken, TestSubdomain, linkedEntityTypes, query);

        result.Should().NotBeNull().And.HaveCount(2);
        result.First().Id.Should().Be(1);
        result.First().Name.Should().Be("Lead 1");
        result.First().AccountId.Should().Be(100);
        result.Last().Id.Should().Be(2);
        result.Last().Name.Should().Be("Lead 2");
        result.Last().AccountId.Should().Be(101);

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task GetLeadsInternalAsync_ValidQuery_ReturnsLeads()
    {
        const string query = "test-query";
        Lead[] leads1 = [new Lead { Id = 1, Name = "Lead 1", AccountId = 100 }];
        Lead[] leads2 = [new Lead { Id = 2, Name = "Lead 2", AccountId = 101 }];
        var response1 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Leads = leads1 },
            PaginationLinks = new PaginationLinksResponse { Next = new NavigationLink { Uri = "  https://example.amocrm.ru/api/v4/leads?page=2&limit=1" } }
        };
        var response2 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Leads = leads2 }
        };

        ResponseHandlerMock
            .SetupSequence(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response1)
            .ReturnsAsync(response2);

        IReadOnlyCollection<Lead> result = await Client.GetLeadsInternalAsync(TestAccessToken, query);

        result.Should().NotBeNull().And.HaveCount(2);
        result.First().Id.Should().Be(1);
        result.First().Name.Should().Be("Lead 1");
        result.First().AccountId.Should().Be(100);
        result.Last().Id.Should().Be(2);
        result.Last().Name.Should().Be("Lead 2");
        result.Last().AccountId.Should().Be(101);

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task GetLeadsInternalAsync_ValidQueryAndLinkedEntityTypes_ReturnsLeads()
    {
        const string query = "test-query";
        EntityType[] linkedEntityTypes = new[] { EntityType.Contacts, EntityType.Customers };
        Lead[] leads1 = [new Lead { Id = 1, Name = "Lead 1", AccountId = 100 }];
        Lead[] leads2 = [new Lead { Id = 2, Name = "Lead 2", AccountId = 101 }];
        var response1 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Leads = leads1 },
            PaginationLinks = new PaginationLinksResponse { Next = new NavigationLink { Uri = "https://example.amocrm.ru/api/v4/leads?page=2&limit=1" } }
        };
        var response2 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Leads = leads2 }
        };

        ResponseHandlerMock
            .SetupSequence(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response1)
            .ReturnsAsync(response2);

        IReadOnlyCollection<Lead> result = await Client.GetLeadsInternalAsync(TestAccessToken, linkedEntityTypes, query);

        result.Should().NotBeNull().And.HaveCount(2);
        result.First().Id.Should().Be(1);
        result.First().Name.Should().Be("Lead 1");
        result.First().AccountId.Should().Be(100);
        result.Last().Id.Should().Be(2);
        result.Last().Name.Should().Be("Lead 2");
        result.Last().AccountId.Should().Be(101);

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task GetLeadsAsync_ValidIds_ReturnsLeads()
    {
        int[] ids = [1, 2];
        Lead[] leads =
        [
            new Lead { Id = 1, Name = "Lead 1", AccountId = 100 },
            new Lead { Id = 2, Name = "Lead 2", AccountId = 101 }
        ];
        var response = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Leads = leads }
        };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Lead> result = await Client.GetLeadsAsync(TestAccessToken, TestSubdomain, ids);

        result.Should().NotBeNull().And.HaveCount(2);
        result.First().Id.Should().Be(1);
        result.Last().Id.Should().Be(2);

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetLeadsAsync_ValidIdsAndLinkedEntityTypes_ReturnsLeads()
    {
        int[] ids = [1, 2];
        EntityType[] linkedEntityTypes = new[] { EntityType.Contacts };
        Lead[] leads =
        [
            new Lead { Id = 1, Name = "Lead 1", AccountId = 100 },
        new Lead { Id = 2, Name = "Lead 2", AccountId = 101 }
        ];
        var response = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Leads = leads }
        };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Lead> result = await Client.GetLeadsAsync(TestAccessToken, TestSubdomain, linkedEntityTypes, ids);

        result.Should().NotBeNull().And.HaveCount(2);
        result.First().Id.Should().Be(1);
        result.Last().Id.Should().Be(2);

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetLeadsInternalAsync_ValidIds_ReturnsLeads()
    {
        int[] ids = [1, 2];
        Lead[] leads =
        [
            new Lead { Id = 1, Name = "Lead 1", AccountId = 100 },
            new Lead { Id = 2, Name = "Lead 2", AccountId = 101 }
        ];
        var response = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Leads = leads }
        };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Lead> result = await Client.GetLeadsInternalAsync(TestAccessToken, ids);

        result.Should().NotBeNull().And.HaveCount(2);
        result.First().Id.Should().Be(1);
        result.Last().Id.Should().Be(2);

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetLeadsInternalAsync_ValidIdsAndLinkedEntityTypes_ReturnsLeads()
    {
        int[] ids = [1, 2];
        EntityType[] linkedEntityTypes = new[] { EntityType.Companies };
        Lead[] leads =
        [
            new Lead { Id = 1, Name = "Lead 1", AccountId = 100 },
        new Lead { Id = 2, Name = "Lead 2", AccountId = 101 }
        ];
        var response = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Leads = leads }
        };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Lead> result = await Client.GetLeadsInternalAsync(TestAccessToken, linkedEntityTypes, ids);

        result.Should().NotBeNull().And.HaveCount(2);
        result.First().Id.Should().Be(1);
        result.Last().Id.Should().Be(2);

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AddLeadsAsync_ValidRequests_ReturnsCreatedLeads()
    {
        AddLeadRequest[] requests = [new AddLeadRequest { Name = "Lead 1", Price = 1000 }];
        Lead[] expectedLeads = [new Lead { Id = 1, Name = "Lead 1", Price = 1000 }];
        var response = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Leads = expectedLeads }
        };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Lead> result = await Client.AddLeadsAsync(TestAccessToken, TestSubdomain, requests);

        result.Should().NotBeNull().And.HaveCount(1);
        result.First().Id.Should().Be(1);
        result.First().Name.Should().Be("Lead 1");
        result.First().Price.Should().Be(1000);

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AddLeadsInternalAsync_ValidRequests_ReturnsCreatedLeads()
    {
        AddLeadRequest[] requests = [new AddLeadRequest { Name = "Lead 1", Price = 1000 }];
        Lead[] expectedLeads = [new Lead { Id = 1, Name = "Lead 1", Price = 1000 }];
        var response = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Leads = expectedLeads }
        };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Lead> result = await Client.AddLeadsInternalAsync(TestAccessToken, requests);

        result.Should().NotBeNull().And.HaveCount(1);
        result.First().Id.Should().Be(1);
        result.First().Name.Should().Be("Lead 1");
        result.First().Price.Should().Be(1000);

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task UpdateLeadsAsync_ValidRequests_ReturnsUpdatedLeads()
    {
        UpdateLeadRequest[] requests = [new UpdateLeadRequest(1) { Name = "Updated Lead", Price = 2000 }];
        Lead[] expectedLeads = [new Lead { Id = 1, Name = "Updated Lead", Price = 2000 }];
        var response = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Leads = expectedLeads }
        };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Lead> result = await Client.UpdateLeadsAsync(TestAccessToken, TestSubdomain, requests);

        result.Should().NotBeNull().And.HaveCount(1);
        result.First().Id.Should().Be(1);
        result.First().Name.Should().Be("Updated Lead");
        result.First().Price.Should().Be(2000);

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task UpdateLeadsInternalAsync_ValidRequests_ReturnsUpdatedLeads()
    {
        UpdateLeadRequest[] requests = [new UpdateLeadRequest(1) { Name = "Updated Lead", Price = 2000 }];
        Lead[] expectedLeads = [new Lead { Id = 1, Name = "Updated Lead", Price = 2000 }];
        var response = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Leads = expectedLeads }
        };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Lead> result = await Client.UpdateLeadsInternalAsync(TestAccessToken, requests);

        result.Should().NotBeNull().And.HaveCount(1);
        result.First().Id.Should().Be(1);
        result.First().Name.Should().Be("Updated Lead");
        result.First().Price.Should().Be(2000);

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetCompaniesAsync_ValidQuery_ReturnsCompanies()
    {
        const string query = "test";
        Company[] companies1 = [new Company { Id = 1, Name = "Company 1" }];
        Company[] companies2 = [new Company { Id = 2, Name = "Company 2" }];
        var response1 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Companies = companies1 },
            PaginationLinks = new PaginationLinksResponse { Next = new NavigationLink { Uri = "  https://example.amocrm.ru/api/v4/companies?page=2&limit=1" } }
        };
        var response2 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Companies = companies2 }
        };

        ResponseHandlerMock
            .SetupSequence(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response1)
            .ReturnsAsync(response2);

        IReadOnlyCollection<Company> result = await Client.GetCompaniesAsync(TestAccessToken, TestSubdomain, query);

        result.Should().NotBeNull().And.HaveCount(2);
        result.First().Id.Should().Be(1);
        result.First().Name.Should().Be("Company 1");
        result.Last().Id.Should().Be(2);
        result.Last().Name.Should().Be("Company 2");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task GetCompaniesAsync_ValidQueryAndLinkedEntityTypes_ReturnsCompanies()
    {
        const string query = "test";
        EntityType[] linkedEntityTypes = new[] { EntityType.Contacts, EntityType.Leads };
        Company[] companies1 = [new Company { Id = 1, Name = "Company 1" }];
        Company[] companies2 = [new Company { Id = 2, Name = "Company 2" }];
        var response1 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Companies = companies1 },
            PaginationLinks = new PaginationLinksResponse { Next = new NavigationLink { Uri = "https://example.amocrm.ru/api/v4/companies?page=2&limit=1" } }
        };
        var response2 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Companies = companies2 }
        };

        ResponseHandlerMock
            .SetupSequence(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response1)
            .ReturnsAsync(response2);

        IReadOnlyCollection<Company> result = await Client.GetCompaniesAsync(TestAccessToken, TestSubdomain, linkedEntityTypes, query);

        result.Should().NotBeNull().And.HaveCount(2);
        result.First().Id.Should().Be(1);
        result.First().Name.Should().Be("Company 1");
        result.Last().Id.Should().Be(2);
        result.Last().Name.Should().Be("Company 2");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task GetCompaniesInternalAsync_ValidQuery_ReturnsCompanies()
    {
        const string query = "test";
        Company[] companies1 = [new Company { Id = 1, Name = "Company 1" }];
        Company[] companies2 = [new Company { Id = 2, Name = "Company 2" }];
        var response1 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Companies = companies1 },
            PaginationLinks = new PaginationLinksResponse { Next = new NavigationLink { Uri = "  https://example.amocrm.ru/api/v4/companies?page=2&limit=1" } }
        };
        var response2 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Companies = companies2 }
        };

        ResponseHandlerMock
            .SetupSequence(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response1)
            .ReturnsAsync(response2);

        IReadOnlyCollection<Company> result = await Client.GetCompaniesInternalAsync(TestAccessToken, query);

        result.Should().NotBeNull().And.HaveCount(2);
        result.First().Id.Should().Be(1);
        result.First().Name.Should().Be("Company 1");
        result.Last().Id.Should().Be(2);
        result.Last().Name.Should().Be("Company 2");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task GetCompaniesInternalAsync_ValidQueryAndLinkedEntityTypes_ReturnsCompanies()
    {
        const string query = "test";
        EntityType[] linkedEntityTypes = new[] { EntityType.Contacts, EntityType.Leads };
        Company[] companies1 = [new Company { Id = 1, Name = "Company 1" }];
        Company[] companies2 = [new Company { Id = 2, Name = "Company 2" }];
        var response1 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Companies = companies1 },
            PaginationLinks = new PaginationLinksResponse { Next = new NavigationLink { Uri = "https://example.amocrm.ru/api/v4/companies?page=2&limit=1" } }
        };
        var response2 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Companies = companies2 }
        };

        ResponseHandlerMock
            .SetupSequence(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response1)
            .ReturnsAsync(response2);

        IReadOnlyCollection<Company> result = await Client.GetCompaniesInternalAsync(TestAccessToken, linkedEntityTypes, query);

        result.Should().NotBeNull().And.HaveCount(2);
        result.First().Id.Should().Be(1);
        result.First().Name.Should().Be("Company 1");
        result.Last().Id.Should().Be(2);
        result.Last().Name.Should().Be("Company 2");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task AddCompaniesAsync_ValidRequests_ReturnsCreatedCompanies()
    {
        AddCompanyRequest[] requests = [new AddCompanyRequest { Name = "Company 1" }];
        Company[] expectedCompanies = [new Company { Id = 1, Name = "Company 1" }];
        var response = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Companies = expectedCompanies }
        };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Company> result = await Client.AddCompaniesAsync(TestAccessToken, TestSubdomain, requests);

        result.Should().NotBeNull().And.HaveCount(1);
        result.First().Id.Should().Be(1);
        result.First().Name.Should().Be("Company 1");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AddCompaniesInternalAsync_ValidRequests_ReturnsCreatedCompanies()
    {
        AddCompanyRequest[] requests = [new AddCompanyRequest { Name = "Company 1" }];
        Company[] expectedCompanies = [new Company { Id = 1, Name = "Company 1" }];
        var response = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Companies = expectedCompanies }
        };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Company> result = await Client.AddCompaniesInternalAsync(TestAccessToken, requests);

        result.Should().NotBeNull().And.HaveCount(1);
        result.First().Id.Should().Be(1);
        result.First().Name.Should().Be("Company 1");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task UpdateCompaniesAsync_ValidRequests_ReturnsUpdatedCompanies()
    {
        UpdateCompanyRequest[] requests = [new UpdateCompanyRequest(1) { Name = "Updated Company" }];
        Company[] expectedCompanies = [new Company { Id = 1, Name = "Updated Company" }];
        var response = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Companies = expectedCompanies }
        };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Company> result = await Client.UpdateCompaniesAsync(TestAccessToken, TestSubdomain, requests);

        result.Should().NotBeNull().And.HaveCount(1);
        result.First().Id.Should().Be(1);
        result.First().Name.Should().Be("Updated Company");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task UpdateCompaniesInternalAsync_ValidRequests_ReturnsUpdatedCompanies()
    {
        UpdateCompanyRequest[] requests = [new UpdateCompanyRequest(1) { Name = "Updated Company" }];
        Company[] expectedCompanies = [new Company { Id = 1, Name = "Updated Company" }];
        var response = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Companies = expectedCompanies }
        };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Company> result = await Client.UpdateCompaniesInternalAsync(TestAccessToken, requests);

        result.Should().NotBeNull().And.HaveCount(1);
        result.First().Id.Should().Be(1);
        result.First().Name.Should().Be("Updated Company");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetTasksAsync_ValidParameters_ReturnsTasks()
    {
        AmoCrmTask[] tasks1 = [new AmoCrmTask { Id = 1, Description = "Task 1" }];
        AmoCrmTask[] tasks2 = [new AmoCrmTask { Id = 2, Description = "Task 2" }];
        var response1 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Tasks = tasks1 },
            PaginationLinks = new PaginationLinksResponse { Next = new NavigationLink { Uri = "https://example.amocrm.ru/api/v4/tasks?page=2&limit=1" } }
        };
        var response2 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Tasks = tasks2 }
        };

        ResponseHandlerMock
            .SetupSequence(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response1)
            .ReturnsAsync(response2);

        IReadOnlyCollection<AmoCrmTask> result = await Client.GetTasksAsync(TestAccessToken, TestSubdomain);

        result.Should().NotBeNull().And.HaveCount(2);
        result.First().Id.Should().Be(1);
        result.First().Description.Should().Be("Task 1");
        result.Last().Id.Should().Be(2);
        result.Last().Description.Should().Be("Task 2");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task GetTasksInternalAsync_ValidParameters_ReturnsTasks()
    {
        AmoCrmTask[] tasks1 = [new AmoCrmTask { Id = 1, Description = "Task 1" }];
        AmoCrmTask[] tasks2 = [new AmoCrmTask { Id = 2, Description = "Task 2" }];
        var response1 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Tasks = tasks1 },
            PaginationLinks = new PaginationLinksResponse { Next = new NavigationLink { Uri = "  https://example.amocrm.ru/api/v4/tasks?page=2&limit=1" } }
        };
        var response2 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Tasks = tasks2 }
        };

        ResponseHandlerMock
            .SetupSequence(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response1)
            .ReturnsAsync(response2);

        IReadOnlyCollection<AmoCrmTask> result = await Client.GetTasksInternalAsync(TestAccessToken);

        result.Should().NotBeNull().And.HaveCount(2);
        result.First().Id.Should().Be(1);
        result.First().Description.Should().Be("Task 1");
        result.Last().Id.Should().Be(2);
        result.Last().Description.Should().Be("Task 2");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task GetTasksAsync_ValidParametersAndFilter_ReturnsTasks()
    {
        var filter = new TasksFilter
        {
            EntityIds = [11, 12],
            EntityType = EntityType.Leads,
            TaskIds = [1, 2],
            TaskTypeIds = [111, 112],
            IsCompleted = false,
            ResponsibleUserIds = [123, 124],
            UpdatedAtFrom = 11111111,
            UpdatedAtTo = 22222222
        };
        AmoCrmTask[] tasks1 = [new AmoCrmTask { Id = 1, Description = "Task 1", EntityType = EntityType.Leads, EntityId = 11, TaskTypeId = 111, ResponsibleUserId = 123, UpdatedAt = 12121212 }];
        AmoCrmTask[] tasks2 = [new AmoCrmTask { Id = 2, Description = "Task 2", EntityType = EntityType.Leads, EntityId = 12, TaskTypeId = 112, ResponsibleUserId = 124, UpdatedAt = 13131313 }];
        var response1 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Tasks = tasks1 },
            PaginationLinks = new PaginationLinksResponse
            {
                Next = new NavigationLink
                {
                    Uri = "https://example.amocrm.ru/api/v4/tasks?filter[id][0]=1&filter[id][1]=2&filter[task_type][0]=111&filter[task_type][1]=112&filter[responsible_user_id][0]=123&filter[responsible_user_id][1]=124&filter[entity_id][0]=11&filter[entity_id][1]=12&filter[entity_type]=leads&filter[updated_at][from]=11111111&filter[updated_at][to]=22222222&filter[is_completed]=0&page=2&limit=250"
                }
            }
        };
        var response2 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Tasks = tasks2 }
        };

        string[] expectedQueryStrings =
        [
            "?filter[id][0]=1&filter[id][1]=2&filter[task_type][0]=111&filter[task_type][1]=112&filter[responsible_user_id][0]=123&filter[responsible_user_id][1]=124&filter[entity_id][0]=11&filter[entity_id][1]=12&filter[entity_type]=leads&filter[updated_at][from]=11111111&filter[updated_at][to]=22222222&filter[is_completed]=0&page=1&limit=250",
            "?filter[id][0]=1&filter[id][1]=2&filter[task_type][0]=111&filter[task_type][1]=112&filter[responsible_user_id][0]=123&filter[responsible_user_id][1]=124&filter[entity_id][0]=11&filter[entity_id][1]=12&filter[entity_type]=leads&filter[updated_at][from]=11111111&filter[updated_at][to]=22222222&filter[is_completed]=0&page=2&limit=250",
        ];

        var responseArgs = new List<HttpResponseMessage>();

        ResponseHandlerMock
            .SetupSequence(x => x.HandleAsync<EntitiesResponse>(Capture.In(responseArgs), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response1)
            .ReturnsAsync(response2);

        IReadOnlyCollection<AmoCrmTask> result = await Client.GetTasksAsync(TestAccessToken, TestSubdomain, filter);

        result.Should().NotBeNull().And.HaveCount(2);
        result.First().Id.Should().Be(1);
        result.First().Description.Should().Be("Task 1");
        result.First().EntityType.Should().Be(EntityType.Leads);
        result.First().EntityId.Should().Be(11);
        result.First().TaskTypeId.Should().Be(111);
        result.First().ResponsibleUserId.Should().Be(123);
        result.First().UpdatedAt.Should().Be(12121212);
        result.Last().Id.Should().Be(2);
        result.Last().Description.Should().Be("Task 2");
        result.Last().EntityType.Should().Be(EntityType.Leads);
        result.Last().EntityId.Should().Be(12);
        result.Last().TaskTypeId.Should().Be(112);
        result.Last().ResponsibleUserId.Should().Be(124);
        result.Last().UpdatedAt.Should().Be(13131313);

        responseArgs[0].RequestMessage.Should().NotBeNull();
        responseArgs[0].RequestMessage!.RequestUri.Should().NotBeNull();
        responseArgs[0].RequestMessage!.RequestUri!.Query.Should().Be(expectedQueryStrings[0]);
        responseArgs[1].RequestMessage.Should().NotBeNull();
        responseArgs[1].RequestMessage!.RequestUri.Should().NotBeNull();
        responseArgs[1].RequestMessage!.RequestUri!.Query.Should().Be(expectedQueryStrings[1]);

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task GetTasksInternalAsync_ValidParametersAndFilter_ReturnsTasks()
    {
        var filter = new TasksFilter
        {
            EntityIds = [11, 12],
            EntityType = EntityType.Leads,
            TaskIds = [1, 2],
            TaskTypeIds = [111, 112],
            IsCompleted = false,
            ResponsibleUserIds = [123, 124],
            UpdatedAtFrom = 11111111,
            UpdatedAtTo = 22222222
        };
        AmoCrmTask[] tasks1 = [new AmoCrmTask { Id = 1, Description = "Task 1", EntityType = EntityType.Leads, EntityId = 11, TaskTypeId = 111, ResponsibleUserId = 123, UpdatedAt = 12121212 }];
        AmoCrmTask[] tasks2 = [new AmoCrmTask { Id = 2, Description = "Task 2", EntityType = EntityType.Leads, EntityId = 12, TaskTypeId = 112, ResponsibleUserId = 124, UpdatedAt = 13131313 }];
        var response1 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Tasks = tasks1 },
            PaginationLinks = new PaginationLinksResponse
            {
                Next = new NavigationLink
                {
                    Uri = "https://example.amocrm.ru/api/v4/tasks?filter[id][0]=1&filter[id][1]=2&filter[task_type][0]=111&filter[task_type][1]=112&filter[responsible_user_id][0]=123&filter[responsible_user_id][1]=124&filter[entity_id][0]=11&filter[entity_id][1]=12&filter[entity_type]=leads&filter[updated_at][from]=11111111&filter[updated_at][to]=22222222&filter[is_completed]=0&page=2&limit=250"
                }
            }
        };
        var response2 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Tasks = tasks2 }
        };

        string[] expectedQueryStrings =
        [
            "?filter[id][0]=1&filter[id][1]=2&filter[task_type][0]=111&filter[task_type][1]=112&filter[responsible_user_id][0]=123&filter[responsible_user_id][1]=124&filter[entity_id][0]=11&filter[entity_id][1]=12&filter[entity_type]=leads&filter[updated_at][from]=11111111&filter[updated_at][to]=22222222&filter[is_completed]=0&page=1&limit=250",
            "?filter[id][0]=1&filter[id][1]=2&filter[task_type][0]=111&filter[task_type][1]=112&filter[responsible_user_id][0]=123&filter[responsible_user_id][1]=124&filter[entity_id][0]=11&filter[entity_id][1]=12&filter[entity_type]=leads&filter[updated_at][from]=11111111&filter[updated_at][to]=22222222&filter[is_completed]=0&page=2&limit=250",
        ];

        var responseArgs = new List<HttpResponseMessage>();

        ResponseHandlerMock
            .SetupSequence(x => x.HandleAsync<EntitiesResponse>(Capture.In(responseArgs), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response1)
            .ReturnsAsync(response2);

        IReadOnlyCollection<AmoCrmTask> result = await Client.GetTasksInternalAsync(TestAccessToken, filter);

        result.Should().NotBeNull().And.HaveCount(2);
        result.First().Id.Should().Be(1);
        result.First().Description.Should().Be("Task 1");
        result.First().EntityType.Should().Be(EntityType.Leads);
        result.First().EntityId.Should().Be(11);
        result.First().TaskTypeId.Should().Be(111);
        result.First().ResponsibleUserId.Should().Be(123);
        result.First().UpdatedAt.Should().Be(12121212);
        result.Last().Id.Should().Be(2);
        result.Last().Description.Should().Be("Task 2");
        result.Last().EntityType.Should().Be(EntityType.Leads);
        result.Last().EntityId.Should().Be(12);
        result.Last().TaskTypeId.Should().Be(112);
        result.Last().ResponsibleUserId.Should().Be(124);
        result.Last().UpdatedAt.Should().Be(13131313);

        responseArgs[0].RequestMessage.Should().NotBeNull();
        responseArgs[0].RequestMessage!.RequestUri.Should().NotBeNull();
        responseArgs[0].RequestMessage!.RequestUri!.Query.Should().Be(expectedQueryStrings[0]);
        responseArgs[1].RequestMessage.Should().NotBeNull();
        responseArgs[1].RequestMessage!.RequestUri.Should().NotBeNull();
        responseArgs[1].RequestMessage!.RequestUri!.Query.Should().Be(expectedQueryStrings[1]);

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task AddTasksAsync_ValidRequests_ReturnsCreatedTasks()
    {
        AddTaskRequest[] requests = [new AddTaskRequest("Task 1", 1234567890, EntityType.Leads) { EntityId = 1 }];
        AmoCrmTask[] expectedTasks =
        [
            new AmoCrmTask { Id = 1, Description = "Task 1", EntityId = 1, EntityType = EntityType.Leads }
        ];
        var response = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Tasks = expectedTasks }
        };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<AmoCrmTask> result = await Client.AddTasksAsync(TestAccessToken, TestSubdomain, requests);

        result.Should().NotBeNull().And.HaveCount(1);
        result.First().Id.Should().Be(1);
        result.First().Description.Should().Be("Task 1");
        result.First().EntityId.Should().Be(1);

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AddTasksInternalAsync_ValidRequests_ReturnsCreatedTasks()
    {
        AddTaskRequest[] requests = [new AddTaskRequest("Task 1", 1234567890, EntityType.Leads) { EntityId = 1 }];
        AmoCrmTask[] expectedTasks =
        [
            new AmoCrmTask { Id = 1, Description = "Task 1", EntityId = 1, EntityType = EntityType.Leads }
        ];
        var response = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Tasks = expectedTasks }
        };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<AmoCrmTask> result = await Client.AddTasksInternalAsync(TestAccessToken, requests);

        result.Should().NotBeNull().And.HaveCount(1);
        result.First().Id.Should().Be(1);
        result.First().Description.Should().Be("Task 1");
        result.First().EntityId.Should().Be(1);

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task UpdateTasksAsync_ValidRequests_ReturnsUpdatedTasks()
    {
        UpdateTaskRequest[] requests = [new UpdateTaskRequest(1, "Updated Task", 123123123123)];
        AmoCrmTask[] expectedTasks =
        [
            new AmoCrmTask { Id = 1, Description = "Updated Task", EntityId = 1, CompleteTill = 123123123123 }
        ];
        var response = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Tasks = expectedTasks }
        };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<AmoCrmTask> result = await Client.UpdateTasksAsync(TestAccessToken, TestSubdomain, requests);

        result.Should().NotBeNull().And.HaveCount(1);
        result.First().Id.Should().Be(1);
        result.First().Description.Should().Be("Updated Task");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task UpdateTasksInternalAsync_ValidRequests_ReturnsUpdatedTasks()
    {
        UpdateTaskRequest[] requests = [new UpdateTaskRequest(1, "Updated Task", 123123123123)];
        AmoCrmTask[] expectedTasks =
        [
            new AmoCrmTask { Id = 1, Description = "Updated Task", EntityId = 1, CompleteTill = 123123123123 }
        ];
        var response = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Tasks = expectedTasks }
        };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<AmoCrmTask> result = await Client.UpdateTasksInternalAsync(TestAccessToken, requests);

        result.Should().NotBeNull().And.HaveCount(1);
        result.First().Id.Should().Be(1);
        result.First().Description.Should().Be("Updated Task");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetCustomersAsync_ValidQuery_ReturnsCustomers()
    {
        const string query = "test";
        Customer[] customers1 = [new Customer { Id = 1, Name = "Customer 1" }];
        Customer[] customers2 = [new Customer { Id = 2, Name = "Customer 2" }];
        var response1 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Customers = customers1 },
            PaginationLinks = new PaginationLinksResponse { Next = new NavigationLink { Uri = "  https://example.amocrm.ru/api/v4/customers?page=2&limit=1" } }
        };
        var response2 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Customers = customers2 }
        };

        ResponseHandlerMock
            .SetupSequence(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response1)
            .ReturnsAsync(response2);

        IReadOnlyCollection<Customer> result = await Client.GetCustomersAsync(TestAccessToken, TestSubdomain, query);

        result.Should().NotBeNull().And.HaveCount(2);
        result.First().Id.Should().Be(1);
        result.First().Name.Should().Be("Customer 1");
        result.Last().Id.Should().Be(2);
        result.Last().Name.Should().Be("Customer 2");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task GetCustomersAsync_ValidQueryAndLinkedEntityTypes_ReturnsCustomers()
    {
        const string query = "test";
        EntityType[] linkedEntityTypes = new[] { EntityType.Contacts, EntityType.Leads };
        Customer[] customers1 = [new Customer { Id = 1, Name = "Customer 1" }];
        Customer[] customers2 = [new Customer { Id = 2, Name = "Customer 2" }];
        var response1 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Customers = customers1 },
            PaginationLinks = new PaginationLinksResponse { Next = new NavigationLink { Uri = "https://example.amocrm.ru/api/v4/customers?page=2&limit=1" } }
        };
        var response2 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Customers = customers2 }
        };

        ResponseHandlerMock
            .SetupSequence(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response1)
            .ReturnsAsync(response2);

        IReadOnlyCollection<Customer> result = await Client.GetCustomersAsync(TestAccessToken, TestSubdomain, linkedEntityTypes, query);

        result.Should().NotBeNull().And.HaveCount(2);
        result.First().Id.Should().Be(1);
        result.First().Name.Should().Be("Customer 1");
        result.Last().Id.Should().Be(2);
        result.Last().Name.Should().Be("Customer 2");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task GetCustomersInternalAsync_ValidQuery_ReturnsCustomers()
    {
        const string query = "test";
        Customer[] customers1 = [new Customer { Id = 1, Name = "Customer 1" }];
        Customer[] customers2 = [new Customer { Id = 2, Name = "Customer 2" }];
        var response1 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Customers = customers1 },
            PaginationLinks = new PaginationLinksResponse { Next = new NavigationLink { Uri = "  https://example.amocrm.ru/api/v4/customers?page=2&limit=1" } }
        };
        var response2 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Customers = customers2 }
        };

        ResponseHandlerMock
            .SetupSequence(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response1)
            .ReturnsAsync(response2);

        IReadOnlyCollection<Customer> result = await Client.GetCustomersInternalAsync(TestAccessToken, query);

        result.Should().NotBeNull().And.HaveCount(2);
        result.First().Id.Should().Be(1);
        result.First().Name.Should().Be("Customer 1");
        result.Last().Id.Should().Be(2);
        result.Last().Name.Should().Be("Customer 2");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task GetCustomersInternalAsync_ValidQueryAndLinkedEntityTypes_ReturnsCustomers()
    {
        const string query = "test";
        EntityType[] linkedEntityTypes = new[] { EntityType.Companies, EntityType.Contacts };
        Customer[] customers1 = [new Customer { Id = 1, Name = "Customer 1" }];
        Customer[] customers2 = [new Customer { Id = 2, Name = "Customer 2" }];
        var response1 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Customers = customers1 },
            PaginationLinks = new PaginationLinksResponse { Next = new NavigationLink { Uri = "https://example.amocrm.ru/api/v4/customers?page=2&limit=1" } }
        };
        var response2 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Customers = customers2 }
        };

        ResponseHandlerMock
            .SetupSequence(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response1)
            .ReturnsAsync(response2);

        IReadOnlyCollection<Customer> result = await Client.GetCustomersInternalAsync(TestAccessToken, linkedEntityTypes, query);

        result.Should().NotBeNull().And.HaveCount(2);
        result.First().Id.Should().Be(1);
        result.First().Name.Should().Be("Customer 1");
        result.Last().Id.Should().Be(2);
        result.Last().Name.Should().Be("Customer 2");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task AddCustomersAsync_ValidRequests_ReturnsCreatedCustomers()
    {
        AddCustomerRequest[] requests = [new AddCustomerRequest("Customer 1") { NextPrice = 500 }];
        Customer[] expectedCustomers = [new Customer { Id = 1, Name = "Customer 1", NextPrice = 500 }];
        var response = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Customers = expectedCustomers }
        };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Customer> result = await Client.AddCustomersAsync(TestAccessToken, TestSubdomain, requests);

        result.Should().NotBeNull().And.HaveCount(1);
        result.First().Id.Should().Be(1);
        result.First().Name.Should().Be("Customer 1");
        result.First().NextPrice.Should().Be(500);

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AddCustomersInternalAsync_ValidRequests_ReturnsCreatedCustomers()
    {
        AddCustomerRequest[] requests = [new AddCustomerRequest("Customer 1") { NextPrice = 500 }];
        Customer[] expectedCustomers = [new Customer { Id = 1, Name = "Customer 1", NextPrice = 500 }];
        var response = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Customers = expectedCustomers }
        };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Customer> result = await Client.AddCustomersInternalAsync(TestAccessToken, requests);

        result.Should().NotBeNull().And.HaveCount(1);
        result.First().Id.Should().Be(1);
        result.First().Name.Should().Be("Customer 1");
        result.First().NextPrice.Should().Be(500);

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task UpdateCustomersAsync_ValidRequests_ReturnsUpdatedCustomers()
    {
        UpdateCustomerRequest[] requests = [new UpdateCustomerRequest(1, "Updated Customer") { NextPrice = 600 }];
        Customer[] expectedCustomers = [new Customer { Id = 1, Name = "Updated Customer", NextPrice = 600 }];
        var response = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Customers = expectedCustomers }
        };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Customer> result = await Client.UpdateCustomersAsync(TestAccessToken, TestSubdomain, requests);

        result.Should().NotBeNull().And.HaveCount(1);
        result.First().Id.Should().Be(1);
        result.First().Name.Should().Be("Updated Customer");
        result.First().NextPrice.Should().Be(600);

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task UpdateCustomersInternalAsync_ValidRequests_ReturnsUpdatedCustomers()
    {
        UpdateCustomerRequest[] requests = [new UpdateCustomerRequest(1, "Updated Customer") { NextPrice = 600 }];
        Customer[] expectedCustomers = [new Customer { Id = 1, Name = "Updated Customer", NextPrice = 600 }];
        var response = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Customers = expectedCustomers }
        };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Customer> result = await Client.UpdateCustomersInternalAsync(TestAccessToken, requests);

        result.Should().NotBeNull().And.HaveCount(1);
        result.First().Id.Should().Be(1);
        result.First().Name.Should().Be("Updated Customer");
        result.First().NextPrice.Should().Be(600);

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetAccountAsync_ValidParameters_ReturnsAccount()
    {
        var expectedAccount = new AccountModel { Id = 1, Name = "Test Account" };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<AccountModel>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedAccount);

        AccountModel result = await Client.GetAccountAsync(TestAccessToken, TestSubdomain);

        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Name.Should().Be("Test Account");

        ResponseHandlerMock.Verify(x => x.HandleAsync<AccountModel>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetAccountInternalAsync_ValidParameters_ReturnsAccount()
    {
        var expectedAccount = new AccountModel { Id = 1, Name = "Test Account" };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<AccountModel>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedAccount);

        AccountModel result = await Client.GetAccountInternalAsync(TestAccessToken);

        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Name.Should().Be("Test Account");

        ResponseHandlerMock.Verify(x => x.HandleAsync<AccountModel>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetUsersAsync_ValidParameters_ReturnsUsers()
    {
        User[] users1 = [new User { Id = 1, FullName = "User 1" }];
        User[] users2 = [new User { Id = 2, FullName = "User 2" }];
        var response1 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Users = users1 },
            PaginationLinks = new PaginationLinksResponse { Next = new NavigationLink { Uri = "  https://example.amocrm.ru/api/v4/users?page=2&limit=1" } }
        };
        var response2 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Users = users2 }
        };

        ResponseHandlerMock
            .SetupSequence(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response1)
            .ReturnsAsync(response2);

        IReadOnlyCollection<User> result = await Client.GetUsersAsync(TestAccessToken, TestSubdomain);

        result.Should().NotBeNull().And.HaveCount(2);
        result.First().Id.Should().Be(1);
        result.First().FullName.Should().Be("User 1");
        result.Last().Id.Should().Be(2);
        result.Last().FullName.Should().Be("User 2");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task GetUsersInternalAsync_ValidParameters_ReturnsUsers()
    {
        User[] users1 = [new User { Id = 1, FullName = "User 1" }];
        User[] users2 = [new User { Id = 2, FullName = "User 2" }];
        var response1 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Users = users1 },
            PaginationLinks = new PaginationLinksResponse { Next = new NavigationLink { Uri = "  https://example.amocrm.ru/api/v4/users?page=2&limit=1" } }
        };
        var response2 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Users = users2 }
        };

        ResponseHandlerMock
            .SetupSequence(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response1)
            .ReturnsAsync(response2);

        IReadOnlyCollection<User> result = await Client.GetUsersInternalAsync(TestAccessToken);

        result.Should().NotBeNull().And.HaveCount(2);
        result.First().Id.Should().Be(1);
        result.First().FullName.Should().Be("User 1");
        result.Last().Id.Should().Be(2);
        result.Last().FullName.Should().Be("User 2");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task GetWidgetAsync_ValidParameters_ReturnsWidget()
    {
        const string widgetCode = "test-widget";
        var expectedWidget = new Widget { Id = 1, Code = "test-widget" };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<Widget>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedWidget);

        Widget result = await Client.GetWidgetAsync(TestAccessToken, TestSubdomain, widgetCode);

        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Code.Should().Be("test-widget");

        ResponseHandlerMock.Verify(x => x.HandleAsync<Widget>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetContactsAsync_ValidQuery_ReturnsContacts()
    {
        const string query = "test";
        Contact[] contacts1 = [new Contact { Id = 1, Name = "Contact 1" }];
        Contact[] contacts2 = [new Contact { Id = 2, Name = "Contact 2" }];
        var response1 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Contacts = contacts1 },
            PaginationLinks = new PaginationLinksResponse { Next = new NavigationLink { Uri = "  https://example.amocrm.ru/api/v4/contacts?page=2&limit=1" } }
        };
        var response2 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Contacts = contacts2 }
        };

        ResponseHandlerMock
            .SetupSequence(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response1)
            .ReturnsAsync(response2);

        IReadOnlyCollection<Contact> result = await Client.GetContactsAsync(TestAccessToken, TestSubdomain, query);

        result.Should().NotBeNull().And.HaveCount(2);
        result.First().Id.Should().Be(1);
        result.First().Name.Should().Be("Contact 1");
        result.Last().Id.Should().Be(2);
        result.Last().Name.Should().Be("Contact 2");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task GetContactsAsync_ValidQueryAndLinkedEntityTypes_ReturnsContacts()
    {
        const string query = "test";
        EntityType[] linkedEntityTypes = new[] { EntityType.Leads, EntityType.Companies };
        Contact[] contacts1 = [new Contact { Id = 1, Name = "Contact 1" }];
        Contact[] contacts2 = [new Contact { Id = 2, Name = "Contact 2" }];
        var response1 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Contacts = contacts1 },
            PaginationLinks = new PaginationLinksResponse { Next = new NavigationLink { Uri = "https://example.amocrm.ru/api/v4/contacts?page=2&limit=1" } }
        };
        var response2 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Contacts = contacts2 }
        };

        ResponseHandlerMock
            .SetupSequence(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response1)
            .ReturnsAsync(response2);

        IReadOnlyCollection<Contact> result = await Client.GetContactsAsync(TestAccessToken, TestSubdomain, linkedEntityTypes, query);

        result.Should().NotBeNull().And.HaveCount(2);
        result.First().Id.Should().Be(1);
        result.First().Name.Should().Be("Contact 1");
        result.Last().Id.Should().Be(2);
        result.Last().Name.Should().Be("Contact 2");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task GetContactsInternalAsync_ValidQuery_ReturnsContacts()
    {
        const string query = "test";
        Contact[] contacts1 = [new Contact { Id = 1, Name = "Contact 1" }];
        Contact[] contacts2 = [new Contact { Id = 2, Name = "Contact 2" }];
        var response1 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Contacts = contacts1 },
            PaginationLinks = new PaginationLinksResponse { Next = new NavigationLink { Uri = "  https://example.amocrm.ru/api/v4/contacts?page=2&limit=1" } }
        };
        var response2 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Contacts = contacts2 }
        };

        ResponseHandlerMock
            .SetupSequence(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response1)
            .ReturnsAsync(response2);

        IReadOnlyCollection<Contact> result = await Client.GetContactsInternalAsync(TestAccessToken, query);

        result.Should().NotBeNull().And.HaveCount(2);
        result.First().Id.Should().Be(1);
        result.First().Name.Should().Be("Contact 1");
        result.Last().Id.Should().Be(2);
        result.Last().Name.Should().Be("Contact 2");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task GetContactsInternalAsync_ValidQueryAndLinkedEntityTypes_ReturnsContacts()
    {
        const string query = "test";
        EntityType[] linkedEntityTypes = new[] { EntityType.Customers, EntityType.Leads };
        Contact[] contacts1 = [new Contact { Id = 1, Name = "Contact 1" }];
        Contact[] contacts2 = [new Contact { Id = 2, Name = "Contact 2" }];
        var response1 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Contacts = contacts1 },
            PaginationLinks = new PaginationLinksResponse { Next = new NavigationLink { Uri = "https://example.amocrm.ru/api/v4/contacts?page=2&limit=1" } }
        };
        var response2 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Contacts = contacts2 }
        };

        ResponseHandlerMock
            .SetupSequence(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response1)
            .ReturnsAsync(response2);

        IReadOnlyCollection<Contact> result = await Client.GetContactsInternalAsync(TestAccessToken, linkedEntityTypes, query);

        result.Should().NotBeNull().And.HaveCount(2);
        result.First().Id.Should().Be(1);
        result.First().Name.Should().Be("Contact 1");
        result.Last().Id.Should().Be(2);
        result.Last().Name.Should().Be("Contact 2");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task GetContactByIdAsync_ValidId_ReturnsContact()
    {
        const int contactId = 1;
        var expectedContact = new Contact { Id = 1, Name = "Contact 1", FirstName = "John" };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<Contact>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedContact);

        Contact result = await Client.GetContactByIdAsync(TestAccessToken, TestSubdomain, contactId);

        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Name.Should().Be("Contact 1");

        ResponseHandlerMock.Verify(x => x.HandleAsync<Contact>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetContactByIdAsync_ValidIdAndLinkedEntityTypes_ReturnsContact()
    {
        const int contactId = 1;
        EntityType[] linkedEntityTypes = new[] { EntityType.Leads };
        var expectedContact = new Contact { Id = 1, Name = "Contact 1", FirstName = "John" };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<Contact>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedContact);

        Contact result = await Client.GetContactByIdAsync(TestAccessToken, TestSubdomain, contactId, linkedEntityTypes);

        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Name.Should().Be("Contact 1");

        ResponseHandlerMock.Verify(x => x.HandleAsync<Contact>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetContactByIdInternalAsync_ValidId_ReturnsContact()
    {
        const int contactId = 1;
        var expectedContact = new Contact { Id = 1, Name = "Contact 1", FirstName = "John" };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<Contact>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedContact);

        Contact result = await Client.GetContactByIdInternalAsync(TestAccessToken, contactId);

        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Name.Should().Be("Contact 1");

        ResponseHandlerMock.Verify(x => x.HandleAsync<Contact>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetContactByIdInternalAsync_ValidIdAndLinkedEntityTypes_ReturnsContact()
    {
        const int contactId = 1;
        EntityType[] linkedEntityTypes = new[] { EntityType.Companies };
        var expectedContact = new Contact { Id = 1, Name = "Contact 1", FirstName = "John" };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<Contact>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedContact);

        Contact result = await Client.GetContactByIdInternalAsync(TestAccessToken, contactId, linkedEntityTypes);

        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Name.Should().Be("Contact 1");

        ResponseHandlerMock.Verify(x => x.HandleAsync<Contact>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AddContactsAsync_ValidRequests_ReturnsCreatedContacts()
    {
        AddContactRequest[] requests = [new AddContactRequest { Name = "Contact 1", FirstName = "John" }];
        Contact[] expectedContacts = [new Contact { Id = 1, Name = "Contact 1", FirstName = "John" }];
        var response = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Contacts = expectedContacts }
        };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Contact> result = await Client.AddContactsAsync(TestAccessToken, TestSubdomain, requests);

        result.Should().NotBeNull().And.HaveCount(1);
        result.First().Id.Should().Be(1);
        result.First().Name.Should().Be("Contact 1");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AddContactsInternalAsync_ValidRequests_ReturnsCreatedContacts()
    {
        AddContactRequest[] requests = [new AddContactRequest { Name = "Contact 1", FirstName = "John" }];
        Contact[] expectedContacts = [new Contact { Id = 1, Name = "Contact 1", FirstName = "John" }];
        var response = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Contacts = expectedContacts }
        };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Contact> result = await Client.AddContactsInternalAsync(TestAccessToken, requests);

        result.Should().NotBeNull().And.HaveCount(1);
        result.First().Id.Should().Be(1);
        result.First().Name.Should().Be("Contact 1");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task UpdateContactsAsync_ValidRequests_ReturnsUpdatedContacts()
    {
        UpdateContactRequest[] requests = [new UpdateContactRequest(1) { Name = "Updated Contact" }];
        Contact[] expectedContacts = [new Contact { Id = 1, Name = "Updated Contact" }];
        var response = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Contacts = expectedContacts }
        };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Contact> result = await Client.UpdateContactsAsync(TestAccessToken, TestSubdomain, requests);

        result.Should().NotBeNull().And.HaveCount(1);
        result.First().Id.Should().Be(1);
        result.First().Name.Should().Be("Updated Contact");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task UpdateContactsInternalAsync_ValidRequests_ReturnsUpdatedContacts()
    {
        UpdateContactRequest[] requests = [new UpdateContactRequest(1) { Name = "Updated Contact" }];
        Contact[] expectedContacts = [new Contact { Id = 1, Name = "Updated Contact" }];
        var response = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Contacts = expectedContacts }
        };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Contact> result = await Client.UpdateContactsInternalAsync(TestAccessToken, requests);

        result.Should().NotBeNull().And.HaveCount(1);
        result.First().Id.Should().Be(1);
        result.First().Name.Should().Be("Updated Contact");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetTransactionsAsync_ValidParameters_ReturnsTransactions()
    {
        const int customerId = 1;
        Transaction[] transactions1 = [new Transaction { Id = 1, Price = 100 }];
        Transaction[] transactions2 = [new Transaction { Id = 2, Price = 200 }];
        var response1 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Transactions = transactions1 },
            PaginationLinks = new PaginationLinksResponse { Next = new NavigationLink { Uri = "  https://example.amocrm.ru/api/v4/customers/transactions?page=2&limit=1" } }
        };
        var response2 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Transactions = transactions2 }
        };

        ResponseHandlerMock
            .SetupSequence(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response1)
            .ReturnsAsync(response2);

        IReadOnlyCollection<Transaction> result = await Client.GetTransactionsAsync(TestAccessToken, TestSubdomain, customerId);

        result.Should().NotBeNull().And.HaveCount(2);
        result.First().Id.Should().Be(1);
        result.First().Price.Should().Be(100);
        result.Last().Id.Should().Be(2);
        result.Last().Price.Should().Be(200);

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task GetTransactionsInternalAsync_ValidParameters_ReturnsTransactions()
    {
        const int customerId = 1;
        Transaction[] transactions1 = [new Transaction { Id = 1, Price = 100 }];
        Transaction[] transactions2 = [new Transaction { Id = 2, Price = 200 }];
        var response1 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Transactions = transactions1 },
            PaginationLinks = new PaginationLinksResponse { Next = new NavigationLink { Uri = "  https://example.amocrm.ru/api/v4/customers/transactions?page=2&limit=1" } }
        };
        var response2 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Transactions = transactions2 }
        };

        ResponseHandlerMock
            .SetupSequence(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response1)
            .ReturnsAsync(response2);

        IReadOnlyCollection<Transaction> result = await Client.GetTransactionsInternalAsync(TestAccessToken, customerId);

        result.Should().NotBeNull().And.HaveCount(2);
        result.First().Id.Should().Be(1);
        result.First().Price.Should().Be(100);
        result.Last().Id.Should().Be(2);
        result.Last().Price.Should().Be(200);

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task AddTransactionsAsync_ValidRequests_ReturnsCreatedTransactions()
    {
        const int customerId = 1;
        AddTransactionRequest[] requests = [new AddTransactionRequest(1000) { Comment = "Transaction 1" }];
        Transaction[] expectedTransactions = [new Transaction { Id = 1, CustomerId = customerId, Price = 1000, Comment = "Transaction 1" }];
        var response = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Transactions = expectedTransactions }
        };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Transaction> result = await Client.AddTransactionsAsync(TestAccessToken, TestSubdomain, customerId, requests);

        result.Should().NotBeNull().And.HaveCount(1);
        result.First().Id.Should().Be(1);
        result.First().CustomerId.Should().Be(customerId);
        result.First().Price.Should().Be(1000);

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AddTransactionsInternalAsync_ValidRequests_ReturnsCreatedTransactions()
    {
        const int customerId = 1;
        AddTransactionRequest[] requests = [new AddTransactionRequest(1000) { Comment = "Transaction 1" }];
        Transaction[] expectedTransactions = [new Transaction { Id = 1, CustomerId = customerId, Price = 1000, Comment = "Transaction 1" }];
        var response = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Transactions = expectedTransactions }
        };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Transaction> result = await Client.AddTransactionsInternalAsync(TestAccessToken, customerId, requests);

        result.Should().NotBeNull().And.HaveCount(1);
        result.First().Id.Should().Be(1);
        result.First().CustomerId.Should().Be(customerId);
        result.First().Price.Should().Be(1000);

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task DeleteTransactionAsync_DoesNotThrow()
    {
        var transactionId = 123456;

        await FluentActions
            .Invoking(async () => await Client.DeleteTransactionAsync(TestAccessToken, TestSubdomain, transactionId).ConfigureAwait(false))
            .Should().NotThrowAsync();
    }

    [Fact]
    public async Task DeleteTransactionInternalAsync_DoesNotThrow()
    {
        var transactionId = 123456;

        await FluentActions
            .Invoking(async () => await Client.DeleteTransactionInternalAsync(TestAccessToken, transactionId).ConfigureAwait(false))
            .Should().NotThrowAsync();
    }

    [Fact]
    public async Task GetCustomFieldsAsync_ValidParameters_ReturnsCustomFields()
    {
        EntityType entityType = EntityType.Leads;
        CustomField[] customFields1 = [new CustomField { Id = 1, Name = "Field 1" }];
        CustomField[] customFields2 = [new CustomField { Id = 2, Name = "Field 2" }];
        var response1 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { CustomFields = customFields1 },
            PaginationLinks = new PaginationLinksResponse { Next = new NavigationLink { Uri = "https://example.amocrm.ru/api/v4/leads/custom_fields?page=2&limit=1" } }
        };
        var response2 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { CustomFields = customFields2 }
        };

        ResponseHandlerMock
            .SetupSequence(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response1)
            .ReturnsAsync(response2);

        IReadOnlyCollection<CustomField> result = await Client.GetCustomFieldsAsync(TestAccessToken, TestSubdomain, entityType);

        result.Should().NotBeNull().And.HaveCount(2);
        result.First().Id.Should().Be(1);
        result.First().Name.Should().Be("Field 1");
        result.Last().Id.Should().Be(2);
        result.Last().Name.Should().Be("Field 2");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task GetCustomFieldsInternalAsync_ValidParameters_ReturnsCustomFields()
    {
        EntityType entityType = EntityType.Leads;
        CustomField[] customFields1 = [new CustomField { Id = 1, Name = "Field 1" }];
        CustomField[] customFields2 = [new CustomField { Id = 2, Name = "Field 2" }];
        var response1 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { CustomFields = customFields1 },
            PaginationLinks = new PaginationLinksResponse { Next = new NavigationLink { Uri = "  https://example.amocrm.ru/api/v4/leads/custom_fields?page=2&limit=1" } }
        };
        var response2 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { CustomFields = customFields2 }
        };

        ResponseHandlerMock
            .SetupSequence(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response1)
            .ReturnsAsync(response2);

        IReadOnlyCollection<CustomField> result = await Client.GetCustomFieldsInternalAsync(TestAccessToken, entityType);

        result.Should().NotBeNull().And.HaveCount(2);
        result.First().Id.Should().Be(1);
        result.First().Name.Should().Be("Field 1");
        result.Last().Id.Should().Be(2);
        result.Last().Name.Should().Be("Field 2");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task GetLeadStatusesAsync_ValidParameters_ReturnsLeadStatuses()
    {
        LeadStatus[] statuses1 = [new LeadStatus { Id = 1, Name = "Status 1" }];
        LeadStatus[] statuses2 = [new LeadStatus { Id = 2, Name = "Status 2" }];
        var response1 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse
            {
                Pipelines = [new Pipeline() { AvailableStatuses = new PipelineStatusesContainer { Statuses = statuses1 } }]
            },
            PaginationLinks = new PaginationLinksResponse { Next = new NavigationLink { Uri = "https://example.amocrm.ru/api/v4/pipelines?page=2&limit=1" } }
        };
        var response2 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse
            {
                Pipelines = [new Pipeline() { AvailableStatuses = new PipelineStatusesContainer { Statuses = statuses2 } }]
            }
        };

        ResponseHandlerMock
            .SetupSequence(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response1)
            .ReturnsAsync(response2);

        IReadOnlyCollection<LeadStatus> result = await Client.GetLeadStatusesAsync(TestAccessToken, TestSubdomain);

        result.Should().NotBeNull().And.HaveCount(2);
        result.First().Id.Should().Be(1);
        result.First().Name.Should().Be("Status 1");
        result.Last().Id.Should().Be(2);
        result.Last().Name.Should().Be("Status 2");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task GetLeadStatusesInternalAsync_ValidParameters_ReturnsLeadStatuses()
    {
        LeadStatus[] statuses1 = [new LeadStatus { Id = 1, Name = "Status 1" }];
        LeadStatus[] statuses2 = [new LeadStatus { Id = 2, Name = "Status 2" }];
        var response1 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse
            {
                Pipelines = [new Pipeline() { AvailableStatuses = new PipelineStatusesContainer { Statuses = statuses1 } }]
            },
            PaginationLinks = new PaginationLinksResponse { Next = new NavigationLink { Uri = "  https://example.amocrm.ru/api/v4/pipelines?page=2&limit=1" } }
        };
        var response2 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse
            {
                Pipelines = [new Pipeline() { AvailableStatuses = new PipelineStatusesContainer { Statuses = statuses2 } }]
            }
        };

        ResponseHandlerMock
            .SetupSequence(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response1)
            .ReturnsAsync(response2);

        IReadOnlyCollection<LeadStatus> result = await Client.GetLeadStatusesInternalAsync(TestAccessToken);

        result.Should().NotBeNull().And.HaveCount(2);
        result.First().Id.Should().Be(1);
        result.First().Name.Should().Be("Status 1");
        result.Last().Id.Should().Be(2);
        result.Last().Name.Should().Be("Status 2");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task GetNotesAsync_ValidParameters_ReturnsNotes()
    {
        EntityType entityType = EntityType.Leads;
        NoteType noteType = NoteType.Common;
        Note[] notes1 = [new Note { Id = 1, EntityId = 1, NoteType = NoteType.Common }];
        Note[] notes2 = [new Note { Id = 2, EntityId = 2, NoteType = NoteType.Common }];
        var response1 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Notes = notes1 },
            PaginationLinks = new PaginationLinksResponse { Next = new NavigationLink { Uri = "  https://example.amocrm.ru/api/v4/notes?page=2&limit=1" } }
        };
        var response2 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Notes = notes2 }
        };

        ResponseHandlerMock
            .SetupSequence(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response1)
            .ReturnsAsync(response2);

        IReadOnlyCollection<Note> result = await Client.GetNotesAsync(TestAccessToken, TestSubdomain, entityType, noteType);

        result.Should().NotBeNull().And.HaveCount(2);
        result.First().Id.Should().Be(1);
        result.First().EntityId.Should().Be(1);
        result.Last().Id.Should().Be(2);
        result.Last().EntityId.Should().Be(2);

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task GetNotesInternalAsync_ValidParameters_ReturnsNotes()
    {
        EntityType entityType = EntityType.Leads;
        NoteType noteType = NoteType.Common;
        Note[] notes1 = [new Note { Id = 1, EntityId = 1, NoteType = NoteType.Common }];
        Note[] notes2 = [new Note { Id = 2, EntityId = 2, NoteType = NoteType.Common }];
        var response1 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Notes = notes1 },
            PaginationLinks = new PaginationLinksResponse { Next = new NavigationLink { Uri = "  https://example.amocrm.ru/api/v4/notes?page=2&limit=1" } }
        };
        var response2 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Notes = notes2 }
        };

        ResponseHandlerMock
            .SetupSequence(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response1)
            .ReturnsAsync(response2);

        IReadOnlyCollection<Note> result = await Client.GetNotesInternalAsync(TestAccessToken, entityType, noteType);

        result.Should().NotBeNull().And.HaveCount(2);
        result.First().Id.Should().Be(1);
        result.First().EntityId.Should().Be(1);
        result.Last().Id.Should().Be(2);
        result.Last().EntityId.Should().Be(2);

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task AddNotesAsync_ValidRequests_ReturnsCreatedNotes()
    {
        EntityType entityType = EntityType.Leads;
        AddNoteRequest[] requests = [new AddNoteRequest(1, NoteType.Common) { Parameters = new NoteDetails { Text = "Note 1" } }];
        Note[] expectedNotes = [new Note { Id = 1, EntityId = 1, NoteType = NoteType.Common, Parameters = new NoteDetails { Text = "Note 1" } }];
        var response = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Notes = expectedNotes }
        };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Note> result = await Client.AddNotesAsync(TestAccessToken, TestSubdomain, entityType, requests);

        result.Should().NotBeNull().And.HaveCount(1);
        result.First().Id.Should().Be(1);
        result.First().EntityId.Should().Be(1);
        result.First().Parameters.Should().NotBeNull();
        result.First().Parameters!.Text.Should().Be("Note 1");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AddNotesInternalAsync_ValidRequests_ReturnsCreatedNotes()
    {
        EntityType entityType = EntityType.Leads;
        AddNoteRequest[] requests = [new AddNoteRequest(1, NoteType.Common) { Parameters = new NoteDetails { Text = "Note 1" } }];
        Note[] expectedNotes = [new Note { Id = 1, EntityId = 1, NoteType = NoteType.Common, Parameters = new NoteDetails { Text = "Note 1" } }];
        var response = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Notes = expectedNotes }
        };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Note> result = await Client.AddNotesInternalAsync(TestAccessToken, entityType, requests);

        result.Should().NotBeNull().And.HaveCount(1);
        result.First().Id.Should().Be(1);
        result.First().EntityId.Should().Be(1);
        result.First().Parameters.Should().NotBeNull();
        result.First().Parameters!.Text.Should().Be("Note 1");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetLinksAsync_ValidFilter_ReturnsLinks()
    {
        var filter = new EntityLinksFilter([11, 12]) { LinkedEntityId = 1, LinkedEntityType = EntityType.Contacts };
        EntityLink[] links1 = [new EntityLink { EntityId = 11, LinkedEntityId = 1, LinkedEntityType = EntityType.Contacts }];
        EntityLink[] links2 = [new EntityLink { EntityId = 12, LinkedEntityId = 1, LinkedEntityType = EntityType.Contacts }];
        var response1 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Links = links1 },
            PaginationLinks = new PaginationLinksResponse
            {
                Next = new NavigationLink
                {
                    Uri = "https://example.amocrm.ru/api/v4/leads/links?filter[entity_id][0]=11&filter[entity_id][1]=12&filter[to_entity_type]=contacts&filter[to_entity_id]=1&page=2&limit=1"
                }
            }
        };
        var response2 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Links = links2 }
        };

        ResponseHandlerMock
            .SetupSequence(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response1)
            .ReturnsAsync(response2);

        IReadOnlyCollection<EntityLink> result = await Client.GetLinksAsync(TestAccessToken, TestSubdomain, EntityType.Leads, filter);

        result.Should().NotBeNull().And.HaveCount(2);
        result.First().EntityId.Should().Be(11);
        result.First().LinkedEntityId.Should().Be(1);
        result.First().LinkedEntityType.Should().Be(EntityType.Contacts);
        result.Last().EntityId.Should().Be(12);
        result.Last().LinkedEntityId.Should().Be(1);
        result.Last().LinkedEntityType.Should().Be(EntityType.Contacts);

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task GetLinksInternalAsync_ValidFilter_ReturnsLinks()
    {
        var filter = new EntityLinksFilter([11, 12]) { LinkedEntityId = 1, LinkedEntityType = EntityType.Contacts };
        EntityLink[] links1 = [new EntityLink { EntityId = 11, LinkedEntityId = 1, LinkedEntityType = EntityType.Contacts }];
        EntityLink[] links2 = [new EntityLink { EntityId = 12, LinkedEntityId = 1, LinkedEntityType = EntityType.Contacts }];
        var response1 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Links = links1 },
            PaginationLinks = new PaginationLinksResponse
            {
                Next = new NavigationLink
                {
                    Uri = "https://example.amocrm.ru/api/v4/leads/links?filter[entity_id][0]=11&filter[entity_id][1]=12&filter[to_entity_type]=contacts&filter[to_entity_id]=1&page=2&limit=1"
                }
            }
        };
        var response2 = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Links = links2 }
        };

        ResponseHandlerMock
            .SetupSequence(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response1)
            .ReturnsAsync(response2);

        IReadOnlyCollection<EntityLink> result = await Client.GetLinksInternalAsync(TestAccessToken, EntityType.Leads, filter);

        result.Should().NotBeNull().And.HaveCount(2);
        result.First().EntityId.Should().Be(11);
        result.First().LinkedEntityId.Should().Be(1);
        result.First().LinkedEntityType.Should().Be(EntityType.Contacts);
        result.Last().EntityId.Should().Be(12);
        result.Last().LinkedEntityId.Should().Be(1);
        result.Last().LinkedEntityType.Should().Be(EntityType.Contacts);

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task LinkAsync_ValidRequests_ReturnsCreatedLinks()
    {
        EntityType entityType = EntityType.Leads;
        LinkEntitiesRequest[] requests = [new LinkEntitiesRequest(entityId: 11) { LinkedEntityId = 1, LinkedEntityType = EntityType.Contacts }];
        EntityLink[] expectedLinks = [new EntityLink { EntityId = 11, LinkedEntityId = 1, LinkedEntityType = EntityType.Contacts }];
        var response = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Links = expectedLinks }
        };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<EntityLink> result = await Client.LinkAsync(TestAccessToken, TestSubdomain, entityType, requests);

        result.Should().NotBeNull().And.HaveCount(1);
        result.First().EntityId.Should().Be(11);
        result.First().LinkedEntityId.Should().Be(1);
        result.First().LinkedEntityType.Should().Be(EntityType.Contacts);

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task LinkInternalAsync_ValidRequests_ReturnsCreatedLinks()
    {
        EntityType entityType = EntityType.Leads;
        LinkEntitiesRequest[] requests = [new LinkEntitiesRequest(entityId: 11) { LinkedEntityId = 1, LinkedEntityType = EntityType.Contacts }];
        EntityLink[] expectedLinks = [new EntityLink { EntityId = 11, LinkedEntityId = 1, LinkedEntityType = EntityType.Contacts }];
        var response = new EntitiesResponse
        {
            Embedded = new EmbeddedEntitiesResponse { Links = expectedLinks }
        };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<EntityLink> result = await Client.LinkInternalAsync(TestAccessToken, entityType, requests);

        result.Should().NotBeNull().And.HaveCount(1);
        result.First().EntityId.Should().Be(11);
        result.First().LinkedEntityId.Should().Be(1);
        result.First().LinkedEntityType.Should().Be(EntityType.Contacts);

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AuthorizeAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        const string authCode = "auth-code";
        const string redirectDomain = "  https://example.com  ";

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.AuthorizeAsync(TestSubdomain, authCode, redirectDomain).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AuthorizeInternalAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.AuthorizeInternalAsync().ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task RefreshTokenAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        const string refreshToken = "refresh-token";
        const string redirectDomain = "https://example.com  ";

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.RefreshTokenAsync(TestSubdomain, refreshToken, redirectDomain).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task RefreshTokenInternalAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        const string refreshToken = "internal-refresh-token";

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.RefreshTokenInternalAsync(refreshToken).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetLeadsAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.GetLeadsAsync(TestAccessToken, TestSubdomain).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetLeadsAsync_LinkedEntityTypes_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        EntityType[] linkedEntityTypes = new[] { EntityType.Contacts };
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.GetLeadsAsync(TestAccessToken, TestSubdomain, linkedEntityTypes).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetLeadsInternalAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.GetLeadsInternalAsync(TestAccessToken).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetLeadsInternalAsync_LinkedEntityTypes_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        EntityType[] linkedEntityTypes = new[] { EntityType.Leads };
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.GetLeadsInternalAsync(TestAccessToken, linkedEntityTypes).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AddLeadsAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        AddLeadRequest[] requests = [new AddLeadRequest { Name = "Lead 1" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.AddLeadsAsync(TestAccessToken, TestSubdomain, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AddLeadsInternalAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        AddLeadRequest[] requests = [new AddLeadRequest { Name = "Lead 1" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.AddLeadsInternalAsync(TestAccessToken, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task UpdateLeadsAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        UpdateLeadRequest[] requests = [new UpdateLeadRequest(1) { Name = "Updated Lead" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.UpdateLeadsAsync(TestAccessToken, TestSubdomain, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task UpdateLeadsInternalAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        UpdateLeadRequest[] requests = [new UpdateLeadRequest(1) { Name = "Updated Lead" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.UpdateLeadsInternalAsync(TestAccessToken, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetCompaniesAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.GetCompaniesAsync(TestAccessToken, TestSubdomain).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetCompaniesAsync_LinkedEntityTypes_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        EntityType[] linkedEntityTypes = new[] { EntityType.Contacts };
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.GetCompaniesAsync(TestAccessToken, TestSubdomain, linkedEntityTypes).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetCompaniesInternalAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.GetCompaniesInternalAsync(TestAccessToken).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetCompaniesInternalAsync_LinkedEntityTypes_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        EntityType[] linkedEntityTypes = new[] { EntityType.Contacts };
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.GetCompaniesInternalAsync(TestAccessToken, linkedEntityTypes).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AddCompaniesAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        AddCompanyRequest[] requests = [new AddCompanyRequest { Name = "Company 1" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.AddCompaniesAsync(TestAccessToken, TestSubdomain, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AddCompaniesInternalAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        AddCompanyRequest[] requests = [new AddCompanyRequest { Name = "Company 1" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.AddCompaniesInternalAsync(TestAccessToken, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task UpdateCompaniesAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        UpdateCompanyRequest[] requests = [new UpdateCompanyRequest(1) { Name = "Updated Company" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.UpdateCompaniesAsync(TestAccessToken, TestSubdomain, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task UpdateCompaniesInternalAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        UpdateCompanyRequest[] requests = [new UpdateCompanyRequest(1) { Name = "Updated Company" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.UpdateCompaniesInternalAsync(TestAccessToken, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetTasksAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.GetTasksAsync(TestAccessToken, TestSubdomain).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetTasksInternalAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.GetTasksInternalAsync(TestAccessToken).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetTasksAsync_WithFilter_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        var filter = new TasksFilter { EntityIds = [11], EntityType = EntityType.Leads, TaskIds = [1, 2] };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.GetTasksAsync(TestAccessToken, TestSubdomain, filter).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetTasksInternalAsync_WithFilter_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        var filter = new TasksFilter { EntityIds = [11], EntityType = EntityType.Leads, TaskIds = [1, 2] };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.GetTasksInternalAsync(TestAccessToken, filter).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AddTasksAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        AddTaskRequest[] requests = [new AddTaskRequest("Task 1", 1234567890, EntityType.Leads) { EntityId = 1 }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.AddTasksAsync(TestAccessToken, TestSubdomain, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AddTasksInternalAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        AddTaskRequest[] requests = [new AddTaskRequest("Task 1", 1234567890, EntityType.Leads) { EntityId = 1 }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.AddTasksInternalAsync(TestAccessToken, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task UpdateTasksAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        UpdateTaskRequest[] requests = [new UpdateTaskRequest(1, "Updated Task", 123123123123)];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.UpdateTasksAsync(TestAccessToken, TestSubdomain, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task UpdateTasksInternalAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        UpdateTaskRequest[] requests = [new UpdateTaskRequest(1, "Updated Task", 123123123123)];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.UpdateTasksInternalAsync(TestAccessToken, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetCustomersAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.GetCustomersAsync(TestAccessToken, TestSubdomain).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetCustomersAsync_LinkedEntityTypes_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        EntityType[] linkedEntityTypes = new[] { EntityType.Contacts };
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.GetCustomersAsync(TestAccessToken, TestSubdomain, linkedEntityTypes).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetCustomersInternalAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.GetCustomersInternalAsync(TestAccessToken).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetCustomersInternalAsync_LinkedEntityTypes_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        EntityType[] linkedEntityTypes = new[] { EntityType.Leads };
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.GetCustomersInternalAsync(TestAccessToken, linkedEntityTypes).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AddCustomersAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        AddCustomerRequest[] requests = [new AddCustomerRequest("Customer 1") { NextPrice = 500 }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.AddCustomersAsync(TestAccessToken, TestSubdomain, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AddCustomersInternalAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        AddCustomerRequest[] requests = [new AddCustomerRequest("Customer 1") { NextPrice = 500 }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.AddCustomersInternalAsync(TestAccessToken, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task UpdateCustomersAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        UpdateCustomerRequest[] requests = [new UpdateCustomerRequest(1, "Updated Customer")];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.UpdateCustomersAsync(TestAccessToken, TestSubdomain, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task UpdateCustomersInternalAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        UpdateCustomerRequest[] requests = [new UpdateCustomerRequest(1, "Updated Customer")];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.UpdateCustomersInternalAsync(TestAccessToken, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetAccountAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<AccountModel>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.GetAccountAsync(TestAccessToken, TestSubdomain).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<AccountModel>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetAccountInternalAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<AccountModel>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.GetAccountInternalAsync(TestAccessToken).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<AccountModel>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetUsersAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.GetUsersAsync(TestAccessToken, TestSubdomain).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetUsersInternalAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.GetUsersInternalAsync(TestAccessToken).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetWidgetAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        const string widgetCode = "test-widget";

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<Widget>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.GetWidgetAsync(TestAccessToken, TestSubdomain, widgetCode).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<Widget>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetContactsAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.GetContactsAsync(TestAccessToken, TestSubdomain).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetContactsAsync_LinkedEntityTypes_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        EntityType[] linkedEntityTypes = new[] { EntityType.Leads };
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.GetContactsAsync(TestAccessToken, TestSubdomain, linkedEntityTypes).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetContactsInternalAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.GetContactsInternalAsync(TestAccessToken).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetContactsInternalAsync_LinkedEntityTypes_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        EntityType[] linkedEntityTypes = new[] { EntityType.Companies };
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.GetContactsInternalAsync(TestAccessToken, linkedEntityTypes).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetContactByIdAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        const int contactId = 1;

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<Contact>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.GetContactByIdAsync(TestAccessToken, TestSubdomain, contactId).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<Contact>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetContactByIdAsync_LinkedEntityTypes_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        const int contactId = 1;
        EntityType[] linkedEntityTypes = new[] { EntityType.Customers };
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<Contact>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.GetContactByIdAsync(TestAccessToken, TestSubdomain, contactId, linkedEntityTypes).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<Contact>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetContactByIdInternalAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        const int contactId = 1;

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<Contact>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.GetContactByIdInternalAsync(TestAccessToken, contactId).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<Contact>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetContactByIdInternalAsync_LinkedEntityTypes_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        const int contactId = 1;
        EntityType[] linkedEntityTypes = new[] { EntityType.Customers };
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<Contact>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.GetContactByIdInternalAsync(TestAccessToken, contactId, linkedEntityTypes).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<Contact>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AddContactsAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        AddContactRequest[] requests = [new AddContactRequest { Name = "Contact 1", FirstName = "John" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.AddContactsAsync(TestAccessToken, TestSubdomain, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AddContactsInternalAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        AddContactRequest[] requests = [new AddContactRequest { Name = "Contact 1", FirstName = "John" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.AddContactsInternalAsync(TestAccessToken, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task UpdateContactsAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        UpdateContactRequest[] requests = [new UpdateContactRequest(1) { Name = "Updated Contact" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.UpdateContactsAsync(TestAccessToken, TestSubdomain, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task UpdateContactsInternalAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        UpdateContactRequest[] requests = [new UpdateContactRequest(1) { Name = "Updated Contact" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.UpdateContactsInternalAsync(TestAccessToken, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetTransactionsAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        const int customerId = 1;

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.GetTransactionsAsync(TestAccessToken, TestSubdomain, customerId).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetTransactionsInternalAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        const int customerId = 1;

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.GetTransactionsInternalAsync(TestAccessToken, customerId).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AddTransactionsAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        const int customerId = 1;
        AddTransactionRequest[] requests = [new AddTransactionRequest(1000) { Comment = "Transaction 1" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.AddTransactionsAsync(TestAccessToken, TestSubdomain, customerId, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AddTransactionsInternalAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        const int customerId = 1;
        AddTransactionRequest[] requests = [new AddTransactionRequest(1000) { Comment = "Transaction 1" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.AddTransactionsInternalAsync(TestAccessToken, customerId, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetCustomFieldsAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        EntityType entityType = EntityType.Leads;

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.GetCustomFieldsAsync(TestAccessToken, TestSubdomain, entityType).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetCustomFieldsInternalAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        EntityType entityType = EntityType.Leads;

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.GetCustomFieldsInternalAsync(TestAccessToken, entityType).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetLeadStatusesAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.GetLeadStatusesAsync(TestAccessToken, TestSubdomain).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetLeadStatusesInternalAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.GetLeadStatusesInternalAsync(TestAccessToken).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetNotesAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        EntityType entityType = EntityType.Leads;
        NoteType noteType = NoteType.Common;

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.GetNotesAsync(TestAccessToken, TestSubdomain, entityType, noteType).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetNotesInternalAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        EntityType entityType = EntityType.Leads;
        NoteType noteType = NoteType.Common;

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.GetNotesInternalAsync(TestAccessToken, entityType, noteType).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AddNotesAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        EntityType entityType = EntityType.Leads;
        AddNoteRequest[] requests = [new AddNoteRequest(1, NoteType.Common) { Parameters = new NoteDetails { Text = "Note 1" } }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.AddNotesAsync(TestAccessToken, TestSubdomain, entityType, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AddNotesInternalAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        EntityType entityType = EntityType.Leads;
        AddNoteRequest[] requests = [new AddNoteRequest(1, NoteType.Common) { Parameters = new NoteDetails { Text = "Note 1" } }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.AddNotesInternalAsync(TestAccessToken, entityType, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetLinksAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.GetLinksAsync(TestAccessToken, TestSubdomain, EntityType.Leads, new EntityLinksFilter([1])).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetLinksInternalAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.GetLinksInternalAsync(TestAccessToken, EntityType.Leads, new EntityLinksFilter([1])).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task LinkAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        EntityType entityType = EntityType.Leads;
        LinkEntitiesRequest[] requests = [new LinkEntitiesRequest(entityId: 11) { LinkedEntityId = 1, LinkedEntityType = EntityType.Contacts }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.LinkAsync(TestAccessToken, TestSubdomain, entityType, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task LinkInternalAsync_AuthorizationError_ThrowsAmoCrmAuthorizationException()
    {
        EntityType entityType = EntityType.Leads;
        LinkEntitiesRequest[] requests = [new LinkEntitiesRequest(entityId: 11) { LinkedEntityId = 1, LinkedEntityType = EntityType.Contacts }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmAuthorizationException("Authorization failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmAuthorizationException> exception = await FluentActions
            .Invoking(async () => await Client.LinkInternalAsync(TestAccessToken, entityType, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmAuthorizationException>();

        exception.WithMessage("*Authorization failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AuthorizeAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        const string authCode = "auth-code";
        const string redirectDomain = "  https://example.com  ";

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.AuthorizeAsync(TestSubdomain, authCode, redirectDomain).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AuthorizeInternalAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.AuthorizeInternalAsync().ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task RefreshTokenAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        const string refreshToken = "refresh-token";
        const string redirectDomain = "https://example.com  ";

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.RefreshTokenAsync(TestSubdomain, refreshToken, redirectDomain).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task RefreshTokenInternalAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        const string refreshToken = "internal-refresh-token";

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.RefreshTokenInternalAsync(refreshToken).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetLeadsAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        const string accessToken = "access-token";

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.GetLeadsAsync(accessToken, TestSubdomain).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetLeadsAsync_LinkedEntityTypes_HttpError_ThrowsAmoCrmHttpException()
    {
        EntityType[] linkedEntityTypes = new[] { EntityType.Customers };
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.GetLeadsAsync(TestAccessToken, TestSubdomain, linkedEntityTypes).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetLeadsInternalAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        const string accessToken = "access-token";

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.GetLeadsInternalAsync(accessToken).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetLeadsInternalAsync_LinkedEntityTypes_HttpError_ThrowsAmoCrmHttpException()
    {
        EntityType[] linkedEntityTypes = new[] { EntityType.Contacts };
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.GetLeadsInternalAsync(TestAccessToken, linkedEntityTypes).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AddLeadsAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        AddLeadRequest[] requests = [new AddLeadRequest { Name = "Lead 1" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.AddLeadsAsync(TestAccessToken, TestSubdomain, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AddLeadsInternalAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        AddLeadRequest[] requests = [new AddLeadRequest { Name = "Lead 1" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.AddLeadsInternalAsync(TestAccessToken, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task UpdateLeadsAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        UpdateLeadRequest[] requests = [new UpdateLeadRequest(1) { Name = "Updated Lead" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.UpdateLeadsAsync(TestAccessToken, TestSubdomain, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task UpdateLeadsInternalAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        UpdateLeadRequest[] requests = [new UpdateLeadRequest(1) { Name = "Updated Lead" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.UpdateLeadsInternalAsync(TestAccessToken, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetCompaniesAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.GetCompaniesAsync(TestAccessToken, TestSubdomain).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetCompaniesAsync_LinkedEntityTypes_HttpError_ThrowsAmoCrmHttpException()
    {
        EntityType[] linkedEntityTypes = new[] { EntityType.Leads };
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.GetCompaniesAsync(TestAccessToken, TestSubdomain, linkedEntityTypes).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetCompaniesInternalAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.GetCompaniesInternalAsync(TestAccessToken).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetCompaniesInternalAsync_LinkedEntityTypes_HttpError_ThrowsAmoCrmHttpException()
    {
        EntityType[] linkedEntityTypes = new[] { EntityType.Leads };
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.GetCompaniesInternalAsync(TestAccessToken, linkedEntityTypes).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AddCompaniesAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        AddCompanyRequest[] requests = [new AddCompanyRequest { Name = "Company 1" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.AddCompaniesAsync(TestAccessToken, TestSubdomain, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AddCompaniesInternalAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        AddCompanyRequest[] requests = [new AddCompanyRequest { Name = "Company 1" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.AddCompaniesInternalAsync(TestAccessToken, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task UpdateCompaniesAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        UpdateCompanyRequest[] requests = [new UpdateCompanyRequest(1) { Name = "Updated Company" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.UpdateCompaniesAsync(TestAccessToken, TestSubdomain, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task UpdateCompaniesInternalAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        UpdateCompanyRequest[] requests = [new UpdateCompanyRequest(1) { Name = "Updated Company" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.UpdateCompaniesInternalAsync(TestAccessToken, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetTasksAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.GetTasksAsync(TestAccessToken, TestSubdomain).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetTasksInternalAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.GetTasksInternalAsync(TestAccessToken).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetTasksAsync_WithFilter_HttpError_ThrowsAmoCrmHttpException()
    {
        var filter = new TasksFilter { EntityIds = [11], EntityType = EntityType.Leads, TaskIds = [1, 2] };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.GetTasksAsync(TestAccessToken, TestSubdomain, filter).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetTasksInternalAsync_WithFilter_HttpError_ThrowsAmoCrmHttpException()
    {
        var filter = new TasksFilter { EntityIds = [11], EntityType = EntityType.Leads, TaskIds = [1, 2] };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.GetTasksInternalAsync(TestAccessToken, filter).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AddTasksAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        AddTaskRequest[] requests = [new AddTaskRequest("Task 1", 1234567890, EntityType.Leads) { EntityId = 1 }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.AddTasksAsync(TestAccessToken, TestSubdomain, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AddTasksInternalAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        AddTaskRequest[] requests = [new AddTaskRequest("Task 1", 1234567890, EntityType.Leads) { EntityId = 1 }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.AddTasksInternalAsync(TestAccessToken, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task UpdateTasksAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        UpdateTaskRequest[] requests = [new UpdateTaskRequest(1, "Updated Task", 123123123123)];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.UpdateTasksAsync(TestAccessToken, TestSubdomain, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task UpdateTasksInternalAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        UpdateTaskRequest[] requests = [new UpdateTaskRequest(1, "Updated Task", 123123123123)];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.UpdateTasksInternalAsync(TestAccessToken, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetCustomersAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.GetCustomersAsync(TestAccessToken, TestSubdomain).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetCustomersAsync_LinkedEntityTypes_HttpError_ThrowsAmoCrmHttpException()
    {
        EntityType[] linkedEntityTypes = new[] { EntityType.Companies };
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.GetCustomersAsync(TestAccessToken, TestSubdomain, linkedEntityTypes).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetCustomersInternalAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.GetCustomersInternalAsync(TestAccessToken).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetCustomersInternalAsync_LinkedEntityTypes_HttpError_ThrowsAmoCrmHttpException()
    {
        EntityType[] linkedEntityTypes = new[] { EntityType.Companies };
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.GetCustomersInternalAsync(TestAccessToken, linkedEntityTypes).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AddCustomersAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        AddCustomerRequest[] requests = [new AddCustomerRequest("Customer 1") { NextPrice = 500 }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.AddCustomersAsync(TestAccessToken, TestSubdomain, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AddCustomersInternalAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        AddCustomerRequest[] requests = [new AddCustomerRequest("Customer 1") { NextPrice = 500 }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.AddCustomersInternalAsync(TestAccessToken, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task UpdateCustomersAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        UpdateCustomerRequest[] requests = [new UpdateCustomerRequest(1, "Updated Customer")];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.UpdateCustomersAsync(TestAccessToken, TestSubdomain, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task UpdateCustomersInternalAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        UpdateCustomerRequest[] requests = [new UpdateCustomerRequest(1, "Updated Customer")];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.UpdateCustomersInternalAsync(TestAccessToken, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetAccountAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<AccountModel>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.GetAccountAsync(TestAccessToken, TestSubdomain).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<AccountModel>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetAccountInternalAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<AccountModel>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.GetAccountInternalAsync(TestAccessToken).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<AccountModel>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetUsersAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.GetUsersAsync(TestAccessToken, TestSubdomain).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetUsersInternalAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.GetUsersInternalAsync(TestAccessToken).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetWidgetAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        const string widgetCode = "test-widget";

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<Widget>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.GetWidgetAsync(TestAccessToken, TestSubdomain, widgetCode).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<Widget>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetContactsAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.GetContactsAsync(TestAccessToken, TestSubdomain).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetContactsAsync_LinkedEntityTypes_HttpError_ThrowsAmoCrmHttpException()
    {
        EntityType[] linkedEntityTypes = new[] { EntityType.Leads };
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.GetContactsAsync(TestAccessToken, TestSubdomain, linkedEntityTypes).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetContactsInternalAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.GetContactsInternalAsync(TestAccessToken).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetContactsInternalAsync_LinkedEntityTypes_HttpError_ThrowsAmoCrmHttpException()
    {
        EntityType[] linkedEntityTypes = new[] { EntityType.Companies };
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.GetContactsInternalAsync(TestAccessToken, linkedEntityTypes).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetContactByIdAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        const int contactId = 1;

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<Contact>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.GetContactByIdAsync(TestAccessToken, TestSubdomain, contactId).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<Contact>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetContactByIdAsync_LinkedEntityTypes_HttpError_ThrowsAmoCrmHttpException()
    {
        const int contactId = 1;
        EntityType[] linkedEntityTypes = new[] { EntityType.Customers };
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<Contact>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.GetContactByIdAsync(TestAccessToken, TestSubdomain, contactId, linkedEntityTypes).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<Contact>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetContactByIdInternalAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        const int contactId = 1;

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<Contact>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.GetContactByIdInternalAsync(TestAccessToken, contactId).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<Contact>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetContactByIdInternalAsync_LinkedEntityTypes_HttpError_ThrowsAmoCrmHttpException()
    {
        const int contactId = 1;
        EntityType[] linkedEntityTypes = new[] { EntityType.Customers };
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<Contact>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.GetContactByIdInternalAsync(TestAccessToken, contactId, linkedEntityTypes).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<Contact>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AddContactsAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        AddContactRequest[] requests = [new AddContactRequest { Name = "Contact 1", FirstName = "John" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.AddContactsAsync(TestAccessToken, TestSubdomain, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AddContactsInternalAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        AddContactRequest[] requests = [new AddContactRequest { Name = "Contact 1", FirstName = "John" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.AddContactsInternalAsync(TestAccessToken, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task UpdateContactsAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        UpdateContactRequest[] requests = [new UpdateContactRequest(1) { Name = "Updated Contact" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.UpdateContactsAsync(TestAccessToken, TestSubdomain, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task UpdateContactsInternalAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        UpdateContactRequest[] requests = [new UpdateContactRequest(1) { Name = "Updated Contact" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.UpdateContactsInternalAsync(TestAccessToken, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetTransactionsAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        const int customerId = 1;

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.GetTransactionsAsync(TestAccessToken, TestSubdomain, customerId).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetTransactionsInternalAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        const int customerId = 1;

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.GetTransactionsInternalAsync(TestAccessToken, customerId).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AddTransactionsAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        const int customerId = 1;
        AddTransactionRequest[] requests = [new AddTransactionRequest(1000) { Comment = "Transaction 1" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.AddTransactionsAsync(TestAccessToken, TestSubdomain, customerId, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AddTransactionsInternalAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        const int customerId = 1;
        AddTransactionRequest[] requests = [new AddTransactionRequest(1000) { Comment = "Transaction 1" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.AddTransactionsInternalAsync(TestAccessToken, customerId, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetCustomFieldsAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        EntityType entityType = EntityType.Leads;

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.GetCustomFieldsAsync(TestAccessToken, TestSubdomain, entityType).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetCustomFieldsInternalAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        EntityType entityType = EntityType.Leads;

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.GetCustomFieldsInternalAsync(TestAccessToken, entityType).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetLeadStatusesAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.GetLeadStatusesAsync(TestAccessToken, TestSubdomain).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetLeadStatusesInternalAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.GetLeadStatusesInternalAsync(TestAccessToken).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetNotesAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        EntityType entityType = EntityType.Leads;
        NoteType noteType = NoteType.Common;

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.GetNotesAsync(TestAccessToken, TestSubdomain, entityType, noteType).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetNotesInternalAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        EntityType entityType = EntityType.Leads;
        NoteType noteType = NoteType.Common;

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.GetNotesInternalAsync(TestAccessToken, entityType, noteType).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AddNotesAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        EntityType entityType = EntityType.Leads;
        AddNoteRequest[] requests = [new AddNoteRequest(1, NoteType.Common) { Parameters = new NoteDetails { Text = "Note 1" } }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.AddNotesAsync(TestAccessToken, TestSubdomain, entityType, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AddNotesInternalAsync_HttpError_ThrowsAmoCrmHttpException()
    {
        EntityType entityType = EntityType.Leads;
        AddNoteRequest[] requests = [new AddNoteRequest(1, NoteType.Common) { Parameters = new NoteDetails { Text = "Note 1" } }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.AddNotesInternalAsync(TestAccessToken, entityType, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetLinksAsync_HttpError_ThrowsAmoCrmAuthorizationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.GetLinksAsync(TestAccessToken, TestSubdomain, EntityType.Leads, new EntityLinksFilter([1])).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetLinksInternalAsync_HttpError_ThrowsAmoCrmAuthorizationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.GetLinksInternalAsync(TestAccessToken, EntityType.Leads, new EntityLinksFilter([1])).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task LinkAsync_HttpError_ThrowsAmoCrmAuthorizationException()
    {
        EntityType entityType = EntityType.Leads;
        LinkEntitiesRequest[] requests = [new LinkEntitiesRequest(entityId: 11) { LinkedEntityId = 1, LinkedEntityType = EntityType.Contacts }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.LinkAsync(TestAccessToken, TestSubdomain, entityType, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task LinkInternalAsync_HttpError_ThrowsAmoCrmAuthorizationException()
    {
        EntityType entityType = EntityType.Leads;
        LinkEntitiesRequest[] requests = [new LinkEntitiesRequest(entityId: 11) { LinkedEntityId = 1, LinkedEntityType = EntityType.Contacts }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmHttpException("HTTP request failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmHttpException> exception = await FluentActions
            .Invoking(async () => await Client.LinkInternalAsync(TestAccessToken, entityType, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmHttpException>();

        exception.WithMessage("*HTTP request failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AuthorizeAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        const string authCode = "auth-code";
        const string redirectDomain = "https://example.com  ";

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.AuthorizeAsync(TestSubdomain, authCode, redirectDomain).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AuthorizeInternalAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.AuthorizeInternalAsync().ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task RefreshTokenAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        const string refreshToken = "refresh-token";
        const string redirectDomain = "https://example.com  ";

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.RefreshTokenAsync(TestSubdomain, refreshToken, redirectDomain).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task RefreshTokenInternalAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        const string refreshToken = "internal-refresh-token";

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.RefreshTokenInternalAsync(refreshToken).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<AuthorizationTokens>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetLeadsAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.GetLeadsAsync(TestAccessToken, TestSubdomain).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetLeadsAsync_LinkedEntityTypes_ValidationError_ThrowsAmoCrmValidationException()
    {
        EntityType[] linkedEntityTypes = new[] { EntityType.Companies };
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.GetLeadsAsync(TestAccessToken, TestSubdomain, linkedEntityTypes).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetLeadsInternalAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.GetLeadsInternalAsync(TestAccessToken).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetLeadsInternalAsync_LinkedEntityTypes_ValidationError_ThrowsAmoCrmValidationException()
    {
        EntityType[] linkedEntityTypes = new[] { EntityType.Leads };
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.GetLeadsInternalAsync(TestAccessToken, linkedEntityTypes).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AddLeadsAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        AddLeadRequest[] requests = [new AddLeadRequest { Name = "Lead 1" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.AddLeadsAsync(TestAccessToken, TestSubdomain, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AddLeadsInternalAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        AddLeadRequest[] requests = [new AddLeadRequest { Name = "Lead 1" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.AddLeadsInternalAsync(TestAccessToken, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task UpdateLeadsAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        UpdateLeadRequest[] requests = [new UpdateLeadRequest(1) { Name = "Updated Lead" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.UpdateLeadsAsync(TestAccessToken, TestSubdomain, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task UpdateLeadsInternalAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        UpdateLeadRequest[] requests = [new UpdateLeadRequest(1) { Name = "Updated Lead" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.UpdateLeadsInternalAsync(TestAccessToken, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetCompaniesAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.GetCompaniesAsync(TestAccessToken, TestSubdomain).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetCompaniesAsync_LinkedEntityTypes_ValidationError_ThrowsAmoCrmValidationException()
    {
        EntityType[] linkedEntityTypes = new[] { EntityType.Customers };
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.GetCompaniesAsync(TestAccessToken, TestSubdomain, linkedEntityTypes).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetCompaniesInternalAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.GetCompaniesInternalAsync(TestAccessToken).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetCompaniesInternalAsync_LinkedEntityTypes_ValidationError_ThrowsAmoCrmValidationException()
    {
        EntityType[] linkedEntityTypes = new[] { EntityType.Customers };
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.GetCompaniesInternalAsync(TestAccessToken, linkedEntityTypes).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AddCompaniesAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        AddCompanyRequest[] requests = [new AddCompanyRequest { Name = "Company 1" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.AddCompaniesAsync(TestAccessToken, TestSubdomain, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AddCompaniesInternalAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        AddCompanyRequest[] requests = [new AddCompanyRequest { Name = "Company 1" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.AddCompaniesInternalAsync(TestAccessToken, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task UpdateCompaniesAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        UpdateCompanyRequest[] requests = [new UpdateCompanyRequest(1) { Name = "Updated Company" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.UpdateCompaniesAsync(TestAccessToken, TestSubdomain, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task UpdateCompaniesInternalAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        UpdateCompanyRequest[] requests = [new UpdateCompanyRequest(1) { Name = "Updated Company" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.UpdateCompaniesInternalAsync(TestAccessToken, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetTasksAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.GetTasksAsync(TestAccessToken, TestSubdomain).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetTasksInternalAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.GetTasksInternalAsync(TestAccessToken).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetTasksAsync_WithFilter_ValidationError_ThrowsAmoCrmValidationException()
    {
        var filter = new TasksFilter { EntityIds = [11], EntityType = EntityType.Leads, TaskIds = [1, 2] };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.GetTasksAsync(TestAccessToken, TestSubdomain, filter).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetTasksInternalAsync_WithFilter_ValidationError_ThrowsAmoCrmValidationException()
    {
        var filter = new TasksFilter { EntityIds = [11], EntityType = EntityType.Leads, TaskIds = [1, 2] };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.GetTasksInternalAsync(TestAccessToken, filter).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AddTasksAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        AddTaskRequest[] requests = [new AddTaskRequest("Task 1", 1234567890, EntityType.Leads) { EntityId = 1 }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.AddTasksAsync(TestAccessToken, TestSubdomain, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AddTasksInternalAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        AddTaskRequest[] requests = [new AddTaskRequest("Task 1", 1234567890, EntityType.Leads) { EntityId = 1 }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.AddTasksInternalAsync(TestAccessToken, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task UpdateTasksAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        UpdateTaskRequest[] requests = [new UpdateTaskRequest(1, "Updated Task", 123123123123)];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.UpdateTasksAsync(TestAccessToken, TestSubdomain, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task UpdateTasksInternalAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        UpdateTaskRequest[] requests = [new UpdateTaskRequest(1, "Updated Task", 123123123123)];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.UpdateTasksInternalAsync(TestAccessToken, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetCustomersAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.GetCustomersAsync(TestAccessToken, TestSubdomain).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetCustomersAsync_LinkedEntityTypes_ValidationError_ThrowsAmoCrmValidationException()
    {
        EntityType[] linkedEntityTypes = new[] { EntityType.Customers };
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.GetCustomersAsync(TestAccessToken, TestSubdomain, linkedEntityTypes).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetCustomersInternalAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.GetCustomersInternalAsync(TestAccessToken).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetCustomersInternalAsync_LinkedEntityTypes_ValidationError_ThrowsAmoCrmValidationException()
    {
        EntityType[] linkedEntityTypes = new[] { EntityType.Contacts };
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.GetCustomersInternalAsync(TestAccessToken, linkedEntityTypes).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AddCustomersAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        AddCustomerRequest[] requests = [new AddCustomerRequest("Customer 1") { NextPrice = 500 }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.AddCustomersAsync(TestAccessToken, TestSubdomain, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AddCustomersInternalAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        AddCustomerRequest[] requests = [new AddCustomerRequest("Customer 1") { NextPrice = 500 }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.AddCustomersInternalAsync(TestAccessToken, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task UpdateCustomersAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        UpdateCustomerRequest[] requests = [new UpdateCustomerRequest(1, "Updated Customer")];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.UpdateCustomersAsync(TestAccessToken, TestSubdomain, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task UpdateCustomersInternalAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        UpdateCustomerRequest[] requests = [new UpdateCustomerRequest(1, "Updated Customer")];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.UpdateCustomersInternalAsync(TestAccessToken, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetAccountAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<AccountModel>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.GetAccountAsync(TestAccessToken, TestSubdomain).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<AccountModel>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetAccountInternalAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<AccountModel>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.GetAccountInternalAsync(TestAccessToken).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<AccountModel>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetUsersAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.GetUsersAsync(TestAccessToken, TestSubdomain).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetUsersInternalAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.GetUsersInternalAsync(TestAccessToken).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetWidgetAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        const string widgetCode = "test-widget";

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<Widget>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.GetWidgetAsync(TestAccessToken, TestSubdomain, widgetCode).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<Widget>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetContactsAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.GetContactsAsync(TestAccessToken, TestSubdomain).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetContactsAsync_LinkedEntityTypes_ValidationError_ThrowsAmoCrmValidationException()
    {
        EntityType[] linkedEntityTypes = new[] { EntityType.Leads };
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.GetContactsAsync(TestAccessToken, TestSubdomain, linkedEntityTypes).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetContactsInternalAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.GetContactsInternalAsync(TestAccessToken).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetContactsInternalAsync_LinkedEntityTypes_ValidationError_ThrowsAmoCrmValidationException()
    {
        EntityType[] linkedEntityTypes = new[] { EntityType.Leads };
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.GetContactsInternalAsync(TestAccessToken, linkedEntityTypes).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetContactByIdAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        const int contactId = 1;

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<Contact>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.GetContactByIdAsync(TestAccessToken, TestSubdomain, contactId).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<Contact>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetContactByIdAsync_LinkedEntityTypes_ValidationError_ThrowsAmoCrmValidationException()
    {
        const int contactId = 1;
        EntityType[] linkedEntityTypes = new[] { EntityType.Customers };
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<Contact>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.GetContactByIdAsync(TestAccessToken, TestSubdomain, contactId, linkedEntityTypes).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<Contact>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetContactByIdInternalAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        const int contactId = 1;

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<Contact>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.GetContactByIdInternalAsync(TestAccessToken, contactId).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<Contact>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetContactByIdInternalAsync_LinkedEntityTypes_ValidationError_ThrowsAmoCrmValidationException()
    {
        const int contactId = 1;
        EntityType[] linkedEntityTypes = new[] { EntityType.Customers };
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<Contact>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.GetContactByIdInternalAsync(TestAccessToken, contactId, linkedEntityTypes).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<Contact>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AddContactsAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        AddContactRequest[] requests = [new AddContactRequest { Name = "Contact 1", FirstName = "John" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.AddContactsAsync(TestAccessToken, TestSubdomain, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AddContactsInternalAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        AddContactRequest[] requests = [new AddContactRequest { Name = "Contact 1", FirstName = "John" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.AddContactsInternalAsync(TestAccessToken, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task UpdateContactsAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        UpdateContactRequest[] requests = [new UpdateContactRequest(1) { Name = "Updated Contact" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.UpdateContactsAsync(TestAccessToken, TestSubdomain, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task UpdateContactsInternalAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        UpdateContactRequest[] requests = [new UpdateContactRequest(1) { Name = "Updated Contact" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.UpdateContactsInternalAsync(TestAccessToken, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetTransactionsAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        const int customerId = 1;

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.GetTransactionsAsync(TestAccessToken, TestSubdomain, customerId).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetTransactionsInternalAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        const int customerId = 1;

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.GetTransactionsInternalAsync(TestAccessToken, customerId).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AddTransactionsAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        const int customerId = 1;
        AddTransactionRequest[] requests = [new AddTransactionRequest(1000) { Comment = "Transaction 1" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.AddTransactionsAsync(TestAccessToken, TestSubdomain, customerId, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AddTransactionsInternalAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        const int customerId = 1;
        AddTransactionRequest[] requests = [new AddTransactionRequest(1000) { Comment = "Transaction 1" }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.AddTransactionsInternalAsync(TestAccessToken, customerId, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetCustomFieldsAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        EntityType entityType = EntityType.Leads;

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.GetCustomFieldsAsync(TestAccessToken, TestSubdomain, entityType).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetCustomFieldsInternalAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        EntityType entityType = EntityType.Leads;

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.GetCustomFieldsInternalAsync(TestAccessToken, entityType).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetLeadStatusesAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.GetLeadStatusesAsync(TestAccessToken, TestSubdomain).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetLeadStatusesInternalAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.GetLeadStatusesInternalAsync(TestAccessToken).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetNotesAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        EntityType entityType = EntityType.Leads;
        NoteType noteType = NoteType.Common;

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.GetNotesAsync(TestAccessToken, TestSubdomain, entityType, noteType).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetNotesInternalAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        EntityType entityType = EntityType.Leads;
        NoteType noteType = NoteType.Common;

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.GetNotesInternalAsync(TestAccessToken, entityType, noteType).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AddNotesAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        EntityType entityType = EntityType.Leads;
        AddNoteRequest[] requests = [new AddNoteRequest(1, NoteType.Common) { Parameters = new NoteDetails { Text = "Note 1" } }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.AddNotesAsync(TestAccessToken, TestSubdomain, entityType, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AddNotesInternalAsync_ValidationError_ThrowsAmoCrmValidationException()
    {
        EntityType entityType = EntityType.Leads;
        AddNoteRequest[] requests = [new AddNoteRequest(1, NoteType.Common) { Parameters = new NoteDetails { Text = "Note 1" } }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.AddNotesInternalAsync(TestAccessToken, entityType, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetLinksAsync_ValidationError_ThrowsAmoCrmAuthorizationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.GetLinksAsync(TestAccessToken, TestSubdomain, EntityType.Leads, new EntityLinksFilter([1])).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetLinksInternalAsync_ValidationError_ThrowsAmoCrmAuthorizationException()
    {
        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.GetLinksInternalAsync(TestAccessToken, EntityType.Leads, new EntityLinksFilter([1])).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task LinkAsync_ValidationError_ThrowsAmoCrmAuthorizationException()
    {
        EntityType entityType = EntityType.Leads;
        LinkEntitiesRequest[] requests = [new LinkEntitiesRequest(entityId: 11) { LinkedEntityId = 1, LinkedEntityType = EntityType.Contacts }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.LinkAsync(TestAccessToken, TestSubdomain, entityType, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task LinkInternalAsync_ValidationError_ThrowsAmoCrmAuthorizationException()
    {
        EntityType entityType = EntityType.Leads;
        LinkEntitiesRequest[] requests = [new LinkEntitiesRequest(entityId: 11) { LinkedEntityId = 1, LinkedEntityType = EntityType.Contacts }];

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .Throws(new AmoCrmValidationException("Validation failed"));

        FluentAssertions.Specialized.ExceptionAssertions<AmoCrmValidationException> exception = await FluentActions
            .Invoking(async () => await Client.LinkInternalAsync(TestAccessToken, entityType, requests).ConfigureAwait(false))
            .Should().ThrowAsync<AmoCrmValidationException>();

        exception.WithMessage("*Validation failed*");

        ResponseHandlerMock.Verify(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task AddLeadsAsync_EmptyRequests_ReturnsEmptyList()
    {
        AddLeadRequest[] requests = [];
        var response = new EntitiesResponse { Embedded = new EmbeddedEntitiesResponse { Leads = [] } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Lead> result = await Client.AddLeadsAsync(TestAccessToken, TestSubdomain, requests);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task AddLeadsInternalAsync_EmptyRequests_ReturnsEmptyList()
    {
        AddLeadRequest[] requests = [];
        var response = new EntitiesResponse { Embedded = new EmbeddedEntitiesResponse { Leads = [] } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Lead> result = await Client.AddLeadsInternalAsync(TestAccessToken, requests);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task AddLeadsAsync_NullRequests_ThrowsArgumentNullException()
    {
        IReadOnlyCollection<AddLeadRequest> requests = null!;

        await FluentActions
            .Invoking(async () => await Client.AddLeadsAsync(TestAccessToken, TestSubdomain, requests).ConfigureAwait(false))
            .Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task AddLeadsInternalAsync_NullRequests_ThrowsArgumentNullException()
    {
        IReadOnlyCollection<AddLeadRequest> requests = null!;

        await FluentActions
            .Invoking(async () => await Client.AddLeadsInternalAsync(TestAccessToken, requests).ConfigureAwait(false))
            .Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task UpdateLeadsAsync_EmptyRequests_ReturnsEmptyList()
    {
        UpdateLeadRequest[] requests = [];
        var response = new EntitiesResponse { Embedded = new EmbeddedEntitiesResponse { Leads = [] } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Lead> result = await Client.UpdateLeadsAsync(TestAccessToken, TestSubdomain, requests);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateLeadsInternalAsync_EmptyRequests_ReturnsEmptyList()
    {
        UpdateLeadRequest[] requests = [];
        var response = new EntitiesResponse { Embedded = new EmbeddedEntitiesResponse { Leads = [] } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Lead> result = await Client.UpdateLeadsInternalAsync(TestAccessToken, requests);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateLeadsAsync_NullRequests_ThrowsArgumentNullException()
    {
        IReadOnlyCollection<UpdateLeadRequest> requests = null!;

        await FluentActions
            .Invoking(async () => await Client.UpdateLeadsAsync(TestAccessToken, TestSubdomain, requests).ConfigureAwait(false))
            .Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task UpdateLeadsInternalAsync_NullRequests_ThrowsArgumentNullException()
    {
        IReadOnlyCollection<UpdateLeadRequest> requests = null!;

        await FluentActions
            .Invoking(async () => await Client.UpdateLeadsInternalAsync(TestAccessToken, requests).ConfigureAwait(false))
            .Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task AddCompaniesAsync_EmptyRequests_ReturnsEmptyList()
    {
        AddCompanyRequest[] requests = [];
        var response = new EntitiesResponse { Embedded = new EmbeddedEntitiesResponse { Companies = [] } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Company> result = await Client.AddCompaniesAsync(TestAccessToken, TestSubdomain, requests);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task AddCompaniesInternalAsync_EmptyRequests_ReturnsEmptyList()
    {
        AddCompanyRequest[] requests = [];
        var response = new EntitiesResponse { Embedded = new EmbeddedEntitiesResponse { Companies = [] } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Company> result = await Client.AddCompaniesInternalAsync(TestAccessToken, requests);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task AddCompaniesAsync_NullRequests_ThrowsArgumentNullException()
    {
        IReadOnlyCollection<AddCompanyRequest> requests = null!;

        await FluentActions
            .Invoking(async () => await Client.AddCompaniesAsync(TestAccessToken, TestSubdomain, requests).ConfigureAwait(false))
            .Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task AddCompaniesInternalAsync_NullRequests_ThrowsArgumentNullException()
    {
        IReadOnlyCollection<AddCompanyRequest> requests = null!;

        await FluentActions
            .Invoking(async () => await Client.AddCompaniesInternalAsync(TestAccessToken, requests).ConfigureAwait(false))
            .Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task UpdateCompaniesAsync_EmptyRequests_ReturnsEmptyList()
    {
        UpdateCompanyRequest[] requests = [];
        var response = new EntitiesResponse { Embedded = new EmbeddedEntitiesResponse { Companies = [] } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Company> result = await Client.UpdateCompaniesAsync(TestAccessToken, TestSubdomain, requests);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateCompaniesInternalAsync_EmptyRequests_ReturnsEmptyList()
    {
        UpdateCompanyRequest[] requests = [];
        var response = new EntitiesResponse { Embedded = new EmbeddedEntitiesResponse { Companies = [] } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Company> result = await Client.UpdateCompaniesInternalAsync(TestAccessToken, requests);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateCompaniesAsync_NullRequests_ThrowsArgumentNullException()
    {
        IReadOnlyCollection<UpdateCompanyRequest> requests = null!;

        await FluentActions
            .Invoking(async () => await Client.UpdateCompaniesAsync(TestAccessToken, TestSubdomain, requests).ConfigureAwait(false))
            .Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task UpdateCompaniesInternalAsync_NullRequests_ThrowsArgumentNullException()
    {
        IReadOnlyCollection<UpdateCompanyRequest> requests = null!;

        await FluentActions
            .Invoking(async () => await Client.UpdateCompaniesInternalAsync(TestAccessToken, requests).ConfigureAwait(false))
            .Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task AddTasksAsync_EmptyRequests_ReturnsEmptyList()
    {
        AddTaskRequest[] requests = [];
        var response = new EntitiesResponse { Embedded = new EmbeddedEntitiesResponse { Tasks = [] } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<AmoCrmTask> result = await Client.AddTasksAsync(TestAccessToken, TestSubdomain, requests);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task AddTasksInternalAsync_EmptyRequests_ReturnsEmptyList()
    {
        AddTaskRequest[] requests = [];
        var response = new EntitiesResponse { Embedded = new EmbeddedEntitiesResponse { Tasks = [] } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<AmoCrmTask> result = await Client.AddTasksInternalAsync(TestAccessToken, requests);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task AddTasksAsync_NullRequests_ThrowsArgumentNullException()
    {
        IReadOnlyCollection<AddTaskRequest> requests = null!;

        await FluentActions
            .Invoking(async () => await Client.AddTasksAsync(TestAccessToken, TestSubdomain, requests).ConfigureAwait(false))
            .Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task AddTasksInternalAsync_NullRequests_ThrowsArgumentNullException()
    {
        IReadOnlyCollection<AddTaskRequest> requests = null!;

        await FluentActions
            .Invoking(async () => await Client.AddTasksInternalAsync(TestAccessToken, requests).ConfigureAwait(false))
            .Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task UpdateTasksAsync_EmptyRequests_ReturnsEmptyList()
    {
        UpdateTaskRequest[] requests = [];
        var response = new EntitiesResponse { Embedded = new EmbeddedEntitiesResponse { Tasks = [] } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<AmoCrmTask> result = await Client.UpdateTasksAsync(TestAccessToken, TestSubdomain, requests);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateTasksInternalAsync_EmptyRequests_ReturnsEmptyList()
    {
        UpdateTaskRequest[] requests = [];
        var response = new EntitiesResponse { Embedded = new EmbeddedEntitiesResponse { Tasks = [] } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<AmoCrmTask> result = await Client.UpdateTasksInternalAsync(TestAccessToken, requests);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateTasksAsync_NullRequests_ThrowsArgumentNullException()
    {
        IReadOnlyCollection<UpdateTaskRequest> requests = null!;

        await FluentActions
            .Invoking(async () => await Client.UpdateTasksAsync(TestAccessToken, TestSubdomain, requests).ConfigureAwait(false))
            .Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task UpdateTasksInternalAsync_NullRequests_ThrowsArgumentNullException()
    {
        IReadOnlyCollection<UpdateTaskRequest> requests = null!;

        await FluentActions
            .Invoking(async () => await Client.UpdateTasksInternalAsync(TestAccessToken, requests).ConfigureAwait(false))
            .Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task AddCustomersAsync_EmptyRequests_ReturnsEmptyList()
    {
        AddCustomerRequest[] requests = [];
        var response = new EntitiesResponse { Embedded = new EmbeddedEntitiesResponse { Customers = [] } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Customer> result = await Client.AddCustomersAsync(TestAccessToken, TestSubdomain, requests);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task AddCustomersInternalAsync_EmptyRequests_ReturnsEmptyList()
    {
        AddCustomerRequest[] requests = [];
        var response = new EntitiesResponse { Embedded = new EmbeddedEntitiesResponse { Customers = [] } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Customer> result = await Client.AddCustomersInternalAsync(TestAccessToken, requests);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task AddCustomersAsync_NullRequests_ThrowsArgumentNullException()
    {
        IReadOnlyCollection<AddCustomerRequest> requests = null!;

        await FluentActions
            .Invoking(async () => await Client.AddCustomersAsync(TestAccessToken, TestSubdomain, requests).ConfigureAwait(false))
            .Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task AddCustomersInternalAsync_NullRequests_ThrowsArgumentNullException()
    {
        IReadOnlyCollection<AddCustomerRequest> requests = null!;

        await FluentActions
            .Invoking(async () => await Client.AddCustomersInternalAsync(TestAccessToken, requests).ConfigureAwait(false))
            .Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task UpdateCustomersAsync_EmptyRequests_ReturnsEmptyList()
    {
        UpdateCustomerRequest[] requests = [];
        var response = new EntitiesResponse { Embedded = new EmbeddedEntitiesResponse { Customers = [] } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Customer> result = await Client.UpdateCustomersAsync(TestAccessToken, TestSubdomain, requests);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateCustomersInternalAsync_EmptyRequests_ReturnsEmptyList()
    {
        UpdateCustomerRequest[] requests = [];
        var response = new EntitiesResponse { Embedded = new EmbeddedEntitiesResponse { Customers = [] } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Customer> result = await Client.UpdateCustomersInternalAsync(TestAccessToken, requests);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateCustomersAsync_NullRequests_ThrowsArgumentNullException()
    {
        IReadOnlyCollection<UpdateCustomerRequest> requests = null!;

        await FluentActions
            .Invoking(async () => await Client.UpdateCustomersAsync(TestAccessToken, TestSubdomain, requests).ConfigureAwait(false))
            .Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task UpdateCustomersInternalAsync_NullRequests_ThrowsArgumentNullException()
    {
        IReadOnlyCollection<UpdateCustomerRequest> requests = null!;

        await FluentActions
            .Invoking(async () => await Client.UpdateCustomersInternalAsync(TestAccessToken, requests).ConfigureAwait(false))
            .Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task AddContactsAsync_EmptyRequests_ReturnsEmptyList()
    {
        AddContactRequest[] requests = [];
        var response = new EntitiesResponse { Embedded = new EmbeddedEntitiesResponse { Contacts = [] } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Contact> result = await Client.AddContactsAsync(TestAccessToken, TestSubdomain, requests);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task AddContactsInternalAsync_EmptyRequests_ReturnsEmptyList()
    {
        AddContactRequest[] requests = [];
        var response = new EntitiesResponse { Embedded = new EmbeddedEntitiesResponse { Contacts = [] } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Contact> result = await Client.AddContactsInternalAsync(TestAccessToken, requests);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task AddContactsAsync_NullRequests_ThrowsArgumentNullException()
    {
        IReadOnlyCollection<AddContactRequest> requests = null!;

        await FluentActions
            .Invoking(async () => await Client.AddContactsAsync(TestAccessToken, TestSubdomain, requests).ConfigureAwait(false))
            .Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task AddContactsInternalAsync_NullRequests_ThrowsArgumentNullException()
    {
        IReadOnlyCollection<AddContactRequest> requests = null!;

        await FluentActions
            .Invoking(async () => await Client.AddContactsInternalAsync(TestAccessToken, requests).ConfigureAwait(false))
            .Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task UpdateContactsAsync_EmptyRequests_ReturnsEmptyList()
    {
        UpdateContactRequest[] requests = [];
        var response = new EntitiesResponse { Embedded = new EmbeddedEntitiesResponse { Contacts = [] } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Contact> result = await Client.UpdateContactsAsync(TestAccessToken, TestSubdomain, requests);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateContactsInternalAsync_EmptyRequests_ReturnsEmptyList()
    {
        UpdateContactRequest[] requests = [];
        var response = new EntitiesResponse { Embedded = new EmbeddedEntitiesResponse { Contacts = [] } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Contact> result = await Client.UpdateContactsInternalAsync(TestAccessToken, requests);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateContactsAsync_NullRequests_ThrowsArgumentNullException()
    {
        IReadOnlyCollection<UpdateContactRequest> requests = null!;

        await FluentActions
            .Invoking(async () => await Client.UpdateContactsAsync(TestAccessToken, TestSubdomain, requests).ConfigureAwait(false))
            .Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task UpdateContactsInternalAsync_NullRequests_ThrowsArgumentNullException()
    {
        IReadOnlyCollection<UpdateContactRequest> requests = null!;

        await FluentActions
            .Invoking(async () => await Client.UpdateContactsInternalAsync(TestAccessToken, requests).ConfigureAwait(false))
            .Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task AddTransactionsAsync_EmptyRequests_ReturnsEmptyList()
    {
        const int customerId = 1;
        AddTransactionRequest[] requests = [];
        var response = new EntitiesResponse { Embedded = new EmbeddedEntitiesResponse { Transactions = [] } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Transaction> result = await Client.AddTransactionsAsync(TestAccessToken, TestSubdomain, customerId, requests);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task AddTransactionsInternalAsync_EmptyRequests_ReturnsEmptyList()
    {
        const int customerId = 1;
        AddTransactionRequest[] requests = [];
        var response = new EntitiesResponse { Embedded = new EmbeddedEntitiesResponse { Transactions = [] } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Transaction> result = await Client.AddTransactionsInternalAsync(TestAccessToken, customerId, requests);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task AddTransactionsAsync_NullRequests_ThrowsArgumentNullException()
    {
        const int customerId = 1;
        IReadOnlyCollection<AddTransactionRequest> requests = null!;

        await FluentActions
            .Invoking(async () => await Client.AddTransactionsAsync(TestAccessToken, TestSubdomain, customerId, requests).ConfigureAwait(false))
            .Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task AddTransactionsInternalAsync_NullRequests_ThrowsArgumentNullException()
    {
        const int customerId = 1;
        IReadOnlyCollection<AddTransactionRequest> requests = null!;

        await FluentActions
            .Invoking(async () => await Client.AddTransactionsInternalAsync(TestAccessToken, customerId, requests).ConfigureAwait(false))
            .Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task AddNotesAsync_EmptyRequests_ReturnsEmptyList()
    {
        EntityType entityType = EntityType.Leads;
        AddNoteRequest[] requests = [];
        var response = new EntitiesResponse { Embedded = new EmbeddedEntitiesResponse { Notes = [] } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Note> result = await Client.AddNotesAsync(TestAccessToken, TestSubdomain, entityType, requests);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task AddNotesInternalAsync_EmptyRequests_ReturnsEmptyList()
    {
        EntityType entityType = EntityType.Leads;
        AddNoteRequest[] requests = [];
        var response = new EntitiesResponse { Embedded = new EmbeddedEntitiesResponse { Notes = [] } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<Note> result = await Client.AddNotesInternalAsync(TestAccessToken, entityType, requests);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task AddNotesAsync_NullRequests_ThrowsArgumentNullException()
    {
        EntityType entityType = EntityType.Leads;
        IReadOnlyCollection<AddNoteRequest> requests = null!;

        await FluentActions
            .Invoking(async () => await Client.AddNotesAsync(TestAccessToken, TestSubdomain, entityType, requests).ConfigureAwait(false))
            .Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task AddNotesInternalAsync_NullRequests_ThrowsArgumentNullException()
    {
        EntityType entityType = EntityType.Leads;
        IReadOnlyCollection<AddNoteRequest> requests = null!;

        await FluentActions
            .Invoking(async () => await Client.AddNotesInternalAsync(TestAccessToken, entityType, requests).ConfigureAwait(false))
            .Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task GetLinksAsync_NullFilter_ThrowsArgumentNullException()
    {
        EntityType entityType = EntityType.Leads;
        EntityLinksFilter filter = null!;

        await FluentActions
            .Invoking(async () => await Client.GetLinksAsync(TestAccessToken, TestSubdomain, entityType, filter).ConfigureAwait(false))
            .Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task GetLinksInternalAsync_NullFilter_ThrowsArgumentNullException()
    {
        EntityType entityType = EntityType.Leads;
        EntityLinksFilter filter = null!;

        await FluentActions
            .Invoking(async () => await Client.GetLinksInternalAsync(TestAccessToken, entityType, filter).ConfigureAwait(false))
            .Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task LinkAsync_EmptyRequests_ReturnsEmptyList()
    {
        EntityType entityType = EntityType.Leads;
        LinkEntitiesRequest[] requests = [];
        var response = new EntitiesResponse { Embedded = new EmbeddedEntitiesResponse { Notes = [] } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<EntityLink> result = await Client.LinkAsync(TestAccessToken, TestSubdomain, entityType, requests);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task LinkInternalAsync_EmptyRequests_ReturnsEmptyList()
    {
        EntityType entityType = EntityType.Leads;
        LinkEntitiesRequest[] requests = [];
        var response = new EntitiesResponse { Embedded = new EmbeddedEntitiesResponse { Notes = [] } };

        ResponseHandlerMock
            .Setup(x => x.HandleAsync<EntitiesResponse>(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        IReadOnlyCollection<EntityLink> result = await Client.LinkInternalAsync(TestAccessToken, entityType, requests);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task LinkAsync_NullRequests_ThrowsArgumentNullException()
    {
        EntityType entityType = EntityType.Leads;
        IReadOnlyCollection<LinkEntitiesRequest> requests = null!;

        await FluentActions
            .Invoking(async () => await Client.LinkAsync(TestAccessToken, TestSubdomain, entityType, requests).ConfigureAwait(false))
            .Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task LinkInternalAsync_NullRequests_ThrowsArgumentNullException()
    {
        EntityType entityType = EntityType.Leads;
        IReadOnlyCollection<LinkEntitiesRequest> requests = null!;

        await FluentActions
            .Invoking(async () => await Client.LinkInternalAsync(TestAccessToken, entityType, requests).ConfigureAwait(false))
            .Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task UnlinkAsync_EmptyRequests_DoesNotThrow()
    {
        EntityType entityType = EntityType.Leads;
        UnlinkEntitiesRequest[] requests = [];

        await FluentActions
            .Invoking(async () => await Client.UnlinkAsync(TestAccessToken, TestSubdomain, entityType, requests).ConfigureAwait(false))
            .Should().NotThrowAsync();
    }

    [Fact]
    public async Task UnlinkInternalAsync_EmptyRequests_DoesNotThrow()
    {
        EntityType entityType = EntityType.Leads;
        UnlinkEntitiesRequest[] requests = [];

        await FluentActions
            .Invoking(async () => await Client.UnlinkInternalAsync(TestAccessToken, entityType, requests).ConfigureAwait(false))
            .Should().NotThrowAsync();
    }

    [Fact]
    public async Task UnlinkAsync_NullRequests_ThrowsArgumentNullException()
    {
        EntityType entityType = EntityType.Leads;
        IReadOnlyCollection<UnlinkEntitiesRequest> requests = null!;

        await FluentActions
            .Invoking(async () => await Client.UnlinkAsync(TestAccessToken, TestSubdomain, entityType, requests).ConfigureAwait(false))
            .Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task UnlinkInternalAsync_NullRequests_ThrowsArgumentNullException()
    {
        EntityType entityType = EntityType.Leads;
        IReadOnlyCollection<UnlinkEntitiesRequest> requests = null!;

        await FluentActions
            .Invoking(async () => await Client.UnlinkInternalAsync(TestAccessToken, entityType, requests).ConfigureAwait(false))
            .Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task GetTasksAsync_NullFilter_ThrowsArgumentNullException()
    {
        TasksFilter filter = null!;

        await FluentActions
            .Invoking(async () => await Client.GetTasksAsync(TestAccessToken, TestSubdomain, filter).ConfigureAwait(false))
            .Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task GetTasksInternalAsync_NullFilter_ThrowsArgumentNullException()
    {
        TasksFilter filter = null!;

        await FluentActions
            .Invoking(async () => await Client.GetTasksInternalAsync(TestAccessToken, filter).ConfigureAwait(false))
            .Should().ThrowAsync<ArgumentNullException>();
    }
}
