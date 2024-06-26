using ClubService.Domain.Event.TennisClub;
using ClubService.Domain.Model.Enum;
using ClubService.Domain.Model.ValueObject;

namespace ClubService.Domain.ReadModel;

public class TennisClubReadModel
{
    public TennisClubId TennisClubId { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public SubscriptionTierId SubscriptionTierId { get; private set; } = null!;
    public TennisClubStatus Status { get; private set; }
    public int MemberCount { get; private set; }
    
    private TennisClubReadModel()
    {
    }
    
    private TennisClubReadModel(
        TennisClubId tennisClubId,
        string name,
        SubscriptionTierId subscriptionTierId,
        TennisClubStatus status,
        int memberCount)
    {
        TennisClubId = tennisClubId;
        Name = name;
        SubscriptionTierId = subscriptionTierId;
        Status = status;
        MemberCount = memberCount;
    }
    
    public static TennisClubReadModel FromDomainEvent(TennisClubRegisteredEvent tennisClubRegisteredEvent)
    {
        return new TennisClubReadModel(
            tennisClubRegisteredEvent.TennisClubId,
            tennisClubRegisteredEvent.Name,
            tennisClubRegisteredEvent.SubscriptionTierId,
            tennisClubRegisteredEvent.Status,
            0
        );
    }
    
    public void Lock()
    {
        Status = TennisClubStatus.LOCKED;
    }
    
    public void Unlock()
    {
        Status = TennisClubStatus.ACTIVE;
    }
    
    public void ChangeName(string name)
    {
        Name = name;
    }
    
    public void ChangeSubscriptionTier(SubscriptionTierId subscriptionTierId)
    {
        SubscriptionTierId = subscriptionTierId;
    }
    
    public void IncreaseMemberCount()
    {
        MemberCount++;
    }
    
    public void DecreaseMemberCount()
    {
        if (MemberCount <= 0)
        {
            return;
        }
        
        MemberCount--;
    }
}