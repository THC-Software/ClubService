namespace ClubService.Domain.Repository;

public interface IProcessedEventRepository
{
    Task Add(Guid id);
    Task<List<Guid>> GetAllProcessedEvents();
}