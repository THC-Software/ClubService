namespace ClubService.Application.Api;

public interface IUpdateMemberService
{
    Task<string> LockMember(string id);
    Task<string> UnlockMember(string id);
    Task<string> ChangeFullName(string id, string firstName, string lastName);
    Task<string> ChangeEmail(string id, string email);
}