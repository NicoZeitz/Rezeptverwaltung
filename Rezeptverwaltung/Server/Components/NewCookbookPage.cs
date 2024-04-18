

using Server.Resources;

namespace Server.Components;

public class NewCookbookPage : ContainerComponent
{
    public NewCookbookPage(TemplateLoader templateLoader)
        : base(templateLoader)
    {
    }

    public override Task<string> RenderAsync()
    {
        throw new NotImplementedException();
    }
}
