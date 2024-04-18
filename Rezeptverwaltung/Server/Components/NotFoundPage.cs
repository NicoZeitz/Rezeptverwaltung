


namespace Server.Components;

public class NotFoundPage : ContainerComponent
{
    public NotFoundPage(ResourceLoader.ResourceLoader resourceLoader)
        : base(resourceLoader)
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
