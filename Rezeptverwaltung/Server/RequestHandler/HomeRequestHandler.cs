using Core.Services;
using Server.Component;
using Server.ResourceLoader;
using Server.Resources;
using Server.Session;
using System.Net;

namespace Server.RequestHandler;

public class HomeRequestHandler : HTMLRequestHandler
{
    private readonly ResourceLoader.ResourceLoader resourceLoader;
    private readonly TemplateLoader templateLoader;
    private readonly SessionService sessionService;
    private readonly ShowRecipes showRecipes;

    public HomeRequestHandler(ResourceLoader.ResourceLoader resourceLoader, SessionService sessionService, ShowRecipes showRecipes)
    {
        this.resourceLoader = resourceLoader;
        this.templateLoader = new TemplateLoader(resourceLoader);
        this.sessionService = sessionService;
        this.showRecipes = showRecipes;
    }

    public bool CanHandle(HttpListenerRequest request)
    {
        if (request.HttpMethod != "GET")
        {
            return false;
        }

        return request.Url!.AbsolutePath is "/" or "/index.html";
    }

    public async Task<string> HandleHtmlFileRequest(HttpListenerRequest request)
    {
        var currentChef = sessionService.GetCurrentChef(request);
        var homeTemplate = templateLoader.LoadTemplate("home.html")!;
        var recipies = showRecipes.ShowRecipesVisibleTo(currentChef);

        return await homeTemplate.RenderAsync(new
        {
            Header = await new Header(resourceLoader).RenderAsync(currentChef),
            RecipeList = await new RecipeList(resourceLoader).RenderAsync(recipies)
        });
    }
}
