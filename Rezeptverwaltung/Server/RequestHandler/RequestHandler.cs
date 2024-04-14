using System.Net;

namespace Server.RequestHandler;

public interface RequestHandler
{
    bool CanHandle(HttpListenerRequest request);

    Task Handle(HttpListenerRequest request, HttpListenerResponse response);
}
