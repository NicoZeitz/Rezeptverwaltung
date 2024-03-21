using Core.Entities;
using Core.Services;
using Core.ValueObjects;
using Scriban;
using Server.Component;
using Server.ResourceLoader;
using Server.Session;
using System.Net;
using System.Text;
using System.Web;

namespace Server.RequestHandler;

public class LoginRequestHandler : IRequestHandler
{
    private const int EXPIRE_AFTER_HOURS = 3;
    private static readonly ErrorMessage INVALID_CREDENTIALS_ERROR_MESSAGE = new ErrorMessage("Benutzername und/oder Passwort falsch!");

    private readonly ChefLoginService chefLoginService;
    private readonly IResourceLoader resourceLoader;
    private readonly ISessionService<Chef> sessionService;

    public LoginRequestHandler(
      ChefLoginService chefLoginService,
      IResourceLoader resourceLoader,
      ISessionService<Chef> sessionService
  )
    {
        this.chefLoginService = chefLoginService;
        this.resourceLoader = resourceLoader;
        this.sessionService = sessionService;
    }

    public bool CanHandle(HttpListenerRequest request)
    {
        if (request.Url?.AbsolutePath != "/login")
        {
            return false;
        }

        if (request.HttpMethod == "GET")
        {
            return true;
        }

        return request.HttpMethod == "POST" && request.HasEntityBody;
    }

    public Task Handle(HttpListenerRequest request, HttpListenerResponse response)
    {
        if (request.HttpMethod == "GET")
        {
            return RenderLoginPage(
                request,
                response,
                HttpStatus.OK
            );
        }

        return HandlePost(request, response);
    }

    public Task HandlePost(HttpListenerRequest request, HttpListenerResponse response)
    {
        var usernameAndPassword = GetUserNameAndPasswordFromRequest(request);
        if (!usernameAndPassword.HasValue)
        {
            return RenderLoginPage(
                request,
                response,
                HttpStatus.UNAUTHORIZED,
                INVALID_CREDENTIALS_ERROR_MESSAGE
            );
        }

        var (username, password) = usernameAndPassword.Value;
        var chef = chefLoginService.LoginChef(username, password);

        if (chef is null)
        {
            return RenderLoginPage(
                request,
                response,
                HttpStatus.FORBIDDEN,
                INVALID_CREDENTIALS_ERROR_MESSAGE
            );
        }

        var sessionId = sessionService.CreateSession(chef, new Duration(TimeSpan.FromHours(EXPIRE_AFTER_HOURS)));
        var sessionCookie = new Cookie("session", sessionId.ToString())
        {
            HttpOnly = true,
            Comment = "Session Cookie",
            Expires = DateTime.Now.AddHours(EXPIRE_AFTER_HOURS),
        };

        response.SetCookie(sessionCookie);
        response.StatusCode = HttpStatus.SEE_OTHER.Code;
        response.StatusDescription = HttpStatus.SEE_OTHER.Description;
        response.RedirectLocation = "/";
        return Task.CompletedTask;
    }

    private async Task RenderLoginPage(HttpListenerRequest request, HttpListenerResponse response, HttpStatus httpStatus, ErrorMessage? errorMessage = null)
    {
        Chef? currentChef = null;
        if (request.Cookies["session"] is Cookie cookie && !cookie.Expired && cookie.Value is string sessionId)
        {
            currentChef = sessionService.GetBySessionId(Identifier.Parse(sessionId));
        }

        using var loginStream = resourceLoader.LoadResource("login.html")!;
        var loginContent = new StreamReader(loginStream).ReadToEnd();
        var loginTemplate = Template.Parse(loginContent);

        var loginPage = await loginTemplate.RenderAsync(new
        {
            ErrorMessage = errorMessage,
            Header = await new Header(resourceLoader).RenderAsync(currentChef),
        });

        response.StatusCode = httpStatus.Code;
        response.StatusDescription = httpStatus.Description;
        await response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(loginPage));
    }

    private (Username, Password)? GetUserNameAndPasswordFromRequest(HttpListenerRequest request)
    {
        using var body = request.InputStream;
        using var reader = new StreamReader(body);
        var bodyString = reader.ReadToEnd();

        var queryValues = HttpUtility.ParseQueryString(bodyString);

        if (queryValues["username"] is null || queryValues["password"] is null)
        {
            return null;
        }

        return (new Username(queryValues["username"]!), new Password(queryValues["password"]!));
    }
}
