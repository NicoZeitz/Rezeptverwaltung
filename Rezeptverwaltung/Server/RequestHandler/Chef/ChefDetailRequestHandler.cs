using Server.Component;
using Server.Resources;
using Server.Session;
using System.Net;
using System.Text.RegularExpressions;

namespace Server.RequestHandler.Chef;

public partial class ChefDetailRequestHandler : HTMLRequestHandler
{
    private readonly ResourceLoader.ResourceLoader resourceLoader;
    private readonly SessionService sessionService;
    private readonly TemplateLoader templateLoader;

    [GeneratedRegex("/chef/(?<chef_username>[A-Z0-9_ ]+)", RegexOptions.NonBacktracking | RegexOptions.IgnoreCase, matchTimeoutMilliseconds: 1000)]
    private static partial Regex chefDetailUrlPathRegex();

    public ChefDetailRequestHandler(ResourceLoader.ResourceLoader resourceLoader, SessionService sessionService)
    {
        this.resourceLoader = resourceLoader;
        this.sessionService = sessionService;
        this.templateLoader = new TemplateLoader(resourceLoader);
    }

    public bool CanHandle(HttpListenerRequest request) => chefDetailUrlPathRegex().IsMatch(request.Url?.AbsolutePath ?? "");

    public async Task<string> HandleHtmlFileRequest(HttpListenerRequest request)
    {
        var currentChef = sessionService.GetCurrentChef(request);
        var chefDetailTemplate = templateLoader.LoadTemplate("chef_detail.html")!;

        return await chefDetailTemplate.RenderAsync(new
        {
            Header = await new Header(resourceLoader).RenderAsync(currentChef)
        });
    }
}
