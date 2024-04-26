using System.Net.WebSockets;
using System.Net;
using System.Text;
namespace ASP.NET_Core_WebSocket
{
    public class Server
    {
        //configure method to set up websocket
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();
            var wsOptions = new WebSocketOptions { KeepAliveInterval = TimeSpan.FromSeconds(120) };
            app.UseWebSockets(wsOptions); //enable websocket 

            app.Use(async (HttpContext context, RequestDelegate next) =>
            {
                if (context.Request.Path == "/send")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        //accept the websocket connection
                        using (WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync())
                        {
                            //handle sending and receiving messages
                            await Send(context, webSocket);
                        }
                    }
                    else
                    {
                        //if it's not a websocket request respond with a bad request status code
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    }
                }
            });
        }

        //private method that handles and receiving messages on websocket
        private async Task Send(HttpContext context, WebSocket webSocket)
        { 
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), System.Threading.CancellationToken.None);
            if (result != null)
            {
                while (!result.CloseStatus.HasValue)
                {
                    string msg = Encoding.UTF8.GetString(new ArraySegment<byte>(buffer, 0, result.Count));
                    Console.WriteLine($"{msg}  {DateTime.Now:f}");

                    //send response message back to the client
                    await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes($"The Message was Delivered on the server on:{DateTime.Now:f}")), result.MessageType, result.EndOfMessage, System.Threading.CancellationToken.None);
                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                }

                if (result != null)
                    await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, System.Threading.CancellationToken.None);
                return;
            }
        }
    }
}
