using Core.ValueObjects;
using Server.Components;
using Server.Service;
using System.Net;

namespace Server.RequestHandler;

public class RegisterPageRenderer
{
    private readonly Header header;
    private readonly HTMLFileWriter htmlFileWriter;
    private readonly RegisterPage registerPage;

    public RegisterPageRenderer(
        Header header,
        HTMLFileWriter htmlFileWriter,
        RegisterPage registerPage)
        : base()
    {
        this.header = header;
        this.htmlFileWriter = htmlFileWriter;
        this.registerPage = registerPage;
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

        header.CurrentChef = currentChef;
        registerPage.SlottedChildren["Header"] = header;
        registerPage.Children = errorMessages.Select(errorMessage => new DisplayableComponent(errorMessage));

        var htmlString = await registerPage.RenderAsync();
        htmlFileWriter.WriteHtmlFile(response, htmlString, httpStatus);
    }
}
