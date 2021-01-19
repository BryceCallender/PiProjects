using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LEDControl
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddDebug();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    IPHostEntry heserver = Dns.GetHostEntry(Dns.GetHostName());
                    string networkAddress = "*";

                    foreach(IPAddress address in heserver.AddressList)
                    {
                        if(address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            networkAddress = address.ToString();
                            break;
                        }
                    }

                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls($"http://{networkAddress}:5000");
                });
    }
}
