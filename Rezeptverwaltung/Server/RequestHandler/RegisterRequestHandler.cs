using Core.Services;
using Core.ValueObjects;
using Server.Component;
using Server.ResourceLoader;
using Server.ContentParser;
using Server.Session;
using System.Net;
using System.Text;
using Server.Resources;

namespace Server.RequestHandler;

record struct RegisterData(Username Username, Name Name, Password Password, Password PasswordRepeat, ContentData profileImage);

public class RegisterRequestHandler : IRequestHandler
{
    private readonly ChefRegisterService chefRegisterService;
    private readonly IResourceLoader resourceLoader;
    private readonly ISessionService sessionService;
    private readonly TemplateLoader templateLoader;


    public RegisterRequestHandler(
      ChefRegisterService chefRegisterService,
      IResourceLoader resourceLoader,
      ISessionService sessionService
  )
    {
        this.chefRegisterService = chefRegisterService;
        this.resourceLoader = resourceLoader;
        this.sessionService = sessionService;
        this.templateLoader = new TemplateLoader(resourceLoader);
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
                HttpStatusCode.OK,
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
                HttpStatusCode.BadRequest,
                new ErrorMessage[] { new ErrorMessage("Es ist ein Fehler aufgetreten") }
            ); ;
        }

        var (username, name, password, passwordRepeat, profileImage) = postData.Value;

        if (password != passwordRepeat)
        {
            return RenderRegisterPage(
                request,
                response,
                HttpStatusCode.BadRequest,
                new ErrorMessage[] { new ErrorMessage("Passwörter stimmen nicht überein") }
            );
        }

        if(!MimeType.ALL_IMAGE_MIMETYPES.OfType<MimeType?>().Contains(profileImage.FileMimeType))
        {
            return RenderRegisterPage(
                request,
                response,
                HttpStatusCode.BadRequest,
                new ErrorMessage[] { new ErrorMessage($"Profilbild {profileImage.FileName} ist nicht erlaubt!") }
            );
        }

        // TODO: save file service

        var registerResult = chefRegisterService.RegisterChef(username, name, password, new Image(""));
        if (registerResult.IsError)
        {
            // Remove file on error
            return RenderRegisterPage(
                request,
                response,
                HttpStatusCode.BadRequest,
                registerResult.ErrorMessages
            );
        }


        var chef = registerResult.Chef!;

        sessionService.Login(request, response, chef);

        response.StatusCode = (int)HttpStatusCode.SeeOther;
        response.RedirectLocation = "/";
        return Task.CompletedTask;
    }

    private async Task RenderRegisterPage(HttpListenerRequest request, HttpListenerResponse response, HttpStatusCode httpStatus, IEnumerable<ErrorMessage> errorMessages)
    {
        var currentChef = sessionService.GetCurrentChef(request);
        
        if (currentChef is not null)
        {
            response.StatusCode = (int) HttpStatusCode.SeeOther;
            response.Redirect("/");
            return;
        }

        var registerTemplate = templateLoader.LoadTemplate("register.html");

        var registerPage = await registerTemplate.RenderAsync(new
        {
            ErrorMessages = errorMessages,
            Header = await new Header(resourceLoader).RenderAsync(null),
        });

        response.StatusCode = (int)httpStatus;
        await response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(registerPage));
    }

    private RegisterData? ParsePostData(HttpListenerRequest request)
    {
        var contentParser = ContentParserFactory.CreateContentParser(request.ContentType);
        if (!contentParser.CanParse(request))
        {
            return null;
        }

        var content = contentParser.ParseRequest(request);

        if (!content.TryGetValue("username", out var username) && username!.IsText)
        {
            return null;
        }
        if (!content.TryGetValue("first_name", out var firstName) && firstName!.IsText)
        {
            return null;
        }
        if (!content.TryGetValue("last_name", out var lastName) && lastName!.IsText)
        {
            return null;
        }
        if (!content.TryGetValue("password", out var password) && password!.IsText)
        {
            return null;
        }
        if (!content.TryGetValue("password_repeat", out var passwordRepeated) && passwordRepeated!.IsText)
        {
            return null;
        }
        if (!content.TryGetValue("profile_image", out var profileImage) && profileImage!.IsFile)
        {
            return null;
        }

        return new RegisterData(
            new Username(username.TextValue!),
            new Name(firstName.TextValue!, lastName.TextValue!),
            new Password(password.TextValue!),
            new Password(passwordRepeated.TextValue!),
            profileImage!
        );
    }


}
