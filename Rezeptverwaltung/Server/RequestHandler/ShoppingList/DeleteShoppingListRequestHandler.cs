using Core.Services;
using Core.ValueObjects;
using Server.Service;
using Server.Session;
using System.Net;
using System.Text.RegularExpressions;

namespace Server.RequestHandler;

public partial class DeleteShoppingListRequestHandler : RequestHandler
{
    [GeneratedRegex("^/shopping-list/(?<shopping_list_id>[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-4[a-fA-F0-9]{3}-[89AB][a-fA-F0-9]{3}-[a-fA-F0-9]{12})/delete/?$", RegexOptions.NonBacktracking | RegexOptions.IgnoreCase, matchTimeoutMilliseconds: 1000)]
    private static partial Regex deleteShoppingListUrlPathRegex();

    private readonly SessionService sessionService;
    private readonly DeleteShoppingListService deleteShoppingListService;
    private readonly RedirectService redirectService;

    public DeleteShoppingListRequestHandler(
        SessionService sessionService,
        DeleteShoppingListService deleteShoppingListService,
        RedirectService redirectService)
        : base()
    {
        this.sessionService = sessionService;
        this.deleteShoppingListService = deleteShoppingListService;
        this.redirectService = redirectService;
    }

    public bool CanHandle(HttpListenerRequest request) =>
        request.HttpMethod == HttpMethod.Post.Method &&
        deleteShoppingListUrlPathRegex().IsMatch(request.Url?.AbsolutePath ?? "");

    public Task Handle(HttpListenerRequest request, HttpListenerResponse response)
    {
        var currentChef = sessionService.GetCurrentChef(request);
        if (currentChef is null)
        {
            response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return Task.CompletedTask;
        }

        var match = deleteShoppingListUrlPathRegex().Match(request.Url?.AbsolutePath ?? "");
        if (!match.Success)
        {
            response.StatusCode = (int)HttpStatusCode.NotFound;
            return Task.CompletedTask;
        }

        var shoppingListId = Identifier.Parse(match.Groups["shopping_list_id"].Value);
        if (shoppingListId is null)
        {
            response.StatusCode = (int)HttpStatusCode.NotFound;
            return Task.CompletedTask;
        }

        var ok = deleteShoppingListService.DeleteShoppingList(shoppingListId.Value, currentChef);
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