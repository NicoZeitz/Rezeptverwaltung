using Server.Service;
using System.Net;

namespace Server.RequestHandler;

public class EditCookbookRequestHandler : HTMLRequestHandler
{
    public EditCookbookRequestHandler(HTMLFileWriter htmlFileWriter)
        : base(htmlFileWriter)
    {
    }

    public override bool CanHandle(HttpListenerRequest request) =>
        request.HttpMethod == HttpMethod.Get.Method
        && request.Url?.AbsolutePath == "/cookbook/edit";

    public override Task<string> HandleHtmlFileRequest(HttpListenerRequest request)
    {
        return Task.FromResult("TODO: IMPLEMENT EDIT COOKBOOK REQUEST HANDLER");
    }
}