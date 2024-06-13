using ClubService.Domain.Repository;
using Moq;

namespace ClubService.IntegrationTests;

public class TestBase
{
    protected readonly IEventRepository EventRepository = IntegrationTestSetup.EventRepository;
    protected readonly HttpClient HttpClient = IntegrationTestSetup.HttpClient;

    protected readonly Mock<IAdminReadModelRepository> MockAdminReadModelRepository =
        IntegrationTestSetup.MockAdminReadModelRepository;

    protected readonly Mock<IMemberReadModelRepository> MockMemberReadModelRepository =
        IntegrationTestSetup.MockMemberReadModelRepository;

    protected readonly Mock<ISubscriptionTierReadModelRepository> MockSubscriptionTierReadModelRepository =
        IntegrationTestSetup.MockSubscriptionTierReadModelRepository;

    protected readonly Mock<ITennisClubReadModelRepository> MockTennisClubReadModelRepository =
        IntegrationTestSetup.MockTennisClubReadModelRepository;

    [SetUp]
    public async Task Setup()
    {
        await IntegrationTestSetup.EventStoreDbContext.Database.EnsureCreatedAsync();
    }

    [TearDown]
    public async Task TearDown()
    {
        await IntegrationTestSetup.EventStoreDbContext.Database.EnsureDeletedAsync();
    }
}