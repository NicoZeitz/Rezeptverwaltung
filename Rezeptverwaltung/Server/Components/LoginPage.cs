

namespace Server.Components;

public class LoginPage : ContainerComponent
{
    public LoginPage(ResourceLoader.ResourceLoader resourceLoader)
        : base(resourceLoader)
    {
    }

    public override async Task<string> RenderAsync()
    {
        return await templateLoader.LoadTemplate("LoginPage.html")!.RenderAsync(new
        {
            Header = await GetRenderedSlottedChild("Header")
        });
    }
}
