using Core.ValueObjects;

namespace Server;

public class MimeTypeToImageType
{
    public ImageType? ConvertMimeTypeToImageType(MimeType mimeType)
    {
        if (mimeType == MimeType.PNG)
            return ImageType.PNG;
        if (mimeType == MimeType.JPEG)
            return ImageType.JPEG;
        if (mimeType == MimeType.GIF)
            return ImageType.GIF;
        if (mimeType == MimeType.SVG)
            return ImageType.SVG;
        if (mimeType == MimeType.WEBP)
            return ImageType.WEBP;
        if (mimeType == MimeType.ICO)
            return ImageType.ICO;

        return null;
    }
}
