using Core.Services;
using Server.Components;
using Server.Service;
using Server.Session;
using System.Net;

namespace Server.RequestHandler;

public class HomeRequestHandler : HTMLRequestHandler
{
    private readonly ComponentProvider componentProvider;
    private readonly SessionService sessionService;
    private readonly ShowRecipes showRecipes;

    public HomeRequestHandler(
        ComponentProvider componentProvider,
        HTMLFileWriter htmlFileWriter,
        SessionService sessionService,
        ShowRecipes showRecipes)
        : base(htmlFileWriter)
    {
        this.componentProvider = componentProvider;
        this.sessionService = sessionService;
        this.showRecipes = showRecipes;
    }

    public override bool CanHandle(HttpListenerRequest request)
    {
        if (request.HttpMethod != HttpMethod.Get.Method)
        {
            return false;
        }

        return request.Url!.AbsolutePath is "/" or "/index.html";
    }

    public override async Task<string> HandleHtmlFileRequest(HttpListenerRequest request)
    {
        var currentChef = sessionService.GetCurrentChef(request);
        var recipes = showRecipes.ShowAllRecipes(currentChef);

        var header = componentProvider.GetComponent<Header>();
        var homePage = componentProvider.GetComponent<HomePage>();
        var recipeList = componentProvider.GetComponent<RecipeList>();

        header.CurrentChef = currentChef;
        recipeList.Recipes = recipes;

        homePage.SlottedChildren[HomePage.HEADER_SLOT] = header;
        homePage.Children = [recipeList];

        return await homePage.RenderAsync();
    }
}
