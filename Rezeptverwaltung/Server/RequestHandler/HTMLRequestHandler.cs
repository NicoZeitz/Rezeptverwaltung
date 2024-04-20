using Server.Service;
using System.Net;

namespace Server.RequestHandler;

public abstract class HTMLRequestHandler : RequestHandler
{
    public HTMLFileWriter htmlFileWriter;

    public HTMLRequestHandler(HTMLFileWriter htmlFileWriter)
    {
        this.htmlFileWriter = htmlFileWriter;
    }

    public abstract bool CanHandle(HttpListenerRequest request);

    public async Task Handle(HttpListenerRequest request, HttpListenerResponse response)
    {
        var htmlFile = await HandleHtmlFileRequest(request);
        htmlFileWriter.WriteHtmlFile(response, htmlFile);
    }

    public abstract Task<string> HandleHtmlFileRequest(HttpListenerRequest request);
}