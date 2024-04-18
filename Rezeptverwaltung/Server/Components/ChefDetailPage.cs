
using Core.Entities;

namespace Server.Components;

public class ChefDetailPage : ContainerComponent
{
    public Chef? Chef { get; set; }

    public ChefDetailPage(ResourceLoader.ResourceLoader resourceLoader)
        : base(resourceLoader)
    {
    }

    public override async Task<string> RenderAsync()
    {
        return await templateLoader
            .LoadTemplate("ChefDetailPage.html")
            .RenderAsync(new
            {
                ChefImage = "https://picsum.photos/200/300", // TODO: image service
                Header = await GetRenderedSlottedChild("Header"),
                Chef,
                ChefRecipes = await GetRenderedSlottedChild("Recipes")
            });
    }
}
