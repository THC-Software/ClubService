using ClubService.Application.Dto.Enums;
using ClubService.Domain.Model.Enum;

namespace ClubService.Application.EventHandlers
{
    public static class EnumExtensions
    {
        public static UserStatus ToUserStatus(this MemberStatus memberStatus)
        {
            switch (memberStatus)
            {
                case MemberStatus.ACTIVE:
                    return UserStatus.ACTIVE;
                case MemberStatus.LOCKED:
                    return UserStatus.LOCKED;
                case MemberStatus.DELETED:
                    return UserStatus.DELETED;
                default:
                    throw new ArgumentOutOfRangeException(nameof(memberStatus), memberStatus, null);
            }
        }

        public static UserStatus ToUserStatus(this AdminStatus adminStatus)
        {
            switch (adminStatus)
            {
                case AdminStatus.ACTIVE:
                    return UserStatus.ACTIVE;
                case AdminStatus.DELETED:
                    return UserStatus.DELETED;
                default:
                    throw new ArgumentOutOfRangeException(nameof(adminStatus), adminStatus, null);
            }
        }
    }
}