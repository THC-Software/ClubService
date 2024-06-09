using ClubService.Domain.ReadModel;

namespace ClubService.Domain.Repository;

public interface IProcessedEventRepository
{
    Task Add(ProcessedEvent processedEvent);
    Task<List<ProcessedEvent>> GetAllProcessedEvents();
}