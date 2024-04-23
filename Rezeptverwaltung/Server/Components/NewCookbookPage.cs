

using Core.Entities;
using Server.Resources;

namespace Server.Components;

public class NewCookbookPage(TemplateLoader templateLoader) : ContainerComponent(templateLoader)
{
    public const string HEADER_SLOT = "Header";

    public IEnumerable<Recipe> Recipes { get; set; } = [];

    public override async Task<string> RenderAsync()
    {
        return await templateLoader
            .LoadTemplate("NewCookbookPage.html")!
            .RenderAsync(new
            {
                Recipes,
                Header = await GetRenderedSlottedChild(HEADER_SLOT),
                Children = await GetRenderedChildren()
            });
    }
}
