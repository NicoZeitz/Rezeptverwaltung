using Server.ResourceLoader;
using System.Net;

namespace Server.RequestHandler;

public class AssetRequestHandler : IRequestHandler
{
    public readonly IResourceLoader resourceLoader;

    public AssetRequestHandler(IResourceLoader resourceLoader)
    {
        this.resourceLoader = resourceLoader;
    }

    public bool CanHandle(HttpListenerRequest request)
    {
        if (request.HttpMethod != "GET")
        {
            return false;
        }

        if (request.Url is null || !request.Url.AbsolutePath.StartsWith("/assets/"))
        {
            return false;
        }

        var path = TransformUrlToFilePath(request.Url);
        if (path is null)
        {
            return false;
        }

        using var resource = resourceLoader.LoadResource(path);
        return resource is not null;
    }

    public async Task Handle(HttpListenerRequest request, HttpListenerResponse response)
    {
        var path = TransformUrlToFilePath(request.Url)!;
        using var resource = resourceLoader.LoadResource(path)!;

        response.StatusCode = HttpStatus.OK.Code;
        response.StatusDescription = HttpStatus.OK.Description;
        response.ContentType = FileExtension.FromFileName(Path.GetFileName(path)).GetMimeType().Text;

        await resource.CopyToAsync(response.OutputStream);
    }

    private string? TransformUrlToFilePath(Uri? url)
    {
        if (url is null)
        {
            return null;
        }

        var path = url.AbsolutePath[7..];
        var lastPart = path[(path.LastIndexOf('/') + 1)..];

        if (lastPart.Contains('.'))
        {
            return path;
        }

        if (path.Last() == '/')
        {
            return path + "index.html";
        }
        else
        {
            return path + "/index.html";
        }

    }
}
