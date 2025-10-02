
namespace Planfact.AmoCrm.Client.Tests;

public class AmoCrmUriBuilderFactoryTests
{
    protected const string TestSubdomain = "example.amocrm.ru";

    private readonly AmoCrmUriBuilderFactory _factory;
    private readonly Mock<IOptions<AmoCrmClientOptions>> _optionsMock;

    public AmoCrmUriBuilderFactoryTests()
    {
        _optionsMock = new Mock<IOptions<AmoCrmClientOptions>>();
        var options = new AmoCrmClientOptions();
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
}
