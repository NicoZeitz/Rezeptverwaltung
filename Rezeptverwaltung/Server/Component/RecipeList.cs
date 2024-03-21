using Core.Entities;
using Scriban;
using Server.ResourceLoader;

namespace Server.Component;

public class RecipeList
{
    private readonly IResourceLoader resourceLoader;

    public RecipeList(IResourceLoader resourceLoader)
    {
        this.resourceLoader = resourceLoader;
    }

    public ValueTask<string> RenderAsync(IEnumerable<Recipe> recipes)
    {
        using var recipePreviewStream = resourceLoader.LoadResource("recipe_list.html")!;
        var recipePreviewContent = new StreamReader(recipePreviewStream).ReadToEnd();
        var recipePreviewTemplate = Template.Parse(recipePreviewContent);

        return recipePreviewTemplate.RenderAsync(new { Recipes = recipes });
    }
}
