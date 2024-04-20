using Core.Services;
using Server.Session;
using System.Net;

namespace Server.RequestHandler;

public class NewRecipeRequestHandler : RequestHandler
{
    private readonly NewRecipePageRenderer newRecipePageRenderer;
    private readonly SessionService sessionService;
    private readonly ShowRecipes showRecipes;

    public NewRecipeRequestHandler(
        NewRecipePageRenderer newRecipePageRenderer,
        SessionService sessionService,
        ShowRecipes showRecipes)
        : base()
    {
        this.newRecipePageRenderer = newRecipePageRenderer;
        this.sessionService = sessionService;
        this.showRecipes = showRecipes;
    }

    public bool CanHandle(HttpListenerRequest request) =>
        (request.HttpMethod == HttpMethod.Get.Method || request.HttpMethod == HttpMethod.Post.Method)
        && request.Url?.AbsolutePath == "/recipe/new";

    public Task Handle(HttpListenerRequest request, HttpListenerResponse response)
    {
        if (request.HttpMethod == HttpMethod.Get.Method)
        {
            return HandleGetRequest(request, response);
        }
        else
        {
            return HandlePostRequest(request, response);
        }
    }

    private Task HandleGetRequest(HttpListenerRequest request, HttpListenerResponse response)
    {
        var currentChef = sessionService.GetCurrentChef(request);
        var tags = showRecipes.ShowAllTags(currentChef);
        var ingredients = showRecipes.ShowAllIngredients(currentChef);
        return newRecipePageRenderer.RenderPage(
            response,
            HttpStatusCode.OK,
            null,
            currentChef,
            tags,
            ingredients
        );
    }

    private Task HandlePostRequest(HttpListenerRequest request, HttpListenerResponse response)
    {
        var currentChef = sessionService.GetCurrentChef(request);

        //var formData = GetFormData(request); // TODO:

        return Task.CompletedTask;
    }
}