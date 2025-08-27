namespace AIDocSummarizer.Services;

public interface IAiSummarizer
{
    Task<string> SummarizeAsync(string text, CancellationToken ct = default);
}