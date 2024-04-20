using Server.Session;
using System.Net;

namespace Server.RequestHandler;

public class LogoutRequestHandler : RequestHandler
{
    private readonly SessionService sessionService;

    public LogoutRequestHandler(SessionService sessionService)
    {
        this.sessionService = sessionService;
    }

    public bool CanHandle(HttpListenerRequest request) =>
        request.HttpMethod == HttpMethod.Post.Method
        && request.Url?.AbsolutePath is "/logout";

    public Task Handle(HttpListenerRequest request, HttpListenerResponse response)
    {
        sessionService.Logout(request, response);

        response.StatusCode = (int)HttpStatusCode.SeeOther;
        response.RedirectLocation = "/";
        return Task.CompletedTask;
    }
}
