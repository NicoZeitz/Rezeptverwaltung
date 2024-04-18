using Server.Resources;

namespace Server.Components;

public abstract class TemplateComponent : Component
{
    protected readonly TemplateLoader templateLoader;

    public TemplateComponent(ResourceLoader.ResourceLoader resourceLoader)
    {
        templateLoader = new TemplateLoader(resourceLoader);
    }

    public abstract Task<string> RenderAsync();
}
