
namespace Planfact.AmoCrm.Client.Tests;

public class AmoCrmUriBuilderFactoryTests
{
    protected const string TestClientId = "test-client-id";
    protected const string TestClientSecret = "test-client-secret";
    protected const string TestAccessToken = "access-token";
    protected const string TestSubdomain = "example.amocrm.ru";
    protected const string TestRedirectUri = "https://example.com";

    private readonly AmoCrmUriBuilderFactory _factory;
    private readonly Mock<IOptions<AmoCrmClientOptions>> _optionsMock;

    public AmoCrmUriBuilderFactoryTests()
    {
        _optionsMock = new Mock<IOptions<AmoCrmClientOptions>>();
        var options = new AmoCrmClientOptions()
        {
            ClientId = TestClientId,
            ClientSecret = TestClientSecret,
            ServerIntegrationSubdomain = TestSubdomain,
            ServerIntegrationRedirectUri = TestRedirectUri
        };
        _optionsMock.Setup(o => o.Value).Returns(options);
        _factory = new AmoCrmUriBuilderFactory(_optionsMock.Object);
    }

    [Fact]
    public void CreateForAuthorization_ReturnsCorrectUriBuilder()
    {
        // Act
        UriBuilder uriBuilder = _factory.CreateForAuthorization(TestSubdomain);

        // Assert
        uriBuilder.Scheme.Should().Be("https");
        uriBuilder.Host.Should().Be(TestSubdomain);
        uriBuilder.Path.Should().Be("oauth2/access_token");
    }

    [Fact]
    public void CreateForCompanies_ReturnsCorrectUriBuilder()
    {
        // Act
        UriBuilder uriBuilder = _factory.CreateForCompanies(TestSubdomain);

        // Assert
        uriBuilder.Scheme.Should().Be("https");
        uriBuilder.Host.Should().Be(TestSubdomain);
        uriBuilder.Path.Should().Be("api/v4/companies");
    }

    [Fact]
    public void CreateForLeads_ReturnsCorrectUriBuilder()
    {
        // Act
        UriBuilder uriBuilder = _factory.CreateForLeads(TestSubdomain);

        // Assert
        uriBuilder.Scheme.Should().Be("https");
        uriBuilder.Host.Should().Be(TestSubdomain);
        uriBuilder.Path.Should().Be("api/v4/leads");
    }

    [Fact]
    public void CreateForTasks_ReturnsCorrectUriBuilder()
    {
        // Act
        UriBuilder uriBuilder = _factory.CreateForTasks(TestSubdomain);

        // Assert
        uriBuilder.Scheme.Should().Be("https");
        uriBuilder.Host.Should().Be(TestSubdomain);
        uriBuilder.Path.Should().Be("api/v4/tasks");
    }

    [Fact]
    public void CreateForCustomers_ReturnsCorrectUriBuilder()
    {
        // Act
        UriBuilder uriBuilder = _factory.CreateForCustomers(TestSubdomain);

        // Assert
        uriBuilder.Scheme.Should().Be("https");
        uriBuilder.Host.Should().Be(TestSubdomain);
        uriBuilder.Path.Should().Be("api/v4/customers");
    }

    [Fact]
    public void CreateForAccount_ReturnsCorrectUriBuilder()
    {
        // Act
        UriBuilder uriBuilder = _factory.CreateForAccount(TestSubdomain);

        // Assert
        uriBuilder.Scheme.Should().Be("https");
        uriBuilder.Host.Should().Be(TestSubdomain);
        uriBuilder.Path.Should().Be("api/v4/account");
    }

    [Fact]
    public void CreateForUsers_ReturnsCorrectUriBuilder()
    {
        // Act
        UriBuilder uriBuilder = _factory.CreateForUsers(TestSubdomain);

        // Assert
        uriBuilder.Scheme.Should().Be("https");
        uriBuilder.Host.Should().Be(TestSubdomain);
        uriBuilder.Path.Should().Be("api/v4/users");
    }

    [Fact]
    public void CreateForWidget_ReturnsCorrectUriBuilder()
    {
        // Arrange
        var widgetCode = "W001";

        // Act
        UriBuilder uriBuilder = _factory.CreateForWidget(TestSubdomain, widgetCode);

        // Assert
        uriBuilder.Scheme.Should().Be("https");
        uriBuilder.Host.Should().Be(TestSubdomain);
        uriBuilder.Path.Should().Be($"api/v4/widgets/{widgetCode}");
    }

