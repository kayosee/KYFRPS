using System.Configuration;

namespace KYFRPS.Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var server = new Server();
            var port = ConfigurationManager.AppSettings["port"];
            server.Start(int.Parse(port));
            Console.ReadKey();
        }
    }
}
