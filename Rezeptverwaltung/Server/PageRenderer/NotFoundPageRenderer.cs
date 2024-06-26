using Core.Entities;
using Server.Components;

namespace Server.PageRenderer;

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
        notFoundPage.SlottedChildren[NotFoundPage.HEADER_SLOT] = header;
        return notFoundPage.RenderAsync();
    }
}