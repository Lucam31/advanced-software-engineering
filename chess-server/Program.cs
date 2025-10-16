using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace chess_server;
class Program
{
    static async Task Main()
    {
        // 1. Listener erstellen
        var listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:8080/"); // URL-Prefix
        listener.Start();

        Console.WriteLine("Server läuft auf http://localhost:8080/");

        // 2. Endlos-Loop für eingehende Requests
        while (true)
        {
            var context = await listener.GetContextAsync(); // wartet auf Anfrage
            var request = context.Request;
            var response = context.Response;

            Console.WriteLine($"{request.HttpMethod} {request.Url.AbsolutePath}");

            // 3. Routing (z. B. /hello)
            if (request.HttpMethod == "GET" && request.Url.AbsolutePath == "/hello")
            {
                var message = "Hallo Welt!";
                var buffer = Encoding.UTF8.GetBytes(message);
                response.ContentLength64 = buffer.Length;
                response.StatusCode = 200;
                await response.OutputStream.WriteAsync(buffer);
            }
            else
            {
                response.StatusCode = 404;
            }

            // 4. Antwort schließen
            response.Close();
        }
    }
}