namespace ClubService.Application.Dto;

public class MemberDto(string memberId, string firstName, string lastName, string email, bool isLocked)
{
    public string MemberId { get; } = memberId;
    public string FirstName { get; } = firstName;
    public string LastName { get; } = lastName;
    public string Email { get; } = email;
    public bool IsLocked { get; } = isLocked;
}