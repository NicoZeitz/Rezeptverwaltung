using Server.Service;
using System.Net;

namespace Server.RequestHandler;

public class EditRecipeRequestHandler : HTMLRequestHandler
{
    public EditRecipeRequestHandler(HTMLFileWriter htmlFileWriter)
        : base(htmlFileWriter)
    {
    }

    public override bool CanHandle(HttpListenerRequest request) =>
        request.HttpMethod == HttpMethod.Get.Method
        && request.Url?.AbsolutePath == "/recipe/edit";

    public override Task<string> HandleHtmlFileRequest(HttpListenerRequest request)
    {
        return Task.FromResult("TODO: IMPLEMENT EDIT RECIPE REQUEST HANDLER");
    }
}