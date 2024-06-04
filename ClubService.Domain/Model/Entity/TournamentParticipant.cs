namespace ClubService.Domain.Model.Entity;

public class TournamentParticipant(Guid participantId)
{
    public Guid ParticipantId { get; } = participantId;
}