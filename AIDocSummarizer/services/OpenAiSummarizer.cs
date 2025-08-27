using System.Text.Json.Nodes;

namespace AIDocSummarizer.Services;

public class OpenAiSummarizer(HttpClient http, IConfiguration config) : IAiSummarizer
{
    public async Task<string> SummarizeAsync(string text, CancellationToken ct = default)
    {
        // Summarize via OpenAI
        var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? config["OPENAI_API_KEY"];
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("OPENAI_API_KEY not set.");

        http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

        var body = new
        {
            model = "gpt-4o-mini",
            messages = new object[]
            {
            new { role = "system", content = "You summarize documents for a public sector DMS. Return 5 concise bullet points." },
            new { role = "user", content = text.Length > 6000 ? text[..6000] : text }
            },
            temperature = 0.2
        };

        var aiResp = await http.PostAsJsonAsync("https://api.openai.com/v1/chat/completions", body);
        if (!aiResp.IsSuccessStatusCode)
        {
            var err = await aiResp.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"OpenAi error: {err}");
        }
        var node = await aiResp.Content.ReadFromJsonAsync<JsonNode>();

        return
            node?["choices"]?[0]?["message"]?["content"]?.GetValue<string>()
            ?? "(no summary)";
    }
}