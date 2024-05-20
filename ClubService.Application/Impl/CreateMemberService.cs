using ClubService.Application.Api;
using ClubService.Application.Commands;
using ClubService.Domain.Model.Entity;
using ClubService.Domain.Repository;

namespace ClubService.Application.Impl;

public class CreateMemberService(IEventRepository eventRepository) : ICreateMemberService
{
    public async Task<string> CreateMember(MemberCreateCommand memberCreateCommand)
    {
        // TODO: add error handling
        // does tennisClubRegistered event exist?
        // is member limit reached? if yes create MemberLimitExceeded event if it does not exist yet?
        
        var member = Member.Create();
        
        var memberDomainEvents = member.ProcessMemberCreatedCommand(
            memberCreateCommand.FirstName,
            memberCreateCommand.LastName,
            memberCreateCommand.Email,
            memberCreateCommand.TennisClubId
        );
        
        foreach (var memberDomainEvent in memberDomainEvents)
        {
            member.Apply(memberDomainEvent);
            await eventRepository.Save(memberDomainEvent);
        }
        
        return member.MemberId.Id.ToString();
    }
}