using ClubService.Domain.Model.Entity;

namespace ClubService.Domain.Event.Tournament;

public class TournamentConfirmedEvent(
    Guid id,
    bool isCanceled,
    int maximumParticipants,
    HashSet<TournamentCourt> courts,
    HashSet<TournamentDay> days,
    HashSet<TournamentParticipant> participants) : ITournamentDomainEvent
{
    public Guid Id { get; } = id;
    public bool IsCanceled { get; } = isCanceled;
    public int MaximumParticipants { get; } = maximumParticipants;
    public HashSet<TournamentCourt> Courts { get; } = courts;
    private HashSet<TournamentDay> Days { get; } = days;
    private HashSet<TournamentParticipant> Participants { get; } = participants;
}