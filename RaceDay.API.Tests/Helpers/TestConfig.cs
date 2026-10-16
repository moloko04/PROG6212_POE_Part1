using Microsoft.Extensions.Configuration;

namespace RaceDay.Api.Tests.Helpers;

public static class TestConfig
{
    public static IConfiguration GetConfiguration()
    {
        var settings = new Dictionary<string, string>
        {
            { "Jwt:Key", "ThisIsMySecretKeyForRaceDayApi1234567890" },
            { "Jwt:Issuer", "RaceDayApi" },
            { "Jwt:Audience", "RaceDayClient" }
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(settings!)
            .Build();
    }
}