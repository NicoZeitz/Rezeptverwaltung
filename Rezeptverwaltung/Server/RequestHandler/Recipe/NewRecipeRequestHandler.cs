using Core.Entities;
using Core.Services;
using Core.Services.Serialization;
using Core.ValueObjects;
using Server.DataParser;
using Server.PageRenderer;
using Server.Service;
using Server.Session;
using System.Net;

namespace Server.RequestHandler;

public class NewRecipeRequestHandler : AuthorizedRequestHandler
{
    private readonly NewRecipePageRenderer newRecipePageRenderer;
    private readonly RecipePostDataParser recipePostDataParser;
    private readonly CreateRecipeService createRecipeService;
    private readonly ImageTypeMimeTypeConverter imageTypeMimeTypeConverter;
    private readonly RedirectService redirectService;

    public NewRecipeRequestHandler(
        NewRecipePageRenderer newRecipePageRenderer,
        SessionService sessionService,
        RecipePostDataParser recipePostDataParser,
        CreateRecipeService createRecipeService,
        ImageTypeMimeTypeConverter imageTypeMimeTypeConverter,
        RedirectService redirectService,
        NotFoundPageRenderer notFoundPageRenderer,
        HTMLFileWriter htmlFileWriter)
        : base(htmlFileWriter, notFoundPageRenderer, sessionService)
    {
        this.newRecipePageRenderer = newRecipePageRenderer;
        this.recipePostDataParser = recipePostDataParser;
        this.createRecipeService = createRecipeService;
        this.imageTypeMimeTypeConverter = imageTypeMimeTypeConverter;
        this.redirectService = redirectService;
    }

    public override bool CanHandle(HttpListenerRequest request) =>
        (request.HttpMethod == HttpMethod.Get.Method || request.HttpMethod == HttpMethod.Post.Method)
        && request.Url?.AbsolutePath == "/recipe/new";

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