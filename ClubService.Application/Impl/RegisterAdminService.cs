using ClubService.Application.Api;
using ClubService.Application.Api.Exceptions;
using ClubService.Application.Commands;
using ClubService.Domain.Event.TennisClub;
using ClubService.Domain.Model.Entity;
using ClubService.Domain.Model.ValueObject;
using ClubService.Domain.Repository;

namespace ClubService.Application.Impl;

public class RegisterAdminService(IEventRepository eventRepository) : IRegisterAdminService
{
    public async Task<string> RegisterAdmin(AdminRegisterCommand adminRegisterCommand)
    {
        if (string.IsNullOrWhiteSpace(adminRegisterCommand.Username) ||
            string.IsNullOrWhiteSpace(adminRegisterCommand.FirstName) ||
            string.IsNullOrWhiteSpace(adminRegisterCommand.LastName) || 
            string.IsNullOrWhiteSpace(adminRegisterCommand.TennisClubId))
        {
            throw new ArgumentException("You have to provide a username, firstname, lastname, and tennisclub id!");
        }
        
        var admin = new Admin();
        
        var tennisClubDomainEvents =
            eventRepository.GetEventsForEntity<ITennisClubDomainEvent>(new Guid(adminRegisterCommand.TennisClubId));
        
        //TODO: what if deleted?
        if (tennisClubDomainEvents.Count == 0)
        {
            throw new TennisClubNotFoundException($"Tennis Club '{adminRegisterCommand.TennisClubId}' not found!");
        }
        
        var adminDomainEvents = admin.ProcessAdminRegisteredCommand(adminRegisterCommand.Username,
            new FullName(adminRegisterCommand.FirstName, adminRegisterCommand.LastName),
            new TennisClubId(new Guid(adminRegisterCommand.TennisClubId)));
        
        foreach (var adminDomainEvent in adminDomainEvents)
        {
            admin.Apply(adminDomainEvent);
            await eventRepository.Save(adminDomainEvent);
        }
        
        return admin.AdminId.Id.ToString();
    }
}