using Microsoft.Extensions.Options;
using Moq;

using Planfact.AmoCrm.Client.Configuration;

namespace Planfact.AmoCrm.Client.Tests;

[TestFixture]
public class AmoCrmUriBuilderFactoryTests
{
    private AmoCrmUriBuilderFactory _factory;
    private Mock<IOptions<AmoCrmClientOptions>> _optionsMock;

    [SetUp]
    public void SetUp()
    {
        _optionsMock = new Mock<IOptions<AmoCrmClientOptions>>();
        var options = new AmoCrmClientOptions();
        _optionsMock.Setup(o => o.Value).Returns(options);
        _factory = new AmoCrmUriBuilderFactory(_optionsMock.Object);
    }

    [Test]
    public void CreateForAuthorization_ReturnsCorrectUriBuilder()
    {
        // Arrange
        var subdomain = "example.amocrm.ru";

        // Act
        UriBuilder uriBuilder = _factory.CreateForAuthorization(subdomain);

        // Assert
        uriBuilder.Scheme.Should().Be("https");
        uriBuilder.Host.Should().Be(subdomain);
        uriBuilder.Path.Should().Be("oauth2/access_token");
    }

    [Test]
    public void CreateForCompanies_ReturnsCorrectUriBuilder()
    {
        // Arrange
        var subdomain = "example.amocrm.ru";

        // Act
        UriBuilder uriBuilder = _factory.CreateForCompanies(subdomain);

        // Assert
        uriBuilder.Scheme.Should().Be("https");
        uriBuilder.Host.Should().Be(subdomain);
        uriBuilder.Path.Should().Be("api/v4/companies");
    }

    [Test]
    public void CreateForLeads_ReturnsCorrectUriBuilder()
    {
        // Arrange
        var subdomain = "example.amocrm.ru";

        // Act
        UriBuilder uriBuilder = _factory.CreateForLeads(subdomain);

        // Assert
        uriBuilder.Scheme.Should().Be("https");
        uriBuilder.Host.Should().Be(subdomain);
        uriBuilder.Path.Should().Be("api/v4/leads");
    }

    [Test]
    public void CreateForTasks_ReturnsCorrectUriBuilder()
    {
        // Arrange
        var subdomain = "example.amocrm.ru";

        // Act
        UriBuilder uriBuilder = _factory.CreateForTasks(subdomain);

        // Assert
        uriBuilder.Scheme.Should().Be("https");
        uriBuilder.Host.Should().Be(subdomain);
        uriBuilder.Path.Should().Be("api/v4/tasks");
    }

    [Test]
    public void CreateForCustomers_ReturnsCorrectUriBuilder()
    {
        // Arrange
        var subdomain = "example.amocrm.ru";

        // Act
        UriBuilder uriBuilder = _factory.CreateForCustomers(subdomain);

        // Assert
        uriBuilder.Scheme.Should().Be("https");
        uriBuilder.Host.Should().Be(subdomain);
        uriBuilder.Path.Should().Be("api/v4/customers");
    }

    [Test]
    public void CreateForAccount_ReturnsCorrectUriBuilder()
    {
        // Arrange
        var subdomain = "example.amocrm.ru";

        // Act
        UriBuilder uriBuilder = _factory.CreateForAccount(subdomain);

        // Assert
        uriBuilder.Scheme.Should().Be("https");
        uriBuilder.Host.Should().Be(subdomain);
        uriBuilder.Path.Should().Be("api/v4/account");
    }

    [Test]
    public void CreateForUsers_ReturnsCorrectUriBuilder()
    {
        // Arrange
        var subdomain = "example.amocrm.ru";

        // Act
        UriBuilder uriBuilder = _factory.CreateForUsers(subdomain);

        // Assert
        uriBuilder.Scheme.Should().Be("https");
        uriBuilder.Host.Should().Be(subdomain);
        uriBuilder.Path.Should().Be("api/v4/users");
    }