    [Fact]
    public void CreateForContacts_WithNoContactId_ReturnsCorrectUriBuilder()
    {
        // Act
        UriBuilder uriBuilder = _factory.CreateForContacts(TestSubdomain);

        // Assert
        uriBuilder.Scheme.Should().Be("https");
        uriBuilder.Host.Should().Be(TestSubdomain);
        uriBuilder.Path.Should().Be("api/v4/contacts");
    }

    [Fact]
    public void CreateForContacts_WithContactId_ReturnsCorrectUriBuilder()
    {
        // Arrange
        var contactId = 123;

        // Act
        UriBuilder uriBuilder = _factory.CreateForContacts(TestSubdomain, contactId);

        // Assert
        uriBuilder.Scheme.Should().Be("https");
        uriBuilder.Host.Should().Be(TestSubdomain);
        uriBuilder.Path.Should().Be($"api/v4/contacts/{contactId}");
    }

    [Fact]
    public void CreateForTransactions_WithNoCustomerId_ReturnsCorrectUriBuilder()
    {
        // Act
        UriBuilder uriBuilder = _factory.CreateForTransactions(TestSubdomain);

        // Assert
        uriBuilder.Scheme.Should().Be("https");
        uriBuilder.Host.Should().Be(TestSubdomain);
        uriBuilder.Path.Should().Be("api/v4/customers/transactions");
    }

    [Fact]
    public void CreateForTransactions_WithCustomerId_ReturnsCorrectUriBuilder()
    {
        // Arrange
        var customerId = 456;

        // Act
        UriBuilder uriBuilder = _factory.CreateForTransactions(TestSubdomain, customerId);

        // Assert
        uriBuilder.Scheme.Should().Be("https");
        uriBuilder.Host.Should().Be(TestSubdomain);
        uriBuilder.Path.Should().Be($"api/v4/customers/{customerId}/transactions");
    }

    [Fact]
    public void CreateForDeleteTransaction_ReturnsCorrectUriBuilder()
    {
        // Arrange
        var transactionId = 123456;

        // Act
        UriBuilder uriBuilder = _factory.CreateForDeleteTransaction(TestSubdomain, transactionId);

        // Assert
        uriBuilder.Scheme.Should().Be("https");
        uriBuilder.Host.Should().Be(TestSubdomain);
        uriBuilder.Path.Should().Be($"api/v4/customers/transactions/{transactionId}");
    }

    [Fact]
    public void CreateForCustomFields_ReturnsCorrectUriBuilder()
    {
        // Arrange
        var entityType = "leads";

        // Act
        UriBuilder uriBuilder = _factory.CreateForCustomFields(TestSubdomain, entityType);

        // Assert
        uriBuilder.Scheme.Should().Be("https");
        uriBuilder.Host.Should().Be(TestSubdomain);
        uriBuilder.Path.Should().Be("api/v4/leads/custom_fields");
    }

    [Fact]
    public void CreateForPipelines_ReturnsCorrectUriBuilder()
    {
        // Act
        UriBuilder uriBuilder = _factory.CreateForPipelines(TestSubdomain);

        // Assert
        uriBuilder.Scheme.Should().Be("https");
        uriBuilder.Host.Should().Be(TestSubdomain);
        uriBuilder.Path.Should().Be("api/v4/leads/pipelines");
    }

    [Fact]
    public void CreateForNotes_ReturnsCorrectUriBuilder()
    {
        // Arrange
        var entityType = "leads";

        // Act
        UriBuilder uriBuilder = _factory.CreateForNotes(TestSubdomain, entityType);

        // Assert
        uriBuilder.Scheme.Should().Be("https");
        uriBuilder.Host.Should().Be(TestSubdomain);
        uriBuilder.Path.Should().Be("api/v4/leads/notes");
    }

    [Fact]
    public void CreateForAddLinks_ReturnsCorrectUriBuilder()
    {
        // Arrange
        var entityType = "leads";

        // Act
        UriBuilder uriBuilder = _factory.CreateForAddLinks(TestSubdomain, entityType);

        // Assert
        uriBuilder.Scheme.Should().Be("https");
        uriBuilder.Host.Should().Be(TestSubdomain);
        uriBuilder.Path.Should().Be("api/v4/leads/link");
    }

    [Fact]
    public void CreateForDeleteLinks_ReturnsCorrectUriBuilder()
    {
        // Arrange
        var entityType = "contacts";

        // Act
        UriBuilder uriBuilder = _factory.CreateForDeleteLinks(TestSubdomain, entityType);

        // Assert
        uriBuilder.Scheme.Should().Be("https");
        uriBuilder.Host.Should().Be(TestSubdomain);
        uriBuilder.Path.Should().Be("api/v4/contacts/unlink");
    }

