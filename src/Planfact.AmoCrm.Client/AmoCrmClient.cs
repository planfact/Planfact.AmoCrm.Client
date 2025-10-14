using Microsoft.Extensions.Options;

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
/// Основной класс, предоставляющий логику взаимодействия с amoCRM по API v4.
/// </summary>
public class AmoCrmClient : IAmoCrmClient
{
    private readonly IAmoCrmAccountService _accountService;
    private readonly IAmoCrmAuthorizationService _authorizationService;
    private readonly IAmoCrmLeadService _leadService;
    private readonly IAmoCrmCompanyService _companyService;
    private readonly IAmoCrmTaskService _taskService;
    private readonly IAmoCrmCustomerService _customerService;
    private readonly IAmoCrmUserService _userService;
    private readonly IAmoCrmContactService _contactService;
    private readonly IAmoCrmTransactionService _transactionService;
    private readonly IAmoCrmCustomFieldService _customFieldService;
    private readonly IAmoCrmPipelineService _pipelineService;
    private readonly IAmoCrmNoteService _noteService;
    private readonly AmoCrmClientOptions _options;

    public AmoCrmClient(
        IAmoCrmAccountService accountService,
        IAmoCrmAuthorizationService authorizationService,
        IAmoCrmLeadService leadService,
        IAmoCrmCompanyService companyService,
        IAmoCrmTaskService taskService,
        IAmoCrmCustomerService customerService,
        IAmoCrmUserService userService,
        IAmoCrmContactService contactService,
        IAmoCrmTransactionService transactionService,
        IAmoCrmCustomFieldService customFieldService,
        IAmoCrmPipelineService pipelineService,
        IAmoCrmNoteService noteService,
        IOptions<AmoCrmClientOptions> options)
    {
        _accountService = accountService;
        _authorizationService = authorizationService;
        _leadService = leadService;
        _companyService = companyService;
        _taskService = taskService;
        _customerService = customerService;
        _userService = userService;
        _contactService = contactService;
        _transactionService = transactionService;
        _customFieldService = customFieldService;
        _pipelineService = pipelineService;
        _noteService = noteService;
        _options = options.Value;

        AmoCrmClientOptionsValidator.Validate(_options);
    }

