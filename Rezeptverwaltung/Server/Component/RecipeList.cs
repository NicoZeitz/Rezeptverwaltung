using Core.Entities;
using Server.ResourceLoader;
using Server.Resources;

namespace Server.Component;

public class RecipeList
{
    private readonly TemplateLoader templateLoader;

    public RecipeList(IResourceLoader resourceLoader)
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
