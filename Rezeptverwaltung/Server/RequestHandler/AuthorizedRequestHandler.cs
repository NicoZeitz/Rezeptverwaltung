using Core.Entities;
using Server.PageRenderer;
using Server.Service;
using Server.Session;
using System.Net;

namespace Server.RequestHandler;

public abstract class AuthorizedRequestHandler : RequestHandler
{
    protected readonly HTMLFileWriter htmlFileWriter;
    protected readonly NotFoundPageRenderer notFoundPageRenderer;
    protected readonly SessionService sessionService;

    protected AuthorizedRequestHandler(
        HTMLFileWriter htmlFileWriter,
        NotFoundPageRenderer notFoundPageRenderer,
        SessionService sessionService)
        : base()
    {
        this.htmlFileWriter = htmlFileWriter;
        this.notFoundPageRenderer = notFoundPageRenderer;
        this.sessionService = sessionService;
    }

    public abstract bool CanHandle(HttpListenerRequest request);

    public abstract Task Handle(HttpListenerRequest request, HttpListenerResponse response, Chef currentChef);

    public async Task Handle(HttpListenerRequest request, HttpListenerResponse response)
    {
        var currentChef = sessionService.GetCurrentChef(request);
        if (currentChef is null)
        {
            var notFoundPage = await notFoundPageRenderer.RenderPage(null);
            response.StatusCode = (int)HttpStatusCode.NotFound;
            htmlFileWriter.WriteHtmlFile(response, notFoundPage);
            return;
        }

        await Handle(request, response, currentChef);
    }
}