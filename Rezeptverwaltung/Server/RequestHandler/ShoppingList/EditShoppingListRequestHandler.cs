using Server.Service;
using System.Net;

namespace Server.RequestHandler;

public class EditShoppingListRequestHandler : HTMLRequestHandler
{
    public EditShoppingListRequestHandler(HTMLFileWriter htmlFileWriter)
        : base(htmlFileWriter)
    {
    }

    public override bool CanHandle(HttpListenerRequest request) =>
        request.HttpMethod == HttpMethod.Get.Method
        && request.Url?.AbsolutePath == "/shopping-list/edit";

    public override Task<string> HandleHtmlFileRequest(HttpListenerRequest request)
    {
        return Task.FromResult("TODO: IMPLEMENT EDIT SHOPPING LIST REQUEST HANDLER");
    }
}