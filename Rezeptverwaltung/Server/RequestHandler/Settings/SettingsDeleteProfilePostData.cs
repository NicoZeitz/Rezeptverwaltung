using System.Net;
using Core.Services;
using Core.ValueObjects;
using Server.Session;

namespace Server.RequestHandler;

public sealed class SettingsDeleteProfilePostData : SettingsPostData
{
    public Password Password { get; }

    private readonly ChefDeleteService chefDeleteService;
    private readonly SessionService sessionService;
    private readonly SettingsPageRenderer settingsPageRenderer;

    public SettingsDeleteProfilePostData(
        Password password,
        ChefDeleteService chefDeleteService,
        SessionService sessionService,
        SettingsPageRenderer settingsPageRenderer)
        : base()
    {
        this.Password = password;

        this.chefDeleteService = chefDeleteService;
        this.sessionService = sessionService;
        this.settingsPageRenderer = settingsPageRenderer;
    }

    public Task HandlePostRequest(HttpListenerRequest request, HttpListenerResponse response)
    {
        var chef = sessionService.GetCurrentChef(request);
        if (chef is null)
        {
            return settingsPageRenderer.RenderPage(
                request,
                response,
                HttpStatusCode.BadRequest,
                new Dictionary<string, IEnumerable<ErrorMessage>>()
            );
        }

        var result = chefDeleteService.DeleteChef(chef, Password);
        if (result.IsError)
        {
            return settingsPageRenderer.RenderPage(
                request,
                response,
                HttpStatusCode.BadRequest,
                new Dictionary<string, IEnumerable<ErrorMessage>>() {
                    { "DeleteProfileErrors", result.ErrorMessages }
                }
            );
        }

        sessionService.Logout(request, response);
        response.StatusCode = (int)HttpStatusCode.SeeOther;
        response.RedirectLocation = "/";
        return Task.CompletedTask;
    }
}