    [Test]
    public void CreateForWidget_ReturnsCorrectUriBuilder()
    {
        // Arrange
        var subdomain = "example.amocrm.ru";
        var widgetCode = "W001";

        // Act
        UriBuilder uriBuilder = _factory.CreateForWidget(subdomain, widgetCode);

        // Assert
        uriBuilder.Scheme.Should().Be("https");
        uriBuilder.Host.Should().Be(subdomain);
        uriBuilder.Path.Should().Be($"api/v4/widgets/{widgetCode}");
    }

    [Test]
    public void CreateForContacts_WithNoContactId_ReturnsCorrectUriBuilder()
    {
        // Arrange
        var subdomain = "example.amocrm.ru";

        // Act
        UriBuilder uriBuilder = _factory.CreateForContacts(subdomain);

        // Assert
        uriBuilder.Scheme.Should().Be("https");
        uriBuilder.Host.Should().Be(subdomain);
        uriBuilder.Path.Should().Be("api/v4/contacts");
    }

    [Test]
    public void CreateForContacts_WithContactId_ReturnsCorrectUriBuilder()
    {
        // Arrange
        var subdomain = "example.amocrm.ru";
        var contactId = 123;

        // Act
        UriBuilder uriBuilder = _factory.CreateForContacts(subdomain, contactId);

        // Assert
        uriBuilder.Scheme.Should().Be("https");
        uriBuilder.Host.Should().Be(subdomain);
        uriBuilder.Path.Should().Be($"api/v4/contacts/{contactId}");
    }

    [Test]
    public void CreateForTransactions_WithNoCustomerId_ReturnsCorrectUriBuilder()
    {
        // Arrange
        var subdomain = "example.amocrm.ru";

        // Act
        UriBuilder uriBuilder = _factory.CreateForTransactions(subdomain);

        // Assert
        uriBuilder.Scheme.Should().Be("https");
        uriBuilder.Host.Should().Be(subdomain);
        uriBuilder.Path.Should().Be("api/v4/customers/transactions");
    }

    [Test]
    public void CreateForTransactions_WithCustomerId_ReturnsCorrectUriBuilder()
    {
        // Arrange
        var subdomain = "example.amocrm.ru";
        var customerId = 456;

        // Act
        UriBuilder uriBuilder = _factory.CreateForTransactions(subdomain, customerId);

        // Assert
        uriBuilder.Scheme.Should().Be("https");
        uriBuilder.Host.Should().Be(subdomain);
        uriBuilder.Path.Should().Be($"api/v4/customers/{customerId}/transactions");
    }

    [Test]
    public void CreateForCustomFields_ReturnsCorrectUriBuilder()
    {
        // Arrange
        var subdomain = "example.amocrm.ru";
        var entityType = "leads";

        // Act
        UriBuilder uriBuilder = _factory.CreateForCustomFields(subdomain, entityType);

        // Assert
        uriBuilder.Scheme.Should().Be("https");
        uriBuilder.Host.Should().Be(subdomain);
        uriBuilder.Path.Should().Be("api/v4/leads/custom_fields");
    }

    [Test]
    public void CreateForPipelines_ReturnsCorrectUriBuilder()
    {
        // Arrange
        var subdomain = "example.amocrm.ru";

        // Act
        UriBuilder uriBuilder = _factory.CreateForPipelines(subdomain);

        // Assert
        uriBuilder.Scheme.Should().Be("https");
        uriBuilder.Host.Should().Be(subdomain);
        uriBuilder.Path.Should().Be("api/v4/leads/pipelines");
    }

    [Test]
    public void CreateForNotes_ReturnsCorrectUriBuilder()
    {
        // Arrange
        var subdomain = "example.amocrm.ru";
        var entityType = "leads";

        // Act
        UriBuilder uriBuilder = _factory.CreateForNotes(subdomain, entityType);

        // Assert
        uriBuilder.Scheme.Should().Be("https");
        uriBuilder.Host.Should().Be(subdomain);
        uriBuilder.Path.Should().Be("api/v4/leads/notes");
    }
}
