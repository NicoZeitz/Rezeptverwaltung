using Core.Entities;
using Server.Resources;

namespace Server.Components;

public class NewShoppingListPage(TemplateLoader templateLoader) : ContainerComponent(templateLoader)
{
    public const string HEADER_SLOT = "Header";

    public IEnumerable<Recipe> Recipes { get; set; } = [];

    public override async Task<string> RenderAsync()
    {
        return await templateLoader
            .LoadTemplate("NewShoppingListPage.html")!
            .RenderAsync(new
            {
                Recipes,
                Header = await GetRenderedSlottedChild(HEADER_SLOT),
                Children = await GetRenderedChildren()
            });
    }
}
