using Server.PageRenderer;
using Server.Service;
using Server.Session;
using System.Net;
using System.Text.RegularExpressions;

namespace Server.RequestHandler;

public partial class ShoppingListDetailRequestHandler(HTMLFileWriter htmlFileWriter, NotFoundPageRenderer notFoundPageRenderer, SessionService sessionService) : HTMLRequestHandler(htmlFileWriter, notFoundPageRenderer, sessionService)
{
    [GeneratedRegex("^/shopping-list/(?<shopping_list_id>[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-4[a-fA-F0-9]{3}-[89AB][a-fA-F0-9]{3}-[a-fA-F0-9]{12})/?$", RegexOptions.NonBacktracking | RegexOptions.IgnoreCase, matchTimeoutMilliseconds: 1000)]
    private static partial Regex shoppingListUrlPathRegex();

    public override bool CanHandle(HttpListenerRequest request) =>
        request.HttpMethod == HttpMethod.Get.Method
        && shoppingListUrlPathRegex().IsMatch(request.Url?.AbsolutePath ?? "");

    public override Task<string> HandleHtmlFileRequest(HttpListenerRequest request)
    {
        return Task.FromResult("TODO: IMPLEMENT SHOPPING LIST DETAIL REQUEST HANDLER");
    }
}