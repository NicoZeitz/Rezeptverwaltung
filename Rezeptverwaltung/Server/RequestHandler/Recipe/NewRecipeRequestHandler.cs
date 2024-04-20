using Server.Service;
using System.Net;

namespace Server.RequestHandler;

public class NewRecipeRequestHandler : HTMLRequestHandler
{
    public NewRecipeRequestHandler(HTMLFileWriter htmlFileWriter)
        : base(htmlFileWriter)
    {
    }

    public override bool CanHandle(HttpListenerRequest request) =>
        request.HttpMethod == HttpMethod.Get.Method
        && request.Url?.AbsolutePath == "/recipe/new";

    public override Task<string> HandleHtmlFileRequest(HttpListenerRequest request)
    {
        return Task.FromResult("TODO: IMPLEMENT NEW RECIPE REQUEST HANDLER");
    }
}