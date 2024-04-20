using Core.ValueObjects;
using Server.Components;
using Server.Resources;
using Server.Service;
using Server.Session;
using System.Net;

namespace Server.RequestHandler;

public class SettingsPageRenderer
{
    private readonly ComponentProvider componentProvider;
    private readonly HTMLFileWriter htmlFileWriter;
    private readonly NotFoundRequestHandler notFoundRequestHandler;
    private readonly SessionService sessionService;
    private readonly TemplateLoader templateLoader;

    public SettingsPageRenderer(
        ComponentProvider componentProvider,
        HTMLFileWriter htmlFileWriter,
        NotFoundRequestHandler notFoundRequestHandler,
        SessionService sessionService,
        TemplateLoader templateLoader)
        : base()
    {
        this.componentProvider = componentProvider;
        this.htmlFileWriter = htmlFileWriter;
        this.notFoundRequestHandler = notFoundRequestHandler;
        this.sessionService = sessionService;
        this.templateLoader = templateLoader;
    }

    public Task RenderPage(
        HttpListenerRequest request,
        HttpListenerResponse response,
        HttpStatusCode httpStatus,
        IDictionary<string, IEnumerable<ErrorMessage>> slottedErrorMessages = default!)
    {
        slottedErrorMessages ??= new Dictionary<string, IEnumerable<ErrorMessage>>();
        return RenderPage(request, response, httpStatus, null, slottedErrorMessages);
    }

    public Task RenderPage(
        HttpListenerRequest request,
        HttpListenerResponse response,
        HttpStatusCode httpStatus,
        DisplayableComponent? message)
    {
        return RenderPage(request, response, httpStatus, message, new Dictionary<string, IEnumerable<ErrorMessage>>());
    }

    public async Task RenderPage(
        HttpListenerRequest request,
        HttpListenerResponse response,
        HttpStatusCode httpStatus,
        DisplayableComponent? message,
        IDictionary<string, IEnumerable<ErrorMessage>> slottedErrorMessages)
    {
        var header = componentProvider.GetComponent<Header>();
        var settingsPage = componentProvider.GetComponent<SettingsPage>();

        settingsPage.SlottedChildren = new Dictionary<string, Component>();

        var currentChef = sessionService.GetCurrentChef(request);
        if (currentChef is null)
        {
            await notFoundRequestHandler.Handle(request, response);
            return;
        }


        foreach (var (slotName, errorMessages) in slottedErrorMessages)
        {
            settingsPage.SlottedChildren[slotName] = new ComponentSequence(templateLoader)
            {
                Children = errorMessages.Select(errorMessage => new DisplayableComponent(errorMessage))
            };
        }

        header.CurrentChef = currentChef;
        settingsPage.CurrentChef = currentChef;
        settingsPage.SlottedChildren["Header"] = header;
        if (message is not null)
        {
            settingsPage.SlottedChildren["Message"] = message;
        }

        var htmlFile = await settingsPage.RenderAsync();

        htmlFileWriter.WriteHtmlFile(response, htmlFile, httpStatus);
    }
}