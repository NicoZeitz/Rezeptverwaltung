using Core.Entities;
using Core.Services;
using Server.DataParser;
using Server.PageRenderer;
using Server.Service;
using Server.Session;
using System.Net;

namespace Server.RequestHandler;

public class NewShoppingListRequestHandler : RequestHandler
{
    private readonly CreateShoppingListService createShoppingListService;
    private readonly NewShoppingListPageRenderer newShoppingListPageRenderer;
    private readonly NotFoundRequestHandler notFoundRequestHandler;
    private readonly RedirectService redirectService;
    private readonly SessionService sessionService;
    private readonly ShoppingListPostDataParser shoppingListPostDataParser;
    private readonly ShowRecipes showRecipes;

    public NewShoppingListRequestHandler(
        CreateShoppingListService createShoppingListService,
        NewShoppingListPageRenderer newShoppingListPageRenderer,
        NotFoundRequestHandler notFoundRequestHandler,
        RedirectService redirectService,
        SessionService sessionService,
        ShoppingListPostDataParser shoppingListPostDataParser,
        ShowRecipes showRecipes)
        : base()
    {
        this.newShoppingListPageRenderer = newShoppingListPageRenderer;
        this.shoppingListPostDataParser = shoppingListPostDataParser;
        this.createShoppingListService = createShoppingListService;
        this.redirectService = redirectService;
        this.sessionService = sessionService;
        this.notFoundRequestHandler = notFoundRequestHandler;
        this.showRecipes = showRecipes;
    }

    public bool CanHandle(HttpListenerRequest request) =>
        (request.HttpMethod == HttpMethod.Get.Method || request.HttpMethod == HttpMethod.Post.Method)
        && request.Url?.AbsolutePath == "/shopping-list/new";

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
        return newShoppingListPageRenderer.RenderPage(
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

        var data = shoppingListPostDataParser.ParsePostData(request);
        if (data.IsError)
        {
            return newShoppingListPageRenderer.RenderPage(
                response,
                HttpStatusCode.BadRequest,
                recipes,
                currentChef,
                data.ErrorMessages
            );
        }

        var shoppingList = createShoppingListService.CreateShoppingList(
            data.Value.Title,
            currentChef,
            data.Value.Visibility,
            data.Value.PortionedRecipes
        );

        redirectService.RedirectToPage(response, "/shopping-list/" + shoppingList.Identifier);
        return Task.CompletedTask;
    }
}