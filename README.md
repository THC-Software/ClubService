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

## Login

In our application we distinguish between `Members` and `Admins`. Both are accounts that users of the application can login with.
Members use their email address as login while Admins have a username. Since it's theoretically possible that a user would be
member or admin of multiple tennis clubs, the login details (i.e. emails and usernames) only have to be unique within one tennis club.
This means for a login we have to provide a username (which can be an actual username or an email) and the associated tennis club.
In a real world the user wouldn't need to select the tennis club, as we imagined each tennis club to have it's own page, which 
would also allow for individual branding. 

Once the credentials are sent to the backend, we check if it is an Admin or a Member and load the correct user form the database by 
using the unique combination of username and tennis club Id. Once we loaded the correct user, we can verify the password in our login db.



## Domain Driven Design

DDD was used in this project. The following aggregates exist:
- Member
- Admin
- SystemOperator
- SubscriptionTier
- TennisClub
- UserPassword

Additionally, Event Sourcing was used, therefore those Entities all provide a `process` and a `apply` method. 
Process is used to create an Event from a Command, which can then be applied on this Entity. It's important that `apply` always
works if `process` works. 

## Event Sourcing

To give an example of events, lets take a look at the member events.
Each event for the member entities is prefixed with `Member`, all of those events implement the `IMemberDomainEvent` interface,
which in turn implements the `IDomainEvent` interface, which is implemented by all event interfaces.
Furthermore, we created a `DomainEnvelope`, which adds metadata to the Events such as a `Timestamp`, `EventType`, and `EntityType`.
This is also what gets persisted in the database. Since event sourcing is used, the database is append only, events cannot be deleted.

### Optimistic Locking

We implemented optimistic locking directly in the insert sql query.
```java
   private const string InsertSqlQuery = @"
        INSERT INTO ""DomainEvent""(""eventId"", ""entityId"", ""eventType"", ""entityType"", ""timestamp"", ""eventData"")
        SELECT @eventId, @entityId, @eventType, @entityType, @timestamp, @eventData
        WHERE (SELECT COUNT(*) FROM ""DomainEvent"" WHERE ""entityId"" = @entityId) = @expectedEventCount;
    ";
```
This is done by counting the number of events before the insert, if the `expectedEventCount` is not correct the whole query fails.
Therefore guaranteeing a consistent state.

### Debezium

Once an event gets persisted in the database Debezium publishes it to the redis stream.
The event-data of the published message is located in `payload.after`. Debezium also guarantees that the order of the published
messages is correct by using transaction log tailing.





