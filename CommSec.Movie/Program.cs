using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace CommSec.Movie
{
    public class Program
    {
        // Below is just the sample code from the Visual Studio Web Api Template. 
        // Feel free to replace this with whatever implementation you feel is suitable and production ready for a web api.
		
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        }
    }
}