using Core.Entities;
using Core.ValueObjects;
using Server.Session;
using System.Net;

namespace Server.RequestHandler;

public class LogoutRequestHandler : IRequestHandler
{
    private readonly ISessionService<Chef> sessionService;

    public LogoutRequestHandler(ISessionService<Chef> sessionService)
    {
        this.sessionService = sessionService;
    }

    public bool CanHandle(HttpListenerRequest request)
    {
        return request.Url?.AbsolutePath is "/logout" && request.HttpMethod is "POST";
    }

    public Task Handle(HttpListenerRequest request, HttpListenerResponse response)
    {
        if (request.Cookies["session"] is Cookie cookie)
        {
            sessionService.RemoveSession(Identifier.Parse(cookie.Value));
        };

        response.SetCookie(new Cookie("session", "")
        {
            Expired = true,
            HttpOnly = true,
            Expires = DateTime.Now.AddYears(-1),
        });
        response.StatusCode = HttpStatus.SEE_OTHER.Code;
        response.StatusDescription = HttpStatus.SEE_OTHER.Description;
        response.RedirectLocation = "/";
        return Task.CompletedTask;
    }
}
