using Reliable.HttpClient;

using Planfact.AmoCrm.Client.Common;

namespace Planfact.AmoCrm.Client.Notes;

/// <summary>
/// Сервис примечаний в amoCRM
/// </summary>
public sealed class AmoCrmNoteService(
    IHttpClientAdapter httpClient,
    AmoCrmUriBuilderFactory uriBuilderFactory,
    ILogger<AmoCrmNoteService> logger) : AmoCrmServiceBase(httpClient, logger), IAmoCrmNoteService
{
    private readonly AmoCrmUriBuilderFactory _uriBuilderFactory = uriBuilderFactory;
    private readonly ILogger<AmoCrmNoteService> _logger = logger;

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<Note>> GetNotesAsync(
        string accessToken,
        string subdomain,
        EntityType entityType,
        NoteType noteType,
        int? entityId = null,
        CancellationToken cancellationToken = default)
    {
        ValidateCredentials(accessToken, subdomain);

        var entityTypeName = EntityTypeConverter.ToString(entityType);
        UriBuilder uriBuilder = _uriBuilderFactory.CreateForNotes(subdomain, entityTypeName);
        var noteTypeName = NoteTypeConverter.ToString(noteType);

        _logger.LogDebug("Загрузка примечений {NoteType} из аккаунта {Subdomain}. Тип сущности {EntityType}", noteTypeName, subdomain, entityTypeName);

        uriBuilder.Query = $"filter[note_type]={noteTypeName}";

        if (entityId.HasValue)
        {
            uriBuilder.Query += $"&filter[entity_id]={entityId.Value}";
        }

        IAsyncEnumerable<EntitiesResponse> paginationTask = GetPaginatedAsync<EntitiesResponse>(
            uriBuilder.Uri,
            accessToken,
            cancellationToken
        );

        IReadOnlyCollection<Note> response = await CollectPaginatedEntitiesAsync(
            paginationTask,
            r => r.Embedded?.Notes ?? [],
            subdomain,
            OperationDescriptions.GetNotes,
            cancellationToken
        ).ConfigureAwait(false);

        _logger.LogDebug(
            "Загрузка примечений {NoteType} из аккаунта {Subdomain} успешно завершена. Тип сущности {EntityType}. Получено {NotesCount} примечаний",
            noteTypeName,
            subdomain,
            entityTypeName,
            response.Count
        );

        return response;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<Note>> AddNotesAsync(
        string accessToken,
        string subdomain,
        EntityType entityType,
        IReadOnlyCollection<AddNoteRequest> requests,
        CancellationToken cancellationToken = default)
    {
        var entityTypeName = EntityTypeConverter.ToString(entityType);

        _logger.LogDebug("Добавление примечений в аккаунт {Subdomain}. Тип сущности {EntityType}", subdomain, entityTypeName);

        ValidateCredentials(accessToken, subdomain);

        ArgumentNullException.ThrowIfNull(requests);

        if (requests.Count == 0)
        {
            return [];
        }

        UriBuilder uriBuilder = _uriBuilderFactory.CreateForNotes(subdomain, entityTypeName);

        IAsyncEnumerable<EntitiesResponse> batchTask = PostInBatchesAsync<AddNoteRequest, EntitiesResponse>(
            uriBuilder.Uri,
            accessToken,
            requests,
            cancellationToken
        );

        IReadOnlyCollection<Note> response = await CollectPaginatedEntitiesAsync(
            batchTask,
            r => r.Embedded?.Notes ?? [],
            subdomain,
            OperationDescriptions.AddNotes,
            cancellationToken
        ).ConfigureAwait(false);

        _logger.LogDebug(
            "Добавление примечений в аккаунт {Subdomain} успешно завершено. Тип сущности {EntityType}. Добавлено {AddedNotesCount} примечений из {RequestedNotesCount}",
            subdomain,
            entityTypeName,
            response.Count,
            requests.Count
        );

        return response;
    }
}
