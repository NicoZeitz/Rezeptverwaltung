using Core.ValueObjects;
using Server.Components;
using Server.Resources;
using System.Net;
using System.Text;

namespace Server.RequestHandler.Register;

public class RegisterPageRenderer
{
    private readonly TemplateLoader templateLoader;

    public RegisterPageRenderer(TemplateLoader templateLoader) : base()
    {
        this.templateLoader = templateLoader;
    }

    public async Task RenderPage(
        HttpListenerResponse response,
        HttpStatusCode httpStatus,
        Core.Entities.Chef? currentChef,
        IEnumerable<ErrorMessage> errorMessages = default!
    )
    {
        if (currentChef is not null)
        {
            response.StatusCode = (int)HttpStatusCode.SeeOther;
            response.Redirect("/");
            return;
        }

        errorMessages ??= Enumerable.Empty<ErrorMessage>();

        var component = new RegisterPage(templateLoader)
        {
            SlottedChildren = new Dictionary<string, Component>
            {
                { "Header", new Header(templateLoader) { CurrentChef = currentChef } },
            },
            Children = errorMessages.Select(errorMessage => new DisplayableComponent(errorMessage))
        };

        var registerPage = await component.RenderAsync();

        response.StatusCode = (int)httpStatus;
        await response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(registerPage));
    }
}
