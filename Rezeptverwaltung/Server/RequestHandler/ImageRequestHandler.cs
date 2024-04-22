using Core.Interfaces;
using Core.Services;
using Core.ValueObjects;
using Server.Service;
using System.Net;
using System.Text.RegularExpressions;

namespace Server.RequestHandler;

public partial class ImageRequestHandler : RequestHandler
{
    private readonly ImageService imageService;
    private readonly ImageTypeMimeTypeConverter imageTypeMimeTypeConverter;
    private readonly NotFoundRequestHandler notFoundRequestHandler;
    private readonly ShowChefs showChefs;
    private readonly ShowRecipes showRecipes;

    [GeneratedRegex("^/images/chef/(?<chef_username>[A-Z0-9_ ]+)/?$", RegexOptions.NonBacktracking | RegexOptions.IgnoreCase, matchTimeoutMilliseconds: 1000)]
    private static partial Regex chefImageUrlPathRegex();

    [GeneratedRegex("^/images/recipe/(?<recipe_id>[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-4[a-fA-F0-9]{3}-[89AB][a-fA-F0-9]{3}-[a-fA-F0-9]{12})/?$", RegexOptions.NonBacktracking | RegexOptions.IgnoreCase, matchTimeoutMilliseconds: 1000)]
    private static partial Regex recipeImageUrlPathRegex();

    public ImageRequestHandler(
        ImageService imageService,
        ImageTypeMimeTypeConverter imageTypeMimeTypeConverter,
        NotFoundRequestHandler notFoundRequestHandler,
        ShowChefs showChefs,
        ShowRecipes showRecipes)
        : base()
    {
        this.imageService = imageService;
        this.imageTypeMimeTypeConverter = imageTypeMimeTypeConverter;
        this.notFoundRequestHandler = notFoundRequestHandler;
        this.showChefs = showChefs;
        this.showRecipes = showRecipes;
    }

    public bool CanHandle(HttpListenerRequest request)
    {
        if (request.HttpMethod != HttpMethod.Get.Method)
        {
            return false;
        }

        if (chefImageUrlPathRegex().IsMatch(request.Url?.AbsolutePath ?? ""))
        {
            return true;
        }

        if (recipeImageUrlPathRegex().IsMatch(request.Url?.AbsolutePath ?? ""))
        {
            return true;
        }

        return false;
    }

    public Task Handle(HttpListenerRequest request, HttpListenerResponse response)
    {
        if (chefImageUrlPathRegex().IsMatch(request.Url?.AbsolutePath ?? ""))
        {
            return HandleChefImageRequest(request, response);
        }
        else
        {
            return HandleRecipeImageRequest(request, response);
        }
    }

    private Task HandleChefImageRequest(HttpListenerRequest request, HttpListenerResponse response)
    {
        var chefUsername = chefImageUrlPathRegex().Match(request.Url?.AbsolutePath ?? "")!.Groups["chef_username"]!.Value;
        var chef = showChefs.ShowSingleChef(new Username(chefUsername));
        return WriteImageForNullableEntity(request, response, chef);
    }

    private Task HandleRecipeImageRequest(HttpListenerRequest request, HttpListenerResponse response)
    {
        var recipeId = Identifier.Parse(recipeImageUrlPathRegex().Match(request.Url?.AbsolutePath ?? "")!.Groups["recipe_id"]!.Value);
        var recipe = showRecipes.ShowSingleRecipe(recipeId, null);
        return WriteImageForNullableEntity(request, response, recipe);
    }

    private Task WriteImageForNullableEntity<T>(HttpListenerRequest request, HttpListenerResponse response, T? entity)
        where T : UniqueIdentity
    {
        if (entity is null)
        {
            return notFoundRequestHandler.Handle(request, response);
        }

        var image = imageService.GetImageFor(entity);
        if (image is null)
        {
            return notFoundRequestHandler.Handle(request, response);
        }

        return WriteImageToResponse(response, image.Value);
    }

    private async Task WriteImageToResponse(HttpListenerResponse response, Image image)
    {
        var (data, imageType) = image;
        var mimeType = imageTypeMimeTypeConverter.ConvertImageTypeToMimeType(imageType);

        response.ContentType = mimeType;
        response.ContentLength64 = data.Length;
        await data.CopyToAsync(response.OutputStream);
        image.Data.Close();
    }
}