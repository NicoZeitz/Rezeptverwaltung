
using Core.Entities;
using Server.Resources;
using Server.Service;

namespace Server.Components;

public class ChefDetailPage : ContainerComponent
{
    public const string HEADER_SLOT = "Header";
    public const string RECIPES_SLOT = "Recipes";
    public const string COOKBOOKS_SLOT = "Cookbooks";
    public const string SHOPPING_LISTS_SLOT = "ShoppingLists";

    public Chef? Chef { get; set; }

    private readonly ImageUrlService imageUrlService;

    public ChefDetailPage(
        TemplateLoader templateLoader,
        ImageUrlService imageUrlService)
        : base(templateLoader)
    {
        this.imageUrlService = imageUrlService;
    }

    public override async Task<string> RenderAsync()
    {
        var chefImage = Chef is null
            ? null
            : imageUrlService.GetImageUrlForChef(Chef);

        return await templateLoader
            .LoadTemplate("ChefDetailPage.html")
            .RenderAsync(new
            {
                Chef,
                ChefImage = chefImage,
                Header = await GetRenderedSlottedChild(HEADER_SLOT),
                Recipes = await GetRenderedSlottedChild(RECIPES_SLOT),
                Cookbooks = await GetRenderedSlottedChild(COOKBOOKS_SLOT),
                ShoppingLists = await GetRenderedSlottedChild(SHOPPING_LISTS_SLOT)
            });
    }
}
