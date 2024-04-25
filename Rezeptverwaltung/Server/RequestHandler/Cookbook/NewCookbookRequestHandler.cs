using Core.Entities;
using Core.Services;
using Server.DataParser;
using Server.PageRenderer;
using Server.Service;
using Server.Session;
using System.Net;

namespace Server.RequestHandler;

public class NewCookbookRequestHandler : RequestHandler
{
    private readonly CookbookPostDataParser cookbookPostDataParser;
    private readonly CreateCookbookService createCookbookService;
    private readonly NewCookbookPageRenderer newCookbookPageRenderer;
    private readonly NotFoundRequestHandler notFoundRequestHandler;
    private readonly RedirectService redirectService;
    private readonly SessionService sessionService;
    private readonly ShowRecipes showRecipes;

    public NewCookbookRequestHandler(
        CookbookPostDataParser cookbookPostDataParser,
        CreateCookbookService createCookbookService,
        NewCookbookPageRenderer newCookbookPageRenderer,
        NotFoundRequestHandler notFoundRequestHandler,
        RedirectService redirectService,
        SessionService sessionService,
        ShowRecipes showRecipes)
        : base()
    {
        this.cookbookPostDataParser = cookbookPostDataParser;
        this.createCookbookService = createCookbookService;
        this.newCookbookPageRenderer = newCookbookPageRenderer;
        this.notFoundRequestHandler = notFoundRequestHandler;
        this.redirectService = redirectService;
        this.sessionService = sessionService;
        this.showRecipes = showRecipes;
    }

    public bool CanHandle(HttpListenerRequest request) =>
        (request.HttpMethod == HttpMethod.Get.Method || request.HttpMethod == HttpMethod.Post.Method)
        && request.Url?.AbsolutePath == "/cookbook/new";

    public Task Handle(HttpListenerRequest request, HttpListenerResponse response)
    {
        var currentChef = sessionService.GetCurrentChef(request);
        if (currentChef is null)
            return notFoundRequestHandler.Handle(request, response);

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