namespace Core.ValueObjects;

public record struct FileName(string Name, FileExtension Extension)
{
    public readonly string FullName => $"{Name}.{Extension.Extension}";

    public override string ToString() => FullName;

    public static FileName From(string fileNameWithExtension)
    {
        var parts = fileNameWithExtension.Split('.');
        return new FileName(parts[0], new FileExtension(parts[1]));
    }
}