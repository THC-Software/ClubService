namespace ClubService.Domain.Repository;

public interface IMailService
{
    void Send(string email, string subject, string body);
}