namespace Core.ValueObjects;

public enum ImageType
{
    PNG,
    JPEG,
    GIF,
    SVG,
    WEBP,
    ICO
}

public static class ImageTypeExtensions
{
    public static string GetFileExtension(this ImageType imageType)
    {
        return imageType switch
        {
            ImageType.PNG => "png",
            ImageType.JPEG => "jpeg",
            ImageType.GIF => "gif",
            ImageType.SVG => "svg",
            ImageType.WEBP => "webp",
            ImageType.ICO => "ico",
            _ => throw new NotImplementedException(),
        };
    }

    public static string[] GetFileExtension(ImageType[] imageTypes)
    {
        return imageTypes.Select(GetFileExtension).ToArray();
    }
}

public record struct Image(byte[] Data, ImageType imageType);
