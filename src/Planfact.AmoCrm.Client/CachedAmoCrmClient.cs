using Microsoft.Extensions.Options;
using Reliable.HttpClient.Caching;

using Planfact.AmoCrm.Client.Account;
using Planfact.AmoCrm.Client.Authorization;
using Planfact.AmoCrm.Client.Common;
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

using AmoCrmTask = Planfact.AmoCrm.Client.Tasks.Task;

namespace Planfact.AmoCrm.Client;

/// <summary>
/// Кэшированная реализация клиента amoCRM с использованием Reliable.HttpClient.Caching.
/// Автоматически инвалидирует кэш после операций мутации (добавление/обновление) для предотвращения использования устаревших данных.
/// Кэширование для операций чтения применяется автоматически через <see cref="IHttpClientWithCache"/>.
/// </summary>
public class CachedAmoCrmClient(
    HttpClientWithCache httpClient,
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
    Func<HttpClientWithCache, IAmoCrmNoteService> noteServiceFactory,
    IOptions<AmoCrmClientOptions> options,
    ILogger<CachedAmoCrmClient> logger) : AmoCrmClient(
                                            accountServiceFactory(httpClient),
                                            authorizationServiceFactory(httpClient),
                                            leadServiceFactory(httpClient),
                                            companyServiceFactory(httpClient),
                                            taskServiceFactory(httpClient),
                                            customerServiceFactory(httpClient),
                                            userServiceFactory(httpClient),
                                            contactServiceFactory(httpClient),
                                            transactionServiceFactory(httpClient),
                                            customFieldServiceFactory(httpClient),
                                            pipelineServiceFactory(httpClient),
                                            noteServiceFactory(httpClient),
                                            options,
                                            logger)
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
        CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<Lead> addedLeads = await base.AddLeadsAsync(
            accessToken,
            subdomain,
            requests,
            cancellationToken
        ).ConfigureAwait(false);

        await _httpClient.InvalidateCacheAsync(_options.LeadsApiPath).ConfigureAwait(false);

        return addedLeads;
    }

    /// <inheritdoc />
    /// <remarks>
    /// В случае успешного выполнения кэш сделок инвалидируется
    /// </remarks>
    public override async Task<IReadOnlyCollection<Lead>> UpdateLeadsAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<UpdateLeadRequest> requests,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<Lead> updatedLeads = await base.UpdateLeadsAsync(
            accessToken,
            subdomain,
            requests,
            cancellationToken
        ).ConfigureAwait(false);

        await _httpClient.InvalidateCacheAsync(_options.LeadsApiPath).ConfigureAwait(false);

        return updatedLeads;
    }

    /// <inheritdoc />
    /// <remarks>
    /// В случае успешного выполнения кэш компаний инвалидируется
    /// </remarks>
    public override async Task<IReadOnlyCollection<Company>> AddCompaniesAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<AddCompanyRequest> requests,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<Company> addedCompanies = await base.AddCompaniesAsync(
            accessToken,
            subdomain,
            requests,
            cancellationToken
        ).ConfigureAwait(false);

        await _httpClient.InvalidateCacheAsync(_options.CompaniesApiPath).ConfigureAwait(false);

        return addedCompanies;
    }

    /// <inheritdoc />
    /// <remarks>
    /// В случае успешного выполнения кэш компаний инвалидируется
    /// </remarks>
    public override async Task<IReadOnlyCollection<Company>> UpdateCompaniesAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<UpdateCompanyRequest> requests,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<Company> updatedCompanies = await base.UpdateCompaniesAsync(
            accessToken,
            subdomain,
            requests,
            cancellationToken
        ).ConfigureAwait(false);

        await _httpClient.InvalidateCacheAsync(_options.CompaniesApiPath).ConfigureAwait(false);

        return updatedCompanies;
    }

    /// <inheritdoc />
    /// <remarks>
    /// В случае успешного выполнения кэш задач инвалидируется
    /// </remarks>
    public override async Task<IReadOnlyCollection<AmoCrmTask>> AddTasksAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<AddTaskRequest> requests,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<AmoCrmTask> addedTasks = await base.AddTasksAsync(
            accessToken,
            subdomain,
            requests,
            cancellationToken
        ).ConfigureAwait(false);

        await _httpClient.InvalidateCacheAsync(_options.TasksApiPath).ConfigureAwait(false);

        return addedTasks;
    }

    /// <inheritdoc />
    /// <remarks>
    /// В случае успешного выполнения кэш задач инвалидируется
    /// </remarks>
    public override async Task<IReadOnlyCollection<AmoCrmTask>> UpdateTasksAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<UpdateTaskRequest> requests,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<AmoCrmTask> updatedTasks = await base.UpdateTasksAsync(
            accessToken,
            subdomain,
            requests,
            cancellationToken
        ).ConfigureAwait(false);

        await _httpClient.InvalidateCacheAsync(_options.TasksApiPath).ConfigureAwait(false);

        return updatedTasks;
    }

    /// <inheritdoc />
    /// <remarks>
    /// В случае успешного выполнения кэш покупателей инвалидируется
    /// </remarks>
    public override async Task<IReadOnlyCollection<Customer>> AddCustomersAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<AddCustomerRequest> requests,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<Customer> addedCustomers = await base.AddCustomersAsync(
            accessToken,
            subdomain,
            requests,
            cancellationToken
        ).ConfigureAwait(false);

        await _httpClient.InvalidateCacheAsync(_options.CustomersApiPath).ConfigureAwait(false);

        return addedCustomers;
    }

    /// <inheritdoc />
    /// <remarks>
    /// В случае успешного выполнения кэш покупателей инвалидируется
    /// </remarks>
    public override async Task<IReadOnlyCollection<Customer>> UpdateCustomersAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<UpdateCustomerRequest> requests,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<Customer> updatedCustomers = await base.UpdateCustomersAsync(
            accessToken,
            subdomain,
            requests,
            cancellationToken
        ).ConfigureAwait(false);

        await _httpClient.InvalidateCacheAsync(_options.CustomersApiPath).ConfigureAwait(false);

        return updatedCustomers;
    }

    /// <inheritdoc />
    /// <remarks>
    /// В случае успешного выполнения кэш контактов инвалидируется
    /// </remarks>
    public override async Task<IReadOnlyCollection<Contact>> AddContactsAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<AddContactRequest> requests,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<Contact> addedContacts = await base.AddContactsAsync(
            accessToken,
            subdomain,
            requests,
            cancellationToken
        ).ConfigureAwait(false);

        await _httpClient.InvalidateCacheAsync(_options.ContactsApiPath).ConfigureAwait(false);

        return addedContacts;
    }

    /// <inheritdoc />
    /// <remarks>
    /// В случае успешного выполнения кэш контактов инвалидируется
    /// </remarks>
    public override async Task<IReadOnlyCollection<Contact>> UpdateContactsAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<UpdateContactRequest> requests,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<Contact> updatedContacts = await base.UpdateContactsAsync(
            accessToken,
            subdomain,
            requests,
            cancellationToken
        ).ConfigureAwait(false);

        await _httpClient.InvalidateCacheAsync(_options.ContactsApiPath).ConfigureAwait(false);

        return updatedContacts;
    }

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
        IReadOnlyCollection<Transaction> addedTransactions = await base.AddTransactionsAsync(
            accessToken,
            subdomain,
            customerId,
            requests,
            cancellationToken
        ).ConfigureAwait(false);

        await _httpClient.InvalidateCacheAsync($"{_options.TransactionsApiPath}/{customerId}").ConfigureAwait(false);

        return addedTransactions;
    }

    /// <inheritdoc />
    /// <remarks>
    /// В случае успешного выполнения кэш примечаний инвалидируется
    /// </remarks>
    public override async Task<IReadOnlyCollection<Note>> AddNotesAsync(
        string accessToken,
        string subdomain,
        AmoCrm.Client.Common.EntityTypeEnum entityType,
        IReadOnlyCollection<AddNoteRequest> requests,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<Note> addedNotes = await base.AddNotesAsync(
            accessToken,
            subdomain,
            entityType,
            requests,
            cancellationToken
        ).ConfigureAwait(false);

        var entityTypeName = EntityTypeConverter.ToString(entityType);
        await _httpClient.InvalidateCacheAsync($"{_options.BaseApiPath}/{entityTypeName}/notes").ConfigureAwait(false);

        return addedNotes;
    }
}
