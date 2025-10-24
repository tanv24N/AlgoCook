
using Microsoft.EntityFrameworkCore;

namespace Api;

public class AppDbContext : DbContext
{
    public DbSet<Favorite> Favorites => Set<Favorite>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Favorite>()
            .HasIndex(f => f.RecipeId)
            .IsUnique(false);
    }
}

public class Favorite
{
    public int Id { get; set; }
    public string RecipeId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? ThumbUrl { get; set; }
    public DateTime AddedUtc { get; set; } = DateTime.UtcNow;
}

public record CuisineDto(string Name);
public record RecipeCardDto(string Id, string Title, string? Thumbnail);
public record RecipeDetailDto(
    string Id,
    string Title,
    string Category,
    string Area,
    string Instructions,
    string? Thumbnail,
    List<string> Ingredients,
    List<string> Measures,
    List<string> Tags,
    string? YoutubeUrl,
    string? SourceUrl
);
