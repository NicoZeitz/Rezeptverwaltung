using Core.Interfaces;
using Server.ValueObjects;
using System.Net;
using System.Text;

namespace Server;

public class Server
{
    private readonly Logger logger;
    private readonly HttpListener listener = new HttpListener();
    private readonly IList<RequestHandler.RequestHandler> requestHandlers = new List<RequestHandler.RequestHandler>();

    public Server(ServerConfiguration configuration, Logger logger)
    {
        this.logger = logger;
        listener.Prefixes.Add($"http://{configuration.IPAddress}:{configuration.Port}/");
    }

    public void AddRequestHandler(RequestHandler.RequestHandler handler) => requestHandlers.Add(handler);

    public async Task Run(CancellationToken cancellationToken)
    {
        EnsureStartedListening();

        while (!cancellationToken.IsCancellationRequested)
        {
            var context = await listener.GetContextAsync();
            var request = context.Request;
            var response = context.Response;

            try
            {
                await HandleRequest(request, response);
            }
            catch (Exception exception)
            {
                logger.LogError(exception);
                await WriteInternalServerError(response);
            }
            finally
            {
                response.Close();
            }
        }

        listener.Stop();
    }

    private ValueTask WriteInternalServerError(HttpListenerResponse response)
    {
        response.StatusCode = (int)HttpStatusCode.InternalServerError;
        response.ContentType = MimeType.HTML;
        return response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes("500 Internal Server Error"));
    }

    private void EnsureStartedListening()
    {
        if (!listener.IsListening)
        {
            listener.Start();
        }
    }

    private async Task HandleRequest(HttpListenerRequest request, HttpListenerResponse response)
    {
        foreach (var handler in requestHandlers)
        {
            if (handler.CanHandle(request))
            {
                await handler.Handle(request, response);
                return;
            }
        }

        response.StatusCode = (int)HttpStatusCode.NotFound;
        response.ContentType = MimeType.HTML;
        await response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes("404 Not Found"));
        response.Close();
    }
}
