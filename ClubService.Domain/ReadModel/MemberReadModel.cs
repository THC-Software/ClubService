﻿using ClubService.Domain.Event.Member;
using ClubService.Domain.Model.Enum;
using ClubService.Domain.Model.ValueObject;

namespace ClubService.Domain.ReadModel;

public class MemberReadModel
{
    public MemberId MemberId { get; } = null!;
    public FullName Name { get; } = null!;
    public string Email { get; } = null!;
    public TennisClubId TennisClubId { get; } = null!;
    public MemberStatus Status { get; }
    
    // needed by efcore
    private MemberReadModel()
    {
    }
    
    private MemberReadModel(
        MemberId memberId,
        FullName name,
        string email,
        TennisClubId tennisClubId,
        MemberStatus status)
    {
        MemberId = memberId;
        Name = name;
        Email = email;
        TennisClubId = tennisClubId;
        Status = status;
    }
    
    public static MemberReadModel FromDomainEvent(MemberRegisteredEvent memberRegisteredEvent)
    {
        return new MemberReadModel(
            memberRegisteredEvent.MemberId,
            memberRegisteredEvent.Name,
            memberRegisteredEvent.Email,
            memberRegisteredEvent.TennisClubId,
            memberRegisteredEvent.Status
        );
    }
}