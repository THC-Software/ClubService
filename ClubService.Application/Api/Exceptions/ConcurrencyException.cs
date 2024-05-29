namespace ClubService.Application.Api.Exceptions;

public class ConcurrencyException(string? message, Exception? innerException) : Exception(message, innerException);