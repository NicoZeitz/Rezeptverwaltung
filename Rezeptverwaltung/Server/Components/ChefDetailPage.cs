
using Core.Entities;
using Server.Resources;
using Server.Service;

namespace Server.Components;

public class ChefDetailPage : ContainerComponent
{
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
                ChefImage = chefImage,
                Header = await GetRenderedSlottedChild("Header"),
                Chef,
                ChefRecipes = await GetRenderedSlottedChild("Recipes"),
                ChefCookbooks = await GetRenderedSlottedChild("Cookbooks"),
                ChefShoppingLists = await GetRenderedSlottedChild("ShoppingLists")
            });
    }
}
