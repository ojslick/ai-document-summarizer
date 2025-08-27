using AIDocSummarizer.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using RichardSzalay.MockHttp;

public class AiSummarizerTests
{
    [Fact]
    public async Task SummarizeAsync_Returns_Parsed_Text_From_OpenAI()
    {
        // Arrange
        var mock = new MockHttpMessageHandler();
        mock.When(HttpMethod.Post, "https://api.openai.com/v1/chat/completions")
            .Respond("application/json", """
            {
                "choices":[{"message":{"content":"• a\n• b\n• c\n• d\n• e"}}]
            }
            """);

        var httpClient = new HttpClient(mock);
        var inMemory = new Dictionary<string, string?> { ["OPENAI_API_KEY"] = "test-key" };
        var config = new ConfigurationBuilder().AddInMemoryCollection(inMemory!).Build();

        var svc = new OpenAiSummarizer(httpClient, config);

        // Act
        var result = await svc.SummarizeAsync("hello world");

        // Assert
        result.Should().Contain("• a").And.Contain("• e");
    }
}
