using Core.Entities;
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
    private readonly CreateRecipeService createRecipeService;
    private readonly ImageTypeMimeTypeConverter imageTypeMimeTypeConverter;
    private readonly NewRecipePageRenderer newRecipePageRenderer;
    private readonly NotFoundRequestHandler notFoundRequestHandler;
    private readonly RecipePostDataParser recipePostDataParser;
    private readonly RedirectService redirectService;
    private readonly SessionService sessionService;

    public NewRecipeRequestHandler(
        CreateRecipeService createRecipeService,
        ImageTypeMimeTypeConverter imageTypeMimeTypeConverter,
        NewRecipePageRenderer newRecipePageRenderer,
        NotFoundRequestHandler notFoundRequestHandler,
        RecipePostDataParser recipePostDataParser,
        RedirectService redirectService,
        SessionService sessionService)
        : base()
    {
        this.newRecipePageRenderer = newRecipePageRenderer;
        this.recipePostDataParser = recipePostDataParser;
        this.createRecipeService = createRecipeService;
        this.imageTypeMimeTypeConverter = imageTypeMimeTypeConverter;
        this.redirectService = redirectService;
        this.sessionService = sessionService;
        this.notFoundRequestHandler = notFoundRequestHandler;
    }

    public bool CanHandle(HttpListenerRequest request) =>
        (request.HttpMethod == HttpMethod.Get.Method || request.HttpMethod == HttpMethod.Post.Method)
        && request.Url?.AbsolutePath == "/recipe/new";

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
        return newRecipePageRenderer.RenderPage(
            response,
            HttpStatusCode.OK,
            null,
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

        var data = recipePostDataParser.ParsePostData(request);
        if (data.IsError)
        {
            return newRecipePageRenderer.RenderPage(
                response,
                HttpStatusCode.BadRequest,
                null,
                currentChef,
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
                [ErrorMessages.INVALID_IMAGE_MIME_TYPE]
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