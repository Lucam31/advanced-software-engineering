using System.Net;
using Shared.Logger;

namespace chess_server.Api;

/// <summary>
/// Represents the core API server that listens for and handles HTTP requests.
/// </summary>
public class Api
{
    private readonly IRouter _router;
    private readonly HttpListener _listener;

    /// <summary>
    /// Initializes a new instance of the <see cref="Api"/> class.
    /// </summary>
    /// <param name="router">The router to use for handling requests.</param>
    public Api(Router router)
    {
        _router = router;
        _listener = new HttpListener();
        _listener.Prefixes.Add("http://+:8080/");
    }

    /// <summary>
    /// Starts the server and begins listening for incoming requests.
    /// </summary>
    public async Task Run()
    {
        try
        {
            _listener.Start();


            GameLogger.Info("Server started on http://0.0.0.0:8080");

            while (true)
            {
                var context = await _listener.GetContextAsync();
                GameLogger.Info($"Received request: {context.Request.HttpMethod} {context.Request.Url}");

                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _router.HandleRequest(context);
                    }
                    catch (Exception ex)
                    {
                        GameLogger.Error($"Request handling error: {ex.Message}", ex);
                    }
                });
            }
        }
        catch (Exception ex)
        {
            GameLogger.Fatal($"Server error: {ex.Message}", ex);
        }
        finally
        {
            _listener?.Stop();
            GameLogger.Info("HTTP listener stopped.");
        }
        // ReSharper disable once FunctionNeverReturns
    }
}