namespace Server.Components;

public class HomePage : ContainerComponent
{
    public HomePage(
        ResourceLoader.ResourceLoader resourceLoader)
        : base(resourceLoader)
    {
    }

    public override async Task<string> RenderAsync()
    {
        return await templateLoader.LoadTemplate("HomePage.html")!.RenderAsync(new
        {
            Header = await GetRenderedSlottedChild("Header"),
            RecipeList = await GetRenderedChildren()
        });
    }
}
