using ClubService.Domain.ReadModel;

namespace ClubService.Domain.Repository;

public interface IEmailOutboxRepository
{
    Task Add(EmailMessage emailMessage);
    Task<List<EmailMessage>> GetAllEmails();
    Task RemoveEmails(List<EmailMessage> emails);
}