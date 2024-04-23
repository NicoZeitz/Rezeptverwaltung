using Server.PageRenderer;
using Server.Service;
using Server.Session;
using System.Net;

namespace Server.RequestHandler;

public abstract class HTMLRequestHandler : RequestHandler
{
    protected readonly HTMLFileWriter htmlFileWriter;
    protected readonly NotFoundPageRenderer notFoundPageRenderer;
    protected readonly SessionService sessionService;

    private bool returnNotFound = false;

    public HTMLRequestHandler(HTMLFileWriter htmlFileWriter, NotFoundPageRenderer notFoundPageRenderer, SessionService sessionService)
    {
        this.notFoundPageRenderer = notFoundPageRenderer;
        this.htmlFileWriter = htmlFileWriter;
        this.sessionService = sessionService;
    }

    public abstract bool CanHandle(HttpListenerRequest request);

    public async Task Handle(HttpListenerRequest request, HttpListenerResponse response)
    {
        var htmlFile = await HandleHtmlFileRequest(request);
        if (returnNotFound)
        {
            returnNotFound = false;
            var currentChef = sessionService.GetCurrentChef(request);
            var notFoundHtmlFile = await notFoundPageRenderer.RenderPage(currentChef);
            htmlFileWriter.WriteHtmlFile(response, notFoundHtmlFile, HttpStatusCode.NotFound);
            return;
        }

        htmlFileWriter.WriteHtmlFile(response, htmlFile);
    }

    protected Task<string> ReturnNotFound()
    {
        returnNotFound = true;
        return Task.FromResult("");
    }

    public abstract Task<string> HandleHtmlFileRequest(HttpListenerRequest request);
}