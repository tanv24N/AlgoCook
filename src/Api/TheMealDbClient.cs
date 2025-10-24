
using System.Net.Http.Json;
using System.Text.Json;

namespace Api;

public class TheMealDbClient
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    public TheMealDbClient(HttpClient http)
    {
        _http = http;
        _http.BaseAddress = new Uri("https://www.themealdb.com/api/json/v1/1/");
    }

    public async Task<List<string>> GetCuisinesAsync()
    {
        var obj = await _http.GetFromJsonAsync<JsonElement>("list.php?a=list", _jsonOptions);
        var list = new List<string>();
        foreach (var item in obj.GetProperty("meals").EnumerateArray())
        {
            if (item.TryGetProperty("strArea", out var area))
                list.Add(area.GetString() ?? "");
        }
        return list;
    }

    public async Task<List<RecipeCardDto>> GetByCuisineAsync(string cuisine)
    {
        var obj = await _http.GetFromJsonAsync<JsonElement>($"filter.php?a={Uri.EscapeDataString(cuisine)}", _jsonOptions);
        var list = new List<RecipeCardDto>();
        if (obj.TryGetProperty("meals", out var meals) && meals.ValueKind == JsonValueKind.Array)
        {
            foreach (var m in meals.EnumerateArray())
            {
                var id = m.GetProperty("idMeal").GetString() ?? "";
                var title = m.GetProperty("strMeal").GetString() ?? "";
                var thumb = m.GetProperty("strMealThumb").GetString();
                list.Add(new RecipeCardDto(id, title, thumb));
            }
        }
        return list;
    }

    public async Task<List<RecipeCardDto>> GetBySingleIngredientAsync(string ingredient)
    {
        var obj = await _http.GetFromJsonAsync<JsonElement>($"filter.php?i={Uri.EscapeDataString(ingredient)}", _jsonOptions);
        var list = new List<RecipeCardDto>();
        if (obj.TryGetProperty("meals", out var meals) && meals.ValueKind == JsonValueKind.Array)
        {
            foreach (var m in meals.EnumerateArray())
            {
                var id = m.GetProperty("idMeal").GetString() ?? "";
                var title = m.GetProperty("strMeal").GetString() ?? "";
                var thumb = m.GetProperty("strMealThumb").GetString();
                list.Add(new RecipeCardDto(id, title, thumb));
            }
        }
        return list;
    }

    public async Task<RecipeDetailDto?> GetDetailsAsync(string id)
    {
        var obj = await _http.GetFromJsonAsync<JsonElement>($"lookup.php?i={Uri.EscapeDataString(id)}", _jsonOptions);
        if (!obj.TryGetProperty("meals", out var meals) || meals.ValueKind != JsonValueKind.Array || meals.GetArrayLength() == 0)
            return null;

        var m = meals[0];
        string getStr(string name) => m.TryGetProperty(name, out var v) && v.GetString() is string s ? s : "";

        var ingredients = new List<string>();
        var measures = new List<string>();
        for (int i = 1; i <= 20; i++)
        {
            var ing = getStr($"strIngredient{i}").Trim();
            var mea = getStr($"strMeasure{i}").Trim();
            if (!string.IsNullOrEmpty(ing))
            {
                ingredients.Add(ing);
                measures.Add(mea);
            }
        }

        var tags = (getStr("strTags") ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

        return new RecipeDetailDto(
            Id: getStr("idMeal"),
            Title: getStr("strMeal"),
            Category: getStr("strCategory"),
            Area: getStr("strArea"),
            Instructions: getStr("strInstructions"),
            Thumbnail: getStr("strMealThumb"),
            Ingredients: ingredients,
            Measures: measures,
            Tags: tags,
            YoutubeUrl: getStr("strYoutube"),
            SourceUrl: getStr("strSource")
        );
    }
}
