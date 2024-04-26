namespace ASP.NET_Core_WebSocket
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }


        //create a host builder with the default configurations
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    //configures the webhost to user the server as the startup class
                    webBuilder.UseStartup<Server>();
                });
    }
}