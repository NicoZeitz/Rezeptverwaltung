using Core.ValueObjects;
using Server.Components;
using Server.Service;
using System.Net;

namespace Server.RequestHandler;

public class LoginPageRenderer
{
    private readonly Header header;
    private readonly HTMLFileWriter htmlFileWriter;
    private readonly LoginPage loginPage;

    public LoginPageRenderer(
        Header header,
        HTMLFileWriter htmlFileWriter,
        LoginPage loginPage)
        : base()
    {
        this.header = header;
        this.htmlFileWriter = htmlFileWriter;
        this.loginPage = loginPage;
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

        header.CurrentChef = currentChef;
        loginPage.SlottedChildren["Header"] = header;
        loginPage.Children = errorMessage is null ? new Component[0] : new[] { new DisplayableComponent(errorMessage) };
        var htmlString = await loginPage.RenderAsync();

        htmlFileWriter.WriteHtmlFile(response, htmlString, httpStatus);
    }
}
