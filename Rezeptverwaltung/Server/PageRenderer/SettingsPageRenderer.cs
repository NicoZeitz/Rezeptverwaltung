using Core.Entities;
using Core.ValueObjects;
using Server.Components;
using Server.Resources;
using Server.Service;
using System.Net;

namespace Server.RequestHandler;

public class SettingsPageRenderer
{
    private readonly ComponentProvider componentProvider;
    private readonly HTMLFileWriter htmlFileWriter;
    private readonly NotFoundRequestHandler notFoundRequestHandler;
    private readonly TemplateLoader templateLoader;

    public SettingsPageRenderer(
        ComponentProvider componentProvider,
        HTMLFileWriter htmlFileWriter,
        NotFoundRequestHandler notFoundRequestHandler,
        TemplateLoader templateLoader)
        : base()
    {
        this.componentProvider = componentProvider;
        this.htmlFileWriter = htmlFileWriter;
        this.notFoundRequestHandler = notFoundRequestHandler;
        this.templateLoader = templateLoader;
    }

    public Task RenderPage(
        HttpListenerRequest request,
        HttpListenerResponse response,
        Chef currentChef,
        HttpStatusCode httpStatus,
        IDictionary<string, IEnumerable<ErrorMessage>> slottedErrorMessages = default!)
    {
        slottedErrorMessages ??= new Dictionary<string, IEnumerable<ErrorMessage>>();
        return RenderPage(request, response, currentChef, httpStatus, null, slottedErrorMessages);
    }

    public Task RenderPage(
        HttpListenerRequest request,
        HttpListenerResponse response,
        Chef currentChef,
        HttpStatusCode httpStatus,
        DisplayableComponent? message)
    {
        return RenderPage(request, response, currentChef, httpStatus, message, new Dictionary<string, IEnumerable<ErrorMessage>>());
    }

    public async Task RenderPage(
        HttpListenerRequest request,
        HttpListenerResponse response,
        Chef currentChef,
        HttpStatusCode httpStatus,
        DisplayableComponent? message,
        IDictionary<string, IEnumerable<ErrorMessage>> slottedErrorMessages)
    {
        var header = componentProvider.GetComponent<Header>();
        var settingsPage = componentProvider.GetComponent<SettingsPage>();

        settingsPage.SlottedChildren = new Dictionary<string, Component>();

        foreach (var (slotName, errorMessages) in slottedErrorMessages)
        {
            settingsPage.SlottedChildren[slotName] = new ComponentSequence(templateLoader)
            {
                Children = errorMessages.Select(errorMessage => new DisplayableComponent(errorMessage))
            };
        }

        header.CurrentChef = currentChef;
        settingsPage.CurrentChef = currentChef;
        settingsPage.SlottedChildren[SettingsPage.HEADER_SLOT] = header;
        if (message is not null)
        {
            settingsPage.SlottedChildren[SettingsPage.MESSAGE_SLOT] = message;
        }

        var htmlFile = await settingsPage.RenderAsync();

        htmlFileWriter.WriteHtmlFile(response, htmlFile, httpStatus);
    }
}