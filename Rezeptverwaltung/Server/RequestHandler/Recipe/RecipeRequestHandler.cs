using Core.ValueObjects;
using Scriban;
using Server.ResourceLoader;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Server.RequestHandler;

public partial class RecipeRequestHandler : IRequestHandler
{
    private readonly IResourceLoader resourceLoader;

    public RecipeRequestHandler(IResourceLoader resourceLoader) : base()
    {
        this.resourceLoader = resourceLoader;
    }

    [GeneratedRegex("/recipe/(?<recipe_id>[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-4[a-fA-F0-9]{3}-[89AB][a-fA-F0-9]{3}-[a-fA-F0-9]{12})", RegexOptions.NonBacktracking | RegexOptions.IgnoreCase, matchTimeoutMilliseconds: 1000)]
    private static partial Regex recipeUrlPathRegex();

    public bool CanHandle(HttpListenerRequest request) => recipeUrlPathRegex().IsMatch(request.Url?.AbsolutePath ?? "");

    public async Task Handle(HttpListenerRequest request, HttpListenerResponse response)
    {
        var uuid = Guid.Parse(recipeUrlPathRegex().Match(request.Url!.AbsolutePath).Groups["recipe_id"].Value);
        var recipeIdentifier = new Identifier(uuid);



        // TODO: load recipe from uuid
        Console.WriteLine($"Recipe UUID: {uuid}");

        response.ContentType = MimeType.HTML.Text;
        response.StatusCode = (int)HttpStatusCode.OK;

        var template = Template.Parse("<h1>Hello {{name}}!</h1>");
        var result = template.Render(new { Name = recipeIdentifier }); // => "Hello World!" 


        await response.OutputStream.WriteAsync(
            Encoding.UTF8.GetBytes(result),
            0,
            Encoding.UTF8.GetByteCount(result)
        );
    }
}
