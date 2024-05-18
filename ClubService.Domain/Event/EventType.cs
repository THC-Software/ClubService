namespace ClubService.Domain.Event;

public enum EventType
{
    TENNIS_CLUB_REGISTERED,
    MEMBER_ACCOUNT_CREATED,
    MEMBER_ACCOUNT_LIMIT_EXCEEDED,
    MEMBER_ACCOUNT_DELETED,
    ADMIN_ACCOUNT_REGISTERED,
    ADMIN_ACCOUNT_DELETED,
    TENNIS_CLUB_SUBSCRIPTION_TIER_CHANGED,
    MEMBER_ACCOUNT_LOCKED,
    MEMBER_ACCOUNT_UNLOCKED,
    MEMBER_ACCOUNT_UPDATED,
    TENNIS_CLUB_LOCKED,
    TENNIS_CLUB_UNLOCKED,
    SUBSCRIPTION_TIER_CREATED
}