using Server.Service;
using System.Net;

namespace Server.RequestHandler;

public class NewShoppingListRequestHandler : HTMLRequestHandler
{
    public NewShoppingListRequestHandler(HTMLFileWriter htmlFileWriter)
        : base(htmlFileWriter)
    {
    }

    public override bool CanHandle(HttpListenerRequest request) =>
        request.HttpMethod == HttpMethod.Get.Method
        && request.Url?.AbsolutePath == "/shopping-list/new";

    public override Task<string> HandleHtmlFileRequest(HttpListenerRequest request)
    {
        return Task.FromResult("TODO: IMPLEMENT NEW SHOPPING LIST REQUEST HANDLER");
    }
}