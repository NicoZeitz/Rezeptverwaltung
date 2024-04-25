using Server.ValueObjects;
using System.Net;

namespace Server.RequestHandler;

public class FaviconRequestHandler : RequestHandler
{
    private readonly ResourceLoader.ResourceLoader resourceLoader;

    public FaviconRequestHandler(ResourceLoader.ResourceLoader resourceLoader) : base()
    {
        this.resourceLoader = resourceLoader;
    }

    public bool CanHandle(HttpListenerRequest request) => request.Url?.AbsolutePath == "/favicon.ico";

    public async Task Handle(HttpListenerRequest _request, HttpListenerResponse response)
    {
        response.StatusCode = (int)HttpStatusCode.OK;
        response.ContentType = MimeType.ICO;
        var favicon = resourceLoader.LoadResource("favicon.ico")!;
        await favicon.CopyToAsync(response.OutputStream);
    }
}