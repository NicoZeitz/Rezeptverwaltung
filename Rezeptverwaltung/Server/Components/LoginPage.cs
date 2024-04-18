

using Server.Resources;

namespace Server.Components;

public class LoginPage : ContainerComponent
{
    public LoginPage(TemplateLoader templateLoader)
        : base(templateLoader)
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
