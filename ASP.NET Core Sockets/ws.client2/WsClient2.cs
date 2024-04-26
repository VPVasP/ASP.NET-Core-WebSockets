using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;


namespace ASP.NET_Core_WebSocket
{
    class WsClient2
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("WS.Client 2"); //Message saying the console name
            Console.WriteLine("Enter your name:");//Ask the username
            string? userName = Console.ReadLine();//Read the username

            using (ClientWebSocket client = new ClientWebSocket()) //create a websocket client
            {
                Uri serviceUri = new Uri("Ws://localhost:5221/send"); //uri for the websocket server
                var cTs = new CancellationTokenSource();
                cTs.CancelAfter(TimeSpan.FromSeconds(120)); //timeout for connection
                try
                {
                    await client.ConnectAsync(serviceUri, cTs.Token); //connect to the websocket server

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
                            while (true) //receive messages from the server 
                            {
                                ArraySegment<byte> byteReceived = new ArraySegment<byte>(responseBuffer, offset, packet);
                                WebSocketReceiveResult response = await client.ReceiveAsync(byteReceived, cTs.Token);
                                var responseMessage = Encoding.UTF8.GetString(responseBuffer, offset, response.Count);
                                Console.WriteLine(responseMessage); //display the response message
                                if (response.EndOfMessage)
                                    break; //exit the loop if it is the end of the message
                            }
                        }
                        else
                        {
                            //if the message is empty show it in the console
                            Console.WriteLine("The message is empty.");
                        }
                    }
                }
                catch (WebSocketException exception)
                {
                    //handle any websocket exceptions here
                    Console.WriteLine(exception.ToString());
                }
            }
            Console.ReadLine();
        }
    }
}