using Core.ValueObjects;
using Server.Components;
using Server.Service;
using System.Net;

namespace Server.PageRenderer;

public class RegisterPageRenderer
{
    private readonly ComponentProvider componentProvider;
    private readonly HTMLFileWriter htmlFileWriter;
    private readonly RedirectService redirectService;

    public RegisterPageRenderer(
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
        IEnumerable<ErrorMessage> errorMessages = default!
    )
    {
        if (currentChef is not null)
        {
            redirectService.RedirectToPage(response, "/");
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
