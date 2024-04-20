
using Core.Entities;
using Server.Resources;
using Server.Service;

namespace Server.Components;

public class RecipeDetailPage : ContainerComponent
{
    public Recipe? Recipe { get; set; }

    private readonly ImageUrlService imageUrlService;

    public RecipeDetailPage(TemplateLoader templateLoader, ImageUrlService imageUrlService)
        : base(templateLoader)
    {
        this.imageUrlService = imageUrlService;
    }

    public override async Task<string> RenderAsync()
    {
        var recipeImage = Recipe is null
            ? null
            : imageUrlService.GetImageUrlForRecipe(Recipe);

        return await templateLoader
            .LoadTemplate("RecipeDetailPage.html")
            .RenderAsync(new
            {
                RecipeImage = recipeImage,
                Header = await GetRenderedSlottedChild("Header"),
                Recipe
            })
            .AsTask();
    }
}