    /// <inheritdoc />
    public virtual async Task<AuthorizationTokens> AuthorizeAsync(
        string subdomain,
        string authorizationCode,
        string redirectUri,
        CancellationToken cancellationToken = default)
    {
        return await _authorizationService.AuthorizeAsync(
            subdomain,
            authorizationCode,
            redirectUri,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<AuthorizationTokens> AuthorizeInternalAsync(CancellationToken cancellationToken = default)
    {
        return await _authorizationService.AuthorizeAsync(
            _options.ServerIntegrationSubdomain,
            _options.ServerIntegrationAuthCode,
            _options.ServerIntegrationRedirectUri,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<AuthorizationTokens> RefreshTokenAsync(
        string subdomain,
        string refreshToken,
        string redirectUri,
        CancellationToken cancellationToken = default)
    {
        return await _authorizationService.RefreshTokenAsync(
            subdomain,
            refreshToken,
            redirectUri,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<AuthorizationTokens> RefreshTokenInternalAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        return await _authorizationService.RefreshTokenAsync(
            _options.ServerIntegrationSubdomain,
            refreshToken,
            _options.ServerIntegrationRedirectUri,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<Lead>> GetLeadsAsync(
        string accessToken,
        string subdomain,
        string query = "",
        CancellationToken cancellationToken = default)
    {
        return await _leadService.GetLeadsAsync(
            accessToken,
            subdomain,
            query,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<Lead>> GetLeadsAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<EntityType> linkedEntityTypes,
        string query = "",
        CancellationToken cancellationToken = default)
    {
        return await _leadService.GetLeadsAsync(
            accessToken,
            subdomain,
            linkedEntityTypes,
            query,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<Lead>> GetLeadsInternalAsync(
        string accessToken,
        string query = "",
        CancellationToken cancellationToken = default)
    {
        return await _leadService.GetLeadsAsync(
            accessToken,
            _options.ServerIntegrationSubdomain,
            query,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<Lead>> GetLeadsInternalAsync(
        string accessToken,
        IReadOnlyCollection<EntityType> linkedEntityTypes,
        string query = "",
        CancellationToken cancellationToken = default)
    {
        return await _leadService.GetLeadsAsync(
            accessToken,
            _options.ServerIntegrationSubdomain,
            linkedEntityTypes,
            query,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<Lead>> GetLeadsAsync(
        string accessToken,
        string subdomain,
        IEnumerable<int> ids,
        CancellationToken cancellationToken = default)
    {
        return await _leadService.GetLeadsAsync(
            accessToken,
            subdomain,
            ids,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<Lead>> GetLeadsAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<EntityType> linkedEntityTypes,
        IEnumerable<int> ids,
        CancellationToken cancellationToken = default)
    {
        return await _leadService.GetLeadsAsync(
            accessToken,
            subdomain,
            linkedEntityTypes,
            ids,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<Lead>> GetLeadsInternalAsync(
        string accessToken,
        IEnumerable<int> ids,
        CancellationToken cancellationToken = default)
    {
        return await _leadService.GetLeadsAsync(
            accessToken,
            _options.ServerIntegrationSubdomain,
            ids,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<Lead>> GetLeadsInternalAsync(
        string accessToken,
        IReadOnlyCollection<EntityType> linkedEntityTypes,
        IEnumerable<int> ids,
        CancellationToken cancellationToken = default)
    {
        return await _leadService.GetLeadsAsync(
            accessToken,
            _options.ServerIntegrationSubdomain,
            linkedEntityTypes,
            ids,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<Lead>> AddLeadsAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<AddLeadRequest> requests,
        CancellationToken cancellationToken = default)
    {
        return await _leadService.AddLeadsAsync(
            accessToken,
            subdomain,
            requests,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<Lead>> AddLeadsInternalAsync(
        string accessToken,
        IReadOnlyCollection<AddLeadRequest> requests,
        CancellationToken cancellationToken = default)
    {
        return await _leadService.AddLeadsAsync(
            accessToken,
            _options.ServerIntegrationSubdomain,
            requests,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<Lead>> UpdateLeadsAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<UpdateLeadRequest> requests,
        CancellationToken cancellationToken = default)
    {
        return await _leadService.UpdateLeadsAsync(
            accessToken,
            subdomain,
            requests,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<Lead>> UpdateLeadsInternalAsync(
        string accessToken,
        IReadOnlyCollection<UpdateLeadRequest> requests,
        CancellationToken cancellationToken = default)
    {
        return await _leadService.UpdateLeadsAsync(
            accessToken,
            _options.ServerIntegrationSubdomain,
            requests,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<Company>> GetCompaniesAsync(
        string accessToken,
        string subdomain,
        string query = "",
        CancellationToken cancellationToken = default)
    {
        return await _companyService.GetCompaniesAsync(
            accessToken,
            subdomain,
            query,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<Company>> GetCompaniesAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<EntityType> linkedEntityTypes,
        string query = "",
        CancellationToken cancellationToken = default)
    {
        return await _companyService.GetCompaniesAsync(
            accessToken,
            subdomain,
            linkedEntityTypes,
            query,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<Company>> GetCompaniesInternalAsync(
        string accessToken,
        string query = "",
        CancellationToken cancellationToken = default)
    {
        return await _companyService.GetCompaniesAsync(
            accessToken,
            _options.ServerIntegrationSubdomain,
            query,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<Company>> GetCompaniesInternalAsync(
        string accessToken,
        IReadOnlyCollection<EntityType> linkedEntityTypes,
        string query = "",
        CancellationToken cancellationToken = default)
    {
        return await _companyService.GetCompaniesAsync(
            accessToken,
            _options.ServerIntegrationSubdomain,
            linkedEntityTypes,
            query,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<Company>> AddCompaniesAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<AddCompanyRequest> requests,
        CancellationToken cancellationToken = default)
    {
        return await _companyService.AddCompaniesAsync(
            accessToken,
            subdomain,
            requests,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<Company>> AddCompaniesInternalAsync(
        string accessToken,
        IReadOnlyCollection<AddCompanyRequest> requests,
        CancellationToken cancellationToken = default)
    {
        return await _companyService.AddCompaniesAsync(
            accessToken,
            _options.ServerIntegrationSubdomain,
            requests,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<Company>> UpdateCompaniesAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<UpdateCompanyRequest> requests,
        CancellationToken cancellationToken = default)
    {
        return await _companyService.UpdateCompaniesAsync(
            accessToken,
            subdomain,
            requests,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<Company>> UpdateCompaniesInternalAsync(
        string accessToken,
        IReadOnlyCollection<UpdateCompanyRequest> requests,
        CancellationToken cancellationToken = default)
    {
        return await _companyService.UpdateCompaniesAsync(
            accessToken,
            _options.ServerIntegrationSubdomain,
            requests,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<AmoCrmTask>> GetTasksAsync(
        string accessToken,
        string subdomain,
        CancellationToken cancellationToken = default)
    {
        return await _taskService.GetTasksAsync(
            accessToken,
            subdomain,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<AmoCrmTask>> GetTasksInternalAsync(
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        return await _taskService.GetTasksAsync(
            accessToken,
            _options.ServerIntegrationSubdomain,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<AmoCrmTask>> AddTasksAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<AddTaskRequest> requests,
        CancellationToken cancellationToken = default)
    {
        return await _taskService.AddTasksAsync(
            accessToken,
            subdomain,
            requests,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<AmoCrmTask>> AddTasksInternalAsync(
        string accessToken,
        IReadOnlyCollection<AddTaskRequest> requests,
        CancellationToken cancellationToken = default)
    {
        return await _taskService.AddTasksAsync(
            accessToken,
            _options.ServerIntegrationSubdomain,
            requests,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<AmoCrmTask>> UpdateTasksAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<UpdateTaskRequest> requests,
        CancellationToken cancellationToken = default)
    {
        return await _taskService.UpdateTasksAsync(
            accessToken,
            subdomain,
            requests,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<AmoCrmTask>> UpdateTasksInternalAsync(
        string accessToken,
        IReadOnlyCollection<UpdateTaskRequest> requests,
        CancellationToken cancellationToken = default)
    {
        return await _taskService.UpdateTasksAsync(
            accessToken,
            _options.ServerIntegrationSubdomain,
            requests,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<Customer>> GetCustomersAsync(
        string accessToken,
        string subdomain,
        string query = "",
        CancellationToken cancellationToken = default)
    {
        return await _customerService.GetCustomersAsync(
            accessToken,
            subdomain,
            query,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<Customer>> GetCustomersInternalAsync(
        string accessToken,
        string query = "",
        CancellationToken cancellationToken = default)
    {
        return await _customerService.GetCustomersAsync(
            accessToken,
            _options.ServerIntegrationSubdomain,
            query,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<Customer>> GetCustomersAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<EntityType> linkedEntityTypes,
        string query = "",
        CancellationToken cancellationToken = default)
    {
        return await _customerService.GetCustomersAsync(
            accessToken,
            subdomain,
            linkedEntityTypes,
            query,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<Customer>> GetCustomersInternalAsync(
        string accessToken,
        IReadOnlyCollection<EntityType> linkedEntityTypes,
        string query = "",
        CancellationToken cancellationToken = default)
    {
        return await _customerService.GetCustomersAsync(
            accessToken,
            _options.ServerIntegrationSubdomain,
            linkedEntityTypes,
            query,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<Customer>> AddCustomersAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<AddCustomerRequest> requests,
        CancellationToken cancellationToken = default)
    {
        return await _customerService.AddCustomersAsync(
            accessToken,
            subdomain,
            requests,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<Customer>> AddCustomersInternalAsync(
        string accessToken,
        IReadOnlyCollection<AddCustomerRequest> requests,
        CancellationToken cancellationToken = default)
    {
        return await _customerService.AddCustomersAsync(
            accessToken,
            _options.ServerIntegrationSubdomain,
            requests,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<Customer>> UpdateCustomersAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<UpdateCustomerRequest> requests,
        CancellationToken cancellationToken = default)
    {
        return await _customerService.UpdateCustomersAsync(
            accessToken,
            subdomain,
            requests,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<Customer>> UpdateCustomersInternalAsync(
        string accessToken,
        IReadOnlyCollection<UpdateCustomerRequest> requests,
        CancellationToken cancellationToken = default)
    {
        return await _customerService.UpdateCustomersAsync(
            accessToken,
            _options.ServerIntegrationSubdomain,
            requests,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<AccountResponse> GetAccountAsync(
        string accessToken,
        string subdomain,
        CancellationToken cancellationToken = default)
    {
        return await _accountService.GetAccountAsync(
            accessToken, subdomain, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<AccountResponse> GetAccountInternalAsync(
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        return await _accountService.GetAccountAsync(
            accessToken, _options.ServerIntegrationSubdomain, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<User>> GetUsersAsync(
        string accessToken,
        string subdomain,
        CancellationToken cancellationToken = default)
    {
        return await _userService.GetUsersAsync(
            accessToken,
            subdomain,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<WidgetResponse> GetWidgetAsync(
        string accessToken,
        string subdomain,
        string widgetCode,
        CancellationToken cancellationToken = default)
    {
        return await _accountService.GetWidgetAsync(
            accessToken, subdomain, widgetCode, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<Contact>> GetContactsAsync(
        string accessToken,
        string subdomain,
        string query = "",
        CancellationToken cancellationToken = default)
    {
        return await _contactService.GetContactsAsync(
            accessToken,
            subdomain,
            query,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<Contact>> GetContactsAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<EntityType> linkedEntityTypes,
        string query = "",
        CancellationToken cancellationToken = default)
    {
        return await _contactService.GetContactsAsync(
            accessToken,
            subdomain,
            linkedEntityTypes,
            query,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<Contact>> GetContactsInternalAsync(
        string accessToken,
        string query = "",
        CancellationToken cancellationToken = default)
    {
        return await _contactService.GetContactsAsync(
            accessToken,
            _options.ServerIntegrationSubdomain,
            query,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<Contact>> GetContactsInternalAsync(
        string accessToken,
        IReadOnlyCollection<EntityType> linkedEntityTypes,
        string query = "",
        CancellationToken cancellationToken = default)
    {
        return await _contactService.GetContactsAsync(
            accessToken,
            _options.ServerIntegrationSubdomain,
            linkedEntityTypes,
            query,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<Contact> GetContactByIdAsync(
        string accessToken,
        string subdomain,
        int contactId,
        CancellationToken cancellationToken = default)
    {
        return await _contactService.GetContactByIdAsync(
           accessToken,
           subdomain,
           contactId,
           cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<Contact> GetContactByIdAsync(
        string accessToken,
        string subdomain,
        int contactId,
        IReadOnlyCollection<EntityType> linkedEntityTypes,
        CancellationToken cancellationToken = default)
    {
        return await _contactService.GetContactByIdAsync(
           accessToken,
           subdomain,
           contactId,
           linkedEntityTypes,
           cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<Contact> GetContactByIdInternalAsync(
        string accessToken,
        int contactId,
        CancellationToken cancellationToken = default)
    {
        return await _contactService.GetContactByIdAsync(
           accessToken,
            _options.ServerIntegrationSubdomain,
           contactId,
           cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<Contact> GetContactByIdInternalAsync(
        string accessToken,
        int contactId,
        IReadOnlyCollection<EntityType> linkedEntityTypes,
        CancellationToken cancellationToken = default)
    {
        return await _contactService.GetContactByIdAsync(
           accessToken,
            _options.ServerIntegrationSubdomain,
           contactId,
           linkedEntityTypes,
           cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<Contact>> AddContactsAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<AddContactRequest> requests,
        CancellationToken cancellationToken = default)
    {
        return await _contactService.AddContactsAsync(
            accessToken,
            subdomain,
            requests,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<Contact>> AddContactsInternalAsync(
        string accessToken,
        IReadOnlyCollection<AddContactRequest> requests,
        CancellationToken cancellationToken = default)
    {
        return await _contactService.AddContactsAsync(
            accessToken,
            _options.ServerIntegrationSubdomain,
            requests,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<Contact>> UpdateContactsAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<UpdateContactRequest> requests,
        CancellationToken cancellationToken = default)
    {
        return await _contactService.UpdateContactsAsync(
            accessToken,
            subdomain,
            requests,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<Contact>> UpdateContactsInternalAsync(
        string accessToken,
        IReadOnlyCollection<UpdateContactRequest> requests,
        CancellationToken cancellationToken = default)
    {
        return await _contactService.UpdateContactsAsync(
            accessToken,
            _options.ServerIntegrationSubdomain,
            requests,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<Transaction>> GetTransactionsAsync(
        string accessToken,
        string subdomain,
        int customerId,
        CancellationToken cancellationToken = default)
    {
        return await _transactionService.GetTransactionsAsync(
            accessToken,
            subdomain,
            customerId,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<Transaction>> GetTransactionsInternalAsync(
        string accessToken,
        int customerId,
        CancellationToken cancellationToken = default)
    {
        return await _transactionService.GetTransactionsAsync(
            accessToken,
            _options.ServerIntegrationSubdomain,
            customerId,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<Transaction>> AddTransactionsAsync(
        string accessToken,
        string subdomain,
        int customerId,
        IReadOnlyCollection<AddTransactionRequest> requests,
        CancellationToken cancellationToken = default)
    {
        return await _transactionService.AddTransactionsAsync(
            accessToken,
            subdomain,
            customerId,
            requests,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<Transaction>> AddTransactionsInternalAsync(
        string accessToken,
        int customerId,
        IReadOnlyCollection<AddTransactionRequest> requests,
        CancellationToken cancellationToken = default)
    {
        return await _transactionService.AddTransactionsAsync(
            accessToken,
            _options.ServerIntegrationSubdomain,
            customerId,
            requests,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<CustomField>> GetCustomFieldsAsync(
        string accessToken,
        string subdomain,
        EntityType entityType,
        CancellationToken cancellationToken = default)
    {
        return await _customFieldService.GetCustomFieldsAsync(
            accessToken,
            subdomain,
            entityType,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<CustomField>> GetCustomFieldsInternalAsync(
        string accessToken,
        EntityType entityType,
        CancellationToken cancellationToken = default)
    {
        return await _customFieldService.GetCustomFieldsAsync(
            accessToken,
            _options.ServerIntegrationSubdomain,
            entityType,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<LeadStatus>> GetLeadStatusesAsync(
        string accessToken,
        string subdomain,
        CancellationToken cancellationToken = default)
    {
        return await _pipelineService.GetLeadStatusesAsync(
            accessToken,
            subdomain,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<LeadStatus>> GetLeadStatusesInternalAsync(
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        return await _pipelineService.GetLeadStatusesAsync(
            accessToken,
            _options.ServerIntegrationSubdomain,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<Note>> GetNotesAsync(
        string accessToken,
        string subdomain,
        EntityType entityType,
        NoteType noteType,
        int? entityId = null,
        CancellationToken cancellationToken = default)
    {
        return await _noteService.GetNotesAsync(
            accessToken,
            subdomain,
            entityType,
            noteType,
            entityId,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<Note>> GetNotesInternalAsync(
        string accessToken,
        EntityType entityType,
        NoteType noteType,
        int? entityId = null,
        CancellationToken cancellationToken = default)
    {
        return await _noteService.GetNotesAsync(
            accessToken,
            _options.ServerIntegrationSubdomain,
            entityType,
            noteType,
            entityId,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<Note>> AddNotesAsync(
        string accessToken,
        string subdomain,
        EntityType entityType,
        IReadOnlyCollection<AddNoteRequest> requests,
        CancellationToken cancellationToken = default)
    {
        return await _noteService.AddNotesAsync(
            accessToken,
            subdomain,
            entityType,
            requests,
            cancellationToken
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyCollection<Note>> AddNotesInternalAsync(
        string accessToken,
        EntityType entityType,
        IReadOnlyCollection<AddNoteRequest> requests,
        CancellationToken cancellationToken = default)
    {
        return await _noteService.AddNotesAsync(
            accessToken,
            _options.ServerIntegrationSubdomain,
            entityType,
            requests,
            cancellationToken
        ).ConfigureAwait(false);
    }
}
