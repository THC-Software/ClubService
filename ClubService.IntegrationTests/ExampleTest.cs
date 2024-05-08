namespace ClubService.IntegrationTests;

[TestFixture]
public class ExampleTest : TestBase
{
    [Test]
    public async Task Get_EndpointsReturnSuccessAndCorrectContentType()
    {
        // Given
        var endpointUrl = "/api/v1.0/subscriptionTiers/";
        
        // When
        var response = await HttpClient.GetAsync(endpointUrl);
        
        // Then
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        Assert.That(response.Content.Headers.ContentType.ToString(), Is.EqualTo("application/json; charset=utf-8"));
    }
}