using Core.Entities;
using Server.Resources;
using Server.Service;

namespace Server.Components;

public class Header : Component
{
    public Chef? CurrentChef { get; set; }

    private readonly ImageUrlService imageUrlService;
    private readonly TemplateLoader templateLoader;

    public Header(ImageUrlService imageUrlService, TemplateLoader templateLoader)
        : base()
    {
        this.imageUrlService = imageUrlService;
        this.templateLoader = templateLoader;
    }

    public Task<string> RenderAsync()
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