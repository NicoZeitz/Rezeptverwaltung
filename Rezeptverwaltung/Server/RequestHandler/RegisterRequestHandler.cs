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

public class RegisterRequestHandler : IRequestHandler
{
    private const int EXPIRE_AFTER_HOURS = 3;

    private readonly ChefRegisterService chefRegisterService;
    private readonly IResourceLoader resourceLoader;
    private readonly ISessionService<Chef> sessionService;

    public RegisterRequestHandler(
      ChefRegisterService chefRegisterService,
      IResourceLoader resourceLoader,
      ISessionService<Chef> sessionService
  )
    {
        this.chefRegisterService = chefRegisterService;
        this.resourceLoader = resourceLoader;
        this.sessionService = sessionService;
    }

    public bool CanHandle(HttpListenerRequest request)
    {
        if (request.Url?.AbsolutePath != "/register")
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
            return RenderRegisterPage(
                request,
                response,
                HttpStatus.OK,
                Enumerable.Empty<ErrorMessage>()
            );
        }

        return HandlePost(request, response);
    }

    private Task HandlePost(HttpListenerRequest request, HttpListenerResponse response)
    {
        var postData = ParsePostData(request);
        if (postData is null)
        {
            return RenderRegisterPage(
                request,
                response,
                HttpStatus.BAD_REQUEST,
                new ErrorMessage[] { new ErrorMessage("Es ist ein Fehler aufgetreten") }
            ); ;
        }

        var (username, name, password, passwordRepeat) = postData.Value;

        if (password != passwordRepeat)
        {
            return RenderRegisterPage(
                request,
                response,
                HttpStatus.BAD_REQUEST,
                new ErrorMessage[] { new ErrorMessage("Passwörter stimmen nicht überein") }
            );
        }

        var registerResult = chefRegisterService.RegisterChef(username, name, password);
        if (registerResult.IsError)
        {
            return RenderRegisterPage(
                request,
                response,
                HttpStatus.BAD_REQUEST,
                registerResult.ErrorMessages
            );
        }

        var chef = registerResult.Chef!;

        // TODO: move to service as it is duplicated
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

    private async Task RenderRegisterPage(HttpListenerRequest request, HttpListenerResponse response, HttpStatus httpStatus, IEnumerable<ErrorMessage> errorMessages)
    {
        // TODO: move to service as it is duplicated
        Chef? currentChef = null;
        if (request.Cookies["session"] is Cookie cookie && !cookie.Expired && cookie.Value is string sessionId)
        {
            currentChef = sessionService.GetBySessionId(Identifier.Parse(sessionId));
        }

        if (currentChef is not null)
        {
            response.StatusCode = HttpStatus.SEE_OTHER.Code;
            response.StatusDescription = HttpStatus.SEE_OTHER.Description;
            response.Redirect("/");
            return;
        }

        using var registerStream = resourceLoader.LoadResource("register.html")!;
        var registerContent = new StreamReader(registerStream).ReadToEnd();
        var registerTemplate = Template.Parse(registerContent);

        var registerPage = await registerTemplate.RenderAsync(new
        {
            ErrorMessages = errorMessages,
            Header = await new Header(resourceLoader).RenderAsync(null),
        });

        response.StatusCode = httpStatus.Code;
        response.StatusDescription = httpStatus.Description;
        await response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(registerPage));
    }

    // TODO: change to struct type
    private (Username, Name, Password, Password)? ParsePostData(HttpListenerRequest request)
    {
        using var body = request.InputStream;
        using var reader = new StreamReader(body);
        var bodyString = reader.ReadToEnd();

        var queryValues = HttpUtility.ParseQueryString(bodyString);

        if (queryValues["username"] is null
            || queryValues["first_name"] is null
            || queryValues["last_name"] is null
            || queryValues["password"] is null
            || queryValues["password_repeat"] is null)
        {
            return null;
        }

        return (
            new Username(queryValues["username"]!),
            new Name(queryValues["first_name"]!, queryValues["last_name"]!),
            new Password(queryValues["password"]!),
            new Password(queryValues["password_repeat"]!)
        );
    }
}
