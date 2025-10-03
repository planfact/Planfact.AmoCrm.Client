using Microsoft.Extensions.Options;
using Reliable.HttpClient.Caching;

using Planfact.AmoCrm.Client.Common;
using Planfact.AmoCrm.Client.Companies;
using Planfact.AmoCrm.Client.Contacts;
using Planfact.AmoCrm.Client.Customers;
using Planfact.AmoCrm.Client.Leads;
using Planfact.AmoCrm.Client.Notes;
using Planfact.AmoCrm.Client.Tasks;
using Planfact.AmoCrm.Client.Transactions;

using AmoCrmTask = Planfact.AmoCrm.Client.Tasks.Task;

namespace Planfact.AmoCrm.Client;

/// <summary>
/// Кэшированная реализация клиента amoCRM с использованием Reliable.HttpClient.Caching.
/// Автоматически инвалидирует кэш после операций мутации (добавление/обновление) для предотвращения использования устаревших данных.
/// Кэширование для операций чтения применяется автоматически через <see cref="IHttpClientWithCache"/>.
/// </summary>
public class CachedAmoCrmClient(
    HttpClientWithCache httpClient,
    IAmoCrmServiceFactory serviceFactory,
    IOptions<AmoCrmClientOptions> options) : AmoCrmClient(
        serviceFactory.CreateAccountService(httpClient),
        serviceFactory.CreateAuthorizationService(httpClient),
        serviceFactory.CreateLeadService(httpClient),
        serviceFactory.CreateCompanyService(httpClient),
        serviceFactory.CreateTaskService(httpClient),
        serviceFactory.CreateCustomerService(httpClient),
        serviceFactory.CreateUserService(httpClient),
        serviceFactory.CreateContactService(httpClient),
        serviceFactory.CreateTransactionService(httpClient),
        serviceFactory.CreateCustomFieldService(httpClient),
        serviceFactory.CreatePipelineService(httpClient),
        serviceFactory.CreateNoteService(httpClient),
        options)
{
    private readonly HttpClientWithCache _httpClient = httpClient;
    private readonly AmoCrmClientOptions _options = options.Value;

    /// <inheritdoc />
    /// <remarks>
    /// В случае успешного выполнения кэш сделок инвалидируется
    /// </remarks>
    public override async Task<IReadOnlyCollection<Lead>> AddLeadsAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<AddLeadRequest> requests,
        CancellationToken cancellationToken = default) =>
        await ExecuteWithCacheInvalidationAsync(
            () => base.AddLeadsAsync(accessToken, subdomain, requests, cancellationToken),
            _options.LeadsApiPath
        ).ConfigureAwait(false);

    /// <inheritdoc />
    /// <remarks>
    /// В случае успешного выполнения кэш сделок инвалидируется
    /// </remarks>
    public override async Task<IReadOnlyCollection<Lead>> UpdateLeadsAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<UpdateLeadRequest> requests,
        CancellationToken cancellationToken = default) =>
        await ExecuteWithCacheInvalidationAsync(
            () => base.UpdateLeadsAsync(accessToken, subdomain, requests, cancellationToken),
            _options.LeadsApiPath
        ).ConfigureAwait(false);

    /// <inheritdoc />
    /// <remarks>
    /// В случае успешного выполнения кэш компаний инвалидируется
    /// </remarks>
    public override async Task<IReadOnlyCollection<Company>> AddCompaniesAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<AddCompanyRequest> requests,
        CancellationToken cancellationToken = default) =>
        await ExecuteWithCacheInvalidationAsync(
            () => base.AddCompaniesAsync(accessToken, subdomain, requests, cancellationToken),
            _options.CompaniesApiPath
        ).ConfigureAwait(false);

    /// <inheritdoc />
    /// <remarks>
    /// В случае успешного выполнения кэш компаний инвалидируется
    /// </remarks>
    public override async Task<IReadOnlyCollection<Company>> UpdateCompaniesAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<UpdateCompanyRequest> requests,
        CancellationToken cancellationToken = default) =>
        await ExecuteWithCacheInvalidationAsync(
            () => base.UpdateCompaniesAsync(accessToken, subdomain, requests, cancellationToken),
            _options.CompaniesApiPath
        ).ConfigureAwait(false);

    /// <inheritdoc />
    /// <remarks>
    /// В случае успешного выполнения кэш задач инвалидируется
    /// </remarks>
    public override async Task<IReadOnlyCollection<AmoCrmTask>> AddTasksAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<AddTaskRequest> requests,
        CancellationToken cancellationToken = default) =>
        await ExecuteWithCacheInvalidationAsync(
            () => base.AddTasksAsync(accessToken, subdomain, requests, cancellationToken),
            _options.TasksApiPath
        ).ConfigureAwait(false);

    /// <inheritdoc />
    /// <remarks>
    /// В случае успешного выполнения кэш задач инвалидируется
    /// </remarks>
    public override async Task<IReadOnlyCollection<AmoCrmTask>> UpdateTasksAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<UpdateTaskRequest> requests,
        CancellationToken cancellationToken = default) =>
        await ExecuteWithCacheInvalidationAsync(
            () => base.UpdateTasksAsync(accessToken, subdomain, requests, cancellationToken),
            _options.TasksApiPath
        ).ConfigureAwait(false);

    /// <inheritdoc />
    /// <remarks>
    /// В случае успешного выполнения кэш покупателей инвалидируется
    /// </remarks>
    public override async Task<IReadOnlyCollection<Customer>> AddCustomersAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<AddCustomerRequest> requests,
        CancellationToken cancellationToken = default) =>
        await ExecuteWithCacheInvalidationAsync(
            () => base.AddCustomersAsync(accessToken, subdomain, requests, cancellationToken),
            _options.CustomersApiPath
        ).ConfigureAwait(false);

    /// <inheritdoc />
    /// <remarks>
    /// В случае успешного выполнения кэш покупателей инвалидируется
    /// </remarks>
    public override async Task<IReadOnlyCollection<Customer>> UpdateCustomersAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<UpdateCustomerRequest> requests,
        CancellationToken cancellationToken = default) =>
        await ExecuteWithCacheInvalidationAsync(
            () => base.UpdateCustomersAsync(accessToken, subdomain, requests, cancellationToken),
            _options.CustomersApiPath
        ).ConfigureAwait(false);

    /// <inheritdoc />
    /// <remarks>
    /// В случае успешного выполнения кэш контактов инвалидируется
    /// </remarks>
    public override async Task<IReadOnlyCollection<Contact>> AddContactsAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<AddContactRequest> requests,
        CancellationToken cancellationToken = default) =>
        await ExecuteWithCacheInvalidationAsync(
            () => base.AddContactsAsync(accessToken, subdomain, requests, cancellationToken),
            _options.ContactsApiPath
        ).ConfigureAwait(false);

    /// <inheritdoc />
    /// <remarks>
    /// В случае успешного выполнения кэш контактов инвалидируется
    /// </remarks>
    public override async Task<IReadOnlyCollection<Contact>> UpdateContactsAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<UpdateContactRequest> requests,
        CancellationToken cancellationToken = default) =>
        await ExecuteWithCacheInvalidationAsync(
            () => base.UpdateContactsAsync(accessToken, subdomain, requests, cancellationToken),
            _options.ContactsApiPath
        ).ConfigureAwait(false);

    /// <inheritdoc />
    /// <remarks>
    /// В случае успешного выполнения кэш транзакций инвалидируется
    /// </remarks>
    public override async Task<IReadOnlyCollection<Transaction>> AddTransactionsAsync(
        string accessToken,
        string subdomain,
        int customerId,
        IReadOnlyCollection<AddTransactionRequest> requests,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteWithCacheInvalidationAsync(
            () => base.AddTransactionsAsync(accessToken, subdomain, customerId, requests, cancellationToken),
            $"{_options.TransactionsApiPath}/{customerId}"
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    /// <remarks>
    /// В случае успешного выполнения кэш примечаний инвалидируется
    /// </remarks>
    public override async Task<IReadOnlyCollection<Note>> AddNotesAsync(
        string accessToken,
        string subdomain,
        EntityType entityType,
        IReadOnlyCollection<AddNoteRequest> requests,
        CancellationToken cancellationToken = default)
    {
        var entityTypeName = EntityTypeConverter.ToString(entityType);
        return await ExecuteWithCacheInvalidationAsync(
            () => base.AddNotesAsync(accessToken, subdomain, entityType, requests, cancellationToken),
            $"{_options.BaseApiPath}/{entityTypeName}/{_options.NotesApiResourceName}"
        ).ConfigureAwait(false);
    }

    /// <summary>
    /// Выполняет операцию мутации с автоматической инвалидацией кэша
    /// </summary>
    /// <typeparam name="T">Тип возвращаемого результата</typeparam>
    /// <param name="operation">Операция для выполнения</param>
    /// <param name="cacheKey">Ключ кэша для инвалидации</param>
    /// <returns>Результат операции</returns>
    private async Task<T> ExecuteWithCacheInvalidationAsync<T>(
        Func<Task<T>> operation,
        string cacheKey)
    {
        T result = await operation().ConfigureAwait(false);
        await _httpClient.InvalidateCacheAsync(cacheKey).ConfigureAwait(false);
        return result;
    }
}
