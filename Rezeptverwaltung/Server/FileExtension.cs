namespace Server;

internal struct FileExtension : IEquatable<FileExtension>
{
    public string Extension { get; init; }

    public FileExtension(string extension)
    {
        Extension = extension.StartsWith('.') ? extension[1..] : extension;
    }

    public static FileExtension FromFileName(string filename)
    {
       return new FileExtension(Path.GetExtension(filename));
    }
     
    public MimeType GetMimeType()
    {
        var determiner = new MimeTypeDeterminer();
        return determiner.GetMimeTypeFromExtension(this);
    }

    public bool Equals(FileExtension other) => Extension == other.Extension;

    public override int GetHashCode() => Extension.GetHashCode();

    public override bool Equals(object? obj) => obj is FileExtension fileExtension && Equals(fileExtension);

    public static bool operator ==(FileExtension left, FileExtension right) => left.Equals(right);

    public static bool operator !=(FileExtension left, FileExtension right) => !(left == right);
}
