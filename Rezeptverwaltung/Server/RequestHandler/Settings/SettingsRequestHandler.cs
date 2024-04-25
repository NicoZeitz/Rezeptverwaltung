using Core.Entities;
using Core.ValueObjects;
using Server.Components;
using Server.DataParser;
using Server.PageRenderer;
using Server.Service;
using Server.Session;
using System.Net;

namespace Server.RequestHandler;

public class SettingsRequestHandler : RequestHandler
{
    private readonly NotFoundRequestHandler notFoundRequestHandler;
    private readonly SessionService sessionService;
    private readonly SettingsPageRenderer settingsPageRenderer;
    private readonly SettingsPostDataParser settingsPostDataParser;

    public SettingsRequestHandler(
        NotFoundRequestHandler notFoundRequestHandler,
        SessionService sessionService,
        SettingsPageRenderer settingsPageRenderer,
        SettingsPostDataParser settingsPostDataParser)
        : base()
    {
        this.notFoundRequestHandler = notFoundRequestHandler;
        this.sessionService = sessionService;
        this.settingsPageRenderer = settingsPageRenderer;
        this.settingsPostDataParser = settingsPostDataParser;
    }

    public bool CanHandle(HttpListenerRequest request)
    {
        if (request.Url?.AbsolutePath != "/settings")
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
        var currentChef = sessionService.GetCurrentChef(request);
        if (currentChef is null)
            return notFoundRequestHandler.Handle(request, response);

        if (request.HttpMethod == HttpMethod.Get.Method)
        {
            return settingsPageRenderer.RenderPage(
                request,
                response,
                currentChef,
                HttpStatusCode.OK
            );
        }

        return HandlePostRequest(request, response, currentChef);
    }

    private Task HandlePostRequest(HttpListenerRequest request, HttpListenerResponse response, Chef currentChef)
    {
        var postData = settingsPostDataParser.ParsePostData(request);
        if (postData.IsError)
        {
            return settingsPageRenderer.RenderPage(
                request,
                response,
                currentChef,
                HttpStatusCode.BadRequest,
                new Dictionary<string, IEnumerable<ErrorMessage>>() {
                    { SettingsPage.DELETE_PROFILE_ERRORS_SLOT,  postData.ErrorMessages }
                }
            );
        }

        return postData.Value!.HandlePostRequest(
            request,
            response
        );
    }
}