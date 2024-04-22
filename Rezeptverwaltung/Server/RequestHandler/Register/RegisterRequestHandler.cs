using Core.Services;
using Core.Services.Password;
using Core.ValueObjects;
using Server.DataParser;
using Server.PageRenderer;
using Server.Service;
using Server.Session;
using System.Net;

namespace Server.RequestHandler;

public class RegisterRequestHandler : RequestHandler
{
    private readonly RegisterChefService chefRegisterService;
    private readonly SessionService sessionService;
    private readonly DuplicatePasswordChecker duplicatePasswordChecker;
    private readonly ImageTypeMimeTypeConverter imageTypeMimeTypeConverter;
    private readonly RegisterPageRenderer registerPageRenderer;
    private readonly RegisterPostDataParser registerPostDataParser;
    private readonly RedirectService redirectService;

    public RegisterRequestHandler(
        RegisterChefService chefRegisterService,
        SessionService sessionService,
        DuplicatePasswordChecker duplicatePasswordChecker,
        ImageTypeMimeTypeConverter imageTypeMimeTypeConverter,
        RegisterPageRenderer registerPageRenderer,
        RegisterPostDataParser registerPostDataParser,
        RedirectService redirectService)
        : base()
    {
        this.chefRegisterService = chefRegisterService;
        this.sessionService = sessionService;
        this.duplicatePasswordChecker = duplicatePasswordChecker;
        this.imageTypeMimeTypeConverter = imageTypeMimeTypeConverter;
        this.registerPageRenderer = registerPageRenderer;
        this.registerPostDataParser = registerPostDataParser;
        this.redirectService = redirectService;
    }

    public bool CanHandle(HttpListenerRequest request)
    {
        if (request.Url?.AbsolutePath != "/register")
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
            return HandleGetRequest(request, response);
        }
        else
        {
            return HandlePostRequest(request, response);
        }
    }

    private Task HandleGetRequest(HttpListenerRequest request, HttpListenerResponse response)
    {
        return registerPageRenderer.RenderPage(
            response,
            HttpStatusCode.OK,
            sessionService.GetCurrentChef(request)
        );
    }

    private Task HandlePostRequest(HttpListenerRequest request, HttpListenerResponse response)
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
                [new ErrorMessage("Passwörter stimmen nicht überein")]
            );
        }

        var imageType = imageTypeMimeTypeConverter.ConvertMimeTypeToImageType(profileImage.FileMimeType!.Value);
        if (imageType is null)
        {
            return registerPageRenderer.RenderPage(
                response,
                HttpStatusCode.BadRequest,
                sessionService.GetCurrentChef(request),
                [new ErrorMessage($"Profilbild {profileImage.FileName} ist nicht erlaubt!")]
            );
        }

        var image = new Image(
            profileImage.FileData!,
            imageType.Value
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

        redirectService.RedirectToPage(response, "/");
        return Task.CompletedTask;
    }
}
