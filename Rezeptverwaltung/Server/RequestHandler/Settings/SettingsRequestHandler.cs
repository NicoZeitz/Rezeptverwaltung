using Core.ValueObjects;
using System.Net;

namespace Server.RequestHandler;

public class SettingsRequestHandler : RequestHandler
{
    private readonly SettingsPageRenderer settingsPageRenderer;
    private readonly SettingsPostDataParser settingsPostDataParser;

    public SettingsRequestHandler(
        SettingsPageRenderer settingsPageRenderer,
        SettingsPostDataParser settingsPostDataParser)
        : base()
    {
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
        if (request.HttpMethod == HttpMethod.Get.Method)
        {
            return settingsPageRenderer.RenderPage(
                request,
                response,
                HttpStatusCode.OK
            );
        }

        return HandlePostRequest(request, response);
    }

    private Task HandlePostRequest(HttpListenerRequest request, HttpListenerResponse response)
    {
        var postData = settingsPostDataParser.ParsePostData(request);
        if (postData.IsError)
        {
            return settingsPageRenderer.RenderPage(
                request,
                response,
                HttpStatusCode.BadRequest,
                new Dictionary<string, IEnumerable<ErrorMessage>>() {
                    { "DeleteProfileErrors",  postData.ErrorMessages }
                }
            );
        }

        return postData.Value!.HandlePostRequest(
            request,
            response
        );
    }
}