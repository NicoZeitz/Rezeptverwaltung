using System.Net;

namespace Server.RequestHandler;

public interface SettingsPostData
{
    Task HandlePostRequest(HttpListenerRequest request, HttpListenerResponse response);
}