    [Fact]
    public void CreateForLinks_ReturnsCorrectUriBuilder()
    {
        // Arrange
        var entityType = "customers";

        // Act
        UriBuilder uriBuilder = _factory.CreateForLinks(TestSubdomain, entityType);

        // Assert
        uriBuilder.Scheme.Should().Be("https");
        uriBuilder.Host.Should().Be(TestSubdomain);
        uriBuilder.Path.Should().Be("api/v4/customers/links");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateForAuthorization_WithInvalidSubdomain_ThrowsArgumentException(string subdomain)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => _factory.CreateForAuthorization(subdomain));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateForWidget_WithInvalidWidgetCode_ThrowsArgumentException(string widgetCode)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => _factory.CreateForWidget(TestSubdomain, widgetCode));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("bills")]
    public void CreateForCustomFields_WithInvalidEntityType_ThrowsArgumentException(string entityType)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => _factory.CreateForCustomFields(TestSubdomain, entityType));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("bills")]
    public void CreateForNotes_WithInvalidEntityType_ThrowsArgumentException(string entityType)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => _factory.CreateForNotes(TestSubdomain, entityType));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("bills")]
    public void CreateForAddLinks_WithInvalidEntityType_ThrowsArgumentException(string entityType)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => _factory.CreateForAddLinks(TestSubdomain, entityType));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("bills")]
    public void CreateForDeleteLinks_WithInvalidEntityType_ThrowsArgumentException(string entityType)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => _factory.CreateForDeleteLinks(TestSubdomain, entityType));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("bills")]
    public void CreateForLinks_WithInvalidEntityType_ThrowsArgumentException(string entityType)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => _factory.CreateForLinks(TestSubdomain, entityType));
    }

    [Fact]
    public void CreateForContacts_WithNegativeContactId_ThrowsArgumentOutOfRangeException()
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => _factory.CreateForContacts(TestSubdomain, -1));
    }

    [Fact]
    public void CreateForTransactions_WithNegativeCustomerId_ThrowsArgumentOutOfRangeException()
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => _factory.CreateForTransactions(TestSubdomain, -1));
    }

    [Fact]
    public void CreateForDeleteTransaction_WithNegativeTransactionId_ThrowsArgumentOutOfRangeException()
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => _factory.CreateForDeleteTransaction(TestSubdomain, -1));
    }

    // Тесты с граничными значениями
    [Fact]
    public void CreateForContacts_WithZeroContactId_ReturnsCorrectUriBuilder()
    {
        // Act
        UriBuilder uriBuilder = _factory.CreateForContacts(TestSubdomain, 0);

        // Assert
        uriBuilder.Scheme.Should().Be("https");
        uriBuilder.Host.Should().Be(TestSubdomain);
        uriBuilder.Path.Should().Be("api/v4/contacts/0");
    }

    [Fact]
    public void CreateForTransactions_WithZeroCustomerId_ReturnsCorrectUriBuilder()
    {
        // Act
        UriBuilder uriBuilder = _factory.CreateForTransactions(TestSubdomain, 0);

        // Assert
        uriBuilder.Scheme.Should().Be("https");
        uriBuilder.Host.Should().Be(TestSubdomain);
        uriBuilder.Path.Should().Be("api/v4/customers/0/transactions");
    }

    [Fact]
    public void CreateForDeleteTransaction_WithZeroTransactionId_ReturnsCorrectUriBuilder()
    {
        // Act
        UriBuilder uriBuilder = _factory.CreateForDeleteTransaction(TestSubdomain, 0);

        // Assert
        uriBuilder.Scheme.Should().Be("https");
        uriBuilder.Host.Should().Be(TestSubdomain);
        uriBuilder.Path.Should().Be("api/v4/customers/transactions/0");
    }

    [Fact]
    public void CreateForDeleteTransaction_WithMaxIntTransactionId_ReturnsCorrectUriBuilder()
    {
        // Arrange
        var maxId = int.MaxValue;

        // Act
        UriBuilder uriBuilder = _factory.CreateForDeleteTransaction(TestSubdomain, maxId);

        // Assert
        uriBuilder.Scheme.Should().Be("https");
        uriBuilder.Host.Should().Be(TestSubdomain);
        uriBuilder.Path.Should().Be($"api/v4/customers/transactions/{maxId}");
    }
}
