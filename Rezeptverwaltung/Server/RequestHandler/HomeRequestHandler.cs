using Core.Services;
using Server.Resources;
using Server.Service;
using Server.Session;
using System.Net;

namespace Server.RequestHandler;

public class HomeRequestHandler : HTMLRequestHandler
{
    private readonly TemplateLoader templateLoader;
    private readonly SessionService sessionService;
    private readonly ShowRecipes showRecipes;
    private readonly ImageUrlService imageUrlService;

    public HomeRequestHandler(
        HTMLFileWriter htmlFileWriter,
        TemplateLoader templateLoader,
        SessionService sessionService,
        ShowRecipes showRecipes,
        ImageUrlService imageUrlService)
        : base(htmlFileWriter)
    {
        this.templateLoader = templateLoader;
        this.sessionService = sessionService;
        this.showRecipes = showRecipes;
        this.imageUrlService = imageUrlService;
    }

    public override bool CanHandle(HttpListenerRequest request)
    {
        if (request.HttpMethod != HttpMethod.Get.Method)
        {
            return false;
        }

        return request.Url!.AbsolutePath is "/" or "/index.html";
    }

    public override Task<string> HandleHtmlFileRequest(HttpListenerRequest request)
    {
        var currentChef = sessionService.GetCurrentChef(request);
        var recipes = showRecipes.ShowAllRecipes(currentChef);

        var component = new Components.HomePage(templateLoader)
        {
            SlottedChildren = new Dictionary<string, Components.Component>
            {
                { "Header", new Components.Header(templateLoader, imageUrlService) { CurrentChef = currentChef } },
                { "RecipeList", new Components.RecipeList(templateLoader, imageUrlService) { Recipes = recipes } }
            }
        };

        return component.RenderAsync();
    }
}
