using System.Net;

namespace Server.RequestHandler;

public interface IRequestHandler
{
    bool CanHandle(HttpListenerRequest request);

    Task Handle(HttpListenerRequest request, HttpListenerResponse response);
}
