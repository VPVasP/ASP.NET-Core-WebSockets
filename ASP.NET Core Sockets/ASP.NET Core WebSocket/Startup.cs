using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.WebSockets;
using System.Security.AccessControl;
using System.Net;
using System.Text;
namespace ASP.NET_Core_WebSocket
{
    public class Startup
    {


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();
            var wsOptions = new WebSocketOptions { KeepAliveInterval = TimeSpan.FromSeconds(120) };
            app.UseWebSockets(wsOptions);

            app.Use(async (HttpContext context, RequestDelegate next) =>
            {
                if (context.Request.Path == "/send")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        using (WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync())
                        {
                            await Send(context, webSocket);
                        }
                    }
                    else
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    }
                }
            });
        }
    
        private async Task Send(HttpContext context, WebSocket webSocket)
        {
            var buffer = new byte[1024*4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), System.Threading.CancellationToken.None);
            if (result != null)
            {
                while (!result.CloseStatus.HasValue)
                {
                    string msg = Encoding.UTF8.GetString(new ArraySegment<byte>(buffer, 0, result.Count));
                    Console.WriteLine($"{msg}");
                    await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes($"The Message was Delivered on the server on:{DateTime.UtcNow:f}")), result.MessageType, result.EndOfMessage, System.Threading.CancellationToken.None);
                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                }
            }
            await webSocket.CloseAsync(result.CloseStatus.Value,result.CloseStatusDescription, System.Threading.CancellationToken.None);

        }
    }
}
