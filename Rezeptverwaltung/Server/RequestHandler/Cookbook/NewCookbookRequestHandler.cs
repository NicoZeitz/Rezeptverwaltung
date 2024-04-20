using Server.Service;
using System.Net;

namespace Server.RequestHandler;

public class NewCookbookRequestHandler : HTMLRequestHandler
{
    public NewCookbookRequestHandler(HTMLFileWriter htmlFileWriter)
        : base(htmlFileWriter)
    {
    }

    public override bool CanHandle(HttpListenerRequest request) =>
        request.HttpMethod == HttpMethod.Get.Method
        && request.Url?.AbsolutePath == "/cookbook/new";

    public override Task<string> HandleHtmlFileRequest(HttpListenerRequest request)
    {
        return Task.FromResult("TODO: IMPLEMENT NEW COOKBOOK REQUEST HANDLER");
    }
}