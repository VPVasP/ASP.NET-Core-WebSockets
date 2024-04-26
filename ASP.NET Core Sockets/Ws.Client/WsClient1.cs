using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;


namespace ASP.NET_Core_WebSocket
{
    class WsClient1
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("WS.Client 1"); //Message saying the console name
            Console.WriteLine("Enter your name:");//Ask the username
            string? userName = Console.ReadLine();//Read the username

            using (ClientWebSocket client = new ClientWebSocket()) //create a websocket client
            {
                Uri serviceUri = new Uri("Ws://localhost:5221/send"); //uri for the websocket server
                var cTs = new CancellationTokenSource();
                cTs.CancelAfter(TimeSpan.FromSeconds(120));
                try
                {
                    await client.ConnectAsync(serviceUri, cTs.Token);

                    while (client.State == WebSocketState.Open)
                    {
                        Console.WriteLine("Please Enter your Message");
                        string? message = Console.ReadLine();
                        if (!string.IsNullOrEmpty(message))
                        {
                            //send the message to the server displaying the username and the message content
                            string messageToSend = $"{userName}: {message}";
                            ArraySegment<byte> byteToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(messageToSend));
                            Debug.WriteLine(messageToSend);
                            await client.SendAsync(byteToSend, WebSocketMessageType.Text, true, cTs.Token);

                            var responseBuffer = new byte[1024];
                            var offset = 0;
                            var packet = 1024;
                            while (true)
                            {
                                ArraySegment<byte> byteReceived = new ArraySegment<byte>(responseBuffer, offset, packet);
                                WebSocketReceiveResult response = await client.ReceiveAsync(byteReceived, cTs.Token);
                                var responseMessage =Encoding.UTF8.GetString(responseBuffer, offset, response.Count);
                                Console.WriteLine(responseMessage);
                                if (response.EndOfMessage)
                                    break;
                            }
                        }
                        else
                        {
                            Console.WriteLine("The message is empty.");
                        }
                    }
                }
                catch (WebSocketException e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            Console.ReadLine();
        }
    }
}