using Server.ValueObjects;

namespace Server.ContentParser;

public class ContentData
{
    public string? TextValue { get; }
    public Stream? FileData { get; }
    public string? FileName { get; }
    public MimeType? FileMimeType { get; }

    public bool IsFile => TextValue is null;
    public bool IsText => TextValue is not null;

    private ContentData(string value)
    {
        TextValue = value;
    }

    private ContentData(Stream fileData, string fileName, MimeType fileMimeType)
    {
        FileData = fileData;
        FileName = fileName;
        FileMimeType = fileMimeType;
    }

    public static ContentData FromString(string value) => new ContentData(value);

    public static ContentData FromFile(Stream data, string fileName, MimeType mimeType) => new ContentData(data, fileName, mimeType);
}
