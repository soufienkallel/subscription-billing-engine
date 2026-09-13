using System.Net;
using Application.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ApiTests;

public class DependencyInjectionTests
{
    [Fact]
    public async Task Health_Endpoint_Returns_Ok()
    {
        await using var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => builder.UseEnvironment("Development"));
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Development_Environment_Loads_BillingOptions_From_AppsettingsDevelopment()
    {
        await using var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => builder.UseEnvironment("Development"));

        var options = factory.Services.GetRequiredService<IOptions<BillingOptions>>();

        Assert.Equal("EUR", options.Value.DefaultCurrency);
    }
}
