using Core.Entities;
using Core.Services;
using Core.ValueObjects;
using Scriban;
using Server.Component;
using Server.ResourceLoader;
using Server.ContentParser;
using Server.Session;
using System.Net;
using System.Text;
using Server.Resources;

namespace Server.RequestHandler;
record struct LoginData(Username Username, Password Password);

public class LoginRequestHandler : IRequestHandler
{
    private static readonly ErrorMessage INVALID_CREDENTIALS_ERROR_MESSAGE = new ErrorMessage("Benutzername und/oder Passwort falsch!");

    private readonly ChefLoginService chefLoginService;
    private readonly IResourceLoader resourceLoader;
    private readonly ISessionService sessionService;
    private readonly TemplateLoader templateLoader;

    public LoginRequestHandler(
      ChefLoginService chefLoginService,
      IResourceLoader resourceLoader,
      ISessionService sessionService
  )
    {
        this.chefLoginService = chefLoginService;
        this.resourceLoader = resourceLoader;
        this.sessionService = sessionService;
        this.templateLoader = new TemplateLoader(resourceLoader);
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
            Header = await new Header(resourceLoader).RenderAsync(currentChef),
        });

        response.StatusCode = (int)httpStatus;
        await response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(loginPage));
    }

    private LoginData? ParsePostContentData(HttpListenerRequest request)
    {
        var contentParser = ContentParserFactory.CreateContentParser(request.ContentType);

        if(!contentParser.CanParse(request))
        {
            return null;
        }

        var content = contentParser.ParseRequest(request);

        if(!content.TryGetValue("username", out var username) && username!.IsText)
        {
            return null;
        }
        if(!content.TryGetValue("password", out var password) && password!.IsText)
        {
            return null;
        }

        return new LoginData(
            new Username(username.TextValue!), 
            new Password(password.TextValue!)
        );   
    }
}
