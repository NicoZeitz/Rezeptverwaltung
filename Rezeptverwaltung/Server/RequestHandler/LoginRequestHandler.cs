using Core.Services;
using Core.ValueObjects;
using Server.ContentParser;
using Server.Resources;
using Server.Session;
using System.Net;
using System.Text;

namespace Server.RequestHandler;
record struct LoginData(Username Username, Password Password);

public class LoginRequestHandler : RequestHandler
{
    private static readonly ErrorMessage INVALID_CREDENTIALS_ERROR_MESSAGE = new ErrorMessage("Benutzername und/oder Passwort falsch!");

    private readonly ChefLoginService chefLoginService;
    private readonly SessionService sessionService;
    private readonly TemplateLoader templateLoader;
    private readonly ContentParserFactory contentParserFactory;

    public LoginRequestHandler(
      ChefLoginService chefLoginService,
      TemplateLoader templateLoader,
      SessionService sessionService,
      ContentParserFactory contentParserFactory
  )
    {
        this.chefLoginService = chefLoginService;
        this.templateLoader = templateLoader;
        this.sessionService = sessionService;
        this.contentParserFactory = contentParserFactory;
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
                HttpStatusCode.OK
            );
        }

        return HandlePost(request, response);
    }

    private Task HandlePost(HttpListenerRequest request, HttpListenerResponse response)
    {
        var usernameAndPassword = ParsePostContentData(request);
        if (!usernameAndPassword.HasValue)
        {
            return RenderLoginPage(
                request,
                response,
                HttpStatusCode.Unauthorized,
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
                HttpStatusCode.Forbidden,
                INVALID_CREDENTIALS_ERROR_MESSAGE
            );
        }

        sessionService.Login(request, response, chef);

        response.StatusCode = (int)HttpStatusCode.SeeOther;
        response.RedirectLocation = "/";
        return Task.CompletedTask;
    }

    private async Task RenderLoginPage(HttpListenerRequest request, HttpListenerResponse response, HttpStatusCode httpStatus, ErrorMessage? errorMessage = null)
    {
        var currentChef = sessionService.GetCurrentChef(request);
        var loginTemplate = templateLoader.LoadTemplate("login.html")!;

        var loginPage = await loginTemplate.RenderAsync(new
        {
            ErrorMessage = errorMessage,
            Header = await new Components.Header(templateLoader) { CurrentChef = currentChef }.RenderAsync(),
        });

        response.StatusCode = (int)httpStatus;
        await response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(loginPage));
    }

    private LoginData? ParsePostContentData(HttpListenerRequest request)
    {
        var contentParser = contentParserFactory.CreateContentParser(request.ContentType);

        if (!contentParser.CanParse(request))
        {
            return null;
        }

        var content = contentParser.ParseRequest(request);

        if (!content.TryGetValue("username", out var username) && username!.IsText)
        {
            return null;
        }
        if (!content.TryGetValue("password", out var password) && password!.IsText)
        {
            return null;
        }

        return new LoginData(
            new Username(username.TextValue!),
            new Password(password.TextValue!)
        );
    }
}
