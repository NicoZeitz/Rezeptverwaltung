using Core.Entities;
using Core.Services;
using Server.DataParser;
using Server.PageRenderer;
using Server.Service;
using Server.Session;
using System.Net;

namespace Server.RequestHandler;

public class NewCookbookRequestHandler : AuthorizedRequestHandler
{
    private readonly NewCookbookPageRenderer newCookbookPageRenderer;
    private readonly CookbookPostDataParser cookbookPostDataParser;
    private readonly CreateCookbookService createCookbookService;
    private readonly RedirectService redirectService;
    private readonly ShowRecipes showRecipes;

    public NewCookbookRequestHandler(
        NewCookbookPageRenderer newCookbookPageRenderer,
        CookbookPostDataParser cookbookPostDataParser,
        ShowRecipes showRecipes,
        CreateCookbookService createCookbookService,
        SessionService sessionService,
        NotFoundPageRenderer notFoundPageRenderer,
        RedirectService redirectService,
        HTMLFileWriter htmlFileWriter)
        : base(htmlFileWriter, notFoundPageRenderer, sessionService)
    {
        this.newCookbookPageRenderer = newCookbookPageRenderer;
        this.cookbookPostDataParser = cookbookPostDataParser;
        this.createCookbookService = createCookbookService;
        this.redirectService = redirectService;
        this.showRecipes = showRecipes;
    }

    public override bool CanHandle(HttpListenerRequest request) =>
        (request.HttpMethod == HttpMethod.Get.Method || request.HttpMethod == HttpMethod.Post.Method)
        && request.Url?.AbsolutePath == "/cookbook/new";

    public override Task Handle(HttpListenerRequest request, HttpListenerResponse response, Chef currentChef)
    {
        if (request.HttpMethod == HttpMethod.Get.Method)
        {
            return HandleGetRequest(request, response, currentChef);
        }
        else
        {
            return HandlePostRequest(request, response, currentChef);
        }
    }

    private Task HandleGetRequest(HttpListenerRequest request, HttpListenerResponse response, Chef currentChef)
    {
        var recipes = showRecipes.ShowAllRecipes(currentChef);
        return newCookbookPageRenderer.RenderPage(
            response,
            HttpStatusCode.OK,
            recipes,
            currentChef
        );
    }

    private Task HandlePostRequest(HttpListenerRequest request, HttpListenerResponse response, Chef currentChef)
    {
        if (currentChef is null)
        {
            response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Task.CompletedTask;
        }

        var recipes = showRecipes.ShowAllRecipes(currentChef);

        var data = cookbookPostDataParser.ParsePostData(request);
        if (data.IsError)
        {
            return newCookbookPageRenderer.RenderPage(
                response,
                HttpStatusCode.BadRequest,
                recipes,
                currentChef,
                data.ErrorMessages
            );
        }

        var cookbook = createCookbookService.CreateCookbook(
            data.Value.Title,
            data.Value.Description,
            currentChef,
            data.Value.Visibility,
            data.Value.Recipes
        );

        redirectService.RedirectToPage(response, "/cookbook/" + cookbook.Identifier);
        return Task.CompletedTask;
    }
}