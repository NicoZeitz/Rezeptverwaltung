using Core.ValueObjects;
using Server.Components;
using Server.Service;
using Server.Session;
using System.Net;

namespace Server.RequestHandler;

public class SettingsPageRenderer
{
    private readonly Header header;
    private readonly HTMLFileWriter htmlFileWriter;
    private readonly NotFoundRequestHandler notFoundRequestHandler;
    private readonly SessionService sessionService;
    private readonly SettingsPage settingsPage;

    public SettingsPageRenderer(
        Header header,
        HTMLFileWriter htmlFileWriter,
        NotFoundRequestHandler notFoundRequestHandler,
        SessionService sessionService,
        SettingsPage settingsPage)
        : base()
    {
        this.header = header;
        this.htmlFileWriter = htmlFileWriter;
        this.notFoundRequestHandler = notFoundRequestHandler;
        this.sessionService = sessionService;
        this.settingsPage = settingsPage;
    }

    public async Task RenderPage(
        HttpListenerRequest request,
        HttpListenerResponse response,
        HttpStatusCode httpStatus,
        IEnumerable<ErrorMessage> errorMessages = default!)
    {
        var currentChef = sessionService.GetCurrentChef(request);
        if (currentChef is null)
        {
            await notFoundRequestHandler.Handle(request, response);
            return;
        }

        errorMessages ??= Enumerable.Empty<ErrorMessage>();

        header.CurrentChef = currentChef;
        settingsPage.CurrentChef = currentChef;
        settingsPage.SlottedChildren["Header"] = header;
        settingsPage.Children = errorMessages.Select(errorMessage => new DisplayableComponent(errorMessage));

        var htmlFile = await settingsPage.RenderAsync();

        htmlFileWriter.WriteHtmlFile(response, htmlFile, httpStatus);
    }
}