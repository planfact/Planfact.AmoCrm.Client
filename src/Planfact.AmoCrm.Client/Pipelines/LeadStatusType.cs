
namespace Planfact.AmoCrm.Client.Pipelines;

/// <summary>
/// Виды статусов сделок в amoCRM
/// </summary>
public enum LeadStatusType
{
    /// <summary>
    /// Обычный статус
    /// </summary>
    Regular = 0,
    /// <summary>
    /// Системный статус, относящийся к неразобранным сделкам
    /// </summary>
    Unsorted = 1
}
