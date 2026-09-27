using APIKeyHunter.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace APIKeyHunter.Data.Context;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<ApiKeyModel> ApiKeys => Set<ApiKeyModel>();
    public DbSet<TokenModel> Tokens => Set<TokenModel>();
    public DbSet<ValidationQueueItem> ValidationQueue => Set<ValidationQueueItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApiKeyModel>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Provider).IsRequired().HasMaxLength(100);
            e.Property(x => x.KeyPreview).IsRequired().HasMaxLength(200);
        });

        modelBuilder.Entity<TokenModel>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(200);
            e.Property(x => x.Preview).IsRequired().HasMaxLength(100);
            e.Property(x => x.Provider).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<ValidationQueueItem>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Provider).IsRequired().HasMaxLength(100);
            e.Property(x => x.KeyPreview).IsRequired().HasMaxLength(200);
        });
    }
}
