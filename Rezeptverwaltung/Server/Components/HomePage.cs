using Server.Resources;

namespace Server.Components;

public class HomePage : ContainerComponent
{
    public HomePage(TemplateLoader templateLoader)
        : base(templateLoader)
    {
    }

    public override async Task<string> RenderAsync()
    {
        return await templateLoader.LoadTemplate("HomePage.html")!.RenderAsync(new
        {
            Header = await GetRenderedSlottedChild("Header"),
            Children = await GetRenderedChildren()
        });
    }
}
