using ClubService.Domain.Model.Enum;

namespace ClubService.Domain.Model.Entity;

public class Tournament(
    Guid id,
    Guid clubId,
    bool isCanceled,
    string name,
    string description,
    int maximumParticipants,
    HashSet<TournamentCourt> courts,
    HashSet<TournamentDay> days,
    HashSet<TournamentParticipant> participants,
    int version,
    TournamentStatus status)
{
    public Guid Id { get; } = id;
    public Guid ClubId { get; } = clubId;
    public bool IsCanceled { get; } = isCanceled;
    public string Name { get; } = name;
    public string Description { get; } = description;
    public int MaximumParticipants { get; } = maximumParticipants;
    public HashSet<TournamentCourt> Courts { get; } = courts;
    public HashSet<TournamentDay> Days { get; } = days;
    public HashSet<TournamentParticipant> Participants { get; } = participants;
    public int Version { get; } = version;
    public TournamentStatus Status { get; } = status;
}