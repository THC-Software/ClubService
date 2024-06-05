namespace ClubService.Domain.Model.Entity;

public record TournamentDay(DateOnly Day, TimeOnly StartTime, TimeOnly EndTime);