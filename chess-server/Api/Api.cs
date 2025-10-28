using System.Net;

namespace chess_server.Api;

public class Api
{
    private readonly IRouter _router;
    private readonly HttpListener _listener;
    
    public Api(Router router)
    {
        _router = router;
        _listener = new HttpListener();
        _listener.Prefixes.Add("http://+:8080/");
    }
    
    public async Task Run()
    {
        try
        {
            _listener.Start();
            
            
            Console.WriteLine("Server started on http://0.0.0.0:8080");

            while (true)
            {
                var context = await _listener.GetContextAsync();
                Console.WriteLine($"Received request: {context.Request.Url}");
                
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _router.HandleRequest(context);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Request handling error: {ex.Message}");
                    }
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Server error: {ex.Message}");
        }
        finally
        {
            _listener?.Stop();
        }
        // ReSharper disable once FunctionNeverReturns
    }
}