
using Api;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlite("Data Source=recipes.db");
});

builder.Services.AddHttpClient<TheMealDbClient>();

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("AllowAll", p => p
        .AllowAnyHeader()
        .AllowAnyMethod()
        .SetIsOriginAllowed(_ => true)
        .AllowCredentials());
});

var app = builder.Build();
app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/api/health", () => Results.Ok(new { status = "ok" }));

app.MapGet("/api/cuisines", async (TheMealDbClient client) =>
{
    var list = await client.GetCuisinesAsync();
    return Results.Ok(list);
});

app.MapGet("/api/recipes/by-cuisine", async (string cuisine, TheMealDbClient client) =>
{
    var list = await client.GetByCuisineAsync(cuisine);
    return Results.Ok(list);
});

app.MapGet("/api/recipes/by-ingredients", async (string ingredients, TheMealDbClient client) =>
{
    var ingList = ingredients.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    if (ingList.Length == 0) return Results.Ok(new List<RecipeCardDto>());
    List<RecipeCardDto>? acc = null;
    foreach (var ing in ingList)
    {
        var one = await client.GetBySingleIngredientAsync(ing);
        if (acc is null) acc = one;
        else
        {
            var ids = one.Select(x => x.Id).ToHashSet();
            acc = acc.Where(x => ids.Contains(x.Id)).ToList();
        }
        if (acc.Count == 0) break;
    }
    return Results.Ok(acc ?? new List<RecipeCardDto>());
});

app.MapGet("/api/recipes/{id}", async (string id, TheMealDbClient client) =>
{
    var details = await client.GetDetailsAsync(id);
    return details is null ? Results.NotFound() : Results.Ok(details);
});

app.MapGet("/api/favorites", async (AppDbContext db) =>
{
    var list = await db.Favorites
        .OrderByDescending(f => f.AddedUtc)
        .Select(f => new { f.Id, f.RecipeId, f.Title, f.ThumbUrl, f.AddedUtc })
        .ToListAsync();
    return Results.Ok(list);
});

app.MapPost("/api/favorites", async (Favorite f, AppDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(f.RecipeId) || string.IsNullOrWhiteSpace(f.Title))
        return Results.BadRequest("RecipeId and Title are required.");
    f.AddedUtc = DateTime.UtcNow;
    db.Favorites.Add(f);
    await db.SaveChangesAsync();
    return Results.Created($"/api/favorites/{f.Id}", f);
});

app.MapDelete("/api/favorites/{id:int}", async (int id, AppDbContext db) =>
{
    var found = await db.Favorites.FindAsync(id);
    if (found is null) return Results.NotFound();
    db.Favorites.Remove(found);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
