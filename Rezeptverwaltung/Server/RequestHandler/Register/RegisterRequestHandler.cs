using Core.Services;
using Core.ValueObjects;
using Server.Session;
using System.Net;
using Core.Services.Password;

namespace Server.RequestHandler.Register;

public class RegisterRequestHandler : RequestHandler
{
    private readonly ChefRegisterService chefRegisterService;
    private readonly ResourceLoader.ResourceLoader resourceLoader;
    private readonly SessionService sessionService;
    private readonly DuplicatePasswordChecker duplicatePasswordChecker;

    public RegisterRequestHandler(
      ChefRegisterService chefRegisterService,
      ResourceLoader.ResourceLoader resourceLoader,
      SessionService sessionService,
      DuplicatePasswordChecker duplicatePasswordChecker
  )
    {
        this.chefRegisterService = chefRegisterService;
        this.resourceLoader = resourceLoader;
        this.sessionService = sessionService;
        this.duplicatePasswordChecker = duplicatePasswordChecker;
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
            return new RegisterPageRenderer().RenderPage(
                resourceLoader,
                response,
                HttpStatusCode.OK,
                sessionService.GetCurrentChef(request)
            );
        }

        return HandlePost(request, response);
    }

    private Task HandlePost(HttpListenerRequest request, HttpListenerResponse response)
    {
        var postData = new RegisterPostDataParser().ParsePostData(request);
        if (postData.IsError)
        {
            return new RegisterPageRenderer().RenderPage(
                resourceLoader,
                response,
                HttpStatusCode.BadRequest,
                sessionService.GetCurrentChef(request),
                postData.ErrorMessages
            );
        }

        var (username, name, password, passwordRepeat, profileImage) = postData.Value;

        if (!duplicatePasswordChecker.IsSamePassword(password, passwordRepeat))
        {
            return new RegisterPageRenderer().RenderPage(
                resourceLoader,
                response,
                HttpStatusCode.BadRequest,
                sessionService.GetCurrentChef(request),
                new ErrorMessage[] {
                    new ErrorMessage("Passwörter stimmen nicht überein")
                }
            );
        }

        if (!MimeType.ALL_IMAGE_MIMETYPES.OfType<MimeType?>().Contains(profileImage.FileMimeType))
        {
            return new RegisterPageRenderer().RenderPage(
                resourceLoader,
                response,
                HttpStatusCode.BadRequest,
                sessionService.GetCurrentChef(request),
                new ErrorMessage[] {
                    new ErrorMessage($"Profilbild {profileImage.FileName} ist nicht erlaubt!")
                }
            );
        }

        // TODO: save file service

        var registerResult = chefRegisterService.RegisterChef(username, name, password, new Image(""));
        if (registerResult.IsError)
        {
            return new RegisterPageRenderer().RenderPage(
                resourceLoader,
                response,
                HttpStatusCode.BadRequest,
                sessionService.GetCurrentChef(request),
                registerResult.ErrorMessages
            );
        }

        var chef = registerResult.Value!;

        sessionService.Login(request, response, chef);

        response.StatusCode = (int)HttpStatusCode.SeeOther;
        response.RedirectLocation = "/";
        return Task.CompletedTask;
    }
}
