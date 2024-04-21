

using Core.Entities;
using Server.Resources;
using Server.Service;

namespace Server.Components;

public class SettingsPage : ContainerComponent
{
    public const string HEADER_SLOT = "Header";
    public const string MESSAGE_SLOT = "Message";
    public const string CHANGE_PASSWORD_ERRORS_SLOT = "ChangePasswordErrors";
    public const string DELETE_PROFILE_ERRORS_SLOT = "DeleteProfileErrors";

    private readonly ImageUrlService imageUrlService;

    public Chef? CurrentChef { get; set; }

    public SettingsPage(
        ImageUrlService imageUrlService,
        TemplateLoader templateLoader)
        : base(templateLoader)
    {
        this.imageUrlService = imageUrlService;
    }

    public override async Task<string> RenderAsync()
    {
        var chefImage = CurrentChef is null
            ? null
            : imageUrlService.GetImageUrlForChef(CurrentChef);

        return await templateLoader
            .LoadTemplate("SettingsPage.html")!
            .RenderAsync(new
            {
                CurrentChef,
                CurrentChefImage = chefImage,
                Header = await GetRenderedSlottedChild(HEADER_SLOT),
                Message = await GetRenderedSlottedChild(MESSAGE_SLOT),
                ChangePasswordErrors = await GetRenderedSlottedChild(CHANGE_PASSWORD_ERRORS_SLOT),
                DeleteProfileErrors = await GetRenderedSlottedChild(DELETE_PROFILE_ERRORS_SLOT)
            });
    }
}
