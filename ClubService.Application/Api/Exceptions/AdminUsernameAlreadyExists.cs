﻿namespace ClubService.Application.Api.Exceptions;

public class AdminUsernameAlreadyExists(string username, string tennisClubName, Guid tennisClubId) : Exception
{
    public override string Message { get; } =
        $"Admin username '{username}' already exists in tennis club '{tennisClubName}' ({tennisClubId}).";
}