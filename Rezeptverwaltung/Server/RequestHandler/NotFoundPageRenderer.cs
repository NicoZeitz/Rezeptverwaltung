using Core.Entities;
using Server.Components;

namespace Server.RequestHandler;

public class NotFoundPageRenderer
{
    private readonly ComponentProvider componentProvider;

    public NotFoundPageRenderer(ComponentProvider componentProvider)
        : base()
    {
        this.componentProvider = componentProvider;
    }

    public Task<string> RenderPage(Chef? currentChef)
    {
        var notFoundPage = componentProvider.GetComponent<NotFoundPage>();
        var header = componentProvider.GetComponent<Header>();

        header.CurrentChef = currentChef;
        notFoundPage.SlottedChildren["Header"] = header;
        return notFoundPage.RenderAsync();
    }
}