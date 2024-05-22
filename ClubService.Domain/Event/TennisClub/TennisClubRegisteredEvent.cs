using ClubService.Domain.Model.Enum;
using ClubService.Domain.Model.ValueObject;

namespace ClubService.Domain.Event.TennisClub;

public class TennisClubRegisteredEvent(
    TennisClubId tennisClubId,
    string name,
    SubscriptionTierId subscriptionTierId,
    TennisClubStatus status) : ITennisClubDomainEvent
{
    public TennisClubId TennisClubId { get; private set; } = tennisClubId;
    public string Name { get; private set; } = name;
    public SubscriptionTierId SubscriptionTierId { get; private set; } = subscriptionTierId;
    public TennisClubStatus Status { get; private set; } = status;
}