using Core.Entities;
using Server.Components;

namespace Server.RequestHandler;

public class NotFoundPageRenderer
{
    private readonly Header header;
    private readonly NotFoundPage notFoundPage;

    public NotFoundPageRenderer(
        Header header,
        NotFoundPage notFoundPage)
        : base()
    {
        this.header = header;
        this.notFoundPage = notFoundPage;
    }

    public Task<string> RenderPage(Chef? currentChef)
    {
        header.CurrentChef = currentChef;
        notFoundPage.SlottedChildren["Header"] = header;
        return notFoundPage.RenderAsync();
    }
}