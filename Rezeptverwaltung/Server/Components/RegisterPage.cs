

using Server.Resources;

namespace Server.Components;

public class RegisterPage : ContainerComponent
{
    public RegisterPage(TemplateLoader templateLoader)
        : base(templateLoader)
    {
    }

    public override async Task<string> RenderAsync()
    {
        return await templateLoader
            .LoadTemplate("RegisterPage.html")!
            .RenderAsync(new
            {
                ErrorMessages = await GetRenderedChildren(),
                Header = await GetRenderedSlottedChild("Header")
            });
    }
}
