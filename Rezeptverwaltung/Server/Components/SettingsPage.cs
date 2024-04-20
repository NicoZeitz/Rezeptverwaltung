

using Core.Entities;
using Server.Resources;
using Server.Service;

namespace Server.Components;

public class SettingsPage : ContainerComponent
{
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
                CurrentChefImage = chefImage,
                Message = await GetRenderedSlottedChild("Message"),
                ChangePasswordErrors = await GetRenderedSlottedChild("ChangePasswordErrors"),
                DeleteProfileErrors = await GetRenderedSlottedChild("DeleteProfileErrors"),
                Header = await GetRenderedSlottedChild("Header"),
                CurrentChef
            });
    }
}
