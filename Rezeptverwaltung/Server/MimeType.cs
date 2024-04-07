using System.Reflection;

namespace Server;

public readonly struct MimeType : IEquatable<MimeType>
{
    public static readonly MimeType TEXT = new MimeType("text/plain");
    public static readonly MimeType HTML = new MimeType("text/html");
    public static readonly MimeType CSS = new MimeType("text/css");
    public static readonly MimeType JS = new MimeType("text/javascript");

    public static readonly MimeType PNG = new MimeType("image/png");
    public static readonly MimeType JPEG = new MimeType("image/jpeg");
    public static readonly MimeType GIF = new MimeType("image/gif");
    public static readonly MimeType SVG = new MimeType("image/svg+xml");
    public static readonly MimeType WEBP = new MimeType("image/webp");
    public static readonly MimeType ICO = new MimeType("image/x-icon");

    public static readonly MimeType JSON = new MimeType("application/json");
    public static readonly MimeType OCTET_STREAM = new MimeType("application/octet-stream");
    public static readonly MimeType FORM = new MimeType("application/x-www-form-urlencoded");

    public static readonly MimeType MULTIPART = new MimeType("multipart/form-data");

    // https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/enumeration-classes-over-enum-types
    public static readonly MimeType[] ALL_MIMETYPES = typeof(MimeType)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Select(field => field.GetValue(null))
            .OfType<MimeType>()
            .ToArray();

    public static readonly MimeType[] ALL_IMAGE_MIMETYPES = ALL_MIMETYPES
            .Where(mime => mime.Text.StartsWith("image/"))
            .ToArray();

    public readonly string Text;

    private MimeType(string text)
    {
        Text = text;
    }

    public static MimeType? FromString(string mimeType) 
    {
        return ALL_MIMETYPES.FirstOrDefault(type => type.Text == mimeType);
    }

    public bool Equals(MimeType other) => Text == other.Text;

    public override int GetHashCode() => Text.GetHashCode();

    public override bool Equals(object? obj) => obj is MimeType mimeType && Equals(mimeType);

    public static bool operator ==(MimeType left, MimeType right) => left.Equals(right);

    public static bool operator !=(MimeType left, MimeType right) => !(left == right);

    public static implicit operator string(MimeType mimeType) => mimeType.Text;
}
