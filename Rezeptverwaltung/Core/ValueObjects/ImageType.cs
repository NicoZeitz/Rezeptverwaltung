namespace Core.ValueObjects;

public readonly struct ImageType : IEquatable<ImageType>
{
    public static readonly ImageType PNG = new ImageType("PNG");
    public static readonly ImageType JPEG = new ImageType("JPEG");
    public static readonly ImageType GIF = new ImageType("GIF");
    public static readonly ImageType SVG = new ImageType("SVG");
    public static readonly ImageType WEBP = new ImageType("WEBP");
    public static readonly ImageType ICO = new ImageType("ICO");

    public readonly string FileExtension;

    private ImageType(string fileExtension)
    {
        FileExtension = fileExtension;
    }

    public static ImageType? FromString(string fileExtension)
    {
        return fileExtension switch
        {
            "png" => PNG,
            "jpeg" => JPEG,
            "gif" => GIF,
            "svg" => SVG,
            "webp" => WEBP,
            "ico" => ICO,
            _ => null,
        };
    }

    public static readonly ImageType[] ALL_IMAGE_TYPES = [PNG, JPEG, GIF, SVG, WEBP, ICO];

    public bool Equals(ImageType other) => FileExtension == other.FileExtension;

    public override int GetHashCode() => FileExtension.GetHashCode();

    public override bool Equals(object? obj) => obj is ImageType imageType && Equals(imageType);

    public static bool operator ==(ImageType left, ImageType right) => left.Equals(right);

    public static bool operator !=(ImageType left, ImageType right) => !(left == right);

    public static implicit operator string(ImageType imageType) => imageType.FileExtension;
}