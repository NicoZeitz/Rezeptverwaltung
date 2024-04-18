

using Server.Resources;

namespace Server.Components;

public class RegisterPage : ContainerComponent
{
    public RegisterPage(TemplateLoader templateLoader)
        : base(templateLoader)
    {
    }

    public override Task<string> RenderAsync()
    {
        return templateLoader.LoadTemplate("RegisterPage.html")!.RenderAsync(new
        {
            ErrorMessages = GetSlottedChild("ErrorMessages"),
            Header = GetSlottedChild("Header")
        })
            .AsTask();
    }
}
