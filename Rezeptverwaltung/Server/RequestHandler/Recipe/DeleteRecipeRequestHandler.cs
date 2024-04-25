using Core.Services;
using Core.ValueObjects;
using Server.Service;
using Server.Session;
using System.Net;
using System.Text.RegularExpressions;

namespace Server.RequestHandler;

public partial class DeleteRecipeRequestHandler : RequestHandler
{
    [GeneratedRegex("^/recipe/(?<recipe_id>[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-4[a-fA-F0-9]{3}-[89AB][a-fA-F0-9]{3}-[a-fA-F0-9]{12})/delete/?$", RegexOptions.NonBacktracking | RegexOptions.IgnoreCase, matchTimeoutMilliseconds: 1000)]
    private static partial Regex deleteRecipeUrlPathRegex();

    private readonly SessionService sessionService;
    private readonly DeleteRecipeService deleteRecipeService;
    private readonly RedirectService redirectService;

    public DeleteRecipeRequestHandler(
        SessionService sessionService,
        DeleteRecipeService deleteRecipeService,
        RedirectService redirectService)
        : base()
    {
        this.sessionService = sessionService;
        this.deleteRecipeService = deleteRecipeService;
        this.redirectService = redirectService;
    }

    public bool CanHandle(HttpListenerRequest request) =>
        request.HttpMethod == HttpMethod.Post.Method &&
        deleteRecipeUrlPathRegex().IsMatch(request.Url?.AbsolutePath ?? "");

    public Task Handle(HttpListenerRequest request, HttpListenerResponse response)
    {
        var currentChef = sessionService.GetCurrentChef(request);
        if (currentChef is null)
        {
            response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return Task.CompletedTask;
        }

        var match = deleteRecipeUrlPathRegex().Match(request.Url?.AbsolutePath ?? "");
        if (!match.Success)
        {
            response.StatusCode = (int)HttpStatusCode.NotFound;
            return Task.CompletedTask;
        }

        var recipeId = Identifier.Parse(match.Groups["recipe_id"].Value);
        if (recipeId is null)
        {
            response.StatusCode = (int)HttpStatusCode.NotFound;
            return Task.CompletedTask;
        }

        var ok = deleteRecipeService.DeleteRecipe(recipeId.Value, currentChef);
        if (!ok)
        {
            response.StatusCode = (int)HttpStatusCode.Forbidden;
        }
        else
        {
            redirectService.RedirectToPage(response, "/");
        }

        return Task.CompletedTask;
    }
}