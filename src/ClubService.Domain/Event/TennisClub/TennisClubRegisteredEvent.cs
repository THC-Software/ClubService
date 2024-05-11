using ClubService.Domain.Model.ValueObject;

namespace ClubService.Domain.Event.TennisClub;

public class TennisClubRegisteredEvent(
    TennisClubId tennisClubId,
    string name,
    bool isLocked,
    SubscriptionTierId subscriptionTierId,
    List<MemberId> memberIds) : ITennisClubDomainEvent
{
    public TennisClubId TennisClubId { get; private set; } = tennisClubId;
    public string Name { get; private set; } = name;
    public bool IsLocked { get; private set; } = isLocked;
    public SubscriptionTierId SubscriptionTierId { get; private set; } = subscriptionTierId;
    public List<MemberId> MemberIds { get; private set; } = memberIds;
}