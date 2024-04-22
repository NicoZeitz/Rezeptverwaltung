using System.Net;

namespace Server.ValueObjects.PostData;

public interface SettingsPostData
{
    Task HandlePostRequest(HttpListenerRequest request, HttpListenerResponse response);
}