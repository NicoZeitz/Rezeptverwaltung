

using Server.Resources;

namespace Server.Components;

public class ShoppingListDetailPage : ContainerComponent
{
    public ShoppingListDetailPage(TemplateLoader templateLoader)
        : base(templateLoader)
    {
    }

    public override Task<string> RenderAsync()
    {
        throw new NotImplementedException();
    }
}
