using System.Net;
using System.Text.RegularExpressions;
using Core.Services;
using Core.ValueObjects;
using Server.Service;
using Server.Session;

namespace Server.RequestHandler;

public partial class DeleteCookbookRequestHandler : RequestHandler
{
    [GeneratedRegex("^/cookbook/(?<cookbook_id>[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-4[a-fA-F0-9]{3}-[89AB][a-fA-F0-9]{3}-[a-fA-F0-9]{12})/delete/?$", RegexOptions.NonBacktracking | RegexOptions.IgnoreCase, matchTimeoutMilliseconds: 1000)]
    private static partial Regex deleteCookbookUrlPathRegex();

    private readonly SessionService sessionService;
    private readonly DeleteCookbookService deleteCookbookService;
    private readonly RedirectService redirectService;

    public DeleteCookbookRequestHandler(
        SessionService sessionService,
        DeleteCookbookService deleteCookbookService,
        RedirectService redirectService)
        : base()
    {
        this.sessionService = sessionService;
        this.deleteCookbookService = deleteCookbookService;
        this.redirectService = redirectService;
    }


    public bool CanHandle(HttpListenerRequest request) =>
        request.HttpMethod == HttpMethod.Post.Method &&
        deleteCookbookUrlPathRegex().IsMatch(request.Url?.AbsolutePath ?? "");

    public Task Handle(HttpListenerRequest request, HttpListenerResponse response)
    {
        var currentChef = sessionService.GetCurrentChef(request);
        if (currentChef is null)
        {
            response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return Task.CompletedTask;
        }

        var match = deleteCookbookUrlPathRegex().Match(request.Url?.AbsolutePath ?? "");
        if (!match.Success)
        {
            response.StatusCode = (int)HttpStatusCode.NotFound;
            return Task.CompletedTask;
        }

        var recipeId = Identifier.Parse(match.Groups["cookbook_id"].Value);
        var ok = deleteCookbookService.DeleteCookbook(recipeId, currentChef);
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