using Core.Entities;
using Core.ValueObjects;
using System.Net;

namespace Server.Session;

public class CookieSessionService : SessionService
{
    private static readonly string SESSION_COOKIE_NAME = "session";
    private static readonly Duration REFRESH_LOGIN_TIMEOUT = new Duration(TimeSpan.FromHours(3));

    private readonly SessionBackend<Chef> sessionBackend;
    private readonly DateTimeProvider dateTimeProvider;

    public CookieSessionService(SessionBackend<Chef> sessionBackend, DateTimeProvider dateTimeProvider)
    {
        this.sessionBackend = sessionBackend;
        this.dateTimeProvider = dateTimeProvider;
    }

    public Chef? GetCurrentChef(HttpListenerRequest request)
    {
        if (request.Cookies[SESSION_COOKIE_NAME] is not Cookie cookie)
        {
            return null;
        }

        if (cookie.Expired)
        {
            return null;
        }

        if (cookie.Value is not string sessionId)
        {
            return null;
        }

        return sessionBackend.GetBySessionId(Identifier.Parse(sessionId));
    }

    public void Login(HttpListenerRequest request, HttpListenerResponse response, Chef chef)
    {
        var sessionId = sessionBackend.CreateSession(chef, REFRESH_LOGIN_TIMEOUT);
        var sessionCookie = new Cookie(SESSION_COOKIE_NAME, sessionId.ToString())
        {
            HttpOnly = true,
            Comment = "Session Cookie",
            Expires = dateTimeProvider.Now.Add(REFRESH_LOGIN_TIMEOUT.TimeSpan),
        };

        response.SetCookie(sessionCookie);
    }

    public void Logout(HttpListenerRequest request, HttpListenerResponse response)
    {
        if (request.Cookies[SESSION_COOKIE_NAME] is Cookie cookie)
        {
            sessionBackend.RemoveSession(Identifier.Parse(cookie.Value));
        };

        response.SetCookie(new Cookie(SESSION_COOKIE_NAME, "")
        {
            Expired = true,
            HttpOnly = true,
            Expires = dateTimeProvider.Now.AddYears(-1),
        });
    }
}
