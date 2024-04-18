

namespace Server.Components;

public class RegisterPage : ContainerComponent
{
    public RegisterPage(ResourceLoader.ResourceLoader resourceLoader)
        : base(resourceLoader)
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
