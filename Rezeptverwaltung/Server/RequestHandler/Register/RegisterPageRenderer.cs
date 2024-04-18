using Core.ValueObjects;
using Server.Components;
using System.Net;
using System.Text;

namespace Server.RequestHandler.Register;

public class RegisterPageRenderer
{
    public async Task RenderPage(
        ResourceLoader.ResourceLoader resourceLoader,
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

        var component = new RegisterPage(resourceLoader)
        {
            SlottedChildren = new Dictionary<string, Component>
            {
                { "Header", new Header(resourceLoader) { CurrentChef = currentChef } },
            },
            Children = errorMessages.Select(errorMessage => new DisplayableComponent(errorMessage))
        };

        var registerPage = await component.RenderAsync();

        response.StatusCode = (int)httpStatus;
        await response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(registerPage));
    }
}
