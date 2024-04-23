
using Core.Entities;
using Server.Resources;
using Server.Service;

namespace Server.Components;

public class RecipeDetailPage : ContainerComponent
{
    public const string HEADER_SLOT = "Header";

    public Recipe? Recipe { get; set; }
    public Chef? CurrentChef { get; set; }

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
                Recipe,
                CurrentChef,
                RecipeImage = recipeImage,
                Header = await GetRenderedSlottedChild(HEADER_SLOT)
            })
            .AsTask();
    }
}
