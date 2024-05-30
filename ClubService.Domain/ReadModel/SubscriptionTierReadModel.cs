﻿using ClubService.Domain.Model.ValueObject;

namespace ClubService.Domain.ReadModel;

public class SubscriptionTierReadModel(SubscriptionTierId id, string name, int maxMemberCount)
{
    public SubscriptionTierId Id { get; private set; } = id;
    public string Name { get; private set; } = name;
    public int MaxMemberCount { get; private set; } = maxMemberCount;
}