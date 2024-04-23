using Core.ValueObjects;
using Server.Resources;

namespace Server.Components;

public class SearchPage(TemplateLoader templateLoader) : ContainerComponent(templateLoader)
{
    public const string HEADER_SLOT = "Header";

    public Text? SearchTerm { get; set; }

    public override async Task<string> RenderAsync()
    {
        return await templateLoader
            .LoadTemplate("SearchPage.html")!
            .RenderAsync(new
            {
                SearchTerm,
                Children = await GetRenderedChildren(),
                Header = await GetRenderedSlottedChild(HEADER_SLOT)
            });
    }
}