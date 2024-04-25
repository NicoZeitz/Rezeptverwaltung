using Server.Resources;

namespace Server.Components;

public abstract class ContainerComponent : Component
{
    public IEnumerable<Component> Children { get; set; } = [];
    public IDictionary<string, Component> SlottedChildren { get; set; } = new Dictionary<string, Component>();

    protected readonly TemplateLoader templateLoader;

    public ContainerComponent(TemplateLoader templateLoader)
    {
        this.templateLoader = templateLoader;
    }

    protected Component? GetSlottedChild(string name)
    {
        SlottedChildren.TryGetValue(name, out var component);
        return component;
    }

    protected IEnumerable<Component> GetChildren()
    {
        return Children;
    }

    protected Task<string> GetRenderedSlottedChild(string name)
    {
        var child = GetSlottedChild(name);
        if (child is null)
        {
            return Task.FromResult("");
        }
        return child.RenderAsync();
    }

    protected Task<string[]> GetRenderedChildren()
    {
        return Task.WhenAll(GetChildren().Select(child => child.RenderAsync()));
    }

    public abstract Task<string> RenderAsync();
}
