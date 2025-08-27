namespace AIDocSummarizer.Models;

public class Document
{
    public int Id { get; set; }
    public string FileName { get; set; } = "";
    public string MimeType { get; set; } = "";
    public string ContentText { get; set; } = "";
    public string? Summary { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}