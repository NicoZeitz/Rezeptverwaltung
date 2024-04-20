using Core.ValueObjects;
using Server.ValueObjects;

namespace Server.Service;

public class ImageTypeMimeTypeConverter
{
    public readonly IList<(ImageType, MimeType)> Mapping = new List<(ImageType, MimeType)>
    {
        (ImageType.PNG, MimeType.PNG),
        (ImageType.JPEG, MimeType.JPEG),
        (ImageType.GIF, MimeType.GIF),
        (ImageType.SVG, MimeType.SVG),
        (ImageType.WEBP, MimeType.WEBP),
        (ImageType.ICO, MimeType.ICO)
    };

    public ImageTypeMimeTypeConverter() : base() { }

    public void AddMapping(ImageType imageType, MimeType mimeType)
    {
        Mapping.Add((imageType, mimeType));
    }

    public ImageType? ConvertMimeTypeToImageType(MimeType mimeType)
    {
        foreach (var (imageType, mappedMimeType) in Mapping)
        {
            if (mappedMimeType == mimeType)
                return imageType;
        }
        return null;
    }

    public MimeType? ConvertImageTypeToMimeType(ImageType imageType)
    {
        foreach (var (mappedImageType, mimeType) in Mapping)
        {
            if (mappedImageType == imageType)
                return mimeType;
        }
        return null;
    }
}
