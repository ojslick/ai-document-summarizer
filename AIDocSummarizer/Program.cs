using AIDocSummarizer.Data;
using AIDocSummarizer.Models;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using AIDocSummarizer.Services;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDirectoryBrowser();

var dbPassword = Environment.GetEnvironmentVariable("MSSQL_SA_PASSWORD");
var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "sa";
var dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "AIDocSummarizerDb";

var useInMemory =
    builder.Environment.IsEnvironment("Testing") ||
    builder.Configuration.GetValue<bool>("UseInMemory");

builder.Services.AddDbContext<AppDbContext>(opts =>
{
    if (useInMemory)
    {
        opts.UseInMemoryDatabase("test-db");
    }
    else
    {
        opts.UseSqlServer($"Server=localhost,1433;Database={dbName};User ID={dbUser};Password={dbPassword};Encrypt=True;TrustServerCertificate=True");
    }
});



// Allow larger uploads
builder.Services.Configure<FormOptions>(o => { o.MultipartBodyLengthLimit = 100_000_000; });

builder.Services.AddHttpClient<IAiSummarizer, OpenAiSummarizer>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/documents", async (AppDbContext db) =>
{
    var items = await db.Documents
        .OrderByDescending(d => d.CreatedUtc)
        .Select(d => new { d.Id, d.FileName, d.MimeType, d.CreatedUtc, d.Summary })
        .ToListAsync();
    return Results.Ok(items);
});

app.MapGet("/api/documents/{id:int}", async (int id, AppDbContext db) =>
{
    var doc = await db.Documents.FindAsync(id);
    return doc is null ? Results.NotFound() : Results.Ok(doc);
});

app.MapPost("/api/upload", async (HttpRequest req, AppDbContext db, IAiSummarizer ai) =>
{
    if (!req.HasFormContentType) return Results.BadRequest("Multipart form required.");
    var form = await req.ReadFormAsync();
    var file = form.Files.FirstOrDefault();
    if (file is null || file.Length == 0) return Results.BadRequest("No file provided.");

    using var stream = file.OpenReadStream();
    var text = await DocumentTextExtractor.ExtractAsync(file.FileName, stream);
    if (string.IsNullOrWhiteSpace(text))
        return Results.BadRequest("No readable text found.");

    var summary = await ai.SummarizeAsync(text);

    var doc = new Document
    {
        FileName = file.FileName,
        MimeType = file.ContentType ?? "",
        ContentText = text,
        Summary = summary
    };
    db.Documents.Add(doc);
    await db.SaveChangesAsync();

    return Results.Ok(new { id = doc.Id, summary });
});

app.Run();

public partial class Program { }