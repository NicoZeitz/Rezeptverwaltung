


using Server.Resources;

namespace Server.Components;

public class NotFoundPage : ContainerComponent
{
    public NotFoundPage(TemplateLoader templateLoader)
        : base(templateLoader)
    {
    }

    public override async Task<string> RenderAsync()
    {
        return await templateLoader.LoadTemplate("NotFoundPage.html")!.RenderAsync(new
        {
            Header = await GetRenderedSlottedChild("Header")
        });
    }
}
