namespace ClubService.Application.Api;

public interface IDeleteMemberService
{
    Task<string> DeleteMember(string id);
}