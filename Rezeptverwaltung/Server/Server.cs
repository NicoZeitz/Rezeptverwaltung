﻿using Server.RequestHandler;
using System.Net;
using System.Text;

namespace Server;

public class Server
{
    private readonly HttpListener listener = new HttpListener();
    private readonly IList<IRequestHandler> requestHandlers = new List<IRequestHandler>();

    public Server(IPAddress iPAddress, int port)
    {
        listener.Prefixes.Add($"http://{iPAddress}:{port}/");
    }

    public void AddRequestHandler(IRequestHandler handler) => requestHandlers.Add(handler);

    public async Task Run(CancellationToken cancellationToken)
    {
        EnsureStartedListening();

        while (!cancellationToken.IsCancellationRequested)
        {
            var context = await listener.GetContextAsync();
            var request = context.Request;
            var response = context.Response;

            await HandleRequest(request, response);
            response.Close();
        }

        listener.Stop();
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

        response.StatusCode = HttpStatus.NOT_FOUND.Code;
        response.StatusDescription = HttpStatus.NOT_FOUND.Description;
        response.ContentType = MimeType.HTML;
        await response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes("404 Not Found"));
        response.Close();
    }
}
