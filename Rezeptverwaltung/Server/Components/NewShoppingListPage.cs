

using Server.Resources;

namespace Server.Components;

public class NewShoppingListPage : ContainerComponent
{
    public NewShoppingListPage(TemplateLoader templateLoader)
        : base(templateLoader)
    {
    }

    public override Task<string> RenderAsync()
    {
        throw new NotImplementedException();
    }
}
