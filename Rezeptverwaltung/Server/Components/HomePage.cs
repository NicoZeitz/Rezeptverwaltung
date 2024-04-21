using Server.Resources;

namespace Server.Components;

public class HomePage(TemplateLoader templateLoader) : ContainerComponent(templateLoader)
{
    public const string HEADER_SLOT = "Header";

    public override async Task<string> RenderAsync()
    {
        return await templateLoader.LoadTemplate("HomePage.html")!.RenderAsync(new
        {
            Header = await GetRenderedSlottedChild(HEADER_SLOT),
            Children = await GetRenderedChildren()
        });
    }
}
