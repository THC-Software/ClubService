using ClubService.Domain.Event.TennisClub;
using ClubService.Domain.Model.Enum;
using ClubService.Domain.Model.ValueObject;

namespace ClubService.Domain.ReadModel;

public class TennisClubReadModel
{
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
    
    public TennisClubId TennisClubId { get; private set; }
    public string Name { get; private set; }
    public SubscriptionTierId SubscriptionTierId { get; private set; }
    public TennisClubStatus Status { get; private set; }
    public int MemberCount { get; private set; }
    
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
    
    public void ChangeSubscriptionTier(SubscriptionTierId subscriptionTierId)
    {
        SubscriptionTierId = subscriptionTierId;
    }
}