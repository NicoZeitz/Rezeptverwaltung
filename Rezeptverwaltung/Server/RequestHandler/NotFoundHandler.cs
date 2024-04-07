using Core.Entities;
using Scriban;
using Server.Component;
using Server.ResourceLoader;
using Server.Resources;
using Server.Session;
using System.Net;

namespace Server.RequestHandler;

public class NotFoundHandler : IHTMLRequestHandler
{
    private readonly IResourceLoader resourceLoader;
    private readonly ISessionService sessionService;
    private readonly TemplateLoader templateLoader;

    public NotFoundHandler(
        IResourceLoader resourceLoader,
        ISessionService sessionService
    ) : base()
    {
        this.resourceLoader = resourceLoader;
        this.sessionService = sessionService;
        this.templateLoader = new TemplateLoader(resourceLoader);
    }

    public bool CanHandle(HttpListenerRequest request) => true;

    public async Task<string> HandleHtmlFileRequest(HttpListenerRequest request)
    {
        var currentChef = sessionService.GetCurrentChef(request);
        var notFoundTemplate = templateLoader.LoadTemplate("404.html")!;
        
        return await notFoundTemplate.RenderAsync(new {
            Header = await new Header(resourceLoader).RenderAsync(currentChef),
        });
    }
}
