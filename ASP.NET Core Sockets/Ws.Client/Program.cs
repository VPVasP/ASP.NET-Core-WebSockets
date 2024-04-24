using System.Net.WebSockets;
using System.Text;


namespace ASP.NET_Core_WebSocket
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Enter your name:");
            string userName = Console.ReadLine();

            Console.WriteLine("Press enter to continue...");
            Console.WriteLine();
            using (ClientWebSocket client = new ClientWebSocket())
            {
                Uri serviceUri = new Uri("Ws://localhost:5221/send");
                var cTs = new CancellationTokenSource();
                cTs.CancelAfter(TimeSpan.FromSeconds(120));
                try
                {
                    await client.ConnectAsync(serviceUri, cTs.Token);

                    while (client.State == WebSocketState.Open)
                    {
                        Console.WriteLine("Please Enter your Message");
                        string message = Console.ReadLine();
                        if (!string.IsNullOrEmpty(message))
                        {
                        
                            string messageToSend = $"{userName}: {message}";
                            ArraySegment<byte> byteToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(messageToSend));
                            await client.SendAsync(byteToSend, WebSocketMessageType.Text, true, cTs.Token);

                            var responseBuffer = new byte[1024];
                            var offset = 0;
                            var packet = 1024;
                            while (true)
                            {
                                ArraySegment<byte> byteReceived = new ArraySegment<byte>(responseBuffer, offset, packet);
                                WebSocketReceiveResult response = await client.ReceiveAsync(byteReceived, cTs.Token);
                                var responseMessage = Encoding.UTF8.GetString(responseBuffer, offset, response.Count);
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