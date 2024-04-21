

using Server.Resources;

namespace Server.Components;

public class NewCookbookPage(TemplateLoader templateLoader) : ContainerComponent(templateLoader)
{
    public override Task<string> RenderAsync()
    {
        throw new NotImplementedException();
    }
}
