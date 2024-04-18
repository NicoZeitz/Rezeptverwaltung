

using Server.Resources;

namespace Server.Components;

public class NewRecipePage : ContainerComponent
{
    public NewRecipePage(TemplateLoader templateLoader)
        : base(templateLoader)
    {
    }

    public override Task<string> RenderAsync()
    {
        throw new NotImplementedException();
    }
}
