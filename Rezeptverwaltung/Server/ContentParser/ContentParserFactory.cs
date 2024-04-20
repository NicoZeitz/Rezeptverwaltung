using Server.ValueObjects;

namespace Server.ContentParser;

public class ContentParserFactory
{
    public ContentParserFactory() : base() { }

    public ContentParser CreateContentParser(string? contentType)
    {
        if (contentType is null)
        {
            return new EmptyContentParser();
        }

        if (contentType.StartsWith(MimeType.FORM.Text))
        {
            return new QueryStringContentParser();
        }

        if (contentType.StartsWith(MimeType.MULTIPART.Text))
        {
            return new MultipartContentParser();
        }

        return new EmptyContentParser();
    }
}
