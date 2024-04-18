
using Core.Entities;

namespace Server.Components;

public class RecipeDetailPage : ContainerComponent
{
    public Recipe? Recipe { get; set; }

    public RecipeDetailPage(ResourceLoader.ResourceLoader resourceLoader)
        : base(resourceLoader)
    {
    }

    public override async Task<string> RenderAsync()
    {
        return await templateLoader
            .LoadTemplate("RecipeDetailPage.html")
            .RenderAsync(new
            {
                RecipeImage = "https://picsum.photos/200/300", // TODO: image service
                Header = await GetRenderedSlottedChild("Header"),
                Recipe
            })
            .AsTask();
    }
}
