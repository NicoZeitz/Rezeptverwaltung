using Core.Entities;
using Server.Resources;
using Server.Service;

namespace Server.Components;

public class RecipeList : TemplateComponent
{
    public IEnumerable<Recipe> Recipes { get; set; } = [];

    private readonly ImageUrlService imageUrlService;

    public RecipeList(TemplateLoader templateLoader, ImageUrlService imageUrlService)
        : base(templateLoader)
    {
        this.imageUrlService = imageUrlService;
    }

    public override Task<string> RenderAsync()
    {
        if (!Recipes.Any())
        {
            return Task.FromResult("");
        }

        var recipes = Recipes.Select(recipe => new RecipeWithImage(
            recipe,
            imageUrlService.GetImageUrlForRecipe(recipe)
        ));

        return templateLoader
            .LoadTemplate("RecipeList.html")!
            .RenderAsync(new
            {
                Recipes = recipes
            })
            .AsTask();
    }

    private record struct RecipeWithImage(Recipe Recipe, string Image);
}
