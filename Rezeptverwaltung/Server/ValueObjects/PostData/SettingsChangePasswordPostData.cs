using Core.Services;
using Core.ValueObjects;
using Server.Components;
using Server.RequestHandler;
using Server.Session;
using System.Net;

namespace Server.ValueObjects.PostData;

public class SettingsChangePasswordPostData : SettingsPostData
{
    public Password OldPassword { get; }
    public Password NewPassword { get; }
    public Password NewPasswordRepeated { get; }

    private readonly ChangeChefPasswordService changeChefPasswordService;
    private readonly SessionService sessionService;
    private readonly SettingsPageRenderer settingsPageRenderer;

    public SettingsChangePasswordPostData(
        Password oldPassword,
        Password newPassword,
        Password newPasswordRepeated,
        ChangeChefPasswordService changeChefPasswordService,
        SessionService sessionService,
        SettingsPageRenderer settingsPageRenderer)
        : base()
    {
        OldPassword = oldPassword;
        NewPassword = newPassword;
        NewPasswordRepeated = newPasswordRepeated;
        this.changeChefPasswordService = changeChefPasswordService;
        this.sessionService = sessionService;
        this.settingsPageRenderer = settingsPageRenderer;
    }

    // TODO: move back to SettingsRequestHandler
    public Task HandlePostRequest(HttpListenerRequest request, HttpListenerResponse response)
    {
        var chef = sessionService.GetCurrentChef(request);
        if (chef is null)
        {
            return settingsPageRenderer.RenderPage(
                request,
                response,
                null,
                HttpStatusCode.BadRequest,
                new Dictionary<string, IEnumerable<ErrorMessage>>()
            );
        }
        var result = changeChefPasswordService.ChangePassword(chef, OldPassword, NewPassword, NewPasswordRepeated);
        if (result.IsError)
        {
            return settingsPageRenderer.RenderPage(
                request,
                response,
                chef,
                HttpStatusCode.BadRequest,
                new Dictionary<string, IEnumerable<ErrorMessage>>() {
                    { SettingsPage.CHANGE_PASSWORD_ERRORS_SLOT, result.ErrorMessages }
                }
            );
        }

        return settingsPageRenderer.RenderPage(
            request,
            response,
            chef,
            HttpStatusCode.OK,
            new DisplayableComponent(new Text("Passwort erfolgreich geändert!"))
        );
    }
}