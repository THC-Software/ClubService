namespace ClubService.Application.Api.Exceptions;

public class MemberNotFoundException(string? message) : Exception(message);