using Server.PageRenderer;
using Server.Service;
using Server.Session;
using System.Net;

namespace Server.RequestHandler;

public class NewShoppingListRequestHandler(HTMLFileWriter htmlFileWriter, NotFoundPageRenderer notFoundPageRenderer, SessionService sessionService) : HTMLRequestHandler(htmlFileWriter, notFoundPageRenderer, sessionService)
{
    public override bool CanHandle(HttpListenerRequest request) =>
        request.HttpMethod == HttpMethod.Get.Method
        && request.Url?.AbsolutePath == "/shopping-list/new";

    public override Task<string> HandleHtmlFileRequest(HttpListenerRequest request)
    {
        return Task.FromResult("TODO: IMPLEMENT NEW SHOPPING LIST REQUEST HANDLER");
    }
}