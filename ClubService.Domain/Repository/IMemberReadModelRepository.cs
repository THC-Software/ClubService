using ClubService.Domain.ReadModel;

namespace ClubService.Domain.Repository;

public interface IMemberReadModelRepository
{
    Task Add(MemberReadModel memberReadModel);
    Task Update();
    Task Delete(MemberReadModel memberReadModel);
    Task<MemberReadModel?> GetMemberById(Guid id);
    Task<List<MemberReadModel>> GetMembersByTennisClubId(Guid tennisClubId);
    Task<MemberReadModel?> GetMemberByTennisClubIdAndUsername(Guid tennisClubId, string username);
}