namespace Server.Service;

public class HTMLSanitizer
{
    private readonly Ganss.Xss.HtmlSanitizer sanitizer = new Ganss.Xss.HtmlSanitizer();

    public string Sanitize(string html)
    {
        return sanitizer.Sanitize(html);
    }
}