using Server.Resources;
using Server.Session;
using System.Net;

namespace Server.RequestHandler;

public class NotFoundRequestHandler : HTMLRequestHandler
{
    private readonly TemplateLoader templateLoader;
    private readonly SessionService sessionService;

    public NotFoundRequestHandler(
        TemplateLoader templateLoader,
        SessionService sessionService
    ) : base()
    {
        this.templateLoader = templateLoader;
        this.sessionService = sessionService;
    }

    public bool CanHandle(HttpListenerRequest request) => true;

    public Task<string> HandleHtmlFileRequest(HttpListenerRequest request)
    {
        var currentChef = sessionService.GetCurrentChef(request);

        var component = new Components.NotFoundPage(templateLoader)
        {
            SlottedChildren = new Dictionary<string, Components.Component>
            {
                { "Header", new Components.Header(templateLoader) { CurrentChef = currentChef } }
            }
        };

        return component.RenderAsync();
    }
}
