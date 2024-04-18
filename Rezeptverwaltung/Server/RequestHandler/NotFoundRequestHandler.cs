using Server.Session;
using System.Net;

namespace Server.RequestHandler;

public class NotFoundRequestHandler : HTMLRequestHandler
{
    private readonly ResourceLoader.ResourceLoader resourceLoader;
    private readonly SessionService sessionService;

    public NotFoundRequestHandler(
        ResourceLoader.ResourceLoader resourceLoader,
        SessionService sessionService
    ) : base()
    {
        this.resourceLoader = resourceLoader;
        this.sessionService = sessionService;
    }

    public bool CanHandle(HttpListenerRequest request) => true;

    public Task<string> HandleHtmlFileRequest(HttpListenerRequest request)
    {
        var currentChef = sessionService.GetCurrentChef(request);

        var component = new Components.NotFoundPage(resourceLoader)
        {
            SlottedChildren = new Dictionary<string, Components.Component>
            {
                { "Header", new Components.Header(resourceLoader) { CurrentChef = currentChef } }
            }
        };

        return component.RenderAsync();
    }
}
