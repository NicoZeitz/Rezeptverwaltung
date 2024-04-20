using Core.ValueObjects;
using Server.ValueObjects;

namespace Server.Service;

public class MimeTypeDeterminer
{
    public MimeTypeDeterminer() : base() { }

    public MimeType GetMimeTypeFromExtension(FileExtension extension)
    {
        return extension.Extension switch
        {
            "html" => MimeType.HTML,
            "css" => MimeType.CSS,
            "js" => MimeType.JS,
            "png" => MimeType.PNG,
            "jpg" or "jpeg" => MimeType.JPEG,
            "gif" => MimeType.GIF,
            "svg" => MimeType.SVG,
            "webp" => MimeType.WEBP,
            "ico" => MimeType.ICO,
            _ => MimeType.OCTET_STREAM,
        };
    }
}
