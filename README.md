# ClubService
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=THC-Software_ClubService&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=THC-Software_ClubService)
[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=THC-Software_ClubService&metric=ncloc)](https://sonarcloud.io/summary/new_code?id=THC-Software_ClubService)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=THC-Software_ClubService&metric=bugs)](https://sonarcloud.io/summary/new_code?id=THC-Software_ClubService)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=THC-Software_ClubService&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=THC-Software_ClubService)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=THC-Software_ClubService&metric=coverage)](https://sonarcloud.io/summary/new_code?id=THC-Software_ClubService)

Microservice to manage Tennis Clubs and their members.

## Domain Model
```mermaid
classDiagram
class TennisClub {
    id
    name
    isLocked
}

class Member {
    id
    name
    email
    isLocked
}

class Admin {
    id
    username
    name
}

class SubscriptionTier {
    id
    name
    maxMemberCount
}

TennisClub "*" --> "1" SubscriptionTier
TennisClub "1" --> "*" Member
Admin "*" --> "1" TennisClub
```