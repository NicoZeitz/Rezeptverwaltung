using Scriban;
using Server.ResourceLoader;

namespace Server.Resources;

public class TemplateLoader
{
    private readonly IResourceLoader resourceLoader;

    public TemplateLoader(IResourceLoader resourceLoader)
    {
        this.resourceLoader = resourceLoader;
    }

    public Template LoadTemplate(string templateName)
    {
        using var templateStream = resourceLoader.LoadResource(templateName)!;
        var templateContent = new StreamReader(templateStream).ReadToEnd();
        return Template.Parse(templateContent);
    }   
}
