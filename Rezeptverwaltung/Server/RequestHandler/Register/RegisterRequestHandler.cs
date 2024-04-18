using Core.Services;
using Core.Services.Password;
using Core.ValueObjects;
using Server.Session;
using System.Net;

namespace Server.RequestHandler.Register;

public class RegisterRequestHandler : RequestHandler
{
    private readonly ChefRegisterService chefRegisterService;
    private readonly ResourceLoader.ResourceLoader resourceLoader;
    private readonly SessionService sessionService;
    private readonly DuplicatePasswordChecker duplicatePasswordChecker;
    private readonly MimeTypeToImageType mimeTypeToImageType;
    private readonly RegisterPageRenderer registerPageRenderer;
    private readonly RegisterPostDataParser registerPostDataParser;

    public RegisterRequestHandler(
      ChefRegisterService chefRegisterService,
      ResourceLoader.ResourceLoader resourceLoader,
      SessionService sessionService,
      DuplicatePasswordChecker duplicatePasswordChecker,
      MimeTypeToImageType mimeTypeToImageType,
      RegisterPageRenderer registerPageRenderer,
      RegisterPostDataParser registerPostDataParser
  )
    {
        this.chefRegisterService = chefRegisterService;
        this.resourceLoader = resourceLoader;
        this.sessionService = sessionService;
        this.duplicatePasswordChecker = duplicatePasswordChecker;
        this.mimeTypeToImageType = mimeTypeToImageType;
        this.registerPageRenderer = registerPageRenderer;
        this.registerPostDataParser = registerPostDataParser;
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
            return registerPageRenderer.RenderPage(
                response,
                HttpStatusCode.OK,
                sessionService.GetCurrentChef(request)
            );
        }

        return HandlePost(request, response);
    }

    private Task HandlePost(HttpListenerRequest request, HttpListenerResponse response)
    {
        var postData = registerPostDataParser.ParsePostData(request);
        if (postData.IsError)
        {
            return registerPageRenderer.RenderPage(
                response,
                HttpStatusCode.BadRequest,
                sessionService.GetCurrentChef(request),
                postData.ErrorMessages
            );
        }

        var (username, name, password, passwordRepeat, profileImage) = postData.Value;

        if (!duplicatePasswordChecker.IsSamePassword(password, passwordRepeat))
        {
            return registerPageRenderer.RenderPage(
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
            return registerPageRenderer.RenderPage(
                response,
                HttpStatusCode.BadRequest,
                sessionService.GetCurrentChef(request),
                new ErrorMessage[] {
                    new ErrorMessage($"Profilbild {profileImage.FileName} ist nicht erlaubt!")
                }
            );
        }

        var image = new Image(
            profileImage.FileData!,
            mimeTypeToImageType.ConvertMimeTypeToImageType(profileImage.FileMimeType!.Value)!.Value
        );
        var registerResult = chefRegisterService.RegisterChef(username, name, password, image);

        if (registerResult.IsError)
        {
            return registerPageRenderer.RenderPage(
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
