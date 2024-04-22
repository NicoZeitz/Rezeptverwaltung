using Core.Services;
using Core.ValueObjects;
using Server.Components;
using Server.RequestHandler;
using Server.Service;
using Server.Session;
using System.Net;

namespace Server.ValueObjects.PostData;

public sealed class SettingsDeleteProfilePostData : SettingsPostData
{
    public Password Password { get; }

    private readonly DeleteChefService chefDeleteService;
    private readonly SessionService sessionService;
    private readonly SettingsPageRenderer settingsPageRenderer;
    private readonly RedirectService redirectService;

    public SettingsDeleteProfilePostData(
        Password password,
        DeleteChefService chefDeleteService,
        SessionService sessionService,
        SettingsPageRenderer settingsPageRenderer,
        RedirectService redirectService)
        : base()
    {
        Password = password;

        this.chefDeleteService = chefDeleteService;
        this.sessionService = sessionService;
        this.settingsPageRenderer = settingsPageRenderer;
        this.redirectService = redirectService;
    }

    public Task HandlePostRequest(HttpListenerRequest request, HttpListenerResponse response)
    {
        var chef = sessionService.GetCurrentChef(request);
        if (chef is null)
        {
            response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return Task.CompletedTask;
        }

        var result = chefDeleteService.DeleteChef(chef, Password);
        if (result.IsError)
        {
            return settingsPageRenderer.RenderPage(
                request,
                response,
                chef,
                HttpStatusCode.BadRequest,
                new Dictionary<string, IEnumerable<ErrorMessage>>() {
                    { SettingsPage.DELETE_PROFILE_ERRORS_SLOT, result.ErrorMessages }
                }
            );
        }

        sessionService.Logout(request, response);
        redirectService.RedirectToPage(response, "/");
        return Task.CompletedTask;
    }
}