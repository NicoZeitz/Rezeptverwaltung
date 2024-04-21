


using Server.Resources;

namespace Server.Components;

public class NotFoundPage(TemplateLoader templateLoader) : ContainerComponent(templateLoader)
{
    public const string HEADER_SLOT = "Header";

    public override async Task<string> RenderAsync()
    {
        return await templateLoader.LoadTemplate("NotFoundPage.html")!.RenderAsync(new
        {
            Header = await GetRenderedSlottedChild(HEADER_SLOT)
        });
    }
}
