using System.Net;

namespace Server.Service;

public class RedirectService
{
    public RedirectService() : base() { }

    public void RedirectToPage(HttpListenerResponse response, string page)
    {
        response.Redirect(page);
        response.StatusCode = (int)HttpStatusCode.SeeOther;
    }
}
