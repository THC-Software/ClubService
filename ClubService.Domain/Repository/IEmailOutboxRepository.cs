using ClubService.Domain.ReadModel;

namespace ClubService.Domain.Repository;

public interface IEmailOutboxRepository
{
    Task Add(EmailMessage emailMessage);
    Task<List<EmailMessage>> GetAllEmailMessages();
    Task Delete(EmailMessage emailMessage);
}