using System.Web;

namespace Server.Service;

public class URLEncoder
{
    public string URLEncode(string text)
    {
        return HttpUtility.UrlEncode(text);
    }

    public string URLDecode(string text)
    {
        return HttpUtility.UrlDecode(text);
    }
}