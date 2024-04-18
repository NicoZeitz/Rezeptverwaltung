

using Server.Resources;

namespace Server.Components;

public class CookbookDetailPage : ContainerComponent
{
    public CookbookDetailPage(TemplateLoader templateLoader)
        : base(templateLoader)
    {
    }

    public override Task<string> RenderAsync()
    {
        throw new NotImplementedException();
    }
}
