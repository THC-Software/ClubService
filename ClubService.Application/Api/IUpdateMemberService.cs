namespace ClubService.Application.Api;

public interface IUpdateMemberService
{
    Task<string> LockMember(string id);
    Task<string> UnlockMember(string id);
}