using Core.ValueObjects;

namespace FileSystem;

public class FileSystem
{
    private readonly Core.ValueObjects.Directory directory;

    public FileSystem(Core.ValueObjects.Directory directory)
    {
        this.directory = directory;
        this.directory.Create();
    }

    public bool Exists(FileName fileName)
    {
        var file = new Core.ValueObjects.File(directory, fileName);
        return file.Exists();
    }

    public void Delete(FileName fileName)
    {
        var file = new Core.ValueObjects.File(directory, fileName);
        file.Delete();
    }

    public Stream? Load(FileName fileName)
    {
        var file = new Core.ValueObjects.File(directory, fileName);
        return file.Load();
    }

    public void Save(FileName fileName, Stream data)
    {
        var file = new Core.ValueObjects.File(directory, fileName);
        file.Save(data);
    }
}
