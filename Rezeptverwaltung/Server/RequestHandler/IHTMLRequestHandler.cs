using System.Net;
using System.Text;

namespace Server.RequestHandler;

public interface IHTMLRequestHandler : IRequestHandler
{
    async Task IRequestHandler.Handle(HttpListenerRequest request, HttpListenerResponse response)
    {
        var htmlFile = await HandleHtmlFileRequest(request);

        response.StatusCode = HttpStatus.OK.Code;
        response.StatusDescription = HttpStatus.OK.Description;
        response.ContentType = MimeType.HTML;
        response.OutputStream.Write(Encoding.UTF8.GetBytes(htmlFile));
    }

    Task<string> HandleHtmlFileRequest(HttpListenerRequest request);
}