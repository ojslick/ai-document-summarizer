using System.Text;
using AIDocSummarizer.Services;
using FluentAssertions;

public class DocumentTextExtractorTests
{
    [Fact]
    public async Task Extract_Txt_works()
    {
        var bytes = Encoding.UTF8.GetBytes("Hello\nWorld");
        using var ms = new MemoryStream(bytes);
        var text = await DocumentTextExtractor.ExtractAsync("note.txt", ms);
        text.Should().Be("Hello\nWorld");
    }

    [Fact]
    public async Task Unsupported_Ext_Throws()
    {
        using var ms = new MemoryStream(Encoding.UTF8.GetBytes("x"));
        var act = async () => await DocumentTextExtractor.ExtractAsync("file.csv", ms);
        await act.Should().ThrowAsync<NotSupportedException>();
    }
}