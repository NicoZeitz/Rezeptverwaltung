using Core.ValueObjects;
using Server.Service;
using System.Net;

namespace Server.RequestHandler;

public class StaticRequestHandler : RequestHandler
{
    private readonly string prefix;
    private readonly ResourceLoader.ResourceLoader resourceLoader;
    private readonly MimeTypeDeterminer mimeTypeDeterminer;

    public StaticRequestHandler(string prefix, ResourceLoader.ResourceLoader resourceLoader, MimeTypeDeterminer mimeTypeDeterminer)
    {
        this.prefix = "/" + prefix;
        this.resourceLoader = resourceLoader;
        this.mimeTypeDeterminer = mimeTypeDeterminer;
    }

    public bool CanHandle(HttpListenerRequest request)
    {
        if (request.HttpMethod != HttpMethod.Get.Method)
        {
            return false;
        }

        if (request.Url is null || !request.Url.AbsolutePath.StartsWith(prefix))
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

        response.StatusCode = (int)HttpStatusCode.OK;
        response.ContentType = mimeTypeDeterminer.GetMimeTypeFromExtension(FileExtension.FromFileName(path)).Text;

        await resource.CopyToAsync(response.OutputStream);
    }

    private string? TransformUrlToFilePath(Uri? url)
    {
        if (url is null)
        {
            return null;
        }

        var path = url.AbsolutePath[(prefix.Length - 1)..];
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
