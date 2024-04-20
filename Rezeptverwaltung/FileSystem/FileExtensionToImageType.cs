using Core.ValueObjects;

namespace FileSystem;

public class FileExtensionToImageType
{
    public static ImageType GetImageType(string extension)
    {
        return extension switch
        {
            "jpg" or "jpeg" => ImageType.JPEG,
            "png" => ImageType.PNG,
            "webp" => ImageType.WEBP,
            _ => ImageType.JPEG
        };
    }
}