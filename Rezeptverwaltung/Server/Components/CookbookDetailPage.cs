

using Core.Entities;
using Server.Resources;

namespace Server.Components;

public class CookbookDetailPage(TemplateLoader templateLoader) : ContainerComponent(templateLoader)
{
    public const string HEADER_SLOT = "Header";

    public Cookbook? Cookbook { get; set; }
    public Chef? CurrentChef { get; set; }

    public override async Task<string> RenderAsync()
    {
        return await templateLoader
            .LoadTemplate("CookbookDetailPage.html")
            .RenderAsync(new
            {
                Cookbook,
                CurrentChef,
                Header = await GetRenderedSlottedChild(HEADER_SLOT),
                Children = await GetRenderedChildren()
            })
            .AsTask();
    }
}
