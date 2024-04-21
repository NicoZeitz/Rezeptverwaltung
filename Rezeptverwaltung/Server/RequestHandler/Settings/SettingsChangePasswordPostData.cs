using System.Net;
using Core.Services;
using Core.ValueObjects;
using Server.Components;
using Server.Session;

namespace Server.RequestHandler;

public class SettingsChangePasswordPostData : SettingsPostData
{
    public Password OldPassword { get; }
    public Password NewPassword { get; }
    public Password NewPasswordRepeated { get; }

    private readonly ChefChangePasswordService chefChangePasswordService;
    private readonly SessionService sessionService;
    private readonly SettingsPageRenderer settingsPageRenderer;

    public SettingsChangePasswordPostData(
        Password oldPassword,
        Password newPassword,
        Password newPasswordRepeated,
        ChefChangePasswordService chefChangePasswordService,
        SessionService sessionService,
        SettingsPageRenderer settingsPageRenderer
    ) : base()
    {
        this.OldPassword = oldPassword;
        this.NewPassword = newPassword;
        this.NewPasswordRepeated = newPasswordRepeated;
        this.chefChangePasswordService = chefChangePasswordService;
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
        var result = chefChangePasswordService.ChangePassword(chef, OldPassword, NewPassword, NewPasswordRepeated);
        if (result.IsError)
        {
            return settingsPageRenderer.RenderPage(
                request,
                response,
                HttpStatusCode.BadRequest,
                new Dictionary<string, IEnumerable<ErrorMessage>>() {
                    { SettingsPage.CHANGE_PASSWORD_ERRORS_SLOT, result.ErrorMessages }
                }
            );
        }

        return settingsPageRenderer.RenderPage(
            request,
            response,
            HttpStatusCode.OK,
            new DisplayableComponent(new Text("Passwort erfolgreich geändert!"))
        );
    }
}