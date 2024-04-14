using Core.Entities;
using Core.ValueObjects;
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

    public bool CanHandle(HttpListenerRequest request)
    {
        return request.Url?.AbsolutePath is "/logout" && request.HttpMethod is "POST";
    }

    public Task Handle(HttpListenerRequest request, HttpListenerResponse response)
    {
        sessionService.Logout(request, response);

        response.StatusCode = (int)HttpStatusCode.SeeOther;
        response.RedirectLocation = "/";
        return Task.CompletedTask;
    }
}
