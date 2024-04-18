using Server.Resources;

namespace Server.Components;

public abstract class TemplateComponent : Component
{
    protected readonly TemplateLoader templateLoader;

    public TemplateComponent(TemplateLoader templateLoader)
    {
        this.templateLoader = templateLoader;
    }

    public abstract Task<string> RenderAsync();
}
