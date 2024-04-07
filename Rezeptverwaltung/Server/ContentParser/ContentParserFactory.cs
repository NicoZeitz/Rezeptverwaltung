using System.Net.Mime;

namespace Server.ContentParser;

internal class ContentParserFactory
{
    public static IContentParser CreateContentParser(string? contentType)
    {
        if(contentType is null)
        {
            return new EmptyContentParser();
        }
        
        if (contentType.StartsWith(MimeType.FORM.Text))
        {
            return new QueryStringContentParser();
        }

        if(contentType.StartsWith(MimeType.MULTIPART.Text))
        {
            return new MultipartContentParser();
        }

        return new EmptyContentParser();
    }
}
