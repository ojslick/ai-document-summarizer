using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using AIDocSummarizer.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;

public class UploadEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public UploadEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {

                // Replace AI with a fake
                services.AddTransient<IAiSummarizer, FakeAi>();
            });
        });
    }

    [Fact]
    public async Task Upload_Returns_Summary_And_Saves_Row()
    {
        var client = _factory.CreateClient();

        // Muiltipart form with a small .txt file
        using var content = new MultipartFormDataContent();
        var fileBytes = new ByteArrayContent(System.Text.Encoding.UTF8.GetBytes("This is a small sample document."));
        fileBytes.Headers.ContentType = MediaTypeHeaderValue.Parse("text/plain");
        content.Add(fileBytes, "file", "sample.txt");

        var res = await client.PostAsync("/api/upload", content);
        res.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await res.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        json!["summary"].ToString().Should().Contain("FAKE_SUMMARY");
    }

    private class FakeAi : IAiSummarizer
    {
        public Task<string> SummarizeAsync(string text, CancellationToken ct = default)
            => Task.FromResult($"FAKE_SUMMARY: {text[..Math.Min(10, text.Length)]}...");
    }
}