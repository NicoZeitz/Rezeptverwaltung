using Core.Services;
using Core.ValueObjects;
using Server.Session;
using System.Net;

namespace Server.RequestHandler;

public class LoginRequestHandler : RequestHandler
{
    private static readonly ErrorMessage INVALID_CREDENTIALS_ERROR_MESSAGE = new ErrorMessage("Benutzername und/oder Passwort falsch!");

    private readonly ChefLoginService chefLoginService;
    private readonly SessionService sessionService;
    private readonly LoginPageRenderer loginPageRenderer;
    private readonly LoginPostDataParser loginPostDataParser;

    public LoginRequestHandler(
      ChefLoginService chefLoginService,
      SessionService sessionService,
      LoginPageRenderer loginPageRenderer,
      LoginPostDataParser loginPostDataParser
  )
    {
        this.chefLoginService = chefLoginService;
        this.sessionService = sessionService;
        this.loginPageRenderer = loginPageRenderer;
        this.loginPostDataParser = loginPostDataParser;
    }

    public bool CanHandle(HttpListenerRequest request)
    {
        if (request.Url?.AbsolutePath != "/login")
        {
            return false;
        }

        if (request.HttpMethod == HttpMethod.Get.Method)
        {
            return true;
        }

        return request.HttpMethod == HttpMethod.Post.Method && request.HasEntityBody;
    }

    public Task Handle(HttpListenerRequest request, HttpListenerResponse response)
    {
        if (request.HttpMethod == HttpMethod.Get.Method)
        {
            var currentChef = sessionService.GetCurrentChef(request);
            return loginPageRenderer.RenderPage(
                response,
                HttpStatusCode.OK,
                currentChef
            );
        }

        return HandlePost(request, response);
    }

    private Task HandlePost(HttpListenerRequest request, HttpListenerResponse response)
    {
        var usernameAndPassword = loginPostDataParser.ParsePostData(request);
        if (usernameAndPassword.IsError)
        {
            return loginPageRenderer.RenderPage(
                response,
                HttpStatusCode.Unauthorized,
                null,
                INVALID_CREDENTIALS_ERROR_MESSAGE
            );
        }

        var (username, password) = usernameAndPassword.Value;
        var chef = chefLoginService.LoginChef(username, password);

        if (chef is null)
        {
            return loginPageRenderer.RenderPage(
                response,
                HttpStatusCode.Forbidden,
                null,
                INVALID_CREDENTIALS_ERROR_MESSAGE
            );
        }

        sessionService.Login(request, response, chef);

        response.StatusCode = (int)HttpStatusCode.SeeOther;
        response.RedirectLocation = "/";
        return Task.CompletedTask;
    }
}
