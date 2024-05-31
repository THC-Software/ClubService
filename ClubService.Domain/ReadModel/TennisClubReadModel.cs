using ClubService.Domain.Event.TennisClub;
using ClubService.Domain.Model.Enum;
using ClubService.Domain.Model.ValueObject;

namespace ClubService.Domain.ReadModel;

public class TennisClubReadModel
{
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
    
    public TennisClubId TennisClubId { get; }
    public string Name { get; }
    public SubscriptionTierId SubscriptionTierId { get; }
    public TennisClubStatus Status { get; }
    public int MemberCount { get; }
    
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
}