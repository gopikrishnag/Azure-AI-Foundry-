using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UserManagementService;

namespace UserManagementConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = Host.CreateEmptyApplicationBuilder(settings:null);
            builder.Services.AddMcpServer().WithStdioServerTransport().WithTools<Tools.SmartHomeTools>();
            builder.Services.AddSingleton<ISmartHouseUtilities, SmartHouseUtilities>();
            var app = builder.Build();
            app.Run();

            Console.WriteLine("User Management Console Application is running...");
        }
    }
}
