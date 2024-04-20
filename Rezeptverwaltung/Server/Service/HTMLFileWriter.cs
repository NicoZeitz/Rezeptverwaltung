using Server.ValueObjects;
using System.Net;
using System.Text;

namespace Server.Service;

public class HTMLFileWriter
{
    public void WriteHtmlFile(HttpListenerResponse response, string htmlFile, HttpStatusCode httpStatus = HttpStatusCode.OK)
    {
        var buffer = Encoding.UTF8.GetBytes(htmlFile);

        response.ContentLength64 = buffer.Length;
        response.ContentEncoding = Encoding.UTF8;
        response.StatusCode = (int)httpStatus;
        response.ContentType = MimeType.HTML;
        response.OutputStream.Write(buffer);
    }
}