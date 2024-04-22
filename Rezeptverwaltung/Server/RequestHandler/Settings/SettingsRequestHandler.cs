using Core.Entities;
using Core.ValueObjects;
using Server.Components;
using Server.DataParser;
using Server.PageRenderer;
using Server.Service;
using Server.Session;
using System.Net;

namespace Server.RequestHandler;

public class SettingsRequestHandler : AuthorizedRequestHandler
{
    private readonly SettingsPageRenderer settingsPageRenderer;
    private readonly SettingsPostDataParser settingsPostDataParser;

    public SettingsRequestHandler(
        SettingsPageRenderer settingsPageRenderer,
        SettingsPostDataParser settingsPostDataParser,
        HTMLFileWriter htmlFileWriter,
        NotFoundPageRenderer notFoundPageRenderer,
        SessionService sessionService)
        : base(htmlFileWriter, notFoundPageRenderer, sessionService)
    {
        this.settingsPageRenderer = settingsPageRenderer;
        this.settingsPostDataParser = settingsPostDataParser;
    }

    public override bool CanHandle(HttpListenerRequest request)
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

    public override Task Handle(HttpListenerRequest request, HttpListenerResponse response, Chef currentChef)
    {
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