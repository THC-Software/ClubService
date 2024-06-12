namespace ClubService.Domain.Repository;

public interface IMailService
{
    Task Send(string email, string subject, string body);
}