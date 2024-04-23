namespace Core.ValueObjects;

public readonly struct ImageType : IEquatable<ImageType>
{
    public static readonly ImageType PNG = new ImageType("png");
    public static readonly ImageType JPEG = new ImageType("jpeg");
    public static readonly ImageType JPG = new ImageType("jpg");
    public static readonly ImageType GIF = new ImageType("gif");
    public static readonly ImageType SVG = new ImageType("svg");
    public static readonly ImageType WEBP = new ImageType("webp");
    public static readonly ImageType ICO = new ImageType("ico");

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
            "jpg" => JPG,
            "gif" => GIF,
            "svg" => SVG,
            "webp" => WEBP,
            "ico" => ICO,
            _ => null,
        };
    }

    public static readonly ImageType[] ALL_IMAGE_TYPES = [PNG, JPEG, JPG, GIF, SVG, WEBP, ICO];

    public bool Equals(ImageType other) => FileExtension == other.FileExtension;

    public override int GetHashCode() => FileExtension.GetHashCode();

    public override bool Equals(object? obj) => obj is ImageType imageType && Equals(imageType);

    public static bool operator ==(ImageType left, ImageType right) => left.Equals(right);

    public static bool operator !=(ImageType left, ImageType right) => !(left == right);

    public static implicit operator string(ImageType imageType) => imageType.FileExtension;
}