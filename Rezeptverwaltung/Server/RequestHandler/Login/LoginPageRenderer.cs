using Core.ValueObjects;
using Server.Components;
using Server.Service;
using System.Net;

namespace Server.RequestHandler;

public class LoginPageRenderer
{
    private readonly ComponentProvider componentProvider;
    private readonly HTMLFileWriter htmlFileWriter;

    public LoginPageRenderer(
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
        ErrorMessage? errorMessage = null
    )
    {
        if (currentChef is not null)
        {
            response.StatusCode = (int)HttpStatusCode.SeeOther;
            response.Redirect("/");
            return;
        }

        var header = componentProvider.GetComponent<Header>();
        var loginPage = componentProvider.GetComponent<LoginPage>();

        header.CurrentChef = currentChef;
        loginPage.SlottedChildren[LoginPage.HEADER_SLOT] = header;
        loginPage.Children = errorMessage is null ? Array.Empty<Component>() : [new DisplayableComponent(errorMessage)];
        var htmlString = await loginPage.RenderAsync();

        htmlFileWriter.WriteHtmlFile(response, htmlString, httpStatus);
    }
}
