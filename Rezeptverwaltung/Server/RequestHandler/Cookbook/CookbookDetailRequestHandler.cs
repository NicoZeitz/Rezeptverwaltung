using Server.Service;
using System.Net;
using System.Text.RegularExpressions;

namespace Server.RequestHandler;

public partial class CookbookDetailRequestHandler : HTMLRequestHandler
{
    [GeneratedRegex("/cookbook/(?<cookbook_id>[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-4[a-fA-F0-9]{3}-[89AB][a-fA-F0-9]{3}-[a-fA-F0-9]{12})", RegexOptions.NonBacktracking | RegexOptions.IgnoreCase, matchTimeoutMilliseconds: 1000)]
    private static partial Regex cookbookUrlPathRegex();

    public CookbookDetailRequestHandler(HTMLFileWriter htmlFileWriter)
        : base(htmlFileWriter)
    {
    }

    public override bool CanHandle(HttpListenerRequest request) =>
        request.HttpMethod == HttpMethod.Get.Method
        && cookbookUrlPathRegex().IsMatch(request.Url?.AbsolutePath ?? "");

    public override Task<string> HandleHtmlFileRequest(HttpListenerRequest request)
    {
        return Task.FromResult("TODO: IMPLEMENT COOKBOOK DETAIL REQUEST HANDLER");
    }
}