using Server.PageRenderer;
using Server.Service;
using Server.Session;
using System.Net;

namespace Server.RequestHandler;

public class NotFoundRequestHandler : RequestHandler
{
    private readonly HTMLFileWriter htmlFileWriter;
    private readonly NotFoundPageRenderer notFoundPageRenderer;
    private readonly SessionService sessionService;

    public NotFoundRequestHandler(
        HTMLFileWriter htmlFileWriter,
        NotFoundPageRenderer notFoundPageRenderer,
        SessionService sessionService)
        : base()
    {
        this.htmlFileWriter = htmlFileWriter;
        this.notFoundPageRenderer = notFoundPageRenderer;
        this.sessionService = sessionService;
    }

    public bool CanHandle(HttpListenerRequest request) => true;

    public async Task Handle(HttpListenerRequest request, HttpListenerResponse response)
    {
        var currentChef = sessionService.GetCurrentChef(request);
        var htmlString = await notFoundPageRenderer.RenderPage(currentChef);
        htmlFileWriter.WriteHtmlFile(response, htmlString, HttpStatusCode.NotFound);
    }
}
