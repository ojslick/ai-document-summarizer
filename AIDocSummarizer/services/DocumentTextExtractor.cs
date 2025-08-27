using UglyToad.PdfPig;
using Xceed.Words.NET;

namespace AIDocSummarizer.Services;

public static class DocumentTextExtractor
{
    public static async Task<string> ExtractAsync(string fileName, Stream stream)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();

        if (ext == ".txt")
        {
            using var reader = new StreamReader(stream);
            return (await reader.ReadToEndAsync()).Trim();
        }
        if (ext == ".pdf")
        {
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            ms.Position = 0;
            using var pdf = PdfDocument.Open(ms);
            var sb = new System.Text.StringBuilder();
            foreach (var page in pdf.GetPages()) sb.AppendLine(page.Text);
            return sb.ToString().Trim();
        }
        if (ext == ".docx")
        {
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            ms.Position = 0;
            using var docx = DocX.Load(ms);
            return docx.Text.Trim();
        }

        throw new NotSupportedException("Only .txt, .pdf, .docx are supported.");
    }
}