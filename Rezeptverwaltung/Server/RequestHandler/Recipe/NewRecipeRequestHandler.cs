using Core.Services;
using Core.ValueObjects;
using Server.DataParser;
using Server.PageRenderer;
using Server.Service;
using Server.Session;
using System.Net;

namespace Server.RequestHandler;

public class NewRecipeRequestHandler : RequestHandler
{
    private readonly RecipeEditPageRenderer newRecipePageRenderer;
    private readonly SessionService sessionService;
    private readonly ShowRecipes showRecipes;
    private readonly RecipePostDataParser recipePostDataParser;
    private readonly CreateRecipeService createRecipeService;
    private readonly ImageTypeMimeTypeConverter imageTypeMimeTypeConverter;
    private readonly RedirectService redirectService;

    public NewRecipeRequestHandler(
        RecipeEditPageRenderer newRecipePageRenderer,
        SessionService sessionService,
        ShowRecipes showRecipes,
        RecipePostDataParser recipePostDataParser,
        CreateRecipeService createRecipeService,
        ImageTypeMimeTypeConverter imageTypeMimeTypeConverter,
        RedirectService redirectService)
        : base()
    {
        this.newRecipePageRenderer = newRecipePageRenderer;
        this.sessionService = sessionService;
        this.showRecipes = showRecipes;
        this.recipePostDataParser = recipePostDataParser;
        this.createRecipeService = createRecipeService;
        this.imageTypeMimeTypeConverter = imageTypeMimeTypeConverter;
        this.redirectService = redirectService;
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
        if (currentChef is null)
        {
            response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Task.CompletedTask;
        }

        var tags = showRecipes.ShowAllTags(currentChef);
        var ingredients = showRecipes.ShowAllIngredients(currentChef);

        var data = recipePostDataParser.ParsePostData(request);
        if (data.IsError)
        {
            return newRecipePageRenderer.RenderPage(
                response,
                HttpStatusCode.BadRequest,
                null,
                currentChef,
                tags,
                ingredients,
                data.ErrorMessages
            );
        }

        var imageType = imageTypeMimeTypeConverter.ConvertMimeTypeToImageType(data.Value.Image.FileMimeType!.Value);
        if (imageType is null)
        {
            return newRecipePageRenderer.RenderPage(
                response,
                HttpStatusCode.BadRequest,
                null,
                currentChef,
                tags,
                ingredients,
                [new ErrorMessage("Bilddatei nicht erlaubt! Bitte lade ein anderes Bild hoch")] // TODO: error messages into own class static attributes for dry
            );
        }

        var image = new Image(data.Value.Image.FileData!, imageType.Value);

        var recipe = createRecipeService.CreateRecipe(
            data.Value.Title,
            data.Value.Description,
            currentChef,
            data.Value.Visibility,
            data.Value.Portion,
            data.Value.Duration,
            data.Value.Tags,
            data.Value.PreparationSteps,
            data.Value.Ingredients,
            image
        );

        redirectService.RedirectToPage(response, "/recipe/" + recipe.Identifier);
        return Task.CompletedTask;
    }
}