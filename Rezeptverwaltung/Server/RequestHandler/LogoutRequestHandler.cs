using Server.Service;
using Server.Session;
using System.Net;

namespace Server.RequestHandler;

public class LogoutRequestHandler : RequestHandler
{
    private readonly SessionService sessionService;
    private readonly RedirectService redirectService;

    public LogoutRequestHandler(
        SessionService sessionService,
        RedirectService redirectService)
        : base()
    {
        this.sessionService = sessionService;
        this.redirectService = redirectService;
    }

    public bool CanHandle(HttpListenerRequest request) =>
        request.HttpMethod == HttpMethod.Post.Method
        && request.Url?.AbsolutePath is "/logout";

    public Task Handle(HttpListenerRequest request, HttpListenerResponse response)
    {
        sessionService.Logout(request, response);

        redirectService.RedirectToPage(response, "/");
        return Task.CompletedTask;
    }
}
