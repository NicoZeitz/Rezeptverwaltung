﻿using System.Net;
using System.Text;

namespace Server.RequestHandler;

public interface HTMLRequestHandler : RequestHandler
{
    async Task RequestHandler.Handle(HttpListenerRequest request, HttpListenerResponse response)
    {
        var htmlFile = await HandleHtmlFileRequest(request);

        response.StatusCode = (int)HttpStatusCode.OK;
        response.ContentType = MimeType.HTML;
        response.OutputStream.Write(Encoding.UTF8.GetBytes(htmlFile));
    }

    Task<string> HandleHtmlFileRequest(HttpListenerRequest request);
}