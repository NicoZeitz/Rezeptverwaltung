
using Core.Entities;
using Server.Resources;

namespace Server.Components;

public class RecipeDetailPage : ContainerComponent
{
    public Recipe? Recipe { get; set; }

    public RecipeDetailPage(TemplateLoader templateLoader)
        : base(templateLoader)
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
