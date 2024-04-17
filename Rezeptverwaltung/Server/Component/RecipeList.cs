using Core.Entities;
using Server.Resources;

namespace Server.Component;

public class RecipeList
{
    private readonly TemplateLoader templateLoader;

    public RecipeList(ResourceLoader.ResourceLoader resourceLoader)
    {
        templateLoader = new TemplateLoader(resourceLoader);
    }

    public ValueTask<string> RenderAsync(IEnumerable<Recipe> recipes)
    {
        return templateLoader
            .LoadTemplate("recipe_list.html")!
            .RenderAsync(new
            {
                Recipes = recipes
            });
    }
}
