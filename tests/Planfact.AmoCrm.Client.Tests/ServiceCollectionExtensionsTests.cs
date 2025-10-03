using Reliable.HttpClient;
using Reliable.HttpClient.Caching;
using Reliable.HttpClient.Caching.Abstractions;

namespace Planfact.AmoCrm.Client.Tests;

public class ServiceCollectionExtensionsTests
{
    private readonly IServiceCollection _services;
    private readonly IConfiguration _configuration;

    public ServiceCollectionExtensionsTests()
    {
        _services = new ServiceCollection();

        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            [$"{nameof(AmoCrmClientOptions)}:{nameof(AmoCrmClientOptions.ClientId)}"] = "test-client-id",
            [$"{nameof(AmoCrmClientOptions)}:{nameof(AmoCrmClientOptions.ClientSecret)}"] = "test-client-secret",
            [$"{nameof(AmoCrmClientOptions)}:{nameof(AmoCrmClientOptions.BaseApiPath)}"] = "api/v4",
            [$"{nameof(AmoCrmClientOptions)}:{nameof(AmoCrmClientOptions.LeadsApiPath)}"] = "api/v4/leads",
            [$"{nameof(AmoCrmClientOptions)}:{nameof(AmoCrmClientOptions.CompaniesApiPath)}"] = "api/v4/companies",
            [$"{nameof(AmoCrmClientOptions)}:{nameof(AmoCrmClientOptions.TasksApiPath)}"] = "api/v4/tasks",
            [$"{nameof(AmoCrmClientOptions)}:{nameof(AmoCrmClientOptions.CustomersApiPath)}"] = "api/v4/customers",
            [$"{nameof(AmoCrmClientOptions)}:{nameof(AmoCrmClientOptions.ContactsApiPath)}"] = "api/v4/contacts",
            [$"{nameof(AmoCrmClientOptions)}:{nameof(AmoCrmClientOptions.TransactionsApiPath)}"] = "api/v4/customers/transactions",
            [$"{nameof(AmoCrmClientOptions)}:{nameof(AmoCrmClientOptions.AccountsApiPath)}"] = "api/v4/account",
            [$"{nameof(AmoCrmClientOptions)}:{nameof(AmoCrmClientOptions.UsersApiPath)}"] = "api/v4/users",
            [$"{nameof(AmoCrmClientOptions)}:{nameof(AmoCrmClientOptions.WidgetsApiPath)}"] = "api/v4/widgets",
            [$"{nameof(AmoCrmClientOptions)}:{nameof(AmoCrmClientOptions.TimeoutSeconds)}"] = "120",
            [$"{nameof(AmoCrmClientOptions)}:{nameof(AmoCrmClientOptions.UserAgent)}"] = "AmoCrmClient/1.0"
        });
        _configuration = configurationBuilder.Build();

        _services.AddLogging();
    }

    [Fact]
    public void ConfigurationBinding_AppliesSettingsCorrectly()
    {
        // Act
        _services.AddAmoCrmClient(_configuration);
        ServiceProvider serviceProvider = _services.BuildServiceProvider();

        // Assert
        IOptions<AmoCrmClientOptions> options = serviceProvider.GetRequiredService<IOptions<AmoCrmClientOptions>>();
        options.Value.ClientId.Should().Be("test-client-id");
        options.Value.ClientSecret.Should().Be("test-client-secret");
        options.Value.BaseApiPath.Should().Be("api/v4");
        options.Value.LeadsApiPath.Should().Be("api/v4/leads");
        options.Value.CompaniesApiPath.Should().Be("api/v4/companies");
        options.Value.TasksApiPath.Should().Be("api/v4/tasks");
        options.Value.CustomersApiPath.Should().Be("api/v4/customers");
        options.Value.ContactsApiPath.Should().Be("api/v4/contacts");
        options.Value.TransactionsApiPath.Should().Be("api/v4/customers/transactions");
        options.Value.AccountsApiPath.Should().Be("api/v4/account");
        options.Value.UsersApiPath.Should().Be("api/v4/users");
        options.Value.WidgetsApiPath.Should().Be("api/v4/widgets");
        options.Value.TimeoutSeconds.Should().Be(120);
        options.Value.UserAgent.Should().Be("AmoCrmClient/1.0");
    }

    [Fact]
    public void AddAmoCrmClient_WithoutCaching_ResolvesToBaseClient()
    {
        // Act
        _services.AddAmoCrmClient(_configuration);
        ServiceProvider serviceProvider = _services.BuildServiceProvider();

        // Assert
        serviceProvider.GetRequiredService<IAmoCrmClient>().Should().BeOfType<AmoCrmClient>();
    }

    [Fact]
    public void AddAmoCrmClient_HttpResponseHandlerIsRegistered()
    {
        // Act
        _services.AddAmoCrmClient(_configuration);
        ServiceProvider serviceProvider = _services.BuildServiceProvider();

        // Assert
        serviceProvider.GetRequiredService<IHttpResponseHandler>().Should().BeOfType<AmoCrmHttpResponseHandler>();
    }

    [Fact]
    public void AddAmoCrmClient_UriBuilderFactoryIsRegistered()
    {
        // Act
        _services.AddAmoCrmClient(_configuration);
        ServiceProvider serviceProvider = _services.BuildServiceProvider();

        // Assert
        serviceProvider.GetRequiredService<AmoCrmUriBuilderFactory>().Should().NotBeNull();
    }

    [Fact]
    public void AddAmoCrmClient_MissingConfiguration_ThrowsInvalidOperationException()
    {
        // Arrange
        IConfigurationRoot emptyConfig = new ConfigurationBuilder().Build();

        // Act & Assert
        FluentActions.Invoking(() => _services.AddAmoCrmClient(emptyConfig))
            .Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void AddCachedAmoCrmClient_ResolvesToCachedClient()
    {
        // Act
        _services.AddCachedAmoCrmClient(_configuration);
        ServiceProvider serviceProvider = _services.BuildServiceProvider();

        // Assert
        serviceProvider.GetRequiredService<IAmoCrmClient>().Should().BeOfType<CachedAmoCrmClient>();
    }

    [Fact]
    public void AddCachedAmoCrmClient_HttpResponseHandlerIsRegistered()
    {
        // Act
        _services.AddCachedAmoCrmClient(_configuration);
        ServiceProvider serviceProvider = _services.BuildServiceProvider();

        // Assert
        serviceProvider.GetRequiredService<IHttpResponseHandler>().Should().BeOfType<AmoCrmHttpResponseHandler>();
    }

    [Fact]
    public void AddCachedAmoCrmClient_UriBuilderFactoryIsRegistered()
    {
        // Act
        _services.AddCachedAmoCrmClient(_configuration);
        ServiceProvider serviceProvider = _services.BuildServiceProvider();

        // Assert
        serviceProvider.GetRequiredService<AmoCrmUriBuilderFactory>().Should().NotBeNull();
    }

    [Fact]
    public void AddCachedAmoCrmClient_MissingConfiguration_ThrowsInvalidOperationException()
    {
        // Arrange
        IConfigurationRoot emptyConfig = new ConfigurationBuilder().Build();

        // Act & Assert
        FluentActions.Invoking(() => _services.AddCachedAmoCrmClient(emptyConfig))
            .Should().Throw<InvalidOperationException>()
            .WithMessage($"*Section '{nameof(AmoCrmClientOptions)}' not found in configuration.*");
    }

    [Fact]
    public void AddCachedAmoCrmClient_RegistersCachedHttpClient()
    {
        // Act
        _services.AddCachedAmoCrmClient(_configuration);
        ServiceProvider serviceProvider = _services.BuildServiceProvider();

        // Assert
        IHttpClientWithCache? cachedClient = serviceProvider.GetService<IHttpClientWithCache>();
        cachedClient.Should().NotBeNull();
    }

    [Fact]
    public void AddCachedAmoCrmClient_AppliesCorrectCachingOptions()
    {
        // Act
        _services.AddCachedAmoCrmClient(_configuration);
        ServiceProvider serviceProvider = _services.BuildServiceProvider();

        // Assert
        IOptionsSnapshot<HttpCacheOptions> options = serviceProvider.GetRequiredService<IOptionsSnapshot<HttpCacheOptions>>();
        HttpCacheOptions cacheOptions = options.Get("AmoCrmCached");

        cacheOptions.DefaultExpiry.Should().Be(TimeSpan.FromMinutes(10));
        cacheOptions.MaxCacheSize.Should().Be(1000);
        cacheOptions.DefaultHeaders.Should().ContainKey("User-Agent")
            .WhoseValue.Should().Be("AmoCrmClient/1.0");
    }

    [Fact]
    public void AddCachedAmoCrmClient_UsesScopedLifetime()
    {
        // Act
        _services.AddCachedAmoCrmClient(_configuration);
        ServiceProvider serviceProvider = _services.BuildServiceProvider();

        // Assert
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IAmoCrmClient client1 = scope.ServiceProvider.GetRequiredService<IAmoCrmClient>();
            IAmoCrmClient client2 = scope.ServiceProvider.GetRequiredService<IAmoCrmClient>();
            client1.Should().BeSameAs(client2);
        }

        using (IServiceScope scope1 = serviceProvider.CreateScope())
        using (IServiceScope scope2 = serviceProvider.CreateScope())
        {
            IAmoCrmClient client1 = scope1.ServiceProvider.GetRequiredService<IAmoCrmClient>();
            IAmoCrmClient client2 = scope2.ServiceProvider.GetRequiredService<IAmoCrmClient>();
            client1.Should().NotBeSameAs(client2);
        }
    }
}
