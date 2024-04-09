namespace ClubService.Application.Dto;

public class NotificationDto(string title, string text)
{
    public string Title { get; } = title;
    public string Text { get; } = text;
}