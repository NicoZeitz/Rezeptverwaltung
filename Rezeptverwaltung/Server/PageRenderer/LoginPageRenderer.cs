using Core.ValueObjects;
using Server.Components;
using Server.Service;
using System.Net;

namespace Server.PageRenderer;

public class LoginPageRenderer
{
    private readonly ComponentProvider componentProvider;
    private readonly HTMLFileWriter htmlFileWriter;
    private readonly RedirectService redirectService;

    public LoginPageRenderer(
        ComponentProvider componentProvider,
        HTMLFileWriter htmlFileWriter,
        RedirectService redirectService)
        : base()
    {
        this.componentProvider = componentProvider;
        this.htmlFileWriter = htmlFileWriter;
        this.redirectService = redirectService;
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
            redirectService.RedirectToPage(response, "/");
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
