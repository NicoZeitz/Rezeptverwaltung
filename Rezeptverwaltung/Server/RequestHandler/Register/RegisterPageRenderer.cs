using Core.ValueObjects;
using Server.Components;
using Server.Service;
using System.Net;

namespace Server.RequestHandler;

public class RegisterPageRenderer
{
    private readonly ComponentProvider componentProvider;
    private readonly HTMLFileWriter htmlFileWriter;

    public RegisterPageRenderer(
        ComponentProvider componentProvider,
        HTMLFileWriter htmlFileWriter)
        : base()
    {
        this.componentProvider = componentProvider;
        this.htmlFileWriter = htmlFileWriter;
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

        errorMessages ??= [];

        var header = componentProvider.GetComponent<Header>();
        var registerPage = componentProvider.GetComponent<RegisterPage>();

        header.CurrentChef = currentChef;
        registerPage.SlottedChildren[RegisterPage.HEADER_SLOT] = header;
        registerPage.Children = errorMessages.Select(errorMessage => new DisplayableComponent(errorMessage));

        var htmlString = await registerPage.RenderAsync();
        htmlFileWriter.WriteHtmlFile(response, htmlString, httpStatus);
    }
}
