using Core.Services;
using Server.Session;
using System.Net;

namespace Server.RequestHandler;

public class HomeRequestHandler : HTMLRequestHandler
{
    private readonly ResourceLoader.ResourceLoader resourceLoader;
    private readonly SessionService sessionService;
    private readonly ShowRecipes showRecipes;
    private readonly ImageUrlService imageUrlService;

    public HomeRequestHandler(ResourceLoader.ResourceLoader resourceLoader, SessionService sessionService, ShowRecipes showRecipes, ImageUrlService imageUrlService)
    {
        this.resourceLoader = resourceLoader;
        this.sessionService = sessionService;
        this.showRecipes = showRecipes;
        this.imageUrlService = imageUrlService;
    }

    public bool CanHandle(HttpListenerRequest request)
    {
        if (request.HttpMethod != "GET")
        {
            return false;
        }

        return request.Url!.AbsolutePath is "/" or "/index.html";
    }

    public Task<string> HandleHtmlFileRequest(HttpListenerRequest request)
    {
        var currentChef = sessionService.GetCurrentChef(request);
        var recipes = showRecipes.ShowRecipesVisibleTo(currentChef);

        var component = new Components.HomePage(resourceLoader)
        {
            SlottedChildren = new Dictionary<string, Components.Component>
            {
                { "Header", new Components.Header(resourceLoader) { CurrentChef = currentChef } },
                { "RecipeList", new Components.RecipeList(resourceLoader, imageUrlService) { Recipes = recipes } }
            }
        };

        return component.RenderAsync();
    }
}
