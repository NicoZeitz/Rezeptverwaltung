using Scriban;
using Server.ResourceLoader;
using System.Net;

namespace Server.RequestHandler;

public class NotFoundHandler : IHTMLRequestHandler
{
    private readonly IResourceLoader resourceLoader;

    public NotFoundHandler(IResourceLoader resourceLoader) : base()
    {
        this.resourceLoader = resourceLoader;
    }

    public bool CanHandle(HttpListenerRequest request) => true;

    public Task<string> HandleHtmlFileRequest(HttpListenerRequest request)
    {
        using var notFoundStream = resourceLoader.LoadResource("404.html")!;
        var notFoundContent = new StreamReader(notFoundStream).ReadToEnd();
        var notFoundTemplate = Template.Parse(notFoundContent);

        return notFoundTemplate.RenderAsync(new { }).AsTask();
    }
}
