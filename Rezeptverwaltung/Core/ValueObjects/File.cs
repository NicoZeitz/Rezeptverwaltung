namespace Core.ValueObjects;

public record struct File(Directory Directory, FileName Name)
{
    public readonly string FullName => Path.Combine(Directory.Path, Name.FullName);

    public static File From(Directory directory, string fileNameWithExtension)
    {
        return new File(directory, FileName.From(fileNameWithExtension));
    }

    public bool Exists() => System.IO.File.Exists(FullName);

    public void Delete()
    {
        if (Exists())
        {
            System.IO.File.Delete(FullName);
        }
    }

    public Stream? Load()
    {
        if (!Exists())
            return null;

        return System.IO.File.OpenRead(FullName);
    }

    public void Save(Stream data)
    {
        using var fileStream = System.IO.File.Create(FullName);
        data.CopyTo(fileStream);
    }

    public void Write(string data, bool newLine = true)
    {
        if (newLine)
            data += Environment.NewLine;

        System.IO.File.WriteAllText(FullName, data);
    }

    public void Append(string data, bool newLine = true)
    {
        if (newLine)
            data += Environment.NewLine;

        System.IO.File.AppendAllText(FullName, data);
    }

    public void Create()
    {
        if (!Directory.Exists())
        {
            Directory.Create();
        }

        if (!Exists())
        {
            System.IO.File.Create(FullName).Close();
        }
    }
}