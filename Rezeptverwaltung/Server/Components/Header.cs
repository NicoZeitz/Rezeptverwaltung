using Core.Entities;
using Server.Resources;
using Server.Service;

namespace Server.Components;

public class Header : TemplateComponent
{
    public Chef? CurrentChef { get; set; }

    private readonly ImageUrlService imageUrlService;

    public Header(TemplateLoader templateLoader, ImageUrlService imageUrlService)
        : base(templateLoader)
    {
        this.imageUrlService = imageUrlService;
    }

    public override Task<string> RenderAsync()
    {
        return templateLoader
            .LoadTemplate("Header.html")!
            .RenderAsync(new
            {
                ChefImageUrl = CurrentChef is null
                    ? null
                    : imageUrlService.GetImageUrlForChef(CurrentChef),
                Chef = CurrentChef
            })
            .AsTask();
    }
}