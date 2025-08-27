using Microsoft.EntityFrameworkCore;
using AIDocSummarizer.Models;

namespace AIDocSummarizer.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) { }
    public DbSet<Document> Documents => Set<Document>();
}