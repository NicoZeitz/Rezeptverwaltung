namespace Core.ValueObjects;

public record struct Directory(string Path)
{
    public readonly string Name => System.IO.Path.GetFileName(Path);
    public readonly string? Parent => System.IO.Path.GetDirectoryName(Path);
    public readonly string FullName => Path;

    public Directory Join(Directory subdirectory) => new Directory(System.IO.Path.Combine(Path, subdirectory.Path));

    public bool Exists() => System.IO.Directory.Exists(Path);

    public void Create() => System.IO.Directory.CreateDirectory(Path);
}