using Core.ValueObjects;
using Server.Components;
using Server.Resources;
using System.Net;
using System.Text;

namespace Server.RequestHandler.Login;

public class LoginPageRenderer
{
    private readonly TemplateLoader templateLoader;

    public LoginPageRenderer(TemplateLoader templateLoader) : base()
    {
        this.templateLoader = templateLoader;
    }

    public async Task RenderPage(
        HttpListenerResponse response,
        HttpStatusCode httpStatus,
        Core.Entities.Chef? currentChef,
        ErrorMessage? errorMessage = null
    )
    {
        if (currentChef is not null)
        {
            response.StatusCode = (int)HttpStatusCode.SeeOther;
            response.Redirect("/");
            return;
        }


        var component = new LoginPage(templateLoader)
        {
            SlottedChildren = new Dictionary<string, Component>
            {
                { "Header", new Header(templateLoader) { CurrentChef = currentChef } },
            },
            Children = errorMessage is null ? new Component[0] : new[] { new DisplayableComponent(errorMessage) }
        };

        var registerPage = await component.RenderAsync();

        response.StatusCode = (int)httpStatus;
        await response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(registerPage));
    }
}
